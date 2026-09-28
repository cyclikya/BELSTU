"""Прикладной интерфейс программного средства.

Модуль соединяет два независимых слоя: ядро метода (пакет core), которое
оперирует исключительно битовыми последовательностями, и методы встраивания
(пакет embedding), которые знают о текстовом контейнере, но ничего не знают
о помехоустойчивом кодировании и перемежении.

Разделение выдержано строго: ядро не импортирует ни одного модуля,
работающего с текстом, поэтому метод можно исследовать и тестировать
независимо от способа встраивания, а способ встраивания — заменить, не
затрагивая ядро. Функции embed и extract размещены здесь, а не в ядре,
именно поэтому: они единственные, кому нужны оба слоя сразу.

Графический интерфейс и средства исследования обращаются к программному
средству через этот модуль.
"""

from __future__ import annotations

from .core.pipeline import MESSAGE_ENCODING as _ENCODING
from .core.pipeline import (
    EmbedResult,
    ExtractResult,
    PipelineConfig,
    build_stream,
    frames_required,
    parse_stream,
)
from .embedding.base import Embedder

__all__ = [
    "embed",
    "extract",
    "EmbedResult",
    "ExtractResult",
    "PipelineConfig",
]


def embed(
    container: str,
    message: str,
    embedder: Embedder,
    config: PipelineConfig | None = None,
) -> EmbedResult:
    """Встраивает сообщение в контейнер выбранным методом.

    Возбуждает CapacityError, если вместимости контейнера недостаточно;
    объём необходимой вместимости можно узнать заранее функцией
    core.pipeline.required_capacity.
    """
    settings = config or PipelineConfig()
    stream = build_stream(message, settings)
    stego = embedder.embed(container, stream)
    message_bytes = len(message.encode(_ENCODING))

    return EmbedResult(
        stego=stego,
        message_bytes=message_bytes,
        stream_bits=len(stream),
        capacity_bits=embedder.capacity(container),
        frames=frames_required(message_bytes),
    )


def extract(
    stego: str,
    embedder: Embedder,
    config: PipelineConfig | None = None,
) -> ExtractResult:
    """Извлекает сообщение из стеготекста выбранным методом.

    Параметры извлечения (в частности глубину перемежения) получатель
    читает из заголовка, поэтому config задаёт лишь настройки поиска
    синхромаркеров.
    """
    return parse_stream(embedder.extract(stego), config)
