r"""Сборка конвейера встраивания и извлечения.

Модуль соединяет все слои метода в единую последовательность операций:

    сообщение -> байты -> заголовок + CRC -> помехоустойчивый код
              -> перемежение -> кадрирование маркерами -> встраивание

и выполняет обратное преобразование при извлечении.

Организация кадров
------------------
Поток разбивается на кадры постоянного размера — по FRAME_CODEWORDS кодовых
слов в каждом. Перед кадром размещается синхромаркер.

Размер кадра намеренно не зависит от глубины перемежения. Если бы маркер
ставился перед каждым блоком перемежителя, накладные расходы на
синхронизацию оказались бы разными для разных d: при d = 1 блок состоит
всего из одного кодового слова (8 бит), и маркер длиной 16 бит занимал бы
две трети канала, тогда как при d = 32 — лишь шесть процентов. Сравнение
режимов в таком виде было бы некорректным: различие кривых объяснялось бы
не работой перемежителя, а разной долей служебных бит.

При постоянном размере кадра доля служебных бит одинакова для всех режимов,
и единственной изменяемой величиной остаётся глубина перемежения.

Внутри кадра кодовые слова группируются по d штук, и каждая группа
перемежается независимо. При d = 1 перемежение вырождается в тождественное
преобразование, при d = FRAME_CODEWORDS кадр целиком образует одну матрицу.

    [маркер][заголовок: код, без перемежения]
    [маркер][кадр 1][маркер][кадр 2]...[маркер][кадр M]
             \_____/
             FRAME_CODEWORDS кодовых слов, перемеженных группами по d
"""

from __future__ import annotations

from dataclasses import dataclass, field

from ..errors import HeaderError, MessageTooLongError, PipelineError
from . import hamming
from .bits import Bits, bits_to_bytes, bytes_to_bits
from .framing import (
    HEADER_BITS,
    MARKER_TOLERANCE,
    MAX_MESSAGE_BYTES,
    SYNC_MARKER,
    SYNC_MARKER_LENGTH,
    Header,
    crc16,
    find_marker,
)
from .hamming import BlockStatus
from .interleaver import BlockInterleaver

#: Число кодовых слов в одном кадре.
FRAME_CODEWORDS = 32

#: Полезная ёмкость кадра в битах канала: 32 слова по 8 бит.
FRAME_PAYLOAD_BITS = FRAME_CODEWORDS * hamming.N

#: Объём информационных бит, помещающихся в один кадр.
FRAME_DATA_BITS = FRAME_CODEWORDS * hamming.K

#: Полная длина кадра вместе с маркером.
FRAME_TOTAL_BITS = SYNC_MARKER_LENGTH + FRAME_PAYLOAD_BITS

#: Длина закодированного заголовка: 48 информационных бит -> 96 бит канала.
HEADER_CODED_BITS = HEADER_BITS // hamming.K * hamming.N

#: Глубины перемежения, допустимые при выбранном размере кадра. Глубина
#: должна быть делителем числа кодовых слов в кадре, иначе последняя группа
#: окажется неполной.
ALLOWED_DEPTHS: tuple[int, ...] = tuple(
    d for d in (1, 2, 4, 8, 16, 32) if FRAME_CODEWORDS % d == 0
)

#: Ширина окна поиска маркера по умолчанию. Задаёт допустимое смещение,
#: накопленное из-за вставок и удалений внутри одного кадра.
DEFAULT_SEARCH_WINDOW = FRAME_TOTAL_BITS // 4

#: Предельная ширина окна при повторном захвате синхронизации.
#:
#: После каждого кадра, маркер которого найти не удалось, окно расширяется:
#: пропуск обычно означает, что смещение вышло за пределы обычного окна, и
#: продолжать поиск в прежних границах бессмысленно. Расширение ограничено
#: длиной кадра, поскольку вероятность принять случайный участок нагрузки за
#: маркер растёт пропорционально числу просматриваемых положений.
MAX_RESYNC_WINDOW = FRAME_TOTAL_BITS

#: Кодировка секретного сообщения.
MESSAGE_ENCODING = "utf-8"


@dataclass(frozen=True)
class PipelineConfig:
    """Параметры конвейера.

    Атрибуты:
        depth — глубина перемежения; 1 означает отсутствие перемежения;
        search_window — ширина окна поиска синхромаркера в битах;
        marker_tolerance — допустимое число несовпавших бит маркера.
    """

    depth: int = 8
    search_window: int = DEFAULT_SEARCH_WINDOW
    marker_tolerance: int = MARKER_TOLERANCE

    def __post_init__(self) -> None:
        if self.depth not in ALLOWED_DEPTHS:
            raise PipelineError(
                f"глубина перемежения {self.depth} недопустима; "
                f"возможные значения: {ALLOWED_DEPTHS}"
            )

    @property
    def interleaver(self) -> BlockInterleaver:
        """Перемежитель, соответствующий настройке."""
        return BlockInterleaver(self.depth)

    @property
    def groups_per_frame(self) -> int:
        """Число независимо перемежаемых групп в кадре."""
        return FRAME_CODEWORDS // self.depth


@dataclass(frozen=True)
class EmbedResult:
    """Итог встраивания сообщения в контейнер."""

    stego: str
    message_bytes: int
    stream_bits: int
    capacity_bits: int
    frames: int

    @property
    def utilization(self) -> float:
        """Доля использованной вместимости контейнера."""
        return self.stream_bits / self.capacity_bits if self.capacity_bits else 0.0

    @property
    def overhead(self) -> float:
        """Доля бит канала, не занятых полезной нагрузкой.

        В величину входят четыре составляющие: избыточность помехоустойчивого
        кода (половина канала при R = 0,5), заголовок, синхромаркеры и
        дополнение сообщения до целого числа кадров. Последняя составляющая
        на коротких сообщениях преобладает над остальными: сообщение в один
        байт занимает столько же бит канала, сколько сообщение в шестнадцать.
        """
        payload = self.message_bytes * 8
        return 1.0 - payload / self.stream_bits if self.stream_bits else 0.0


@dataclass(frozen=True)
class ExtractResult:
    """Итог извлечения сообщения из стеготекста."""

    success: bool
    message: str
    reason: str = ""
    header: Header | None = None
    block_statuses: list[BlockStatus] = field(default_factory=list)
    frames_expected: int = 0
    frames_lost: int = 0

    @property
    def corrected_blocks(self) -> int:
        """Число блоков, в которых декодер выполнил исправление.

        Как правило это блоки с одиночной ошибкой. Блоки с тремя и более
        ошибками декодер от них не отличает — для кода с d_min = 4 такие
        кратности лежат вне корректирующей способности, и решение о
        «исправлении» принимается ошибочно. Приёмник не располагает сведениями,
        позволяющими их различить; окончательное заключение о целостности
        сообщения даёт контрольная сумма заголовка.
        """
        return sum(1 for s in self.block_statuses if s is BlockStatus.CORRECTED)

    @property
    def erased_blocks(self) -> int:
        """Число блоков, признанных невосстановимыми."""
        return sum(1 for s in self.block_statuses if s is BlockStatus.ERASED)


# --------------------------------------------------------------------------
# Вспомогательные операции над кадрами
# --------------------------------------------------------------------------

def _pad_to(bits: Bits, multiple: int) -> Bits:
    """Дополняет последовательность нулями до кратной длины."""
    padded = list(bits)
    remainder = len(padded) % multiple
    if remainder:
        padded += [0] * (multiple - remainder)
    return padded


def frames_required(message_bytes: int) -> int:
    """Число кадров, необходимое для сообщения заданной длины."""
    data_bits = message_bytes * 8
    return max(1, -(-data_bits // FRAME_DATA_BITS))


def check_message_length(message: str) -> int:
    """Проверяет допустимость длины сообщения и возвращает её в байтах.

    Поле длины заголовка занимает 16 бит, поэтому сообщение длиннее
    MAX_MESSAGE_BYTES непредставимо независимо от вместимости контейнера.
    Проверка вынесена в отдельную функцию, чтобы интерфейс получал отказ при
    первом же обращении — в том числе при оценке требуемой вместимости, — а
    не в глубине сборки потока.
    """
    message_bytes = len(message.encode(MESSAGE_ENCODING))
    if message_bytes > MAX_MESSAGE_BYTES:
        raise MessageTooLongError(
            f"длина сообщения {message_bytes} байт превышает предельные "
            f"{MAX_MESSAGE_BYTES} байт, представимые полем длины заголовка"
        )
    return message_bytes


def required_capacity(message: str) -> int:
    """Вместимость контейнера, необходимая для встраивания сообщения.

    Позволяет предупредить пользователя о нехватке места до начала
    встраивания, а не в момент отказа.

    От глубины перемежения величина не зависит: перемежитель переставляет
    биты, не добавляя избыточности, а размер кадра постоянен для всех
    режимов.
    """
    message_bytes = check_message_length(message)
    frames = frames_required(message_bytes)
    return (
        SYNC_MARKER_LENGTH
        + HEADER_CODED_BITS
        + frames * FRAME_TOTAL_BITS
    )


def _interleave_frame(frame: Bits, config: PipelineConfig) -> Bits:
    """Перемежает кадр группами по d кодовых слов."""
    interleaver = config.interleaver
    size = interleaver.block_size
    return [
        bit
        for group in range(config.groups_per_frame)
        for bit in interleaver.interleave(
            frame[group * size:(group + 1) * size]
        )
    ]


def _deinterleave_frame(frame: Bits, config: PipelineConfig) -> Bits:
    """Восстанавливает исходный порядок бит в кадре."""
    interleaver = config.interleaver
    size = interleaver.block_size
    return [
        bit
        for group in range(config.groups_per_frame)
        for bit in interleaver.deinterleave(
            frame[group * size:(group + 1) * size]
        )
    ]


# --------------------------------------------------------------------------
# Встраивание
# --------------------------------------------------------------------------

def build_stream(message: str, config: PipelineConfig) -> Bits:
    """Собирает битовый поток, подлежащий встраиванию в контейнер.

    Порядок операций:
        1) сообщение переводится в байты, вычисляется CRC-16;
        2) формируется заголовок и кодируется без перемежения;
        3) информационные биты дополняются до целого числа кадров;
        4) выполняется помехоустойчивое кодирование;
        5) каждый кадр перемежается группами по d кодовых слов;
        6) перед заголовком и каждым кадром размещается синхромаркер.
    """
    check_message_length(message)

    data = message.encode(MESSAGE_ENCODING)
    header = Header(
        message_length=len(data),
        depth=config.depth,
        checksum=crc16(data),
    )

    # Заголовок кодируется, но не перемежается: без него неизвестна
    # глубина d, необходимая для работы деперемежителя.
    stream: Bits = list(SYNC_MARKER) + hamming.encode(header.to_bits())

    # Пустое сообщение тоже занимает один кадр: приёмник рассчитывает число
    # кадров по длине из заголовка и всегда ожидает хотя бы один. Без этого
    # дополнения поток оборвался бы на заголовке, а приёмник продолжал бы
    # искать кадр, которого нет.
    data_bits = _pad_to(bytes_to_bits(data), FRAME_DATA_BITS)
    if not data_bits:
        data_bits = [0] * FRAME_DATA_BITS

    for start in range(0, len(data_bits), FRAME_DATA_BITS):
        frame = hamming.encode(data_bits[start:start + FRAME_DATA_BITS])
        stream.extend(SYNC_MARKER)
        stream.extend(_interleave_frame(frame, config))

    return stream


# --------------------------------------------------------------------------
# Извлечение
# --------------------------------------------------------------------------

def parse_stream(bits: Bits, config: PipelineConfig | None = None) -> ExtractResult:
    """Восстанавливает сообщение из извлечённого битового потока.

    Глубина перемежения берётся из заголовка, а не из настройки: получатель
    не обязан знать параметры, с которыми выполнялось встраивание.
    """
    settings = config or PipelineConfig()
    window = settings.search_window
    tolerance = settings.marker_tolerance

    # Шаг 1. Поиск первого маркера и разбор заголовка.
    position = find_marker(bits, expected=0, window=window, tolerance=tolerance)
    if position is None:
        return ExtractResult(
            success=False,
            message="",
            reason="синхромаркер не найден: в тексте нет скрытого сообщения "
            "либо канал уничтожен",
        )

    cursor = position + SYNC_MARKER_LENGTH
    header_bits, header_statuses = hamming.decode(
        bits[cursor:cursor + HEADER_CODED_BITS]
    )

    # Стирание в заголовке учитывается до разбора полей. Ради этого признака
    # и выбран код с d_min = 4: без него стёртое поле контрольной суммы было
    # бы принято за достоверное, и верно восстановленное сообщение получило
    # бы заключение «контрольная сумма не совпала», не соответствующее
    # действительности.
    erased = [
        index
        for index, status in enumerate(header_statuses)
        if status is BlockStatus.ERASED
    ]
    if erased:
        return ExtractResult(
            success=False,
            message="",
            reason=(
                "заголовок повреждён невосстановимо: "
                f"стёрто блоков — {len(erased)} (номера: "
                + ", ".join(str(i) for i in erased)
                + "). Извлечение невозможно"
            ),
            block_statuses=header_statuses,
        )

    try:
        header = Header.from_bits(header_bits)
    except HeaderError as error:
        return ExtractResult(
            success=False,
            message="",
            reason=f"заголовок не восстановлен: {error}",
            block_statuses=header_statuses,
        )

    # Шаг 2. Настройка деперемежителя по глубине из заголовка.
    try:
        frame_config = PipelineConfig(
            depth=header.depth,
            search_window=window,
            marker_tolerance=tolerance,
        )
    except PipelineError as error:
        return ExtractResult(
            success=False, message="", reason=str(error), header=header
        )

    # Шаг 3. Покадровое чтение с повторной синхронизацией на каждом кадре.
    cursor += HEADER_CODED_BITS
    total_frames = frames_required(header.message_length)
    frames: list[Bits | None] = []

    # Число подряд идущих кадров, маркер которых найти не удалось. Служит
    # признаком того, что накопленное смещение вышло за пределы окна поиска.
    misses = 0

    for _ in range(total_frames):
        # Ненайденный маркер означает, как правило, что смещение превысило
        # окно: приёмник ищет границу там, где её уже нет. Отсчёт вслепую
        # смещение не компенсирует, поэтому после каждого промаха окно
        # расширяется, давая возможность заново захватить синхронизацию.
        # Расширение ограничено: чем шире окно, тем выше вероятность принять
        # за маркер случайный участок полезной нагрузки.
        search = min(window * (misses + 1), MAX_RESYNC_WINDOW)

        marker_at = find_marker(
            bits, expected=cursor, window=search, tolerance=tolerance
        )
        if marker_at is None:
            # Кадр утрачен. Продолжаем со смещением на номинальную длину
            # кадра — это наилучшая доступная оценка положения следующего.
            frames.append(None)
            cursor += FRAME_TOTAL_BITS
            misses += 1
            continue

        misses = 0

        start = marker_at + SYNC_MARKER_LENGTH
        frame = bits[start:start + FRAME_PAYLOAD_BITS]
        if len(frame) < FRAME_PAYLOAD_BITS:
            # Поток оборвался внутри кадра: восстановить его нечем.
            frames.append(None)
            cursor = start + FRAME_PAYLOAD_BITS
            continue

        frames.append(_deinterleave_frame(frame, frame_config))
        cursor = start + FRAME_PAYLOAD_BITS

    # Шаг 4. Декодирование и проверка целостности.
    #
    # Утраченный кадр не декодируется: нулевая последовательность является
    # допустимым кодовым словом, и декодер вернул бы для неё статус «ошибок
    # нет», завышая показатели работы кода в экспериментальных исследованиях.
    # Блоки такого кадра сразу помечаются стёртыми, а место в потоке данных
    # заполняется нулями, чтобы сохранить позиции последующих кадров.
    data_bits: Bits = []
    statuses: list[BlockStatus] = []
    frames_lost = 0

    for received in frames:
        if received is None:
            data_bits.extend([0] * FRAME_DATA_BITS)
            statuses.extend([BlockStatus.ERASED] * FRAME_CODEWORDS)
            frames_lost += 1
            continue
        frame_data, frame_statuses = hamming.decode(received)
        data_bits.extend(frame_data)
        statuses.extend(frame_statuses)

    data = bits_to_bytes(data_bits)[: header.message_length]
    message = data.decode(MESSAGE_ENCODING, errors="replace")
    checksum_ok = crc16(data) == header.checksum

    if checksum_ok:
        reason = ""
    elif frames_lost:
        reason = (
            f"утрачено кадров: {frames_lost} из {total_frames}; "
            "сообщение восстановлено не полностью"
        )
    else:
        reason = (
            "контрольная сумма не совпала: сообщение восстановлено "
            "не полностью"
        )

    return ExtractResult(
        success=checksum_ok,
        message=message,
        reason=reason,
        header=header,
        block_statuses=statuses,
        frames_expected=total_frames,
        frames_lost=frames_lost,
    )


