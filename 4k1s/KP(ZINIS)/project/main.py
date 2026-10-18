"""Запуск приложения без установки пакета.

Сценарий добавляет каталог src в путь поиска модулей и передаёт управление
графическому интерфейсу. Нужен для запуска из каталога проекта:

    .venv\Scripts\python.exe main.py

После установки пакета (pip install -e ".[gui]") доступен также способ
запуска командой textsteg.
"""

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent / "src"))

from textsteg.gui import main  # noqa: E402  (импорт после настройки пути)

if __name__ == "__main__":
    sys.exit(main())
