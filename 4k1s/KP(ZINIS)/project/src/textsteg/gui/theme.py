"""Единое тёмное оформление приложения.

Основа тёмная, с лёгким холодным отливом, чтобы не спорить с акцентом.
Акцентный цвет — приглушённый бирюзовый: он достаточно тёмен, чтобы не
резать глаз на тёмном фоне, и достаточно светел, чтобы нести поверх себя
тёмный текст.

Заливки для разбора взяты одной семьёй с акцентом: ряд от голубого через
бирюзовый и зелёный к оливковому. Соседние оттенки различимы, а вместе они
не рассыпаются на разноцветье.

Контраст проверен расчётом. Светлый текст на фоне окна даёт около
четырнадцати к одному, тёмный текст поверх любой из заливок — от пяти до
шести с половиной. Светлый текст поверх тех же заливок давал бы около трёх,
то есть не читался бы, поэтому поверх заливок текст всегда тёмный.
Нейтральная заливка по той же причине взята светлой: тёмная давала бы с
тёмным текстом лишь два с половиной к одному.
"""

from __future__ import annotations

from PySide6.QtGui import QColor, QPalette
from PySide6.QtWidgets import QApplication

#: Фоны: от самого тёмного к приподнятому.
BG = "#14181b"
PANEL = "#1b2125"
FIELD = "#222a2e"
RAISED = "#2b343a"
LINE = "#39454c"

#: Текст.
TEXT = "#dce3e6"
MUTED = "#8fa0a8"
#: Текст поверх цветных заливок: они средней светлоты, и светлый текст на
#: них не читается.
ON_FILL = "#10161a"

#: Акцент и его затемнённый вариант для нажатия.
ACCENT = "#4e9e8b"
ACCENT_DIM = "#3d7d6e"

#: Семья заливок: голубой, бирюзовый, зелёный, оливковый, кирпичный и
#: нейтральная. Светлота у всех близкая, поэтому поверх любой читается
#: тёмный текст, а между собой они различимы по тону.
FILL_CYAN = "#4a8fa8"
FILL_TEAL = "#4e9e8b"
FILL_GREEN = "#7aa85f"
FILL_GOLD = "#b09a56"
#: Кирпичный: им помечается невосстановимое — стёртые кодовые слова.
FILL_RUST = "#bf7a63"
FILL_GREY = "#8a969c"

#: Исходы. Тоже приглушённые, на тёмном фоне читаются как текст.
SUCCESS = "#5fb39c"
DANGER = "#d08770"

STYLE = f"""
QMainWindow, QDialog {{
    background: {BG};
}}
QWidget {{
    color: {TEXT};
}}
QTabWidget::pane {{
    border: 1px solid {LINE};
    border-radius: 6px;
    background: {PANEL};
    top: -1px;
}}
QTabBar::tab {{
    background: {BG};
    color: {MUTED};
    border: 1px solid {LINE};
    border-bottom: none;
    border-top-left-radius: 6px;
    border-top-right-radius: 6px;
    padding: 8px 20px;
    margin-right: 2px;
}}
QTabBar::tab:selected {{
    background: {PANEL};
    color: {TEXT};
    border-bottom: 2px solid {ACCENT};
}}
QTabBar::tab:hover:!selected {{
    background: {RAISED};
    color: {TEXT};
}}
QGroupBox {{
    border: 1px solid {LINE};
    border-radius: 6px;
    background: {PANEL};
    margin-top: 14px;
    padding-top: 8px;
    font-weight: bold;
}}
QGroupBox::title {{
    subcontrol-origin: margin;
    subcontrol-position: top left;
    left: 10px;
    padding: 0 5px;
    color: {ACCENT};
}}
QPlainTextEdit, QTextEdit {{
    background: {FIELD};
    color: {TEXT};
    border: 1px solid {LINE};
    border-radius: 5px;
    padding: 4px;
    selection-background-color: {ACCENT};
    selection-color: {ON_FILL};
}}
QPlainTextEdit:focus, QTextEdit:focus {{
    border-color: {ACCENT};
}}
QPushButton {{
    background: {RAISED};
    color: {TEXT};
    border: 1px solid {LINE};
    border-radius: 5px;
    padding: 6px 14px;
}}
QPushButton:hover {{
    border-color: {ACCENT};
}}
QPushButton:pressed {{
    background: {FIELD};
}}
QPushButton#primary {{
    background: {ACCENT};
    color: {ON_FILL};
    border: 1px solid {ACCENT};
    font-weight: bold;
}}
QPushButton#primary:hover {{
    background: {FILL_TEAL};
    border-color: {TEXT};
}}
QPushButton#primary:pressed {{
    background: {ACCENT_DIM};
}}
QComboBox QAbstractItemView {{
    background: {FIELD};
    color: {TEXT};
    border: 1px solid {LINE};
    selection-background-color: {ACCENT};
    selection-color: {ON_FILL};
}}
QTableWidget {{
    background: {FIELD};
    alternate-background-color: {PANEL};
    gridline-color: {LINE};
    border: 1px solid {LINE};
    border-radius: 5px;
}}
QTableWidget::item {{
    padding: 3px;
}}
QTableWidget::item:selected {{
    background: {ACCENT};
    color: {ON_FILL};
}}
QHeaderView::section {{
    background: {RAISED};
    color: {MUTED};
    border: none;
    border-right: 1px solid {LINE};
    border-bottom: 1px solid {LINE};
    padding: 5px;
    font-weight: bold;
}}
QScrollBar:vertical {{
    background: {BG};
    width: 12px;
    margin: 0;
}}
QScrollBar::handle:vertical {{
    background: {RAISED};
    border-radius: 6px;
    min-height: 24px;
}}
QScrollBar::handle:vertical:hover {{
    background: {ACCENT_DIM};
}}
QScrollBar::add-line:vertical, QScrollBar::sub-line:vertical {{
    height: 0;
}}
QScrollBar:horizontal {{
    background: {BG};
    height: 12px;
    margin: 0;
}}
QScrollBar::handle:horizontal {{
    background: {RAISED};
    border-radius: 6px;
    min-width: 24px;
}}
QScrollBar::handle:horizontal:hover {{
    background: {ACCENT_DIM};
}}
QScrollBar::add-line:horizontal, QScrollBar::sub-line:horizontal {{
    width: 0;
}}
QSplitter::handle {{
    background: {LINE};
}}
QStatusBar {{
    background: {PANEL};
    color: {MUTED};
    border-top: 1px solid {LINE};
}}
QStatusBar QLabel {{
    color: {MUTED};
}}
QCheckBox {{
    color: {TEXT};
    spacing: 6px;
}}
QToolTip {{
    background: {RAISED};
    color: {TEXT};
    border: 1px solid {LINE};
    padding: 4px;
}}
"""


def apply(app: QApplication) -> None:
    """Накладывает оформление на приложение.

    Кроме таблицы стилей задаётся палитра: выпадающие списки и счётчики
    рисует сама платформа, таблица стилей их не трогает, и без тёмной
    палитры они остались бы светлыми островами. Стиль Fusion выбран
    потому, что он честно берёт цвета из палитры на любой системе.
    """
    app.setStyle("Fusion")

    palette = QPalette()
    palette.setColor(QPalette.ColorRole.Window, QColor(BG))
    palette.setColor(QPalette.ColorRole.WindowText, QColor(TEXT))
    palette.setColor(QPalette.ColorRole.Base, QColor(FIELD))
    palette.setColor(QPalette.ColorRole.AlternateBase, QColor(PANEL))
    palette.setColor(QPalette.ColorRole.Text, QColor(TEXT))
    palette.setColor(QPalette.ColorRole.Button, QColor(RAISED))
    palette.setColor(QPalette.ColorRole.ButtonText, QColor(TEXT))
    palette.setColor(QPalette.ColorRole.Highlight, QColor(ACCENT))
    palette.setColor(QPalette.ColorRole.HighlightedText, QColor(ON_FILL))
    palette.setColor(QPalette.ColorRole.ToolTipBase, QColor(RAISED))
    palette.setColor(QPalette.ColorRole.ToolTipText, QColor(TEXT))
    palette.setColor(QPalette.ColorRole.PlaceholderText, QColor(MUTED))
    palette.setColor(
        QPalette.ColorGroup.Disabled, QPalette.ColorRole.Text, QColor(MUTED)
    )
    palette.setColor(
        QPalette.ColorGroup.Disabled,
        QPalette.ColorRole.ButtonText,
        QColor(MUTED),
    )
    app.setPalette(palette)
    app.setStyleSheet(STYLE)
