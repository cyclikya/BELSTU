"""Графический интерфейс программного средства.

Интерфейс построен на библиотеке PySide6 (привязки Qt 6) и состоит из трёх
вкладок, соответствующих трём задачам пользователя:

    «Встраивание»  — поместить служебное сообщение в текст-контейнер;
    «Извлечение»   — восстановить сообщение из полученного документа;
    «Устойчивость» — наглядно показать вклад перемежения в восстановление
                     сообщения после пакетного повреждения.

Третья вкладка носит исследовательский характер: она воспроизводит на одном
и том же контейнере все допустимые глубины перемежения при одинаковом пакете
ошибок и показывает, при какой глубине сообщение восстанавливается. Именно
это сопоставление составляет содержание темы работы.

Интерфейс обращается к программному средству только через модуль api и не
содержит ни одной строки, относящейся к методу: кодирование, перемежение и
кадрирование целиком остаются в ядре.
"""

from __future__ import annotations

import sys
from pathlib import Path

from PySide6.QtCore import Qt
from PySide6.QtGui import QColor, QFont
from PySide6.QtWidgets import (
    QApplication,
    QCheckBox,
    QComboBox,
    QFileDialog,
    QFormLayout,
    QGroupBox,
    QHBoxLayout,
    QHeaderView,
    QLabel,
    QMainWindow,
    QMessageBox,
    QPlainTextEdit,
    QPushButton,
    QSpinBox,
    QSplitter,
    QTableWidget,
    QTableWidgetItem,
    QTabWidget,
    QVBoxLayout,
    QWidget,
)

from ..api import embed, extract
from ..core.bits import Bits
from ..core.framing import SYNC_MARKER_LENGTH
from ..core.hamming import T as CORRECTABLE_ERRORS
from ..core.pipeline import (
    ALLOWED_DEPTHS,
    HEADER_CODED_BITS,
    ExtractResult,
    PipelineConfig,
    parse_stream,
    required_capacity,
)
from ..embedding import ZeroWidthEmbedder, contains_foreign_symbols
from ..embedding.zerowidth import WJ, ZWJ, ZWNJ, ZWSP
from ..errors import TextStegError

#: Кодировка файлов. Задана жёстко: в однобайтовых кодировках, принятых на
#: русской версии Windows по умолчанию, символы нулевой ширины не
#: представимы, и сохранение стеготекста уничтожило бы скрытое сообщение.
FILE_ENCODING = "utf-8"

#: Видимые обозначения символов алфавита для режима отображения скрытых
#: символов. Надстрочная цифра соответствует значению пары бит.
VISIBLE_SYMBOLS: dict[str, str] = {
    ZWSP: "⁰",  # 00
    ZWNJ: "¹",  # 01
    ZWJ: "²",   # 10
    WJ: "³",    # 11
}

#: Цвета заключений. Подобраны так, чтобы оставаться читаемыми и на
#: светлой, и на тёмной теме оформления Windows.
COLOR_SUCCESS = "#3ba55d"
COLOR_FAILURE = "#e5534b"

#: Сообщение, предлагаемое по умолчанию: служебная отметка о копии документа.
DEFAULT_MESSAGE = "Копия № 17, отдел кадров"

#: Каталог с примерами контейнеров.
SAMPLES_DIR = Path(__file__).resolve().parents[3] / "data" / "containers"


def visualize_hidden(text: str) -> str:
    """Заменяет символы нулевой ширины видимыми обозначениями.

    Нужно только для демонстрации: в обычном режиме стеготекст визуально
    неотличим от контейнера, и убедиться в наличии встроенных данных
    по виду документа невозможно.
    """
    for symbol, visible in VISIBLE_SYMBOLS.items():
        text = text.replace(symbol, visible)
    return text


def read_text_file(parent: QWidget, title: str) -> str | None:
    """Открывает диалог выбора файла и читает его в кодировке UTF-8."""
    path, _ = QFileDialog.getOpenFileName(
        parent, title, str(SAMPLES_DIR), "Текстовые файлы (*.txt);;Все файлы (*)"
    )
    if not path:
        return None
    try:
        return Path(path).read_text(encoding=FILE_ENCODING)
    except (OSError, UnicodeDecodeError) as error:
        QMessageBox.warning(
            parent,
            "Не удалось прочитать файл",
            f"{path}\n\n{error}\n\nФайл должен быть в кодировке UTF-8.",
        )
        return None


def write_text_file(parent: QWidget, title: str, text: str) -> None:
    """Открывает диалог сохранения и записывает текст в кодировке UTF-8."""
    path, _ = QFileDialog.getSaveFileName(
        parent, title, "", "Текстовые файлы (*.txt);;Все файлы (*)"
    )
    if not path:
        return
    try:
        Path(path).write_text(text, encoding=FILE_ENCODING)
    except OSError as error:
        QMessageBox.warning(parent, "Не удалось сохранить файл", str(error))
        return
    QMessageBox.information(
        parent,
        "Файл сохранён",
        f"{path}\n\nКодировка UTF-8: она обязательна, иначе служебные "
        "символы будут утрачены вместе со скрытым сообщением.",
    )


def monospace() -> QFont:
    """Моноширинный шрифт для полей с текстом и битовыми данными."""
    font = QFont("Consolas")
    font.setStyleHint(QFont.StyleHint.Monospace)
    font.setPointSize(10)
    return font


def depth_selector() -> QComboBox:
    """Выпадающий список допустимых глубин перемежения."""
    box = QComboBox()
    for depth in ALLOWED_DEPTHS:
        label = "1 (без перемежения)" if depth == 1 else str(depth)
        box.addItem(label, depth)
    box.setCurrentIndex(ALLOWED_DEPTHS.index(8))
    return box


# --------------------------------------------------------------------------
# Вкладка «Встраивание»
# --------------------------------------------------------------------------

class EmbedTab(QWidget):
    """Встраивание служебного сообщения в текст-контейнер."""

    def __init__(self) -> None:
        super().__init__()
        self.embedder = ZeroWidthEmbedder()
        self.stego = ""

        self.container_edit = QPlainTextEdit()
        self.container_edit.setFont(monospace())
        self.container_edit.setPlaceholderText(
            "Вставьте текст документа или загрузите его из файла"
        )
        self.container_edit.textChanged.connect(self._update_capacity)

        self.message_edit = QPlainTextEdit(DEFAULT_MESSAGE)
        self.message_edit.setFont(monospace())
        self.message_edit.setFixedHeight(70)
        self.message_edit.textChanged.connect(self._update_capacity)

        self.depth_box = depth_selector()

        self.stego_edit = QPlainTextEdit()
        self.stego_edit.setFont(monospace())
        self.stego_edit.setReadOnly(True)
        self.stego_edit.setPlaceholderText(
            "Здесь появится стеготекст — внешне он не отличается от исходного"
        )

        self.show_hidden = QCheckBox(
            "Показать скрытые символы: ⁰ = 00, ¹ = 01, "
            "² = 10, ³ = 11"
        )
        self.show_hidden.toggled.connect(self._render_stego)

        self.capacity_label = QLabel()
        self.capacity_label.setWordWrap(True)
        self.stats_label = QLabel("Сообщение ещё не встроено.")
        self.stats_label.setWordWrap(True)

        self._build_layout()
        self._load_sample()

    def _build_layout(self) -> None:
        load_button = QPushButton("Загрузить из файла…")
        load_button.clicked.connect(self._load_file)
        sample_button = QPushButton("Взять пример")
        sample_button.clicked.connect(self._load_sample)

        container_box = QGroupBox("Текст-контейнер")
        container_layout = QVBoxLayout(container_box)
        container_layout.addWidget(self.container_edit)
        buttons = QHBoxLayout()
        buttons.addWidget(load_button)
        buttons.addWidget(sample_button)
        buttons.addStretch()
        container_layout.addLayout(buttons)
        container_layout.addWidget(self.capacity_label)

        params_box = QGroupBox("Параметры встраивания")
        params_layout = QFormLayout(params_box)
        params_layout.addRow("Секретное сообщение:", self.message_edit)
        params_layout.addRow("Глубина перемежения d:", self.depth_box)

        embed_button = QPushButton("Встроить сообщение")
        embed_button.setMinimumHeight(34)
        embed_button.clicked.connect(self._embed)

        save_button = QPushButton("Сохранить стеготекст…")
        save_button.clicked.connect(self._save)

        stego_box = QGroupBox("Стеготекст")
        stego_layout = QVBoxLayout(stego_box)
        stego_layout.addWidget(self.stego_edit)
        stego_layout.addWidget(self.show_hidden)
        stego_layout.addWidget(self.stats_label)
        stego_layout.addWidget(save_button)

        left = QWidget()
        left_layout = QVBoxLayout(left)
        left_layout.setContentsMargins(0, 0, 0, 0)
        left_layout.addWidget(container_box, stretch=3)
        left_layout.addWidget(params_box)
        left_layout.addWidget(embed_button)

        splitter = QSplitter(Qt.Orientation.Horizontal)
        splitter.addWidget(left)
        splitter.addWidget(stego_box)
        splitter.setSizes([520, 520])

        layout = QVBoxLayout(self)
        layout.addWidget(splitter)

    # -- действия ----------------------------------------------------------

    def _load_sample(self) -> None:
        """Загружает пример контейнера из каталога data/containers."""
        sample = SAMPLES_DIR / "sample_ru.txt"
        if sample.exists():
            self.container_edit.setPlainText(
                sample.read_text(encoding=FILE_ENCODING)
            )
        else:
            self.capacity_label.setText(
                f"Пример не найден: {sample}. Вставьте текст вручную."
            )

    def _load_file(self) -> None:
        text = read_text_file(self, "Выберите текст-контейнер")
        if text is not None:
            self.container_edit.setPlainText(text)

    def _update_capacity(self) -> None:
        """Пересчитывает вместимость контейнера при каждом его изменении."""
        container = self.container_edit.toPlainText()
        message = self.message_edit.toPlainText()
        slots = self.embedder.count_slots(container)
        capacity = self.embedder.capacity(container)

        try:
            needed = required_capacity(message)
        except TextStegError as error:
            self.capacity_label.setText(f"Сообщение недопустимо: {error}")
            return

        verdict = "помещается" if needed <= capacity else "НЕ ПОМЕЩАЕТСЯ"
        note = ""
        if contains_foreign_symbols(container):
            note = (
                "\nВнимание: контейнер уже содержит служебные символы нулевой "
                "ширины. Они будут удалены перед встраиванием — иначе они "
                "смешались бы со скрытыми данными."
            )
        self.capacity_label.setText(
            f"Позиций встраивания: {slots}   •   вместимость: {capacity} бит   "
            f"•   требуется: {needed} бит   •   {verdict}{note}"
        )

    def _embed(self) -> None:
        container = self.container_edit.toPlainText()
        message = self.message_edit.toPlainText()
        depth = int(self.depth_box.currentData())

        try:
            result = embed(
                container, message, self.embedder, PipelineConfig(depth=depth)
            )
        except TextStegError as error:
            QMessageBox.warning(self, "Встраивание не выполнено", str(error))
            return

        self.stego = result.stego
        self._render_stego()
        self.stats_label.setText(
            f"Сообщение: {result.message_bytes} байт   •   "
            f"поток в канале: {result.stream_bits} бит   •   "
            f"кадров: {result.frames}\n"
            f"Занято вместимости: {result.utilization:.1%}   •   "
            f"служебных бит: {result.overhead:.1%}   •   "
            f"символов в тексте: {len(container)} → {len(result.stego)}"
        )

    def _render_stego(self) -> None:
        """Выводит стеготекст в обычном или демонстрационном виде."""
        if self.show_hidden.isChecked():
            self.stego_edit.setPlainText(visualize_hidden(self.stego))
        else:
            self.stego_edit.setPlainText(self.stego)

    def _save(self) -> None:
        if not self.stego:
            QMessageBox.information(
                self, "Нечего сохранять", "Сначала встройте сообщение."
            )
            return
        write_text_file(self, "Сохранить стеготекст", self.stego)


# --------------------------------------------------------------------------
# Вкладка «Извлечение»
# --------------------------------------------------------------------------

class ExtractTab(QWidget):
    """Восстановление сообщения из полученного документа."""

    def __init__(self, embed_tab: EmbedTab) -> None:
        super().__init__()
        self.embedder = ZeroWidthEmbedder()
        self.embed_tab = embed_tab

        self.stego_edit = QPlainTextEdit()
        self.stego_edit.setFont(monospace())
        self.stego_edit.setPlaceholderText(
            "Вставьте полученный документ или загрузите его из файла"
        )

        self.message_edit = QPlainTextEdit()
        self.message_edit.setFont(monospace())
        self.message_edit.setReadOnly(True)
        self.message_edit.setFixedHeight(80)

        self.status_label = QLabel("Извлечение ещё не выполнялось.")
        self.status_label.setWordWrap(True)
        self.details_label = QLabel()
        self.details_label.setWordWrap(True)

        self._build_layout()

    def _build_layout(self) -> None:
        load_button = QPushButton("Загрузить из файла…")
        load_button.clicked.connect(self._load_file)
        take_button = QPushButton("Взять результат встраивания")
        take_button.clicked.connect(self._take_from_embed)

        source_box = QGroupBox("Документ")
        source_layout = QVBoxLayout(source_box)
        source_layout.addWidget(self.stego_edit)
        buttons = QHBoxLayout()
        buttons.addWidget(load_button)
        buttons.addWidget(take_button)
        buttons.addStretch()
        source_layout.addLayout(buttons)

        extract_button = QPushButton("Извлечь сообщение")
        extract_button.setMinimumHeight(34)
        extract_button.clicked.connect(self._extract)

        result_box = QGroupBox("Результат")
        result_layout = QVBoxLayout(result_box)
        result_layout.addWidget(QLabel("Восстановленное сообщение:"))
        result_layout.addWidget(self.message_edit)
        result_layout.addWidget(self.status_label)
        result_layout.addWidget(self.details_label)
        result_layout.addStretch()

        layout = QVBoxLayout(self)
        layout.addWidget(source_box, stretch=3)
        layout.addWidget(extract_button)
        layout.addWidget(result_box, stretch=2)

    # -- действия ----------------------------------------------------------

    def _load_file(self) -> None:
        text = read_text_file(self, "Выберите документ")
        if text is not None:
            self.stego_edit.setPlainText(text)

    def _take_from_embed(self) -> None:
        """Переносит стеготекст из вкладки встраивания без потерь.

        Через буфер обмена операционной системы служебные символы могут не
        пройти, поэтому перенос выполняется напрямую.
        """
        if not self.embed_tab.stego:
            QMessageBox.information(
                self,
                "Нечего переносить",
                "Сначала встройте сообщение на вкладке «Встраивание».",
            )
            return
        self.stego_edit.setPlainText(self.embed_tab.stego)

    def _extract(self) -> None:
        stego = self.stego_edit.toPlainText()

        try:
            result = extract(stego, self.embedder)
        except TextStegError as error:
            QMessageBox.warning(self, "Извлечение не выполнено", str(error))
            return

        self.message_edit.setPlainText(result.message)

        if result.success:
            self.status_label.setText(
                "<b style='color:#3ba55d'>Сообщение восстановлено полностью.</b> "
                "Контрольная сумма совпала."
            )
        else:
            self.status_label.setText(
                f"<b style='color:#e5534b'>Сообщение не восстановлено.</b> "
                f"{result.reason}"
            )

        if result.header is None:
            self.details_label.setText(
                "Заголовок не прочитан: сведения о параметрах недоступны."
            )
            return

        total = len(result.block_statuses)
        self.details_label.setText(
            f"Из заголовка: длина {result.header.message_length} байт, "
            f"глубина перемежения d = {result.header.depth}, "
            f"контрольная сумма 0x{result.header.checksum:04X}\n"
            f"Кадров ожидалось: {result.frames_expected}, "
            f"утрачено: {result.frames_lost}\n"
            f"Блоков всего: {total}, исправлено: {result.corrected_blocks}, "
            f"стёрто: {result.erased_blocks}"
        )


# --------------------------------------------------------------------------
# Вкладка «Устойчивость»
# --------------------------------------------------------------------------

class RobustnessTab(QWidget):
    """Сопоставление глубин перемежения при одинаковом пакете ошибок.

    Вкладка воспроизводит основной эксперимент работы: в стеготекст,
    собранный с разной глубиной перемежения, вносится пакет ошибок одной и
    той же длины, после чего выполняется извлечение. Теория предсказывает,
    что пакет длиной B исправляется при d * t >= B и не исправляется при
    меньшей глубине; таблица позволяет убедиться в этом непосредственно.
    """

    COLUMNS = (
        "Глубина d",
        "B_max = d·t",
        "Пакет, бит",
        "Результат",
        "Исправлено блоков",
        "Стёрто блоков",
    )

    def __init__(self, embed_tab: EmbedTab) -> None:
        super().__init__()
        self.embedder = ZeroWidthEmbedder()
        self.embed_tab = embed_tab

        self.burst_spin = QSpinBox()
        self.burst_spin.setRange(1, 64)
        self.burst_spin.setValue(8)
        self.burst_spin.setSuffix(" бит")

        self.table = QTableWidget(0, len(self.COLUMNS))
        self.table.setHorizontalHeaderLabels(self.COLUMNS)
        self.table.verticalHeader().setVisible(False)
        header = self.table.horizontalHeader()
        header.setSectionResizeMode(QHeaderView.ResizeMode.Stretch)

        self.summary_label = QLabel(
            "Задайте длину пакета ошибок и выполните прогон."
        )
        self.summary_label.setWordWrap(True)

        self._build_layout()

    def _build_layout(self) -> None:
        explanation = QLabel(
            "В стеготекст вносится пакет подряд идущих ошибочных бит — так "
            "выглядит повреждение, вызванное правкой фрагмента документа. "
            "Код Хэмминга (8,4) исправляет одну ошибку в кодовом слове, "
            "поэтому без перемежения пакет уничтожает блок целиком. "
            "Перемежитель глубины d распределяет пакет по d кодовым словам, "
            "и теоретический предел исправляемого пакета составляет "
            "B_max = d·t, где t = 1."
        )
        explanation.setWordWrap(True)

        run_button = QPushButton("Прогнать по всем глубинам")
        run_button.setMinimumHeight(34)
        run_button.clicked.connect(self._run)

        controls = QHBoxLayout()
        controls.addWidget(QLabel("Длина пакета ошибок:"))
        controls.addWidget(self.burst_spin)
        controls.addStretch()

        layout = QVBoxLayout(self)
        layout.addWidget(explanation)
        layout.addLayout(controls)
        layout.addWidget(run_button)
        layout.addWidget(self.table, stretch=1)
        layout.addWidget(self.summary_label)

    # -- действия ----------------------------------------------------------

    def _run(self) -> None:
        container = self.embed_tab.container_edit.toPlainText()
        message = self.embed_tab.message_edit.toPlainText()
        burst = self.burst_spin.value()

        self.table.setRowCount(0)
        recovered: list[int] = []

        for depth in ALLOWED_DEPTHS:
            config = PipelineConfig(depth=depth)
            try:
                result = embed(container, message, self.embedder, config)
            except TextStegError as error:
                QMessageBox.warning(self, "Прогон не выполнен", str(error))
                return

            bits = self.embedder.extract(result.stego)
            damaged = self._damage(bits, burst)
            outcome = parse_stream(damaged, config)

            if outcome.success:
                recovered.append(depth)
            self._add_row(depth, burst, outcome)

        predicted = [d for d in ALLOWED_DEPTHS if d * CORRECTABLE_ERRORS >= burst]
        self.summary_label.setText(
            f"Пакет {burst} бит восстановлен при глубинах: "
            f"{self._format(recovered)}.\n"
            f"Теоретический предел B_max = d·t предсказывает: "
            f"{self._format(predicted)}."
        )

    def _damage(self, bits: Bits, burst: int) -> Bits:
        """Инвертирует пакет бит в начале полезной части первого кадра.

        Служебная часть потока (первый маркер, заголовок и маркер кадра) при
        этом не затрагивается: повреждается именно переносимое сообщение.
        """
        start = SYNC_MARKER_LENGTH + HEADER_CODED_BITS + SYNC_MARKER_LENGTH
        damaged = list(bits)
        for index in range(start, min(start + burst, len(damaged))):
            damaged[index] ^= 1
        return damaged

    def _add_row(
        self, depth: int, burst: int, outcome: ExtractResult
    ) -> None:
        """Добавляет строку с итогом одного режима."""
        row = self.table.rowCount()
        self.table.insertRow(row)

        verdict = "восстановлено" if outcome.success else "не восстановлено"
        values = (
            "1 (без перемежения)" if depth == 1 else str(depth),
            str(depth * CORRECTABLE_ERRORS),
            str(burst),
            verdict,
            str(outcome.corrected_blocks),
            str(outcome.erased_blocks),
        )

        for column, value in enumerate(values):
            item = QTableWidgetItem(value)
            item.setTextAlignment(Qt.AlignmentFlag.AlignCenter)
            if column == 3:
                item.setForeground(
                    QColor(0x3B, 0xA5, 0x5D)
                    if outcome.success
                    else QColor(0xE5, 0x53, 0x4B)
                )
                font = item.font()
                font.setBold(True)
                item.setFont(font)
            self.table.setItem(row, column, item)

    @staticmethod
    def _format(depths: list[int]) -> str:
        return ", ".join(str(d) for d in depths) if depths else "ни при одной"


# --------------------------------------------------------------------------
# Главное окно
# --------------------------------------------------------------------------

class MainWindow(QMainWindow):
    """Главное окно приложения."""

    def __init__(self) -> None:
        super().__init__()
        self.setWindowTitle(
            "Текстовая стеганография: перемежение и помехоустойчивое "
            "кодирование"
        )
        self.resize(1120, 760)

        embed_tab = EmbedTab()
        tabs = QTabWidget()
        tabs.addTab(embed_tab, "Встраивание")
        tabs.addTab(ExtractTab(embed_tab), "Извлечение")
        tabs.addTab(RobustnessTab(embed_tab), "Устойчивость к помехам")
        self.setCentralWidget(tabs)

        self.statusBar().showMessage(
            "Метод: символы нулевой ширины, 2 бита на позицию   •   "
            "код: расширенный Хэмминга (8,4), d_min = 4, SEC-DED   •   "
            "перемежитель: блочный, кадр 32 кодовых слова"
        )


def main() -> int:
    """Точка входа приложения."""
    app = QApplication(sys.argv)
    window = MainWindow()
    window.show()
    return app.exec()


if __name__ == "__main__":
    sys.exit(main())
