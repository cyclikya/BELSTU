"""Вкладка демонстрации устойчивости к искусственным битовым помехам."""

from __future__ import annotations

from random import Random
from typing import TYPE_CHECKING

from PySide6.QtCore import Qt
from PySide6.QtGui import QColor, QFontMetrics
from PySide6.QtWidgets import (
    QGroupBox,
    QHBoxLayout,
    QHeaderView,
    QLabel,
    QMessageBox,
    QPushButton,
    QRadioButton,
    QSpinBox,
    QSplitter,
    QTableWidget,
    QTableWidgetItem,
    QVBoxLayout,
    QWidget,
)

from ..core.pipeline import ALLOWED_DEPTHS, FRAME_PAYLOAD_BITS
from ..errors import TextStegError
from ..research.noise import DepthResult, NoiseMode, choose_positions, compare_depths
from . import theme
from .bitview import monospace

if TYPE_CHECKING:
    from .app import EmbedTab

MAP_DEPTH = 4


class BitErrorMap(QTableWidget):
    """Квадратные клетки показывают биты данных первого кадра."""

    COLUMNS = 16

    def __init__(self) -> None:
        rows = -(-FRAME_PAYLOAD_BITS // self.COLUMNS)
        super().__init__(rows, self.COLUMNS)
        self._flips: dict[int, int] = {}
        self.setAccessibleName("Карта битов первого кадра")
        self.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)
        self.setSelectionMode(QTableWidget.SelectionMode.NoSelection)
        self.setFocusPolicy(Qt.FocusPolicy.NoFocus)
        self.setHorizontalScrollBarPolicy(Qt.ScrollBarPolicy.ScrollBarAlwaysOff)
        self.setVerticalScrollBarPolicy(Qt.ScrollBarPolicy.ScrollBarAlwaysOff)
        self.setVerticalHeaderLabels(
            [str(i) for i in range(1, FRAME_PAYLOAD_BITS + 1, self.COLUMNS)]
        )

        font = monospace()
        self.setFont(font)
        metrics = QFontMetrics(font)
        side = metrics.height() + 6
        self.horizontalHeader().hide()
        for header in (self.horizontalHeader(), self.verticalHeader()):
            header.setMinimumSectionSize(1)
            header.setDefaultSectionSize(side)
            header.setSectionResizeMode(QHeaderView.ResizeMode.Fixed)
        labels_width = metrics.horizontalAdvance(str(FRAME_PAYLOAD_BITS)) + 16
        self.verticalHeader().setFixedWidth(labels_width)
        self.ensurePolished()
        border = 2 * self.frameWidth()
        self.setFixedSize(
            labels_width + self.horizontalHeader().length() + border,
            self.verticalHeader().length() + border,
        )

        for position in range(FRAME_PAYLOAD_BITS):
            item = QTableWidgetItem()
            item.setFlags(Qt.ItemFlag.ItemIsEnabled)
            item.setTextAlignment(Qt.AlignmentFlag.AlignCenter)
            item.setBackground(QColor(theme.RAISED))
            item.setToolTip(f"Бит {position + 1}")
            self.setItem(*divmod(position, self.COLUMNS), item)

    def set_flips(self, flips: dict[int, int]) -> None:
        for position in self._flips.keys() | flips.keys():
            item = self.item(*divmod(position, self.COLUMNS))
            bit = flips.get(position)
            if bit is None:
                item.setText("")
                item.setBackground(QColor(theme.RAISED))
                item.setToolTip(f"Бит {position + 1}")
            else:
                item.setText(str(bit))
                item.setForeground(QColor(theme.ON_FILL))
                item.setBackground(QColor(theme.FILL_RUST))
                item.setToolTip(f"Бит {position + 1}: {bit ^ 1} → {bit}")
        self._flips = flips


class RobustnessTab(QWidget):
    """Сопоставляет результаты одного прогона при всех глубинах."""

    COLUMNS = ("Глубина d", "Ошибок в слове\n(максимум)", "Извлечение")
    EMPTY_MAP = "Номера перевёрнутых битов появятся после прогона."

    def __init__(self, embed_tab: EmbedTab) -> None:
        super().__init__()
        self.embed_tab = embed_tab
        self._rng = Random()
        self._last_burst_start: int | None = None
        self._has_result = False

        self.burst_radio = QRadioButton("Пакет")
        self.scattered_radio = QRadioButton("Рассеянные ошибки")
        self.burst_radio.setChecked(True)
        self.count_spin = QSpinBox()
        self.count_spin.setRange(1, min(64, FRAME_PAYLOAD_BITS))
        self.count_spin.setValue(8)
        self.count_spin.setSuffix(" бит")
        self.run_button = QPushButton("Сравнить глубины")
        self.run_button.setObjectName("primary")
        self.run_button.setMinimumHeight(34)

        self.bit_map = BitErrorMap()
        self.positions_label = QLabel(self.EMPTY_MAP)
        self.positions_label.setWordWrap(True)
        self.positions_label.setTextInteractionFlags(
            Qt.TextInteractionFlag.TextSelectableByMouse
        )

        self.table = QTableWidget(0, len(self.COLUMNS))
        self.table.setHorizontalHeaderLabels(self.COLUMNS)
        self.table.setAccessibleName("Результаты сравнения глубин")
        self.table.verticalHeader().hide()
        self.table.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)
        self.table.setSelectionMode(QTableWidget.SelectionMode.NoSelection)
        self.table.setAlternatingRowColors(True)
        self.table.setShowGrid(False)
        header = self.table.horizontalHeader()
        header.setSectionResizeMode(QHeaderView.ResizeMode.Stretch)
        header.setSectionResizeMode(0, QHeaderView.ResizeMode.ResizeToContents)
        header.setSectionResizeMode(1, QHeaderView.ResizeMode.ResizeToContents)
        row_height = max(26, self.table.fontMetrics().height() + 10)
        self.table.verticalHeader().setDefaultSectionSize(row_height)
        self.table.ensurePolished()
        self.table.setFixedHeight(
            header.sizeHint().height()
            + len(ALLOWED_DEPTHS) * row_height
            + 2 * self.table.frameWidth()
        )

        self._build_layout()
        self.run_button.clicked.connect(self._run)
        self.scattered_radio.toggled.connect(self._mode_changed)
        self.count_spin.valueChanged.connect(self._clear_result)
        self.embed_tab.container_edit.textChanged.connect(self._clear_result)
        self.embed_tab.message_edit.textChanged.connect(self._clear_result)

    def _build_layout(self) -> None:
        settings = QGroupBox("Параметры помехи")
        settings_layout = QVBoxLayout(settings)
        controls = QHBoxLayout()
        controls.addWidget(QLabel("Тип помехи"))
        controls.addWidget(self.burst_radio)
        controls.addWidget(self.scattered_radio)
        controls.addSpacing(16)
        count_label = QLabel("Число ошибок")
        count_label.setBuddy(self.count_spin)
        controls.addWidget(count_label)
        controls.addWidget(self.count_spin)
        controls.addStretch()
        controls.addWidget(self.run_button)
        settings_layout.addLayout(controls)
        source = QLabel(
            "Контейнер и сообщение берутся со вкладки «Встраивание». "
            "Помеха вносится в кодированные данные первого кадра."
        )
        source.setWordWrap(True)
        settings_layout.addWidget(source)

        map_box = QGroupBox("Карта битов первого кадра")
        map_layout = QVBoxLayout(map_box)
        map_layout.addWidget(
            QLabel(f"{FRAME_PAYLOAD_BITS} бит · слева номер первого бита строки")
        )
        map_layout.addWidget(self.bit_map, alignment=Qt.AlignmentFlag.AlignLeft)
        legend = QLabel(
            f'<span style="background-color:{theme.FILL_RUST}">'
            "&nbsp;&nbsp;&nbsp;</span>&nbsp;"
            f"Перевёрнутый бит. Цифра — новое значение при d = {MAP_DEPTH}."
        )
        legend.setWordWrap(True)
        map_layout.addWidget(legend)
        map_layout.addWidget(self.positions_label)
        map_layout.addStretch()

        results_box = QGroupBox("Сравнение глубин")
        results_layout = QVBoxLayout(results_box)
        explanation = QLabel("Одни и те же позиции помехи для всех глубин.")
        explanation.setWordWrap(True)
        results_layout.addWidget(explanation)
        results_layout.addWidget(self.table)
        results_layout.addStretch()

        splitter = QSplitter(Qt.Orientation.Horizontal)
        splitter.setChildrenCollapsible(False)
        splitter.addWidget(map_box)
        splitter.addWidget(results_box)
        splitter.setSizes([540, 540])

        layout = QVBoxLayout(self)
        layout.addWidget(settings)
        layout.addWidget(splitter, stretch=1)

    def _clear_result(self) -> None:
        self._has_result = False
        self.bit_map.set_flips({})
        self.positions_label.setText(self.EMPTY_MAP)
        self.table.setRowCount(0)

    def _mode_changed(self) -> None:
        if self._has_result:
            self._run()

    def _run(self) -> None:
        mode = (
            NoiseMode.SCATTERED if self.scattered_radio.isChecked() else NoiseMode.BURST
        )
        positions = choose_positions(
            mode,
            self.count_spin.value(),
            self._rng,
            previous_start=self._last_burst_start,
        )
        try:
            result = compare_depths(
                self.embed_tab.container_edit.toPlainText(),
                self.embed_tab.message_edit.toPlainText(),
                self.embed_tab.embedder,
                positions,
                map_depth=MAP_DEPTH,
            )
        except TextStegError as error:
            self._clear_result()
            QMessageBox.warning(self, "Прогон не выполнен", str(error))
            return

        self._has_result = True
        if mode is NoiseMode.BURST:
            self._last_burst_start = positions[0]
        self.bit_map.set_flips(result.flips)
        if mode is NoiseMode.BURST and len(positions) > 1:
            numbered = f"{positions[0] + 1}–{positions[-1] + 1}"
        else:
            numbered = ", ".join(str(position + 1) for position in positions)
        self.positions_label.setText(f"Перевёрнутые биты: {numbered}")
        self._show_rows(result.rows)

    def _show_rows(self, results: tuple[DepthResult, ...]) -> None:
        self.table.setRowCount(len(results))
        for row, result in enumerate(results):
            verdict = "Восстановлено" if result.recovered else "Не восстановлено"
            depth = "1 (без перемежения)" if result.depth == 1 else str(result.depth)
            values = (depth, str(result.max_word_errors), verdict)
            for column, value in enumerate(values):
                item = QTableWidgetItem(value)
                item.setTextAlignment(Qt.AlignmentFlag.AlignCenter)
                if column == 2:
                    status = theme.SUCCESS if result.recovered else theme.DANGER
                    item.setForeground(QColor(status))
                    font = item.font()
                    font.setBold(True)
                    item.setFont(font)
                    details = (
                        f"Исправлений декодером: {result.corrected_blocks}\n"
                        f"Стёртых слов: {result.erased_blocks}"
                    )
                    if result.reason:
                        details = f"{result.reason}\n{details}"
                    item.setToolTip(details)
                self.table.setItem(row, column, item)
