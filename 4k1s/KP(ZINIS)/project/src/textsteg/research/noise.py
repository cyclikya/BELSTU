"""Сопоставление глубин перемежения при одинаковой битовой помехе."""

from __future__ import annotations

from dataclasses import dataclass
from enum import Enum, auto
from random import Random

from ..api import embed
from ..core.bits import Bits
from ..core.framing import SYNC_MARKER_LENGTH
from ..core.hamming import N as CODEWORD_LENGTH
from ..core.pipeline import (
    ALLOWED_DEPTHS,
    FRAME_PAYLOAD_BITS,
    HEADER_CODED_BITS,
    MESSAGE_ENCODING,
    PipelineConfig,
    parse_stream,
)
from ..embedding.base import Embedder
from ..errors import PipelineError

FIRST_FRAME_DATA_AT = SYNC_MARKER_LENGTH + HEADER_CODED_BITS + SYNC_MARKER_LENGTH


class NoiseMode(Enum):
    """Один пакет или отдельные случайно выбранные инверсии."""

    BURST = auto()
    SCATTERED = auto()


@dataclass(frozen=True)
class DepthResult:
    depth: int
    max_word_errors: int
    recovered: bool
    corrected_blocks: int
    erased_blocks: int
    reason: str


@dataclass(frozen=True)
class NoiseResult:
    rows: tuple[DepthResult, ...]
    # Позиция от начала данных первого кадра (с нуля) → бит после инверсии.
    flips: dict[int, int]


def choose_positions(
    mode: NoiseMode,
    count: int,
    rng: Random,
    *,
    previous_start: int | None = None,
) -> tuple[int, ...]:
    """Выбирает позиции в одном кадре. Случайная выборка не имеет повторов."""
    if not 1 <= count <= FRAME_PAYLOAD_BITS:
        raise PipelineError("Число ошибок выходит за пределы кадра.")
    if mode is NoiseMode.SCATTERED:
        return tuple(sorted(rng.sample(range(FRAME_PAYLOAD_BITS), count)))

    starts = FRAME_PAYLOAD_BITS - count + 1
    if previous_start is not None and 0 <= previous_start < starts and starts > 1:
        start = rng.randrange(starts - 1)
        if start >= previous_start:
            start += 1
    else:
        start = rng.randrange(starts)
    return tuple(range(start, start + count))


def _max_word_errors(mask: Bits, config: PipelineConfig) -> int:
    """Возвращает максимум по словам после обратного перемежения маски."""
    interleaver = config.interleaver
    size = interleaver.block_size
    maximum = 0
    for start in range(0, len(mask), size):
        group = interleaver.deinterleave(mask[start : start + size])
        for word_start in range(0, size, CODEWORD_LENGTH):
            errors = sum(group[word_start : word_start + CODEWORD_LENGTH])
            maximum = max(maximum, errors)
    return maximum


def compare_depths(
    container: str,
    message: str,
    embedder: Embedder,
    positions: tuple[int, ...],
    *,
    map_depth: int,
) -> NoiseResult:
    """Вносит одну маску в данные первого кадра при каждой глубине."""
    if (
        not positions
        or len(set(positions)) != len(positions)
        or any(not 0 <= position < FRAME_PAYLOAD_BITS for position in positions)
    ):
        raise PipelineError("Позиции ошибок должны быть различными и внутри кадра.")
    if map_depth not in ALLOWED_DEPTHS:
        raise PipelineError("Недопустимая глубина для карты битов.")

    mask = [0] * FRAME_PAYLOAD_BITS
    for position in positions:
        mask[position] = 1

    original = message.encode(MESSAGE_ENCODING)
    rows: list[DepthResult] = []
    flips: dict[int, int] = {}
    for depth in ALLOWED_DEPTHS:
        config = PipelineConfig(depth=depth)
        encoded = embed(container, message, embedder, config)
        damaged = embedder.extract(encoded.stego)
        if len(damaged) < FIRST_FRAME_DATA_AT + FRAME_PAYLOAD_BITS:
            raise PipelineError("В извлечённом потоке нет полного первого кадра.")
        for position in positions:
            damaged[FIRST_FRAME_DATA_AT + position] ^= 1
        if depth == map_depth:
            flips = {
                position: damaged[FIRST_FRAME_DATA_AT + position]
                for position in positions
            }

        outcome = parse_stream(damaged, config)
        recovered = outcome.success and outcome.data == original
        rows.append(
            DepthResult(
                depth=depth,
                max_word_errors=_max_word_errors(mask, config),
                recovered=recovered,
                corrected_blocks=outcome.corrected_blocks,
                erased_blocks=outcome.erased_blocks,
                reason=(
                    "Извлечённые байты отличаются от исходных."
                    if outcome.success and not recovered
                    else outcome.reason
                ),
            )
        )
    return NoiseResult(tuple(rows), flips)
