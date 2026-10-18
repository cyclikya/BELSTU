"""Проверка заголовка, контрольной суммы и поиска синхромаркеров."""

import pytest

from textsteg.core.bits import int_to_bits
from textsteg.core.framing import (
    HEADER_BITS,
    MARKER_TOLERANCE,
    SYNC_MARKER,
    SYNC_MARKER_LENGTH,
    Header,
    HeaderError,
    crc16,
    find_marker,
    marker_distance,
)

# --------------------------------------------------------------------------
# Контрольная сумма
# --------------------------------------------------------------------------

def test_crc16_reference_vector() -> None:
    """Контрольное значение CRC-16/CCITT-FALSE для строки "123456789"."""
    assert crc16(b"123456789") == 0x29B1


def test_crc16_detects_single_bit_change() -> None:
    assert crc16("Копия 17".encode()) != crc16("Копия 18".encode())


# --------------------------------------------------------------------------
# Заголовок
# --------------------------------------------------------------------------

def test_header_roundtrip() -> None:
    header = Header(message_length=1234, depth=16, checksum=0xBEEF)
    assert Header.from_bits(header.to_bits()) == header


def test_header_length_is_48_bits() -> None:
    assert HEADER_BITS == 48
    assert len(Header(message_length=1, depth=1, checksum=0).to_bits()) == 48


def test_header_rejects_unknown_signature() -> None:
    corrupted = int_to_bits(0x00, 8) + [0] * (HEADER_BITS - 8)
    with pytest.raises(HeaderError, match="сигнатура"):
        Header.from_bits(corrupted)


def test_header_rejects_oversized_message() -> None:
    with pytest.raises(HeaderError, match="длина"):
        Header(message_length=1 << 20, depth=8, checksum=0).to_bits()


def test_header_rejects_invalid_depth() -> None:
    with pytest.raises(HeaderError, match="глубина"):
        Header(message_length=10, depth=0, checksum=0).to_bits()


# --------------------------------------------------------------------------
# Синхромаркер
# --------------------------------------------------------------------------

def test_marker_is_balanced() -> None:
    """Равное число нулей и единиц отличает маркер от однородных участков."""
    assert sum(SYNC_MARKER) == SYNC_MARKER_LENGTH // 2


def sidelobe(bits: list[int]) -> int:
    """Максимум модуля апериодической автокорреляции при ненулевом сдвиге.

    Последовательность рассматривается в форме +-1: совпадение бит даёт +1,
    несовпадение -1.
    """
    return max(
        abs(
            sum(
                1 if x == y else -1
                for x, y in zip(
                    bits[shift:], bits[: len(bits) - shift], strict=True
                )
            )
        )
        for shift in range(1, len(bits))
    )


def test_marker_sidelobe_is_minimal() -> None:
    """Боковой лепесток автокорреляции маркера равен заявленным трём.

    Три — наименьшее достижимое значение среди сбалансированных
    шестнадцатиразрядных последовательностей; проверка подтверждает, что
    величина в комментарии к SYNC_MARKER_VALUE соответствует выбранной
    комбинации.
    """
    assert sidelobe(SYNC_MARKER) == 3


def test_marker_autocorrelation_is_low() -> None:
    """Маркер не опознаётся в собственных сдвигах.

    Проверяются сдвиги, при которых перекрытие составляет не менее половины
    длины маркера: именно они способны вызвать ложную синхронизацию. При
    больших сдвигах перекрытие мало, и окно сравнения заполняется соседними
    битами полезной нагрузки, а не сдвинутой копией маркера.
    """
    for shift in range(1, SYNC_MARKER_LENGTH // 2 + 1):
        overlap = SYNC_MARKER_LENGTH - shift
        mismatches = sum(
            1
            for a, b in zip(SYNC_MARKER[shift:], SYNC_MARKER[:overlap], strict=True)
            if a != b
        )
        assert mismatches > MARKER_TOLERANCE, (
            f"сдвиг {shift}: всего {mismatches} несовпадений при допуске "
            f"{MARKER_TOLERANCE}"
        )


def test_marker_found_at_exact_position() -> None:
    stream = [0] * 40 + list(SYNC_MARKER) + [1] * 40
    assert find_marker(stream, expected=40, window=8) == 40


def test_marker_found_after_shift() -> None:
    """Смещение, вызванное удалением символов, компенсируется поиском в окне."""
    stream = [0] * 37 + list(SYNC_MARKER) + [1] * 40
    assert find_marker(stream, expected=40, window=8) == 37


def test_marker_found_with_allowed_errors() -> None:
    """Сам маркер тоже повреждается, поэтому поиск ведётся с допуском."""
    damaged = list(SYNC_MARKER)
    damaged[3] ^= 1
    stream = [0] * 40 + damaged + [1] * 40
    assert find_marker(stream, expected=40, window=8) == 40


def test_marker_not_found_beyond_tolerance() -> None:
    """Превышение допуска означает отказ от синхронизации."""
    damaged = list(SYNC_MARKER)
    for i in range(MARKER_TOLERANCE + 1):
        damaged[i * 3] ^= 1
    stream = [0] * 40 + damaged + [1] * 40
    assert find_marker(stream, expected=40, window=8) is None


def test_marker_not_found_outside_window() -> None:
    """Поиск ограничен окном: глобальный поиск давал бы ложные срабатывания."""
    stream = [0] * 100 + list(SYNC_MARKER) + [1] * 20
    assert find_marker(stream, expected=40, window=8) is None


def test_marker_distance_at_end_of_stream() -> None:
    """Обрыв потока не должен приводить к исключению."""
    assert marker_distance([0, 1, 0], 0) == SYNC_MARKER_LENGTH
