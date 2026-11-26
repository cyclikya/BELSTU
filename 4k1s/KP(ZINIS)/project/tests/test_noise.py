"""Проверки генерации помех и сравнения режимов перемежения."""

from random import Random

import pytest

from textsteg.core.bits import Bits
from textsteg.core.pipeline import (
    ALLOWED_DEPTHS,
    FRAME_PAYLOAD_BITS,
    ExtractResult,
    PipelineConfig,
    build_stream,
)
from textsteg.embedding import ZeroWidthEmbedder
from textsteg.research import noise
from textsteg.research.noise import NoiseMode, choose_positions, compare_depths

CONTAINER = "Текст для проверки " * 600
MESSAGE = "Копия № 17, отдел кадров"


def test_burst_is_contiguous_and_does_not_repeat_previous_start() -> None:
    rng = Random(20)
    first = choose_positions(NoiseMode.BURST, 64, rng)
    second = choose_positions(NoiseMode.BURST, 64, rng, previous_start=first[0])
    assert first[0] != second[0]
    for positions in (first, second):
        assert positions == tuple(range(positions[0], positions[0] + 64))
        assert 0 <= positions[0] <= positions[-1] < FRAME_PAYLOAD_BITS


def test_scattered_errors_are_distinct_bits_of_one_frame() -> None:
    positions = choose_positions(NoiseMode.SCATTERED, 64, Random(42))
    assert len(positions) == 64
    assert positions == tuple(sorted(set(positions)))
    assert 0 <= positions[0] <= positions[-1] < FRAME_PAYLOAD_BITS
    assert positions[-1] - positions[0] > 63


@pytest.mark.parametrize(
    ("start", "maxima", "recovered_at_four"),
    [(0, [8, 4, 2, 1, 1, 1], False), (28, [4, 2, 1, 1, 1, 1], True)],
)
def test_burst_position_changes_how_errors_fall_on_words(
    start: int, maxima: list[int], recovered_at_four: bool
) -> None:
    result = compare_depths(
        CONTAINER,
        MESSAGE,
        ZeroWidthEmbedder(),
        tuple(range(start, start + 8)),
        map_depth=4,
    )
    assert tuple(row.depth for row in result.rows) == ALLOWED_DEPTHS
    assert [row.max_word_errors for row in result.rows] == maxima
    assert next(row.recovered for row in result.rows if row.depth == 4) is (
        recovered_at_four
    )


def test_all_depths_use_the_same_noise_and_map_shows_the_changed_values(
    monkeypatch: pytest.MonkeyPatch,
) -> None:
    positions = (0, 21, 138, FRAME_PAYLOAD_BITS - 1)
    stream_positions = tuple(noise.FIRST_FRAME_DATA_AT + i for i in positions)
    original_parse = noise.parse_stream
    seen: list[int] = []
    expected_flips: dict[int, int] = {}

    def capture(bits: Bits, config: PipelineConfig) -> ExtractResult:
        before = build_stream(MESSAGE, config)
        changed = tuple(
            i for i, (a, b) in enumerate(zip(before, bits, strict=True)) if a != b
        )
        assert changed == stream_positions
        if config.depth == 4:
            expected_flips.update(
                zip(positions, (bits[i] for i in changed), strict=True)
            )
        seen.append(config.depth)
        return original_parse(bits, config)

    monkeypatch.setattr(noise, "parse_stream", capture)
    result = compare_depths(
        CONTAINER, MESSAGE, ZeroWidthEmbedder(), positions, map_depth=4
    )
    assert tuple(seen) == ALLOWED_DEPTHS
    assert result.flips == expected_flips
