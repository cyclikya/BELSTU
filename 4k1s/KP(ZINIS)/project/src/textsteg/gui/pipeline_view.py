"""Панель пошагового разбора, встраиваемая во вкладку «Встраивание».

Панель не выполняет действий сама: вкладка передаёт ей готовый разбор,
полученный модулем trace, и панель его показывает. Для шагов, где важно
устройство данных, выводится таблица полей; для остальных — сам поток, в
котором участки раскрашены по смыслу, с легендой под ним.

Отдельно выводится матрица перемежителя. На ней видно главное свойство
метода: биты одного кодового слова, записанные в строку, после чтения по
столбцам расходятся по всему блоку, и пакет подряд идущих ошибок задевает
каждое слово не более одного раза.
"""

from __future__ import annotations

from PySide6.QtCore import Qt
from PySide6.QtGui import QColor
from PySide6.QtWidgets import (
    QGroupBox,
    QHBoxLayout,
    QHeaderView,
    QLabel,
    QSpinBox,
    QSplitter,
    QTableWidget,
    QTableWidgetItem,
    QTextEdit,
    QVBoxLayout,
    QWidget,
)

from ..core.hamming import N as CODEWORD_LENGTH
from ..trace import (
    EmbedTrace,
    Stage,
    codeword_positions,
    interleaver_matrix,
)
from . import theme
from .bitview import chip, monospace, render_bits, render_fields


class PipelineView(QWidget):
    """Разбор того, что происходит внутри метода при встраивании."""

    COLUMNS = ("№", "Шаг", "Было → стало")

    def __init__(self) -> None:
        super().__init__()
        self.trace: EmbedTrace | None = None

        self.steps_table = QTableWidget(0, len(self.COLUMNS))
        self.steps_table.setHorizontalHeaderLabels(self.COLUMNS)
        self.steps_table.verticalHeader().setVisible(False)
        self.steps_table.setSelectionBehavior(
            QTableWidget.SelectionBehavior.SelectRows
        )
        self.steps_table.setEditTriggers(
            QTableWidget.EditTrigger.NoEditTriggers
        )
        head = self.steps_table.horizontalHeader()
        head.setSectionResizeMode(0, QHeaderView.ResizeMode.ResizeToContents)
        head.setSectionResizeMode(1, QHeaderView.ResizeMode.Stretch)
        head.setSectionResizeMode(2, QHeaderView.ResizeMode.ResizeToContents)
        self.steps_table.itemSelectionChanged.connect(self._show_stage)

        self.detail_label = QLabel(
            "Встройте сообщение, и здесь появится разбор по шагам."
        )
        self.detail_label.setWordWrap(True)
        self.detail_label.setMinimumHeight(56)

        self.bits_view = QTextEdit()
        self.bits_view.setFont(monospace())
        self.bits_view.setReadOnly(True)

        self.legend_label = QLabel()
        self.legend_label.setWordWrap(True)
        self.legend_label.setTextFormat(Qt.TextFormat.RichText)

        self.word_spin = QSpinBox()
        self.word_spin.setRange(0, 0)
        self.word_spin.setPrefix("слово № ")
        self.word_spin.valueChanged.connect(self._show_matrix)

        self.matrix_table = QTableWidget(0, 0)
        self.matrix_table.verticalHeader().setVisible(False)
        self.matrix_table.horizontalHeader().setVisible(False)
        self.matrix_table.setEditTriggers(
            QTableWidget.EditTrigger.NoEditTriggers
        )
        self.matrix_table.horizontalHeader().setSectionResizeMode(
            QHeaderView.ResizeMode.Stretch
        )
        self.matrix_table.verticalHeader().setDefaultSectionSize(20)
        self.matrix_table.verticalHeader().setSectionResizeMode(
            QHeaderView.ResizeMode.Fixed
        )

        self.matrix_label = QLabel()
        self.matrix_label.setWordWrap(True)

        self._build_layout()

    def _build_layout(self) -> None:
        steps_box = QGroupBox("Что происходит с сообщением")
        steps_layout = QVBoxLayout(steps_box)
        steps_layout.addWidget(self.steps_table, stretch=1)
        steps_layout.addWidget(self.detail_label)

        bits_box = QGroupBox("Данные на выбранном шаге")
        bits_layout = QVBoxLayout(bits_box)
        bits_layout.addWidget(self.bits_view, stretch=1)
        bits_layout.addWidget(self.legend_label)

        matrix_box = QGroupBox("Матрица перемежителя")
        matrix_layout = QVBoxLayout(matrix_box)
        controls = QHBoxLayout()
        controls.addWidget(QLabel("Подсветить:"))
        controls.addWidget(self.word_spin)
        controls.addStretch()
        matrix_layout.addLayout(controls)
        matrix_layout.addWidget(self.matrix_table, stretch=1)
        matrix_layout.addWidget(self.matrix_label)

        splitter = QSplitter(Qt.Orientation.Horizontal)
        splitter.addWidget(steps_box)
        splitter.addWidget(bits_box)
        splitter.addWidget(matrix_box)
        splitter.setSizes([400, 380, 340])

        layout = QVBoxLayout(self)
        layout.setContentsMargins(0, 0, 0, 0)
        layout.addWidget(splitter)

    # -- обновление --------------------------------------------------------

    def show_trace(self, trace: EmbedTrace) -> None:
        """Выводит разбор, полученный от вкладки встраивания."""
        self.trace = trace

        self.steps_table.setRowCount(0)
        for stage in trace.stages:
            row = self.steps_table.rowCount()
            self.steps_table.insertRow(row)
            values = (
                str(stage.number),
                stage.title,
                f"{stage.size_in} → {stage.size_out}",
            )
            for column, value in enumerate(values):
                self.steps_table.setItem(row, column, QTableWidgetItem(value))
        self.steps_table.selectRow(0)

        self.word_spin.blockSignals(True)
        self.word_spin.setMaximum(max(0, trace.depth - 1))
        self.word_spin.setValue(0)
        self.word_spin.blockSignals(False)
        self._show_matrix()

    def _show_stage(self) -> None:
        if self.trace is None:
            return
        row = self.steps_table.currentRow()
        if not 0 <= row < len(self.trace.stages):
            return
        stage: Stage = self.trace.stages[row]
        self.detail_label.setText(f"<b>{stage.title}.</b> {stage.detail}")

        if stage.fields:
            self.bits_view.setHtml(render_fields(stage))
            self.legend_label.setText(
                "Каждая строка показывает, во что превращается своя часть "
                "значения; биты идут в том же порядке сверху вниз."
            )
        elif stage.bits is None:
            self.bits_view.setHtml(
                "<p>На этом шаге поток уже не битовый: пары бит "
                "превратились в служебные символы внутри текста.</p>"
            )
            self.legend_label.setText("")
        else:
            self.bits_view.setHtml(render_bits(stage.bits, stage.spans))
            self.legend_label.setText(
                "&nbsp;&nbsp;&nbsp;".join(
                    chip(kind, caption) for kind, caption in stage.legend
                )
            )

    def _show_matrix(self) -> None:
        if self.trace is None or not self.trace.frames_coded:
            return
        depth = self.trace.depth
        width = CODEWORD_LENGTH
        block = self.trace.frames_coded[0][: depth * width]
        rows = interleaver_matrix(block, depth)

        self.matrix_table.setRowCount(depth)
        self.matrix_table.setColumnCount(width)
        chosen = self.word_spin.value()
        for row, bits in enumerate(rows):
            for column, bit in enumerate(bits):
                item = QTableWidgetItem(str(bit))
                item.setTextAlignment(Qt.AlignmentFlag.AlignCenter)
                if row == chosen:
                    item.setBackground(QColor(theme.FILL_GOLD))
                    item.setForeground(QColor(theme.ON_FILL))
                elif column >= CODEWORD_LENGTH - 4:
                    item.setBackground(QColor(theme.FILL_TEAL))
                    item.setForeground(QColor(theme.ON_FILL))
                self.matrix_table.setItem(row, column, item)

        if depth == 1:
            self.matrix_label.setText(
                "Глубина d = 1: матрица из одной строки, порядок бит не "
                "меняется. Пакет ошибок целиком попадает в одно кодовое "
                "слово, а его код выправить не в состоянии."
            )
            return

        places = codeword_positions(depth, chosen)
        self.matrix_label.setText(
            f"Строка это кодовое слово: четыре информационных бита и четыре "
            f"проверочных, они залиты бледным. Запись по строкам, чтение по "
            f"столбцам. Биты слова № {chosen} уезжают на места "
            f"{', '.join(str(place) for place in places)}, между соседними "
            f"встаёт {depth - 1} чужих."
        )
