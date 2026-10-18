"""Проверка работы метода на повреждённом канале.

Эти испытания составляют содержательное ядро проверки: без них реализация
проходила бы все остальные тесты и при полностью отключённом перемежении,
поскольку на неповреждённом потоке перемежитель и помехоустойчивый код не
проявляют себя никак.

Повреждения вносятся непосредственно в битовый поток. Моделирование
воздействий на текст документа (смена оформления, редактирование,
пересохранение) — предмет отдельного модуля и здесь не рассматривается.
"""

import pytest

from textsteg.api import embed
from textsteg.core.bits import Bits
from textsteg.core.hamming import BlockStatus
from textsteg.core.pipeline import (
    ALLOWED_DEPTHS,
    FRAME_PAYLOAD_BITS,
    FRAME_TOTAL_BITS,
    HEADER_CODED_BITS,
    PipelineConfig,
    parse_stream,
)
from textsteg.embedding import ZeroWidthEmbedder

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


def stream_of(embedder: ZeroWidthEmbedder, depth: int) -> Bits:
    """Битовый поток, извлечённый из стеготекста заданного режима."""
    result = embed(CONTAINER, MESSAGE, embedder, PipelineConfig(depth=depth))
    return embedder.extract(result.stego)


def flip_burst(bits: Bits, start: int, length: int) -> Bits:
    """Инвертирует пакет из length подряд идущих бит."""
    damaged = list(bits)
    for i in range(start, start + length):
        damaged[i] ^= 1
    return damaged


# --------------------------------------------------------------------------
# Одиночные ошибки: работа помехоустойчивого кода
# --------------------------------------------------------------------------

@pytest.mark.parametrize("depth", [1, 8, 32])
def test_single_error_per_codeword_is_corrected(
    embedder: ZeroWidthEmbedder, depth: int
) -> None:
    """Одиночная ошибка в кодовом слове исправляется при любой глубине.

    Ошибки расставлены по одной на блок перемежителя (d * 8 бит канала),
    поэтому после деперемежения в каждом кодовом слове оказывается не более
    одной ошибки независимо от глубины. Перемежение здесь роли не играет:
    одиночную ошибку код исправляет сам.
    """
    bits = stream_of(embedder, depth)
    block_size = depth * 8
    damaged = list(bits)
    for block_start in range(
        FIRST_FRAME, FIRST_FRAME + FRAME_PAYLOAD_BITS, block_size
    ):
        damaged[block_start] ^= 1

    result = parse_stream(damaged)
    assert result.success
    assert result.message == MESSAGE
    assert result.corrected_blocks > 0


def test_double_error_in_one_codeword_is_erased_not_miscorrected(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Двойная ошибка помечается стиранием, а не исправляется ложно.

    Это и есть практическое следствие выбора d_min = 4.
    """
    bits = stream_of(embedder, 1)
    damaged = flip_burst(bits, FIRST_FRAME, 2)

    result = parse_stream(damaged)
    assert not result.success
    assert result.erased_blocks >= 1


# --------------------------------------------------------------------------
# Пакетные ошибки: ради чего применяется перемежение
# --------------------------------------------------------------------------

def test_burst_breaks_message_without_interleaving(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Без перемежения пакет из четырёх бит уничтожает сообщение.

    При d = 1 все четыре ошибки попадают в одно кодовое слово, а код
    исправляет только одну.
    """
    bits = stream_of(embedder, 1)
    result = parse_stream(flip_burst(bits, FIRST_FRAME, 4))

    assert not result.success
    assert result.message != MESSAGE


@pytest.mark.parametrize("depth", [8, 16, 32])
def test_interleaving_recovers_the_same_burst(
    embedder: ZeroWidthEmbedder, depth: int
) -> None:
    """Тот же пакет при перемежении восстанавливается полностью.

    Центральное утверждение работы: перемежитель распределяет пакетную
    ошибку по разным кодовым словам, превращая её в набор одиночных.
    """
    bits = stream_of(embedder, depth)
    result = parse_stream(flip_burst(bits, FIRST_FRAME, 4))

    assert result.success, f"глубина {depth}: сообщение не восстановлено"
    assert result.message == MESSAGE


@pytest.mark.parametrize("depth", ALLOWED_DEPTHS)
def test_burst_up_to_b_max_is_recovered(
    embedder: ZeroWidthEmbedder, depth: int
) -> None:
    """Пакет длиной B_max = d * t исправляется при любом его положении.

    Проверяются все положения пакета внутри первого кадра, а не только
    начало кадра: гарантия заявлена как безусловная. Это центральное
    утверждение работы, и подтверждать его на защите нужно на собранном
    конвейере, а не на перемежителе в отрыве от остальных слоёв.
    """
    bits = stream_of(embedder, depth)
    for offset in range(0, FRAME_PAYLOAD_BITS - depth + 1, depth):
        damaged = flip_burst(bits, FIRST_FRAME + offset, depth)
        result = parse_stream(damaged)
        assert result.success, (
            f"глубина {depth}, пакет длиной {depth} со смещения {offset} "
            "не восстановлен"
        )


@pytest.mark.parametrize("depth", ALLOWED_DEPTHS)
def test_burst_beyond_b_max_is_not_recovered(
    embedder: ZeroWidthEmbedder, depth: int
) -> None:
    """Оценка B_max = d * t точная: пакет длиной d + 1 уже не исправляется.

    Без этой проверки утверждение оставалось бы односторонним — доказывалась
    бы достаточность глубины, но не тесность границы.
    """
    bits = stream_of(embedder, depth)
    result = parse_stream(flip_burst(bits, FIRST_FRAME, depth + 1))

    assert not result.success, (
        f"глубина {depth}: пакет длиной {depth + 1} не должен исправляться"
    )


def test_deeper_interleaving_survives_longer_burst(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Предельная длина исправляемого пакета растёт вместе с глубиной.

    Пакет в 16 бит непосилен для d = 4, но восстанавливается при d = 16.
    """
    shallow = parse_stream(flip_burst(stream_of(embedder, 4), FIRST_FRAME, 16))
    deep = parse_stream(flip_burst(stream_of(embedder, 16), FIRST_FRAME, 16))

    assert not shallow.success
    assert deep.success
    assert deep.message == MESSAGE


# --------------------------------------------------------------------------
# Потеря кадра и целостность
# --------------------------------------------------------------------------

def test_lost_frame_is_reported_as_erased(embedder: ZeroWidthEmbedder) -> None:
    """Утраченный кадр помечается стиранием, а не «ошибок нет».

    Нулевая последовательность является допустимым кодовым словом, поэтому
    заполнение потерянного кадра нулями с последующим декодированием дало бы
    статус OK и завысило бы показатели работы кода.
    """
    bits = stream_of(embedder, 8)
    # Уничтожаем маркер первого кадра и его содержимое.
    damaged = flip_burst(bits, FIRST_FRAME - 16, 16 + FRAME_PAYLOAD_BITS)

    result = parse_stream(damaged)
    assert not result.success
    assert result.frames_lost >= 1
    assert result.erased_blocks >= 32
    assert BlockStatus.OK not in result.block_statuses[:32]


def test_checksum_detects_corruption(embedder: ZeroWidthEmbedder) -> None:
    """Проверка целостности срабатывает на повреждённом сообщении.

    Без этого испытания замена признака совпадения контрольной суммы на
    безусловную истину осталась бы незамеченной.
    """
    bits = stream_of(embedder, 1)
    result = parse_stream(flip_burst(bits, FIRST_FRAME, 3))

    assert not result.success
    assert result.message != MESSAGE
    assert "сумма" in result.reason or "кадров" in result.reason


# --------------------------------------------------------------------------
# Повреждение заголовка
# --------------------------------------------------------------------------

def test_erased_header_block_reports_header_not_checksum(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Стирание в заголовке объясняется пользователю как порча заголовка.

    Ранее статусы декодирования заголовка отбрасывались, и верно
    восстановленное сообщение получало заключение «контрольная сумма не
    совпала», не соответствующее действительности.
    """
    bits = stream_of(embedder, 8)
    # Двойная ошибка в кодовом слове поля контрольной суммы заголовка.
    damaged = flip_burst(bits, 16 + 8 * 8, 2)

    result = parse_stream(damaged)
    assert not result.success
    assert "заголовок" in result.reason


def test_destroyed_header_is_reported(embedder: ZeroWidthEmbedder) -> None:
    """Полностью разрушенный заголовок не приводит к аварийному завершению."""
    bits = stream_of(embedder, 8)
    damaged = flip_burst(bits, 16, HEADER_CODED_BITS)

    result = parse_stream(damaged)
    assert not result.success
    assert result.message == ""


# --------------------------------------------------------------------------
# Смещение потока: работа синхромаркеров
# --------------------------------------------------------------------------

#: Граница между первым и вторым кадром: сюда приходится маркер кадра 2.
SECOND_FRAME_MARKER = FIRST_FRAME + FRAME_PAYLOAD_BITS


def test_resynchronization_after_inserted_bits(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Вставка лишних бит компенсируется поиском маркера в окне.

    Моделирует появление в документе новых позиций встраивания, например при
    дописывании слов. Биты вставляются перед маркером второго кадра: первый
    кадр остаётся целым, а маркеры последующих кадров смещаются, и приёмник
    обязан найти их заново.
    """
    bits = stream_of(embedder, 8)
    shifted = (
        bits[:SECOND_FRAME_MARKER] + [0, 0] + bits[SECOND_FRAME_MARKER:]
    )

    result = parse_stream(shifted)
    assert result.success
    assert result.message == MESSAGE


def test_resynchronization_after_deleted_bits(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Удаление бит не распространяется на последующие кадры.

    Моделирует исчезновение позиций встраивания при удалении слов. Удаление
    неизбежно портит тот кадр, внутри которого произошло, — восстановить
    утраченные биты нечем. Существенно другое: смещение не должно уводить
    приёмник с границ всех последующих кадров. Маркеры обязаны быть найдены
    заново, а число утраченных кадров — остаться нулевым.
    """
    bits = stream_of(embedder, 8)
    cut = FIRST_FRAME + 40
    shifted = bits[:cut] + bits[cut + 2:]

    result = parse_stream(shifted)
    assert result.frames_lost == 0, "смещение увело приёмник с границ кадров"

    strict = parse_stream(shifted, PipelineConfig(depth=8, search_window=0))
    assert strict.frames_lost > 0, (
        "без поиска в окне смещённые кадры должны теряться — иначе "
        "предыдущая проверка ничего не доказывает"
    )


def test_large_drift_does_not_destroy_remaining_frames(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Смещение больше окна не уничтожает весь остаток сообщения.

    Удаление крупного фрагмента (несколько строк текста) уводит границы
    кадров за пределы обычного окна поиска. После пропуска окно расширяется,
    и приёмник заново захватывает синхронизацию: теряются кадры в области
    повреждения, а не все последующие.
    """
    bits = stream_of(embedder, 8)
    cut = FIRST_FRAME + 20
    for cut_length in (70, 100, 150, 250):
        damaged = bits[:cut] + bits[cut + cut_length:]
        result = parse_stream(damaged)
        assert result.frames_lost < result.frames_expected, (
            f"удаление {cut_length} бит потеряло все "
            f"{result.frames_expected} кадров — повторного захвата "
            "синхронизации не произошло"
        )


def test_marker_search_is_what_recovers_the_shift(
    embedder: ZeroWidthEmbedder,
) -> None:
    """Восстановление обеспечено именно поиском, а не совпадением.

    При нулевом окне поиска сдвинутый поток перестаёт читаться. Без этой
    проверки поиск маркера можно было бы свести к сравнению на ожидаемой
    позиции, и предыдущие испытания этого не заметили бы.
    """
    bits = stream_of(embedder, 8)
    shifted = (
        bits[:SECOND_FRAME_MARKER] + [0, 0] + bits[SECOND_FRAME_MARKER:]
    )

    result = parse_stream(shifted, PipelineConfig(depth=8, search_window=0))
    assert not result.success


def test_frame_geometry_constants() -> None:
    """Раскладка потока, от которой отсчитываются позиции повреждений."""
    assert FRAME_TOTAL_BITS == 16 + FRAME_PAYLOAD_BITS
    assert FIRST_FRAME == 16 + HEADER_CODED_BITS + 16
