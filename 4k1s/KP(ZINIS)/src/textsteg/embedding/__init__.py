"""Методы встраивания скрытой информации в текстовый контейнер."""

from ..errors import CapacityError
from .base import Embedder
from .zerowidth import ZeroWidthEmbedder, contains_foreign_symbols, sanitize

#: Реестр доступных методов; используется интерфейсом и экспериментами.
EMBEDDERS: dict[str, type[Embedder]] = {
    ZeroWidthEmbedder.name: ZeroWidthEmbedder,
}

__all__ = [
    "EMBEDDERS",
    "CapacityError",
    "Embedder",
    "ZeroWidthEmbedder",
    "contains_foreign_symbols",
    "sanitize",
]
