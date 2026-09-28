"""Проверка битовых преобразований."""

import pytest

from textsteg.core.bits import (
    bits_to_bytes,
    bits_to_int,
    bits_to_text,
    bytes_to_bits,
    hamming_distance,
    int_to_bits,
    text_to_bits,
)


def test_bytes_roundtrip() -> None:
    data = bytes(range(256))
    assert bits_to_bytes(bytes_to_bits(data)) == data


def test_msb_first_order() -> None:
    """Порядок бит — старший первым; он одинаков у кодера и декодера."""
    assert bytes_to_bits(b"\x01") == [0, 0, 0, 0, 0, 0, 0, 1]
    assert bytes_to_bits(b"\x80") == [1, 0, 0, 0, 0, 0, 0, 0]


def test_incomplete_byte_is_padded() -> None:
    """Повреждённый поток может оборваться посреди байта."""
    assert bits_to_bytes([1, 0, 1]) == b"\xa0"


def test_int_roundtrip() -> None:
    for value in (0, 1, 255, 4096, 65535):
        assert bits_to_int(int_to_bits(value, 16)) == value


def test_int_width_validated() -> None:
    with pytest.raises(ValueError):
        int_to_bits(256, 8)


def test_text_roundtrip_cyrillic() -> None:
    message = "Копия № 17, отдел кадров"
    assert bits_to_text(text_to_bits(message)) == message


def test_text_decoding_survives_damage() -> None:
    """Повреждённая последовательность не должна вызывать исключение."""
    bits = text_to_bits("Копия")
    bits[2] ^= 1
    assert isinstance(bits_to_text(bits), str)


def test_hamming_distance_counts_mismatches() -> None:
    assert hamming_distance([0, 1, 0], [0, 0, 0]) == 1


def test_hamming_distance_counts_missing_bits() -> None:
    """Утраченные биты засчитываются как различие целиком."""
    assert hamming_distance([0, 1, 0, 1], [0, 1]) == 2
