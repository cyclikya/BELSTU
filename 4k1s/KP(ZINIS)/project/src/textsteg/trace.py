"""Пошаговый разбор работы метода.

Модуль не выполняет собственных преобразований: он повторяет порядок
действий конвейера, обращаясь к тем же функциям ядра, и сохраняет
промежуточные состояния. Это нужно для демонстрации, где важно показать не
только результат, но и путь, которым он получен: как сообщение превращается
в кодовые слова, как перемежитель переставляет их биты и во что это
складывается в тексте.

Разбор отдаёт два вида разметки. Для шагов, где важно устройство данных,
готовится таблица полей (из чего складывается значение); для остальных —
раскладка потока на участки, которые интерфейс раскрашивает.

Достоверность разбора встраивания проверяется в самом конце: собранный
здесь поток сравнивается с результатом штатной функции build_stream, и при
расхождении возбуждается ошибка. Поэтому показанное на экране не может
разойтись с тем, что программа делает на самом деле.

Разбор извлечения устроен иначе, и по необходимости. Сверять его не с чем:
у приёмника нет эталона — он располагает одним лишь принятым документом.
Поэтому разбор извлечения не повторяет действий приёмника вторым проходом,
а только раскладывает протокол, который ведёт сама parse_stream. Расходиться
ему тем самым не с чем: показываются те же величины, по которым программа
принимала решения.

Язык разбора извлечения выдержан отдельно. Приёмник не знает, где правили
документ, и не знает, сколько ошибок было в кодовом слове на самом деле. Он
знает лишь, какой бит перевернул декодер, где нашёлся маркер и совпала ли
контрольная сумма. Поэтому в пояснениях говорится «декодер правил бит», а не
«ошибка была здесь», и «данных декодер не касался», а не «данные целы».
"""

from __future__ import annotations

from collections.abc import Callable
from dataclasses import dataclass, field

from .core import hamming
from .core.bits import Bits, bytes_to_bits, int_to_bits
from .core.framing import (
    FORMAT_SIGNATURE,
    FORMAT_VERSION,
    HEADER_BITS,
    MARKER_TOLERANCE,
    SYNC_MARKER,
    SYNC_MARKER_LENGTH,
    SYNC_MARKER_VALUE,
    Header,
    crc16,
    marker_distance,
)
from .core.hamming import DecodedBlock
from .core.interleaver import BlockInterleaver
from .core.pipeline import (
    FRAME_CODEWORDS,
    FRAME_DATA_BITS,
    FRAME_PAYLOAD_BITS,
    FRAME_TOTAL_BITS,
    HEADER_CODED_BITS,
    MESSAGE_ENCODING,
    ExtractResult,
    FrameOutcome,
    FrameReport,
    PipelineConfig,
    _interleave_frame,
    _pad_to,
    build_stream,
    check_message_length,
    frames_required,
    parse_stream,
)
from .embedding.base import Embedder
from .errors import PipelineError

__all__ = [
    "Span",
    "Field",
    "Section",
    "Stage",
    "EmbedTrace",
    "ExtractTrace",
    "trace_embed",
    "trace_extract",
    "interleaver_matrix",
    "codeword_positions",
    "channel_position",
    "frames_word",
    "verify_matrix",
    "verify_channel_positions",
]

#: Ширина информационной части кодового слова.
_DATA = hamming.K
#: Число проверочных бит базового кода (7,4).
_PARITY = hamming.N_BASE - hamming.K


@dataclass(frozen=True)
class Span:
    """Участок потока, который интерфейс показывает одним цветом."""

    start: int
    length: int
    kind: str


@dataclass(frozen=True)
class Field:
    """Строка таблицы «из чего складывается значение»."""

    name: str
    value: str
    width: str
    bits: Bits


@dataclass(frozen=True)
class Section:
    """Озаглавленный кусок данных внутри одного шага.

    Нужен там, где шаг охватывает несколько кадров: показывать их все
    одной лентой нельзя — не видно границ, — а по одному за раз значит
    заводить орган переключения ради того, что и так умещается на экране.
    """

    title: str
    bits: Bits = field(default_factory=list)
    spans: tuple[Span, ...] = ()
    note: str = ""


@dataclass(frozen=True)
class Stage:
    """Один шаг конвейера."""

    number: int
    title: str
    detail: str
    size_in: str
    size_out: str
    bits: Bits | None = None
    spans: tuple[Span, ...] = ()
    legend: tuple[tuple[str, str], ...] = ()
    fields: tuple[Field, ...] = ()
    #: Данные, разбитые на озаглавленные куски (например, по кадрам).
    sections: tuple[Section, ...] = ()
    #: Произвольная таблица: заголовки столбцов и строки.
    headers: tuple[str, ...] = ()
    rows: tuple[tuple[str, ...], ...] = ()
    #: Дошёл ли до этого шага приёмник. Недостигнутые шаги не убираются из
    #: списка: по ним видно, на чём именно разбор оборвался.
    reached: bool = True


@dataclass
class EmbedTrace:
    """Разбор встраивания со всеми промежуточными состояниями."""

    stages: list[Stage] = field(default_factory=list)
    depth: int = 0
    header: Header | None = None
    header_bits: Bits = field(default_factory=list)
    header_coded: Bits = field(default_factory=list)
    data_bits: Bits = field(default_factory=list)
    frames_plain: list[Bits] = field(default_factory=list)
    frames_coded: list[Bits] = field(default_factory=list)
    frames_woven: list[Bits] = field(default_factory=list)
    stream: Bits = field(default_factory=list)
    stego: str = ""
    slots_used: int = 0
    slots_total: int = 0


def _plural(count: int, one: str, few: str, many: str) -> str:
    """Согласует существительное с числом по правилам русского счёта."""
    tail, hundred = count % 10, count % 100
    if tail == 1 and hundred != 11:
        word = one
    elif 2 <= tail <= 4 and not 12 <= hundred <= 14:
        word = few
    else:
        word = many
    return f"{count} {word}"


def _bits(count: int) -> str:
    return _plural(count, "бит", "бита", "бит")


def _frames(count: int) -> str:
    return _plural(count, "кадр", "кадра", "кадров")


def _frames_of(count: int) -> str:
    """Родительный падеж после предлога: «у 1 кадра», «внутри 3 кадров»."""
    tail, hundred = count % 10, count % 100
    word = "кадра" if tail == 1 and hundred != 11 else "кадров"
    return f"{count} {word}"


def _all_frames(count: int, one: str, many: str) -> str:
    """«Единственный кадр найден» против «Все 3 кадра найдены»."""
    if count == 1:
        return f"Единственный кадр {one}"
    return f"Все {_frames(count)} {many}"


def frames_word(count: int) -> str:
    """То же согласование числа с «кадром», доступное интерфейсу."""
    return _frames(count)


def plural(count: int, one: str, few: str, many: str) -> str:
    """Согласование числа с существительным, доступное интерфейсу."""
    return _plural(count, one, few, many)


def _codewords(count: int) -> str:
    return _plural(count, "кодовое слово", "кодовых слова", "кодовых слов")


def _words(count: int) -> str:
    return _plural(count, "слово", "слова", "слов")


def _bytes(count: int) -> str:
    return _plural(count, "байт", "байта", "байт")


def _codeword_spans(total_bits: int, offset: int = 0) -> tuple[Span, ...]:
    """Размечает поток кодовых слов на информационные и проверочные биты."""
    spans: list[Span] = []
    for start in range(0, total_bits, hamming.N):
        base = offset + start
        spans.append(Span(base, _DATA, "info"))
        spans.append(Span(base + _DATA, _PARITY, "parity"))
        spans.append(Span(base + _DATA + _PARITY, 1, "overall"))
    return tuple(spans)


_CODEWORD_LEGEND = (
    ("info", "информационные биты, по четыре на слово"),
    ("parity", "проверочные биты кода (7,4)"),
    ("overall", "бит общей проверки на чётность, он и поднимает d* до 4"),
)


def _message_fields(message: str) -> tuple[Field, ...]:
    """Разбирает сообщение посимвольно: сколько байт даёт каждый символ."""
    fields: list[Field] = []
    for char in message:
        raw = char.encode(MESSAGE_ENCODING)
        shown = char if char != " " else "пробел"
        fields.append(
            Field(
                shown,
                f"U+{ord(char):04X}",
                f"{len(raw)} б.",
                bytes_to_bits(raw),
            )
        )
    return tuple(fields)


def _header_fields(header: Header) -> tuple[Field, ...]:
    """Раскладывает заголовок на поля с их значениями и битами."""
    signature = (FORMAT_SIGNATURE << 4) | FORMAT_VERSION
    return (
        Field(
            "Сигнатура и версия",
            f"0x{signature:02X} "
            f"(формат 0x{FORMAT_SIGNATURE:X}, версия {FORMAT_VERSION})",
            "8 бит",
            int_to_bits(signature, 8),
        ),
        Field(
            "Длина сообщения",
            _bytes(header.message_length),
            "16 бит",
            int_to_bits(header.message_length, 16),
        ),
        Field(
            "Глубина перемежения",
            f"d = {header.depth}",
            "8 бит",
            int_to_bits(header.depth, 8),
        ),
        Field(
            "Контрольная сумма",
            f"0x{header.checksum:04X} (CRC-16/CCITT-FALSE)",
            "16 бит",
            int_to_bits(header.checksum, 16),
        ),
    )


def trace_embed(
    container: str,
    message: str,
    embedder: Embedder,
    config: PipelineConfig | None = None,
) -> EmbedTrace:
    """Повторяет встраивание, сохраняя состояние после каждого шага."""
    settings = config or PipelineConfig()
    check_message_length(message)

    trace = EmbedTrace(depth=settings.depth)
    data = message.encode(MESSAGE_ENCODING)

    trace.stages.append(
        Stage(
            1,
            "Сообщение переведено в байты",
            "В кодировке UTF-8 символы занимают разное число байт: латиница, "
            "цифры и знаки по одному, кириллица по два, а знак номера целых "
            "три. Поэтому число байт не равно числу символов.",
            _plural(len(message), "символ", "символа", "символов"),
            _bytes(len(data)),
            bytes_to_bits(data),
            (),
            (),
            _message_fields(message),
        )
    )

    header = Header(
        message_length=len(data),
        depth=settings.depth,
        checksum=crc16(data),
    )
    trace.header = header
    trace.header_bits = header.to_bits()
    trace.stages.append(
        Stage(
            2,
            "Собран заголовок",
            "Получателю приходит один файл, и больше он ничего не знает. "
            "Четыре поля отвечают на четыре вопроса: наш ли это формат, "
            "сколько байт нести, с какой глубиной собран поток и совпало ли "
            "сообщение в конце. Поля ставятся подряд и дают 48 бит.",
            "четыре поля",
            _bits(HEADER_BITS),
            trace.header_bits,
            (),
            (),
            _header_fields(header),
        )
    )

    trace.header_coded = hamming.encode(trace.header_bits)
    trace.stages.append(
        Stage(
            3,
            "Заголовок закодирован",
            "Каждые четыре бита превращаются в восемь: четыре информационных, "
            "три проверочных и бит общей чётности. Перемежению заголовок не "
            "подвергается, иначе приёмник не смог бы его разобрать: глубина "
            "хранится внутри него самого.",
            _bits(HEADER_BITS),
            _bits(len(trace.header_coded)),
            trace.header_coded,
            _codeword_spans(len(trace.header_coded)),
            _CODEWORD_LEGEND,
        )
    )

    message_bits = len(data) * 8
    data_bits = _pad_to(bytes_to_bits(data), FRAME_DATA_BITS)
    if not data_bits:
        data_bits = [0] * FRAME_DATA_BITS
    trace.data_bits = data_bits
    frames = len(data_bits) // FRAME_DATA_BITS
    pad = len(data_bits) - message_bits
    trace.stages.append(
        Stage(
            4,
            "Данные дополнены до целого числа кадров",
            "Возвращаемся к самому сообщению: заголовок уже собран и дальше "
            f"идёт отдельно. Кадр несёт {FRAME_DATA_BITS} информационных бит, "
            f"поэтому к {message_bits} битам сообщения дописывается {pad} "
            "нулей. Лишнее приёмник отбросит по длине из заголовка.",
            _bits(message_bits),
            f"{_bits(len(data_bits))} ({frames} кадра)",
            data_bits,
            (
                Span(0, message_bits, "info"),
                Span(message_bits, pad, "pad"),
            ),
            (
                ("info", "биты самого сообщения"),
                ("pad", "дополнение нулями до границы кадра"),
            ),
        )
    )

    for start in range(0, len(data_bits), FRAME_DATA_BITS):
        plain = data_bits[start:start + FRAME_DATA_BITS]
        coded = hamming.encode(plain)
        trace.frames_plain.append(plain)
        trace.frames_coded.append(coded)
        trace.frames_woven.append(_interleave_frame(coded, settings))

    coded_bits = frames * FRAME_PAYLOAD_BITS
    trace.stages.append(
        Stage(
            5,
            "Кадры закодированы",
            f"В каждом кадре {_codewords(FRAME_CODEWORDS)} по {hamming.N} "
            "бит. Половина объёма уходит на защиту, скорость кода равна 1/2. "
            "Зато любая одиночная ошибка в слове исправляется, а двойная "
            "распознаётся и слово честно помечается стёртым.",
            _bits(len(data_bits)),
            f"{_bits(coded_bits)} ({_codewords(frames * FRAME_CODEWORDS)})",
            [bit for frame in trace.frames_coded for bit in frame],
            _codeword_spans(coded_bits),
            _CODEWORD_LEGEND,
        )
    )

    if settings.depth == 1:
        woven_detail = (
            "Глубина d = 1 означает отсутствие перемежения: порядок бит "
            "остаётся прежним, и все восемь бит слова по-прежнему лежат "
            "кучей. Пакет ошибок целиком попадёт в одно слово. Режим "
            "оставлен для сравнения."
        )
        woven_spans: tuple[Span, ...] = _codeword_spans(coded_bits)
        woven_legend: tuple[tuple[str, str], ...] = _CODEWORD_LEGEND
    else:
        woven_detail = (
            f"Биты пишутся в матрицу из {settings.depth} строк по "
            f"{hamming.N} столбцов и читаются по столбцам. Цвет показывает, "
            "из какого кодового слова пришёл бит: до перемежения восемь бит "
            "одного слова лежали подряд, а теперь соседние биты принадлежат "
            f"разным словам. Поэтому пакет длиной до {settings.depth} бит "
            "оставляет в каждом слове не больше одной ошибки."
        )
        # Цветов всего четыре, и при большей глубине они повторяются по
        # кругу. Шаг повторения равен числу цветов, а не четырём: при d = 2
        # цвет приходится на каждое второе слово, и обещать «через четыре»
        # было бы неверно.
        colours = min(4, settings.depth)
        woven_spans = tuple(
            Span(position, 1, f"cw{(position % settings.depth) % colours}")
            for position in range(coded_bits)
        )
        woven_legend = tuple(
            (
                f"cw{index}",
                f"слова № {index}, {index + colours}, "
                f"{index + 2 * colours} и далее",
            )
            for index in range(colours)
        )

    trace.stages.append(
        Stage(
            6,
            "Кадры перемежены",
            woven_detail,
            _bits(coded_bits),
            "столько же, изменён порядок",
            [bit for frame in trace.frames_woven for bit in frame],
            woven_spans,
            woven_legend,
        )
    )

    stream: Bits = list(SYNC_MARKER) + trace.header_coded
    spans: list[Span] = [
        Span(0, SYNC_MARKER_LENGTH, "marker"),
        Span(SYNC_MARKER_LENGTH, len(trace.header_coded), "header"),
    ]
    cursor = SYNC_MARKER_LENGTH + len(trace.header_coded)
    for woven in trace.frames_woven:
        stream.extend(SYNC_MARKER)
        spans.append(Span(cursor, SYNC_MARKER_LENGTH, "marker"))
        cursor += SYNC_MARKER_LENGTH
        stream.extend(woven)
        spans.append(Span(cursor, len(woven), "frame"))
        cursor += len(woven)
    trace.stream = stream

    trace.stages.append(
        Stage(
            7,
            "Поток собран с синхромаркерами",
            f"Маркер 0x{SYNC_MARKER_VALUE:04X} длиной {SYNC_MARKER_LENGTH} "
            "бит ставится перед заголовком и перед каждым кадром. Правка "
            "текста убирает и добавляет биты, всё последующее съезжает, и "
            "отсчитывать кадры вслепую становится нельзя. По маркеру "
            "приёмник заново находит границы.",
            f"{_frames(frames)} и заголовок",
            _bits(len(stream)),
            stream,
            tuple(spans),
            (
                ("marker", f"синхромаркер, {SYNC_MARKER_LENGTH} бит"),
                ("header", "закодированный заголовок, 96 бит"),
                ("frame", f"тело кадра, {FRAME_PAYLOAD_BITS} бит"),
            ),
        )
    )

    expected = build_stream(message, settings)
    if stream != expected:
        raise PipelineError(
            "разбор разошёлся со сборкой потока: показывать такой результат "
            "нельзя"
        )

    trace.stego = embedder.embed(container, stream)
    trace.slots_total = embedder.count_slots(container)
    trace.slots_used = -(-len(stream) // embedder.bits_per_slot)
    trace.stages.append(
        Stage(
            8,
            "Биты разложены по позициям текста",
            f"В каждую позицию укладывается {embedder.bits_per_slot} бита, и "
            "пара бит становится одним служебным символом нулевой ширины. "
            "Позиция отводится перед каждым пробелом. На экране документ при "
            "этом не меняется.",
            _bits(len(stream)),
            f"{trace.slots_used} позиций из {trace.slots_total}",
        )
    )
    return trace


def interleaver_matrix(block: Bits, depth: int) -> list[Bits]:
    """Раскладывает блок по строкам матрицы перемежителя."""
    width = hamming.N
    if len(block) != depth * width:
        raise PipelineError(
            f"блок должен содержать {depth * width} бит, получено {len(block)}"
        )
    return [block[row * width:(row + 1) * width] for row in range(depth)]


def codeword_positions(depth: int, codeword: int) -> list[int]:
    """Номера мест, куда уезжают биты одного кодового слова."""
    width = hamming.N
    if not 0 <= codeword < depth:
        raise PipelineError(
            f"номер кодового слова {codeword} вне диапазона 0..{depth - 1}"
        )
    return [column * depth + codeword for column in range(width)]


def verify_matrix(depth: int) -> bool:
    """Проверяет, что формула позиций совпадает с самим перемежителем."""
    width = hamming.N
    marker = list(range(depth * width))
    woven = BlockInterleaver(depth).interleave(marker)
    for codeword in range(depth):
        for column, place in enumerate(codeword_positions(depth, codeword)):
            if woven[place] != codeword * width + column:
                return False
    return True


# --------------------------------------------------------------------------
# Разбор извлечения
# --------------------------------------------------------------------------

#: Виды участков принятого потока. Названия намеренно описывают действие
#: приёмника, а не предполагаемую причину: «правлено» значит «декодер
#: перевернул этот бит», а не «здесь была ошибка».
_FIXED = "fixed"
_ERASED = "erased"
_LOST = "lost"
_MARKER = "marker"
_HEADER = "header"
_FRAME = "frame"

#: Сколько строк таблицы кадров показывать. Поле длины заголовка может быть
#: восстановлено неверно, и тогда приёмник честно ожидает тысячи кадров;
#: выводить их все незачем, но и молча обрезать нельзя — число показанных
#: и общее печатаются рядом.
MAX_FRAME_ROWS = 48

_OUTCOME_WORDS = {
    FrameOutcome.CAPTURED: "принят",
    FrameOutcome.MARKER_LOST: "маркер не найден",
    FrameOutcome.TRUNCATED: "поток оборвался",
}


def channel_position(depth: int, block: int, column: int) -> int:
    """Место бита кодового слова в канальном порядке внутри кадра.

    Кадр состоит из FRAME_CODEWORDS // d независимо перемежаемых групп. Внутри
    группы d слов записываются в матрицу построчно, а читаются по столбцам,
    поэтому бит столбца c слова row уезжает на место c * d + row, а к нему
    добавляется начало группы.
    """
    group, row = divmod(block, depth)
    return group * depth * hamming.N + column * depth + row


def verify_channel_positions(depth: int) -> bool:
    """Сверяет формулу канальной позиции с самим перемежителем."""
    width = hamming.N
    groups = FRAME_CODEWORDS // depth
    marker = list(range(FRAME_CODEWORDS * width))
    woven: Bits = []
    interleaver = BlockInterleaver(depth)
    size = interleaver.block_size
    for group in range(groups):
        woven.extend(
            interleaver.interleave(marker[group * size:(group + 1) * size])
        )
    for block in range(FRAME_CODEWORDS):
        for column in range(width):
            place = channel_position(depth, block, column)
            if woven[place] != block * width + column:
                return False
    return True


@dataclass
class ExtractTrace:
    """Разбор извлечения: протокол приёмника, разложенный по шагам.

    Собственных преобразований разбор не делает — все величины взяты из
    ExtractResult, который заполняет parse_stream по ходу работы.
    """

    result: ExtractResult
    bits: Bits
    symbols: int
    bits_per_symbol: int
    stages: list[Stage] = field(default_factory=list)

    @property
    def depth(self) -> int:
        """Глубина перемежения, прочитанная из заголовка."""
        return 0 if self.result.header is None else self.result.header.depth

    @property
    def frames(self) -> list[FrameReport]:
        """Протоколы приёма кадров."""
        return self.result.frames

    def report(self, number: int) -> FrameReport | None:
        """Протокол кадра по его номеру."""
        for item in self.frames:
            if item.number == number:
                return item
        return None



def _word_spans(
    blocks: list[DecodedBlock], place: Callable[[int, int], int]
) -> tuple[Span, ...]:
    """Размечает участки, к которым приёмник притронулся при декодировании.

    Для исправленного слова помечается один бит — тот, который декодер
    перевернул. Для стёртого помечается слово целиком: позиция ошибки при
    стирании недостоверна, и выделять в нём отдельный бит было бы вымыслом.
    """
    spans: list[Span] = []
    for index, block in enumerate(blocks):
        if block.status is hamming.BlockStatus.CORRECTED:
            if block.error_position is not None:
                spans.append(
                    Span(place(index, block.error_position), 1, _FIXED)
                )
        elif block.status is hamming.BlockStatus.ERASED:
            spans.extend(
                Span(place(index, column), 1, _ERASED)
                for column in range(hamming.N)
            )
    return tuple(spans)


def _touched_words(blocks: list[DecodedBlock]) -> tuple[int, int]:
    """Сколько слов декодер правил и сколько признал невосстановимыми."""
    fixed = sum(
        1 for b in blocks if b.status is hamming.BlockStatus.CORRECTED
    )
    erased = sum(1 for b in blocks if b.status is hamming.BlockStatus.ERASED)
    return fixed, erased


_TOUCH_LEGEND = (
    (_FIXED, "бит, который перевернул декодер"),
    (_ERASED, "слово, признанное невосстановимым"),
)


def _spread_note(blocks: list[DecodedBlock], depth: int) -> str:
    """Описывает, как легли в канале места, которых коснулся декодер."""
    places = sorted(
        channel_position(depth, index, block.error_position)
        for index, block in enumerate(blocks)
        if block.status is hamming.BlockStatus.CORRECTED
        and block.error_position is not None
    )
    if len(places) < 2:
        return ""
    span = places[-1] - places[0] + 1
    return (
        f" Места правок лежат в канале с {places[0]}-го по {places[-1]}-й, "
        f"то есть на участке в {_bits(span)}, и задевают "
        f"{_words(len(places))} кода — по одному биту в каждом."
    )


#: Сколько кадров показывать на шагах разбора кадра. Длинное сообщение
#: даёт десятки кадров, и выводить их все значило бы подвесить отрисовку;
#: сколько показано и сколько всего, панель называет прямо.
MAX_FRAME_SECTIONS = 12


def _frame_sections(
    result: ExtractResult, depth: int, channel: bool
) -> tuple[Section, ...]:
    """Собирает куски данных по кадрам: по одному на кадр, по порядку.

    Кадры показываются все, а не по одному за раз: границы между ними
    видны по заголовкам, и переключать нечего. При channel = True берётся
    принятый порядок бит, иначе — восстановленный деперемежителем.
    """
    sections: list[Section] = []
    for report in result.frames[:MAX_FRAME_SECTIONS]:
        if report.outcome is FrameOutcome.MARKER_LOST:
            sections.append(
                Section(
                    f"Кадр {report.number}: маркер не найден",
                    note="Приёмник не знает, где этот кадр лежал и лежал ли "
                    "вообще. Его слова помечены стёртыми без участия кода.",
                )
            )
            continue
        if report.outcome is FrameOutcome.TRUNCATED:
            if channel:
                sections.append(
                    Section(
                        f"Кадр {report.number}: поток оборвался внутри",
                        report.received,
                        note=f"Маркер найден на позиции {report.found}, "
                        f"принято {_bits(len(report.received))} из "
                        f"{FRAME_PAYLOAD_BITS}. Это всё, чем приёмник "
                        "располагает.",
                    )
                )
            else:
                sections.append(
                    Section(
                        f"Кадр {report.number}: деперемежения не было",
                        note="Деперемежитель работает только по целому "
                        f"кадру, а принято {_bits(len(report.received))} из "
                        f"{FRAME_PAYLOAD_BITS}.",
                    )
                )
            continue

        fixed, erased = _touched_words(report.blocks)
        if channel:
            marks = _word_spans(
                report.blocks,
                lambda block, column: channel_position(depth, block, column),
            )
            bits = report.received
        else:
            marks = _word_spans(
                report.blocks,
                lambda block, column: block * hamming.N + column,
            )
            bits = report.plain
        if fixed or erased:
            note = f"правок {fixed}, невосстановимых слов {erased}."
            if channel:
                note += _spread_note(report.blocks, depth)
        else:
            note = "декодер ничего не менял"
        sections.append(
            Section(f"Кадр {report.number}", bits, marks, note)
        )
    return tuple(sections)


def _frames_tail(result: ExtractResult) -> str:
    """Честная приписка, если показаны не все кадры."""
    if len(result.frames) <= MAX_FRAME_SECTIONS:
        return ""
    return (
        f" Показаны первые {_frames(MAX_FRAME_SECTIONS)} из "
        f"{len(result.frames)}: остальные не выводятся, чтобы панель "
        "оставалась читаемой."
    )


def _channel_stage(result: ExtractResult, depth: int) -> Stage:
    """Шаг 6: кадры в том порядке, в каком они пришли по каналу."""
    sections = _frame_sections(result, depth, channel=True)
    if not sections:
        return Stage(
            6,
            "Кадры в канальном порядке",
            "Кадры не читались, принятых бит у них нет.",
            "—",
            "—",
            reached=False,
        )
    fixed = sum(report.corrected for report in result.frames)
    erased = sum(report.erased for report in result.frames)
    if fixed or erased:
        detail = (
            "Так кадры лежали в тексте. Отмечены места, которых декодер "
            f"коснулся на следующем шаге: правок {fixed}, невосстановимых "
            f"слов {erased}. Отметка говорит о действии декодера, а не о "
            "том, где правили документ: этого приёмник не знает."
        )
    else:
        detail = (
            "Так кадры лежали в тексте. Декодер в них ничего не менял: все "
            "кодовые слова приняты без признаков повреждения."
        )
    return Stage(
        6,
        "Кадры в канальном порядке",
        detail + _frames_tail(result),
        f"{_frames(len(result.frames))} по {FRAME_PAYLOAD_BITS} бит",
        "порядок канала",
        sections=sections,
        legend=_TOUCH_LEGEND if fixed or erased else (),
    )


def _decoded_stage(result: ExtractResult, depth: int) -> Stage:
    """Шаг 7: те же кадры после деперемежения и декодирования."""
    sections = _frame_sections(result, depth, channel=False)
    if not sections:
        return Stage(
            7,
            "Кадры деперемежены и декодированы",
            "Кадры не читались, декодировать было нечего.",
            "—",
            "—",
            reached=False,
        )
    fixed = sum(report.corrected for report in result.frames)
    erased = sum(report.erased for report in result.frames)
    if depth == 1:
        order = (
            "Глубина d = 1: порядок бит не менялся, поэтому вид кадров тот "
            "же, что и на предыдущем шаге. Подряд идущие повреждения "
            "целиком достаются одному слову."
        )
    else:
        order = (
            "Биты разложены обратно: принятые по столбцам, прочитаны по "
            f"строкам матрицы {depth} x {hamming.N}. Соседние в канале биты "
            "разъехались по разным словам, и отметки, лежавшие на предыдущем "
            "шаге подряд, встали по одной на слово."
        )
    words = FRAME_CODEWORDS * len(result.frames)
    return Stage(
        7,
        "Кадры деперемежены и декодированы",
        f"{order} Всего {_words(words)} кода: правлено {fixed}, стёрто "
        f"{erased}. «Правлено» значит, что декодер перевернул указанный бит; "
        "была ли ошибка одиночной на самом деле, он сказать не может."
        + _frames_tail(result),
        f"{_frames(len(result.frames))} по {FRAME_PAYLOAD_BITS} бит",
        f"{_words(words)} кода",
        sections=sections,
        legend=_TOUCH_LEGEND if fixed or erased else (),
    )


def _symbol_note(symbols: int, bits: int, per_symbol: int) -> str:
    """Поясняет, откуда взялись биты, не называя причину их числа."""
    found = _plural(
        symbols, "служебный символ", "служебных символа", "служебных символов"
    )
    return (
        f"В документе найдено {found}, каждый несёт {_bits(per_symbol)} — "
        f"получилось {_bits(bits)}. Больше приёмник пока ничего не знает: "
        "ни границ кадров, ни длины сообщения в этой последовательности не "
        "видно. Структура появится на следующих шагах."
    )


def _header_report_fields(result: ExtractResult) -> tuple[Field, ...]:
    """Поля разобранного заголовка с их битами."""
    header = result.header
    if header is None:
        return ()
    signature = (FORMAT_SIGNATURE << 4) | FORMAT_VERSION
    return (
        Field(
            "Сигнатура и версия",
            f"0x{signature:02X} — формат опознан",
            "8 бит",
            int_to_bits(signature, 8),
        ),
        Field(
            "Длина сообщения",
            _bytes(header.message_length),
            "16 бит",
            int_to_bits(header.message_length, 16),
        ),
        Field(
            "Глубина перемежения",
            f"d = {header.depth}",
            "8 бит",
            int_to_bits(header.depth, 8),
        ),
        Field(
            "Контрольная сумма",
            f"0x{header.checksum:04X} (CRC-16/CCITT-FALSE)",
            "16 бит",
            int_to_bits(header.checksum, 16),
        ),
    )


def _checksum_fields(result: ExtractResult) -> tuple[Field, ...]:
    """Сверка контрольных сумм: записанной в заголовке и посчитанной."""
    header = result.header
    if header is None or result.checksum_computed is None:
        return ()
    verdict = "совпала" if result.success else "НЕ совпала"
    return (
        Field(
            "Сумма из заголовка",
            f"0x{header.checksum:04X}",
            "16 бит",
            int_to_bits(header.checksum, 16),
        ),
        Field(
            "Сумма восстановленного",
            f"0x{result.checksum_computed:04X}",
            "16 бит",
            int_to_bits(result.checksum_computed, 16),
        ),
        Field("Сверка", verdict, "—", []),
    )


def _stream_spans(result: ExtractResult) -> tuple[Span, ...]:
    """Размечает весь принятый поток по найденным границам.

    Маркеры кадров приёмник находит не сразу: сначала он отыскивает только
    первый, по которому читает заголовок, и лишь затем, зная число кадров,
    ищет остальные. Поэтому вся разметка появляется здесь, на шаге поиска
    кадров, а не раньше. У кадра, маркер которого не найден, размечать
    нечего: приёмник не знает, где он лежал и лежал ли вообще.
    """
    spans: list[Span] = []
    if result.header_at is not None:
        spans.append(Span(result.header_at, SYNC_MARKER_LENGTH, _MARKER))
        spans.append(
            Span(
                result.header_at + SYNC_MARKER_LENGTH,
                HEADER_CODED_BITS,
                _HEADER,
            )
        )
    for report in result.frames:
        if report.found is None:
            continue
        spans.append(Span(report.found, SYNC_MARKER_LENGTH, _MARKER))
        spans.append(
            Span(
                report.found + SYNC_MARKER_LENGTH,
                len(report.received),
                _FRAME,
            )
        )
    return tuple(spans)


def _drift_note(result: ExtractResult) -> str:
    """Поясняет, что именно показывает смещение найденного маркера.

    Ожидаемое положение маркера отсчитывается от предыдущей найденной
    границы, поэтому смещение измеряет изменение длины участка ПЕРЕД этим
    маркером, а не внутри самого кадра. Правка, убравшая две позиции
    встраивания из кадра, проявится смещением у следующего кадра, а
    повреждение останется в том, внутри которого правили. Без этой
    оговорки таблица читается прямо наоборот.
    """
    moved = [report for report in result.frames if report.offset]
    if not moved:
        return ""
    parts = []
    for report in moved:
        where = (
            "заголовком и им"
            if report.number == 1
            else f"кадром {report.number - 1} и им"
        )
        verb = "укоротился" if report.offset < 0 else "удлинился"
        parts.append(
            f"у кадра {report.number} смещение {report.offset:+d} — между "
            f"{where} поток {verb} на {_bits(abs(report.offset))}"
        )
    # Участок перед первым кадром — это заголовок, и его состояние
    # показывает шаг 3, а не карта кодовых слов: на карте только кадры.
    where_to_look = (
        "на шаге 3, в словах заголовка"
        if all(report.number == 1 for report in moved)
        else "на карте, у предыдущего кадра"
    )
    return (
        " Смещение говорит об участке ПЕРЕД маркером, а не о самом кадре: "
        + "; ".join(parts)
        + ". Сдвинутый кадр маркер выравнивает, а повреждение остаётся там, "
        f"где убыло или прибыло, и разбирается уже кодом — видно его "
        f"{where_to_look}."
    )


def _frame_rows(result: ExtractResult) -> tuple[tuple[str, ...], ...]:
    """Строки таблицы захвата синхронизации."""
    rows: list[tuple[str, ...]] = []
    for item in result.frames[:MAX_FRAME_ROWS]:
        rows.append(
            (
                str(item.number),
                str(item.expected),
                "—" if item.found is None else str(item.found),
                "—" if item.found is None else f"{item.offset:+d}",
                "—" if item.mismatches is None else str(item.mismatches),
                f"\u00b1{item.window}",
                _OUTCOME_WORDS[item.outcome],
            )
        )
    return tuple(rows)


def trace_extract(
    stego: str,
    embedder: Embedder,
    config: PipelineConfig | None = None,
) -> ExtractTrace:
    """Раскладывает протокол приёмника по шагам.

    Собственных вычислений здесь нет: извлечение выполняет штатная пара
    embedder.extract и parse_stream, а разбор лишь показывает величины, по
    которым приёмник принимал решения.
    """
    bits = embedder.extract(stego)
    result = parse_stream(bits, config)
    symbols = len(bits) // embedder.bits_per_slot
    trace = ExtractTrace(
        result=result,
        bits=bits,
        symbols=symbols,
        bits_per_symbol=embedder.bits_per_slot,
    )

    # Шаг 1. Из текста прочитаны биты.
    trace.stages.append(
        Stage(
            1,
            "Из текста прочитаны биты",
            _symbol_note(symbols, len(bits), embedder.bits_per_slot),
            _plural(len(stego), "символ", "символа", "символов")
            + " текста",
            _bits(len(bits)),
            bits,
        )
    )

    # Шаг 2. Поиск первого синхромаркера.
    found = result.header_at
    if found is None:
        trace.stages.append(
            Stage(
                2,
                "Поиск синхромаркера",
                "Маркер в начале потока не найден. Приёмник не различает "
                "двух причин: скрытого сообщения в тексте нет либо оно "
                "разрушено настолько, что опознать границу нечем. "
                f"Причина отказа по ядру: {result.reason}",
                _bits(len(bits)),
                "ничего",
                bits,
                reached=False,
            )
        )
        for number, title in (
            (3, "Заголовок декодирован"),
            (4, "Заголовок разобран"),
            (5, "Кадры найдены по маркерам"),
            (6, "Кадр в канальном порядке"),
            (7, "Кадр деперемежён и декодирован"),
            (8, "Собраны байты и проверена сумма"),
        ):
            trace.stages.append(
                Stage(
                    number,
                    title,
                    "Шаг не выполнялся: разбор оборвался на поиске маркера.",
                    "—",
                    "—",
                    reached=False,
                )
            )
        return trace

    mismatches = marker_distance(bits, found)
    trace.stages.append(
        Stage(
            2,
            "Найден синхромаркер",
            f"Эталон 0x{SYNC_MARKER_VALUE:04X} отыскался на позиции {found} "
            f"при ожидаемой 0; несовпало бит: {mismatches} при допуске "
            f"{MARKER_TOLERANCE}. Маркер сам идёт по повреждаемому каналу, "
            "поэтому поиск ведётся с допуском, но допуск оставлен "
            "наименьшим: при большем случайный участок нагрузки стал бы "
            "выдавать себя за границу кадра.",
            _bits(len(bits)),
            f"граница на бите {found}",
            bits,
            (
                Span(found, SYNC_MARKER_LENGTH, _MARKER),
                Span(
                    found + SYNC_MARKER_LENGTH, HEADER_CODED_BITS, _HEADER
                ),
            ),
            (
                (_MARKER, f"синхромаркер, {SYNC_MARKER_LENGTH} бит"),
                (_HEADER, f"закодированный заголовок, {HEADER_CODED_BITS} бит"),
            ),
        )
    )

    # Шаг 3. Декодирование заголовка.
    header_start = found + SYNC_MARKER_LENGTH
    header_coded = bits[header_start:header_start + HEADER_CODED_BITS]
    fixed, erased = _touched_words(result.header_blocks)
    words_total = HEADER_CODED_BITS // hamming.N
    # Декодер молча отбрасывает недополученное кодовое слово, поэтому по
    # статусам принятых слов оборванный заголовок выглядел бы целым.
    # Говорить о целости, не получив половины слов, нельзя.
    header_cut = len(header_coded) < HEADER_CODED_BITS
    if header_cut:
        trace.stages.append(
            Stage(
                3,
                "Заголовок декодирован",
                f"Поток оборвался внутри заголовка: принято "
                f"{_bits(len(header_coded))} из {HEADER_CODED_BITS}, то есть "
                f"{_words(len(result.header_blocks))} из {words_total}. "
                "Остальные слова не получены вовсе, и судить по ним о "
                "заголовке не по чему.",
                _bits(len(header_coded)),
                f"неполно, нужно {_bits(HEADER_CODED_BITS)}",
                header_coded,
                reached=False,
            )
        )
    else:
        if erased:
            header_detail = (
                f"Из {_words(words_total)} заголовка {erased} код "
                "восстановить не смог. Дальше идти нельзя: поля заголовка "
                "собраны из этих же слов. Ради такого отказа и взят код с "
                "d* = 4 — код с d* = 3 на том же слове выполнил бы "
                "исправление наугад, и неверная длина или подменённая сумма "
                "прошли бы за достоверные."
            )
        elif fixed:
            header_detail = (
                f"Все {_words(words_total)} заголовка прочитаны, но в "
                f"{fixed} из них декодер перевернул по биту. Поля ниже верны "
                "при условии, что ошибки были одиночными; проверить это "
                "приёмнику нечем."
            )
        else:
            header_detail = (
                f"Все {_words(words_total)} заголовка приняты без признаков "
                "повреждения. Заголовок не перемежается: глубина лежит "
                "внутри него самого, и деперемежитель без неё не настроить."
            )
        trace.stages.append(
            Stage(
                3,
                "Заголовок декодирован",
                header_detail,
                _bits(len(header_coded)),
                _bits(HEADER_BITS),
                header_coded,
                _word_spans(
                    result.header_blocks,
                    lambda block, column: block * hamming.N + column,
                ),
                _TOUCH_LEGEND,
            )
        )

    # Шаг 4. Разбор полей заголовка.
    if result.header is None:
        trace.stages.append(
            Stage(
                4,
                "Заголовок разобран",
                f"Поля прочитать не удалось. {result.reason}",
                _bits(HEADER_BITS),
                "отказ",
                reached=False,
            )
        )
    else:
        # Число кадров берётся из того же поля длины, которое показано в
        # таблице рядом: result.frames_expected на отказных путях остаётся
        # нулём, и шаг противоречил бы собственным полям.
        wanted = frames_required(result.header.message_length)
        expected_bits = (
            SYNC_MARKER_LENGTH + HEADER_CODED_BITS + wanted * FRAME_TOTAL_BITS
        )
        delta = len(bits) - expected_bits
        if delta == 0:
            balance = (
                "Принятая длина потока совпала с рассчитанной по заголовку."
            )
        else:
            # Причину расхождения приёмник назвать не может: длину меняет и
            # правка у пробелов, и обрезанный документ, и посторонние
            # служебные символы, и неверно прочитанное поле длины.
            balance = (
                f"По заголовку поток должен быть длиной "
                f"{_bits(expected_bits)}, принято {len(bits)}: расхождение "
                f"{delta:+d}. Откуда оно взялось, приёмнику не видно: так "
                "бывает, когда правка задела межсловные пробелы и число "
                "позиций встраивания изменилось, когда документ обрезан или "
                "из него вырезан кусок, когда в текст попали посторонние "
                "служебные символы — и когда само поле длины прочитано "
                "неверно."
            )
        if not result.frames and result.reason:
            balance += f" Дальше разбор не пошёл: {result.reason}"
        trace.stages.append(
            Stage(
                4,
                "Заголовок разобран",
                "Четыре поля отвечают на четыре вопроса: наш ли формат, "
                "сколько байт нести, с какой глубиной собран поток и с какой "
                "суммой должно совпасть сообщение. Отсюда известно, что "
                f"кадров ожидается {wanted}. {balance}",
                _bits(HEADER_BITS),
                "четыре поля",
                fields=_header_report_fields(result),
            )
        )

    # Шаг 5. Покадровый захват синхронизации.
    rows = _frame_rows(result)
    shown = (
        ""
        if len(rows) == len(result.frames)
        else f" Показано {len(rows)} строк из {len(result.frames)}."
    )
    lost = sum(
        1
        for item in result.frames
        if item.outcome is FrameOutcome.MARKER_LOST
    )
    cut = sum(
        1 for item in result.frames if item.outcome is FrameOutcome.TRUNCATED
    )
    moved = [item for item in result.frames if item.offset]
    if not result.frames:
        capture_detail = "Кадры не читались" + (
            f": {result.reason}" if result.reason else "."
        )
    elif lost or cut:
        # Перечисляем только то, что случилось: «оборвался внутри 0» —
        # бессмысленная строка.
        troubles = []
        if lost:
            troubles.append(f"маркер не найден у {_frames_of(lost)}")
        if cut:
            troubles.append(f"поток оборвался внутри {_frames_of(cut)}")
        capture_detail = (
            f"Принято кадров: {len(result.frames) - lost - cut} из "
            f"{len(result.frames)}; "
            + ", ".join(troubles)
            + ". Слова непринятых кадров помечены стёртыми без участия "
            "кода: декодировать было нечего."
            + _drift_note(result)
            + shown
        )
    elif moved:
        capture_detail = (
            _all_frames(len(result.frames), "захвачен", "захвачены")
            + f", причём у {_frames_of(len(moved))} маркер нашёлся не на "
            "расчётном месте. Отсчёт после каждой находки начинается "
            "заново, поэтому сдвиг не расползается на остаток сообщения."
            + _drift_note(result)
            + shown
        )
    else:
        capture_detail = (
            _all_frames(len(result.frames), "найден", "найдены")
            + (
                " на расчётном месте"
                if len(result.frames) == 1
                else " на расчётных местах"
            )
            + ", накопленного смещения нет. Приёмник не "
            "отсчитывает границы вслепую, а каждый раз отыскивает маркер "
            "заново — иначе одна правка в начале документа сбила бы разрез "
            "всего остатка." + shown
        )
    trace.stages.append(
        Stage(
            5,
            "Кадры найдены по маркерам",
            capture_detail,
            f"по заголовку {_frames(result.frames_expected)}",
            f"утрачено {result.frames_lost}",
            reached=bool(result.frames),
            bits=bits,
            spans=_stream_spans(result),
            legend=(
                (_MARKER, f"синхромаркер, {SYNC_MARKER_LENGTH} бит"),
                (_HEADER, "заголовок"),
                (_FRAME, "тело кадра"),
            ),
            headers=(
                "Кадр",
                "Ждали",
                "Нашли",
                "Смещение",
                "Несовпало",
                "Окно",
                "Исход",
            ),
            rows=rows,
        )
    )

    # Шаги 6 и 7. Разбор кадров: все сразу, с подписями.
    trace.stages.append(_channel_stage(result, trace.depth))
    trace.stages.append(_decoded_stage(result, trace.depth))

    # Шаг 8. Сборка байт и проверка суммы.
    #
    # Признак того, что до сверки дошло, — посчитанная сумма. По одному
    # лишь заголовку судить нельзя: он бывает прочитан, а разбор всё равно
    # обрывается раньше — например, когда глубина из него недопустима.
    if result.header is None or result.checksum_computed is None:
        why = result.reason or "заголовок не прочитан"
        trace.stages.append(
            Stage(
                8,
                "Собраны байты и проверена сумма",
                f"Шаг не выполнялся: {why}. Байты не собирались, сумма не "
                "сверялась — ни о совпадении, ни о расхождении речи нет.",
                "—",
                "—",
                reached=False,
            )
        )
        return trace

    carried = result.frames_expected * FRAME_DATA_BITS
    pad = carried - result.header.message_length * 8
    if result.success:
        verdict = (
            "Суммы совпали. Это лучший довод, которым приёмник располагает, "
            "но не доказательство: сумма шестнадцатиразрядная, и часть "
            "повреждений проходит через неё незамеченными, а сверка идёт с "
            "суммой, прочитанной из того же заголовка. Код отчитывается о "
            "состоянии отдельных слов, о сообщении в целом говорит сумма."
        )
    else:
        verdict = (
            "Суммы разошлись. Значит восстановленному доверять нельзя, но "
            "откуда взялось расхождение, сумма не сообщает: оно могло "
            "прийти и из утраченного кадра, и из ошибки такой кратности, "
            "какую код уже не различает, и из неверно прочитанного "
            "заголовка."
        )
    carries = _plural(
        carried,
        "информационный бит",
        "информационных бита",
        "информационных бит",
    )
    trace.stages.append(
        Stage(
            8,
            "Собраны байты и проверена сумма",
            f"Кадры несут {carries}, из них по "
            f"длине из заголовка оставлено "
            f"{_bits(result.header.message_length * 8)}, а {_bits(pad)} "
            f"дополнения отброшено. {verdict}",
            _bits(carried),
            _bytes(len(result.data)),
            fields=_checksum_fields(result),
        )
    )
    return trace
