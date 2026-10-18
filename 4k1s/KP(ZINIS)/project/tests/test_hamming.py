"""Проверка расширенного кода Хэмминга (8,4)."""

from itertools import product

import pytest

from textsteg.core.hamming import (
    D_MIN,
    RATE,
    BlockStatus,
    K,
    N,
    decode,
    decode_block,
    encode,
    encode_block,
)


def all_codewords() -> list[list[int]]:
    """Все 2^k = 16 кодовых слов кода."""
    return [encode_block(list(data)) for data in product((0, 1), repeat=K)]


def weight(word: list[int]) -> int:
    return sum(word)


def test_parameters() -> None:
    assert (N, K, D_MIN) == (8, 4, 4)
    assert RATE == 0.5


def test_codeword_length() -> None:
    for word in all_codewords():
        assert len(word) == N


def test_systematic_form() -> None:
    """Первые k бит кодового слова совпадают с информационными."""
    for data in product((0, 1), repeat=K):
        assert encode_block(list(data))[:K] == list(data)


def test_minimum_distance_is_four() -> None:
    """d_min кода равно минимальному весу ненулевого кодового слова.

    Именно это свойство обеспечивает режим SEC-DED: исправление одиночной
    ошибки с одновременным обнаружением двойной.
    """
    weights = [weight(word) for word in all_codewords() if any(word)]
    assert min(weights) == D_MIN


def test_no_error_detected_as_ok() -> None:
    for data in product((0, 1), repeat=K):
        result = decode_block(encode_block(list(data)))
        assert result.status is BlockStatus.OK
        assert result.data == list(data)


def test_single_error_corrected_in_every_position() -> None:
    """Одиночная ошибка исправляется в любой из n позиций кодового слова."""
    for data in product((0, 1), repeat=K):
        codeword = encode_block(list(data))
        for position in range(N):
            damaged = list(codeword)
            damaged[position] ^= 1
            result = decode_block(damaged)
            assert result.status is BlockStatus.CORRECTED
            assert result.data == list(data)


def test_double_error_detected_as_erasure() -> None:
    """Двойная ошибка распознаётся и не приводит к ложному исправлению.

    Классический код (7,4) с d_min = 3 в этой ситуации «исправил» бы третий,
    исправный бит и выдал неверные данные без каких-либо признаков отказа.
    """
    for data in product((0, 1), repeat=K):
        codeword = encode_block(list(data))
        for i in range(N):
            for j in range(i + 1, N):
                damaged = list(codeword)
                damaged[i] ^= 1
                damaged[j] ^= 1
                assert decode_block(damaged).status is BlockStatus.ERASED


def test_stream_roundtrip() -> None:
    bits = [1, 0, 1, 1, 0, 0, 1, 0, 1, 1, 1, 0]
    decoded, statuses = decode(encode(bits))
    assert decoded[: len(bits)] == bits
    assert all(s is BlockStatus.OK for s in statuses)


def test_encode_pads_incomplete_block() -> None:
    """Неполный информационный блок дополняется нулями."""
    assert len(encode([1, 0])) == N


def test_block_length_validated() -> None:
    with pytest.raises(ValueError):
        encode_block([1, 0, 1])
    with pytest.raises(ValueError):
        decode_block([1, 0, 1])


def test_is_reliable_reflects_status() -> None:
    """Признак достоверности блока согласован со статусом декодирования.

    По этому признаку вышележащие слои решают, можно ли использовать
    содержимое блока: стёртый блок достоверным не считается.
    """
    codeword = encode_block([1, 0, 1, 1])

    assert decode_block(codeword).is_reliable

    single = list(codeword)
    single[2] ^= 1
    assert decode_block(single).is_reliable

    double = list(codeword)
    double[2] ^= 1
    double[5] ^= 1
    assert not decode_block(double).is_reliable
