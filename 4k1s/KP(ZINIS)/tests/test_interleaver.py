"""Проверка блочного перемежителя."""

import pytest

from textsteg.core.hamming import N, T
from textsteg.core.interleaver import STUDIED_DEPTHS, BlockInterleaver


@pytest.mark.parametrize("depth", STUDIED_DEPTHS)
def test_roundtrip(depth: int) -> None:
    interleaver = BlockInterleaver(depth)
    block = [i % 2 for i in range(interleaver.block_size)]
    assert interleaver.deinterleave(interleaver.interleave(block)) == block


def test_identity_at_depth_one() -> None:
    """Глубина 1 означает отсутствие перемежения."""
    interleaver = BlockInterleaver(1)
    assert interleaver.is_identity
    block = [1, 0, 1, 1, 0, 0, 1, 0]
    assert interleaver.interleave(block) == block


@pytest.mark.parametrize("depth", [4, 8, 16, 32])
def test_burst_is_spread_across_codewords(depth: int) -> None:
    """Пакет длиной B_max = d * t оставляет не более t ошибок в слове.

    Это ключевое свойство перемежения, ради которого оно применяется:
    пакетная ошибка превращается в набор одиночных, исправимых кодом.
    """
    interleaver = BlockInterleaver(depth)
    size = interleaver.block_size
    clean = [0] * size

    # Пакет ошибок длиной B_max в канале.
    damaged = list(clean)
    for i in range(interleaver.max_burst):
        damaged[i] = 1

    restored = interleaver.deinterleave(damaged)

    # После деперемежения ошибки распределяются по кодовым словам.
    for start in range(0, size, N):
        errors = sum(restored[start:start + N])
        assert errors <= T, (
            f"в кодовом слове {start // N} оказалось {errors} ошибок, "
            f"код исправляет только {T}"
        )


@pytest.mark.parametrize("depth", [4, 8, 16])
def test_burst_beyond_limit_breaks_a_codeword(depth: int) -> None:
    """Пакет длиннее B_max уже не гарантирует исправимость.

    Проверка границы применимости: оценка B_max = d * t точная, а не
    заниженная.
    """
    interleaver = BlockInterleaver(depth)
    damaged = [0] * interleaver.block_size
    for i in range(interleaver.max_burst + interleaver.depth):
        damaged[i] = 1

    restored = interleaver.deinterleave(damaged)
    worst = max(
        sum(restored[start:start + N])
        for start in range(0, interleaver.block_size, N)
    )
    assert worst > T


def test_block_size_and_max_burst() -> None:
    interleaver = BlockInterleaver(16)
    assert interleaver.block_size == 16 * N
    assert interleaver.max_burst == 16 * T


def test_invalid_depth_rejected() -> None:
    with pytest.raises(ValueError):
        BlockInterleaver(0)


def test_wrong_block_size_rejected() -> None:
    interleaver = BlockInterleaver(4)
    with pytest.raises(ValueError):
        interleaver.interleave([0, 1, 0])
