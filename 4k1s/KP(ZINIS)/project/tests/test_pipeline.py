"""Сквозная проверка конвейера встраивания и извлечения."""

import pytest

from textsteg.api import embed, extract
from textsteg.core.pipeline import (
    ALLOWED_DEPTHS,
    FRAME_PAYLOAD_BITS,
    FRAME_TOTAL_BITS,
    PipelineConfig,
    required_capacity,
)
from textsteg.embedding import (
    EMBEDDERS,
    ZeroWidthEmbedder,
    contains_foreign_symbols,
    sanitize,
)
from textsteg.errors import (
    CapacityError,
    MessageTooLongError,
    PipelineError,
    TextStegError,
)

CONTAINER = (
    "Электронный документ легко скопировать без потери качества. "
    "Это удобно для распространения, однако затрудняет установление "
    "источника утечки. "
) * 90

MESSAGE = "Копия № 17, отдел кадров"


@pytest.fixture
def embedder() -> ZeroWidthEmbedder:
    return ZeroWidthEmbedder()


@pytest.mark.parametrize("depth", ALLOWED_DEPTHS)
def test_roundtrip_for_every_depth(
    embedder: ZeroWidthEmbedder, depth: int
) -> None:
    config = PipelineConfig(depth=depth)
    result = embed(CONTAINER, MESSAGE, embedder, config)
    extracted = extract(result.stego, embedder, config)

    assert extracted.success
    assert extracted.message == MESSAGE


def test_stego_is_visually_identical(embedder: ZeroWidthEmbedder) -> None:
    """Удаление служебных символов возвращает исходный контейнер.

    Проверка незаметности: видимая часть документа не изменилась.
    """
    result = embed(CONTAINER, MESSAGE, embedder, PipelineConfig(depth=8))
    cleaned = result.stego
    for char in "​‌‍⁠":
        cleaned = cleaned.replace(char, "")
    assert cleaned == CONTAINER


def test_overhead_is_equal_for_all_depths(embedder: ZeroWidthEmbedder) -> None:
    """Доля служебных бит не зависит от глубины перемежения.

    Размер кадра постоянен, поэтому сравнение режимов в разделе
    экспериментальных исследований отражает работу перемежителя, а не
    разную долю накладных расходов.
    """
    overheads = {
        depth: embed(
            CONTAINER, MESSAGE, embedder, PipelineConfig(depth=depth)
        ).overhead
        for depth in ALLOWED_DEPTHS
    }
    assert len(set(round(value, 12) for value in overheads.values())) == 1


def test_depth_is_taken_from_header(embedder: ZeroWidthEmbedder) -> None:
    """Получатель не обязан знать параметры встраивания.

    Глубина перемежения читается из заголовка, а не из настройки приёмника.
    """
    result = embed(CONTAINER, MESSAGE, embedder, PipelineConfig(depth=16))
    extracted = extract(result.stego, embedder, PipelineConfig(depth=4))

    assert extracted.success
    assert extracted.header is not None
    assert extracted.header.depth == 16


def test_clean_container_reports_absence(embedder: ZeroWidthEmbedder) -> None:
    """В тексте без встроенного сообщения маркер не находится."""
    extracted = extract(CONTAINER, embedder)
    assert not extracted.success
    assert "маркер" in extracted.reason


def test_stripped_channel_reports_failure(embedder: ZeroWidthEmbedder) -> None:
    """Очистка служебных символов уничтожает канал целиком."""
    result = embed(CONTAINER, MESSAGE, embedder, PipelineConfig(depth=8))
    cleaned = result.stego
    for char in "​‌‍⁠":
        cleaned = cleaned.replace(char, "")

    extracted = extract(cleaned, embedder)
    assert not extracted.success


def test_capacity_error_on_small_container(
    embedder: ZeroWidthEmbedder
) -> None:
    with pytest.raises(CapacityError):
        embed("слишком короткий контейнер", MESSAGE, embedder)


@pytest.mark.parametrize(
    "message",
    [
        "",
        "а",
        "ровно шестнадцать",
        "Копия № 17, отдел кадров",
        "Служебная отметка: " + "текст " * 6,
    ],
)
def test_required_capacity_matches_actual(
    embedder: ZeroWidthEmbedder, message: str
) -> None:
    """Предварительная оценка вместимости совпадает с фактической длиной.

    Проверяется на сообщениях разной длины, включая пустое и пересекающее
    границу кадра: именно на них расходились расчёт и построение потока.
    """
    result = embed(CONTAINER, message, embedder, PipelineConfig(depth=8))
    assert required_capacity(message) == result.stream_bits


def test_empty_message_roundtrip(embedder: ZeroWidthEmbedder) -> None:
    """Пустое сообщение встраивается и извлекается как пустое."""
    result = embed(CONTAINER, "", embedder, PipelineConfig(depth=8))
    extracted = extract(result.stego, embedder)

    assert extracted.success
    assert extracted.message == ""
    assert extracted.header is not None
    assert extracted.header.message_length == 0


def test_container_with_foreign_symbols_is_cleaned(
    embedder: ZeroWidthEmbedder
) -> None:
    """Посторонние символы алфавита в контейнере не ломают извлечение.

    Документ мог ранее пройти через другую систему обработки. Такие символы
    неотличимы от собственных и внесли бы в поток лишние биты, поэтому
    контейнер очищается перед встраиванием.
    """
    dirty = CONTAINER.replace("документ", "​доку‌мент⁠", 1)
    dirty = "‍" + dirty + "​"

    result = embed(dirty, MESSAGE, embedder, PipelineConfig(depth=8))
    extracted = extract(result.stego, embedder)

    assert extracted.success
    assert extracted.message == MESSAGE


def test_foreign_symbols_are_detectable(embedder: ZeroWidthEmbedder) -> None:
    """Наличие посторонних символов можно обнаружить и сообщить о нём."""
    assert not contains_foreign_symbols(CONTAINER)
    assert contains_foreign_symbols(CONTAINER + "​")
    assert sanitize(CONTAINER + "​") == CONTAINER


def test_frame_geometry() -> None:
    assert FRAME_PAYLOAD_BITS == 256
    assert FRAME_TOTAL_BITS == 272


def test_invalid_depth_rejected() -> None:
    with pytest.raises(PipelineError):
        PipelineConfig(depth=5)


def test_utilization_reports_share_of_capacity(
    embedder: ZeroWidthEmbedder
) -> None:
    """Доля занятой вместимости вычисляется по контейнеру и потоку."""
    result = embed(CONTAINER, MESSAGE, embedder, PipelineConfig(depth=8))

    assert 0.0 < result.utilization < 1.0
    assert result.utilization == pytest.approx(
        result.stream_bits / result.capacity_bits
    )


def test_embedder_registry_lists_available_methods() -> None:
    """Реестр методов встраивания используется интерфейсом для выбора."""
    assert ZeroWidthEmbedder.name in EMBEDDERS
    assert EMBEDDERS[ZeroWidthEmbedder.name] is ZeroWidthEmbedder


def test_too_long_message_is_rejected_before_embedding(
    embedder: ZeroWidthEmbedder
) -> None:
    """Сообщение длиннее предельного отвергается на этапе оценки.

    Поле длины заголовка занимает 16 бит. Отказ должен наступать при первом
    же обращении, а не в глубине сборки потока: интерфейс сначала спрашивает
    требуемую вместимость и принимает по ней решение.
    """
    too_long = "я" * 40000  # кириллица в UTF-8 — два байта на символ

    with pytest.raises(MessageTooLongError):
        required_capacity(too_long)
    with pytest.raises(MessageTooLongError):
        embed(CONTAINER, too_long, embedder)


def test_errors_share_a_common_base() -> None:
    """Все ошибки программного средства перехватываются одним типом.

    Графическому интерфейсу достаточно одного обработчика вместо перечисления
    несвязанных исключений из разных модулей.
    """
    assert issubclass(CapacityError, TextStegError)
    assert issubclass(MessageTooLongError, TextStegError)
    assert issubclass(PipelineError, TextStegError)
