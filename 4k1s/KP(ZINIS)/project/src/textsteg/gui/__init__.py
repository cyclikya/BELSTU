"""Графический интерфейс программного средства (PySide6).

Для запуска требуется необязательная зависимость PySide6:

    pip install -e ".[gui]"
"""

from .app import MainWindow, main

__all__ = ["MainWindow", "main"]
