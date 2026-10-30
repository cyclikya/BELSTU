"""Проверка пошагового разбора работы метода.

Разбор встраивания сверяется со штатной сборкой потока внутри себя, и
отдельной проверки ему почти не нужно. Разбор извлечения сверять не с чем:
у приёмника нет эталона. Поэтому он устроен так, чтобы не вычислять ничего
самостоятельно, а только раскладывать протокол, который ведёт parse_stream,
— и здесь проверяется именно это: показанное совпадает с тем, по чему
программа принимала решения.

Отдельно проверяется формула канальной позиции. Это единственное место
разбора, где порядок бит вычисляется заново, а не берётся готовым, поэтому
она сверяется с самим перемежителем на всех допустимых глубинах.
"""

import pytest

from textsteg.api import embed
from textsteg.core import hamming
from textsteg.core.bits import Bits
from textsteg.core.framing import SYNC_MARKER, Header, crc16
from textsteg.core.hamming import BlockStatus
from textsteg.core.pipeline import (
    ALLOWED_DEPTHS,
    FRAME_CODEWORDS,
    HEADER_CODED_BITS,
    FrameOutcome,
    PipelineConfig,
    build_stream,
    parse_stream,
)
from textsteg.embedding import ZeroWidthEmbedder
from textsteg.embedding.zerowidth import ALPHABET
from textsteg.trace import (
    channel_position,
    trace_embed,
    trace_extract,
    verify_channel_positions,
)

CONTAINER = (
    "Электронный документ легко скопировать без потери качества. "
    "Это удобно для распространения, однако затрудняет установление "
    "источника утечки, если документ оказался за пределами организации. "
) * 90

MESSAGE = "Копия № 17, отдел кадров"

#: Первый бит полезной части первого кадра: маркер, заголовок, маркер.
FIRST_FRAME = 16 + HEADER_CODED_BITS + 16


@pytest.fixture
def embedder() -> ZeroWidthEmbedder:
    return ZeroWidthEmbedder()


def stego_of(embedder: ZeroWidthEmbedder, depth: int = 8) -> str:
    return embed(CONTAINER, MESSAGE, embedder, PipelineConfig(depth=depth)).stego


def damaged(
    embedder: ZeroWidthEmbedder, stego: str, start: int, length: int
) -> str:
    """Возвращает тот же документ с перевёрнутым пакетом бит."""
    bits: Bits = embedder.extract(stego)
    for index in range(start, min(start + length, len(bits))):
        bits[index] ^= 1
    return embedder.embed(stego, bits)


# --------------------------------------------------------------------------
# Формула канальной позиции
# --------------------------------------------------------------------------

@pytest.mark.parametrize("depth", ALLOWED_DEPTHS)
def test_channel_position_matches_the_interleaver(depth: int) -> None:
    """Формула позиции совпадает с перестановкой самого перемежителя."""
    assert verify_channel_positions(depth)


def test_channel_position_spreads_one_word_over_the_block() -> None:
    """Биты одного слова расходятся с шагом d, как и обещает перемежитель."""
    places = [channel_position(8, 0, column) for column in range(8)]
    assert places == [0, 8, 16, 24, 32, 40, 48, 56]


# --------------------------------------------------------------------------
# Разбор извлечения: совпадение с протоколом приёмника
# --------------------------------------------------------------------------

def test_trace_shows_the_same_run_as_parse_stream(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Разбор не пересчитывает извлечение, а показывает его результат."""
    stego = stego_of(embedder)
    trace = trace_extract(stego, embedder)
    direct = parse_stream(embedder.extract(stego))

    assert trace.result.success == direct.success
    assert trace.result.message == direct.message
    assert trace.result.block_statuses == direct.block_statuses
    assert trace.result.frames_lost == direct.frames_lost
    assert trace.result.header == direct.header


def test_clean_document_passes_every_step(
    embedder: ZeroWidthEmbedder,
) -> None:
    """На целом документе разбор проходит все восемь шагов."""
    trace = trace_extract(stego_of(embedder), embedder)
    assert [stage.number for stage in trace.stages] == list(range(1, 9))
    assert all(stage.reached for stage in trace.stages)
    assert trace.result.success


def test_steps_stop_but_remain_when_marker_is_not_found(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Без маркера шаги не исчезают, а гаснут: видно, где оборвалось."""
    trace = trace_extract(CONTAINER, embedder)
    assert len(trace.stages) == 8
    assert trace.stages[0].reached
    assert not any(stage.reached for stage in trace.stages[1:])


def test_header_words_are_reported_even_when_extraction_succeeds(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Слова заголовка возвращаются и при удаче: по ним видно правки в нём."""
    result = parse_stream(embedder.extract(stego_of(embedder)))
    assert result.success
    assert len(result.header_blocks) == HEADER_CODED_BITS // 8
    assert all(b.status is BlockStatus.OK for b in result.header_blocks)


# --------------------------------------------------------------------------
# Показ повреждений
# --------------------------------------------------------------------------

def test_burst_marks_lie_together_in_the_channel(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Пакет ошибок в канале отмечается подряд идущими битами."""
    stego = damaged(embedder, stego_of(embedder, depth=8), FIRST_FRAME, 8)
    trace = trace_extract(stego, embedder)
    channel = next(s for s in trace.stages if s.number == 6)
    first = channel.sections[0]

    places = sorted(span.start for span in first.spans if span.kind == "fixed")
    assert places == list(range(8))


def test_the_same_marks_spread_one_per_word_after_deinterleaving(
    embedder: ZeroWidthEmbedder,
) -> None:
    """После деперемежения те же отметки стоят по одной на кодовое слово.

    Это и есть работа перемежителя, показанная на экране: пакет из восьми
    подряд идущих ошибок достаётся восьми разным словам, и каждое из них
    код исправляет самостоятельно.
    """
    stego = damaged(embedder, stego_of(embedder, depth=8), FIRST_FRAME, 8)
    trace = trace_extract(stego, embedder)
    decoded = next(s for s in trace.stages if s.number == 7)
    first = decoded.sections[0]

    places = sorted(span.start for span in first.spans if span.kind == "fixed")
    assert places == [word * 8 for word in range(8)]
    assert trace.result.success


def test_erased_word_is_marked_whole_without_a_single_bit(
    embedder: ZeroWidthEmbedder,
) -> None:
    """У стёртого слова отмечается всё слово: позиция ошибки недостоверна."""
    stego = damaged(embedder, stego_of(embedder, depth=8), FIRST_FRAME, 16)
    trace = trace_extract(stego, embedder)
    decoded = next(s for s in trace.stages if s.number == 7)
    first = decoded.sections[0]

    assert not [span for span in first.spans if span.kind == "fixed"]
    erased = [span for span in first.spans if span.kind == "erased"]
    assert len(erased) == 8 * 8


def test_erasures_of_a_lost_frame_are_not_credited_to_the_code(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Стирания утраченного кадра отделены от стираний декодера."""
    stego = stego_of(embedder)
    cut = " ".join(stego.split(" ")[:320])
    result = parse_stream(embedder.extract(cut))

    assert result.frames_lost == 2
    assert result.erased_by_code == 0
    assert result.erased_by_loss == 2 * FRAME_CODEWORDS
    assert result.erased_blocks == result.erased_by_code + result.erased_by_loss


def test_frame_outcomes_distinguish_lost_marker_from_cut_stream(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Два способа потерять кадр различаются в протоколе.

    Обрыв потока и ненайденный маркер дают одинаковый вклад в счётчик
    утраченных кадров, но означают разное: в первом случае граница найдена,
    а данных за нею нет.
    """
    stego = stego_of(embedder)
    cut = " ".join(stego.split(" ")[:320])
    result = parse_stream(embedder.extract(cut))

    outcomes = [report.outcome for report in result.frames]
    assert outcomes == [
        FrameOutcome.CAPTURED,
        FrameOutcome.TRUNCATED,
        FrameOutcome.MARKER_LOST,
    ]


def test_marker_offset_is_recorded_for_every_frame(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Протокол хранит ожидаемое и найденное положение каждого маркера."""
    result = parse_stream(embedder.extract(stego_of(embedder)))
    assert [report.number for report in result.frames] == [1, 2, 3]
    assert all(report.offset == 0 for report in result.frames)
    assert all(report.mismatches == 0 for report in result.frames)


def test_aligned_burst_without_interleaving_passes_unnoticed_by_the_code(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Код молчит, а сумма не совпадает — отсюда и нужна проверка суммы.

    При d = 1 пакет в восемь бит целиком ложится на одно кодовое слово.
    Инвертированное слово само принадлежит коду, синдром при этом нулевой,
    и декодер не находит ничего. О повреждении сообщает только CRC.
    """
    stego = damaged(embedder, stego_of(embedder, depth=1), FIRST_FRAME, 8)
    result = parse_stream(embedder.extract(stego))

    assert result.corrected_blocks == 0
    assert result.erased_blocks == 0
    assert not result.success
    assert result.checksum_computed != result.header.checksum


# --------------------------------------------------------------------------
# Отказы: разбор не должен приписывать приёмнику того, чего он не делал
# --------------------------------------------------------------------------

def test_header_erasure_is_not_credited_to_frame_loss(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Стирание в заголовке не записывается в утрату кадров.

    На отказном пути block_statuses хранит слова заголовка, а кадров
    приёмник не читал вовсе. Счётчик, выведенный вычитанием, приписал бы
    утрате кадров работу декодера — ровно ту подмену, ради исключения
    которой счётчики и разделены.
    """
    bits = list(build_stream(MESSAGE, PipelineConfig(depth=8)))
    bits[16] ^= 1
    bits[17] ^= 1
    result = parse_stream(bits)

    assert result.header is None
    assert result.frames == []
    assert result.frames_lost == 0
    assert result.erased_by_loss == 0
    assert result.erased_in_header == 1


def test_step_eight_is_not_reached_without_a_checksum(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Пока сумму не считали, разбор не объявляет её совпавшей или нет.

    Заголовок с глубиной вне допустимых читается успешно, но дальше
    приёмник не идёт: деперемежитель по такой глубине не настроить.
    """
    data = MESSAGE.encode("utf-8")
    header = Header(message_length=len(data), depth=3, checksum=crc16(data))
    stream = list(SYNC_MARKER) + hamming.encode(header.to_bits())
    stream += list(SYNC_MARKER) + hamming.encode([0] * 128)
    trace = trace_extract(embedder.embed(CONTAINER, stream), embedder)

    assert trace.result.header is not None
    assert trace.result.checksum_computed is None
    step = next(stage for stage in trace.stages if stage.number == 8)
    assert not step.reached
    assert "разошлись" not in step.detail
    assert "совпали" not in step.detail


def test_truncated_header_is_not_reported_as_intact(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Недополученный заголовок не объявляется принятым без повреждений.

    Декодер молча отбрасывает неполное кодовое слово, поэтому по статусам
    принятых слов оборванный заголовок выглядел бы целым.
    """
    short = build_stream(MESSAGE, PipelineConfig(depth=8))[:16 + 20]
    trace = trace_extract(embedder.embed(CONTAINER, short), embedder)

    step = next(stage for stage in trace.stages if stage.number == 3)
    assert not step.reached
    assert "без признаков повреждения" not in step.detail
    assert "оборвался" in step.detail


def test_truncated_frame_shows_the_bits_it_received(
    embedder: ZeroWidthEmbedder,
) -> None:
    """У кадра, оборвавшегося внутри, принятые биты показываются.

    Маркер найден, часть кадра пришла, и прятать её значило бы спорить с
    подробностями о слове, которые честно называют число принятых бит.
    """
    stego = stego_of(embedder)
    cut = " ".join(stego.split(" ")[:320])
    trace = trace_extract(cut, embedder)

    report = next(
        item
        for item in trace.result.frames
        if item.outcome is FrameOutcome.TRUNCATED
    )
    channel = next(s for s in trace.stages if s.number == 6)
    decoded = next(s for s in trace.stages if s.number == 7)

    # Кусок этого кадра на шаге 6 несёт принятые биты, на шаге 7 — нет:
    # деперемежать неполный кадр нечем.
    cut_in_channel = channel.sections[report.number - 1]
    cut_in_decoded = decoded.sections[report.number - 1]
    assert cut_in_channel.bits == report.received
    assert f"Кадр {report.number}" in cut_in_channel.title
    assert not cut_in_decoded.bits


def test_interleaving_legend_matches_the_colour_step(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Легенда перемежения обещает тот шаг, с которым цвета и повторяются.

    Цветов четыре, и при d >= 4 цвет возвращается через четыре слова. При
    d = 2 он возвращается через два, и обещание «через четыре» было бы
    неверным — именно в том месте, где глубина и объясняется.
    """
    for depth, step in ((2, 2), (4, 4), (8, 4)):
        trace = trace_embed(
            CONTAINER, MESSAGE, embedder, PipelineConfig(depth=depth)
        )
        stage = next(item for item in trace.stages if item.number == 6)
        words = [
            span.start // 8
            for span in stage.spans[:64]
            if span.kind == "cw0"
        ]
        promised = stage.legend[0][1]
        assert f"№ 0, {step}, {2 * step}" in promised
        # Цвет cw0 стоит на битах слов 0, step, 2*step... в канальном
        # порядке; проверяем по номеру слова в деперемеженном кадре.
        assert words[:1] == [0]


def test_shift_without_damage_still_shows_the_code_at_work(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Сдвиг кадра без правок в кадрах — но не без работы кода.

    Выпавшая позиция встраивания убирает из потока два бита. Если она
    лежала внутри заголовка, хвост заголовка съезжает, код правит его сам,
    а маркер первого кадра находится на два бита раньше. Кадры при этом
    целы, и счётчик по ним показывает ноль правок — поэтому работа кода
    над заголовком считается отдельно, иначе её в сводке не видно вовсе.
    """
    stego = stego_of(embedder)
    alphabet = set(ALPHABET)
    places = [i for i, char in enumerate(stego) if char in alphabet]
    # 54-я позиция приходится на биты 106 и 107, то есть на последнее
    # кодовое слово заголовка (оно занимает биты потока 104..111).
    cut = stego[: places[53]] + stego[places[53] + 1:]

    result = parse_stream(embedder.extract(cut))

    assert result.success
    assert result.corrected_blocks == 0
    assert result.corrected_in_header == 1
    assert result.frames[0].offset == -2
    assert all(report.offset == 0 for report in result.frames[1:])


def test_every_frame_gets_its_own_section(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Кадры выводятся все сразу, каждый под своей подписью.

    Выбирать кадр нечем и незачем: подписи и есть граница между ними.
    """
    trace = trace_extract(stego_of(embedder), embedder)
    for number in (6, 7):
        stage = next(item for item in trace.stages if item.number == number)
        assert len(stage.sections) == len(trace.result.frames)
        for index, section in enumerate(stage.sections, start=1):
            assert section.title.startswith(f"Кадр {index}")
