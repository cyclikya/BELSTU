"""Панель разбора извлечения, встраиваемая во вкладку «Извлечение».

Панель зеркальна разбору встраивания и показывает обратный путь: документ —
биты — маркер — заголовок — кадры — кодовые слова — байты — контрольная
сумма. Сама она ничего не декодирует: вкладка передаёт ей готовый разбор,
а тот, в свою очередь, лишь раскладывает протокол, который ведёт приёмник.

Третья колонка занята картой кодовых слов. Это главный показ повреждений:
на ней сразу видно, сколько слов декодер правил, сколько признал
невосстановимыми и сколько досталось утраченным кадрам, в которых код не
работал вовсе. По клетке карты можно спросить подробности об одном слове.
"""

from __future__ import annotations

from PySide6.QtCore import Qt
from PySide6.QtGui import QColor
from PySide6.QtWidgets import (
    QGroupBox,
    QHeaderView,
    QLabel,
    QSplitter,
    QTableWidget,
    QTableWidgetItem,
    QTextEdit,
    QVBoxLayout,
    QWidget,
)

from ..core.hamming import BlockStatus
from ..core.hamming import N as CODEWORD_LENGTH
from ..core.pipeline import (
    FRAME_CODEWORDS,
    ExtractResult,
    FrameOutcome,
    FrameReport,
)
from ..trace import ExtractTrace, Stage, channel_position, frames_word
from . import theme
from .bitview import (
    chip,
    monospace,
    render_bits,
    render_empty,
    render_fields,
    render_sections,
    render_table,
)

#: Сколько кадров показывать на карте. Поле длины заголовка может быть
#: восстановлено неверно, и тогда приёмник честно ожидает тысячи кадров;
#: карта в этом случае обрезается, о чём написано под ней.
MAX_MAP_FRAMES = 24


class ExtractView(QWidget):
    """Разбор того, как приёмник восстанавливает сообщение."""

    COLUMNS = ("№", "Шаг", "Было → стало")

    def __init__(self) -> None:
        super().__init__()
        self.trace: ExtractTrace | None = None
        self.stages: list[Stage] = []

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
        self.steps_table.verticalHeader().setDefaultSectionSize(20)
        self.steps_table.itemSelectionChanged.connect(self._show_stage)

        self.detail_label = QLabel(
            "Извлеките сообщение, и здесь появится разбор по шагам."
        )
        self.detail_label.setWordWrap(True)
        self.detail_label.setMinimumHeight(56)

        self.data_view = QTextEdit()
        self.data_view.setFont(monospace())
        self.data_view.setReadOnly(True)

        self.legend_label = QLabel()
        self.legend_label.setWordWrap(True)
        self.legend_label.setTextFormat(Qt.TextFormat.RichText)

        self.map_table = QTableWidget(0, 0)
        self.map_table.horizontalHeader().setVisible(False)
        self.map_table.setEditTriggers(
            QTableWidget.EditTrigger.NoEditTriggers
        )
        # Клеток в строке 32, и по умолчанию Qt не даёт столбцу сузиться
        # настолько, чтобы все они поместились: без этого половина карты
        # оказалась бы за правым краем.
        self.map_table.horizontalHeader().setMinimumSectionSize(6)
        self.map_table.horizontalHeader().setSectionResizeMode(
            QHeaderView.ResizeMode.Stretch
        )
        self.map_table.verticalHeader().setDefaultSectionSize(18)
        self.map_table.verticalHeader().setSectionResizeMode(
            QHeaderView.ResizeMode.Fixed
        )
        # Столбцы растянуты по ширине, прокручивать вбок нечего.
        self.map_table.setHorizontalScrollBarPolicy(
            Qt.ScrollBarPolicy.ScrollBarAlwaysOff
        )
        self.map_table.cellClicked.connect(self._explain_block)

        self.map_legend = QLabel()
        self.map_legend.setWordWrap(True)
        self.map_legend.setTextFormat(Qt.TextFormat.RichText)

        self.map_summary = QLabel()
        self.map_summary.setWordWrap(True)

        # Подпись к выбранной клетке живёт отдельно от счётчиков: иначе
        # щелчок по клетке затирал бы итог по всему документу.
        self.map_detail = QLabel(
            "Щёлкните клетку карты, чтобы узнать о кодовом слове подробнее."
        )
        self.map_detail.setWordWrap(True)

        self._build_layout()

    def _build_layout(self) -> None:
        steps_box = QGroupBox("Что делает приёмник")
        steps_layout = QVBoxLayout(steps_box)
        steps_layout.addWidget(self.steps_table, stretch=1)
        steps_layout.addWidget(self.detail_label)

        data_box = QGroupBox("Данные на выбранном шаге")
        data_layout = QVBoxLayout(data_box)
        data_layout.addWidget(self.data_view, stretch=1)
        data_layout.addWidget(self.legend_label)

        map_box = QGroupBox("Карта кодовых слов")
        map_layout = QVBoxLayout(map_box)
        map_layout.addWidget(self.map_table)
        map_layout.addWidget(self.map_legend)
        map_layout.addWidget(self.map_summary)
        map_layout.addWidget(self.map_detail)
        map_layout.addStretch()

        splitter = QSplitter(Qt.Orientation.Horizontal)
        splitter.addWidget(steps_box)
        splitter.addWidget(data_box)
        splitter.addWidget(map_box)
        splitter.setSizes([400, 380, 340])

        layout = QVBoxLayout(self)
        layout.setContentsMargins(0, 0, 0, 0)
        layout.addWidget(splitter)

    # -- обновление --------------------------------------------------------

    def show_trace(self, trace: ExtractTrace) -> None:
        """Выводит разбор, полученный от вкладки извлечения."""
        self.trace = trace
        self.stages = list(trace.stages)
        self._fill_steps()

        self._fill_map()
        self.steps_table.selectRow(0)

    def _fill_steps(self) -> None:
        self.steps_table.setRowCount(0)
        for stage in self.stages:
            row = self.steps_table.rowCount()
            self.steps_table.insertRow(row)
            # Недостигнутый шаг не убирается из списка, а гаснет: по
            # списку видно, на чём именно разбор оборвался.
            values = (
                str(stage.number),
                stage.title,
                f"{stage.size_in} → {stage.size_out}",
            )
            for column, value in enumerate(values):
                item = QTableWidgetItem(value)
                if not stage.reached:
                    item.setForeground(QColor(theme.MUTED))
                self.steps_table.setItem(row, column, item)

    def _show_stage(self) -> None:
        row = self.steps_table.currentRow()
        if not 0 <= row < len(self.stages):
            return
        stage = self.stages[row]
        self.detail_label.setText(f"<b>{stage.title}.</b> {stage.detail}")

        if stage.sections:
            self.data_view.setHtml(render_sections(stage))
            self.legend_label.setText(
                "&nbsp;&nbsp;&nbsp;".join(
                    chip(kind, caption) for kind, caption in stage.legend
                )
                or "Кадры идут по порядку, каждый под своей подписью."
            )
        elif stage.fields:
            self.data_view.setHtml(render_fields(stage))
            self.legend_label.setText(
                "Каждая строка показывает свою часть значения; биты идут в "
                "том же порядке сверху вниз."
            )
        elif stage.rows:
            # Таблица и поток показываются вместе: по таблице видно, где
            # приёмник искал границы, а по потоку — что он в итоге разметил.
            html = render_table(stage)
            if stage.bits is not None:
                html += render_bits(stage.bits, stage.spans)
            self.data_view.setHtml(html)
            self.legend_label.setText(
                "&nbsp;&nbsp;&nbsp;".join(
                    chip(kind, caption) for kind, caption in stage.legend
                )
                or "«Ждали» — куда приёмник отсчитал границу кадра."
            )
        elif stage.bits is None:
            self.data_view.setHtml(render_empty("На этом шаге данных нет."))
            self.legend_label.setText("")
        else:
            self.data_view.setHtml(render_bits(stage.bits, stage.spans))
            self.legend_label.setText(
                "&nbsp;&nbsp;&nbsp;".join(
                    chip(kind, caption) for kind, caption in stage.legend
                )
            )

    # -- карта кодовых слов ------------------------------------------------

    def _fill_map(self) -> None:
        if self.trace is None:
            return
        result = self.trace.result
        frames = self.trace.frames[:MAX_MAP_FRAMES]
        # Первой строкой идёт заголовок: он тоже проходит через код, и без
        # него «правлено 0» по кадрам читалось бы как «нигде ничего не
        # правили». Слов в нём меньше, чем в кадре, и хвост строки пустой.
        self.map_table.setRowCount(1 + len(frames))
        self.map_table.setColumnCount(FRAME_CODEWORDS)
        self.map_table.setVerticalHeaderLabels(
            ["заг."] + [str(report.number) for report in frames]
        )
        for column in range(FRAME_CODEWORDS):
            item = QTableWidgetItem("")
            if column < len(result.header_blocks):
                block = result.header_blocks[column]
                item.setBackground(QColor(_status_color(block.status)))
                item.setToolTip(
                    f"заголовок, слово {column}: "
                    f"{_STATUS_WORDS[block.status]}"
                )
            else:
                # Клетки за пределами заголовка — не «целые слова», а
                # отсутствующие: гасим их тёмным, чтобы не принимали за
                # кодовые слова, которых в заголовке нет.
                item.setBackground(QColor(theme.BG))
                item.setFlags(Qt.ItemFlag.NoItemFlags)
            self.map_table.setItem(0, column, item)
        for row, report in enumerate(frames, start=1):
            for column in range(FRAME_CODEWORDS):
                item = QTableWidgetItem("")
                item.setBackground(QColor(_cell_color(report, column)))
                item.setToolTip(_cell_tip(report, column))
                self.map_table.setItem(row, column, item)
        # Карта занимает ровно столько, сколько в ней кадров: пустое поле
        # под нею выглядело бы как не поместившиеся строки.
        self.map_table.setMaximumHeight(
            self.map_table.verticalHeader().length()
            + 2 * self.map_table.frameWidth()
        )
        self._describe_map()

    def _describe_map(self) -> None:
        if self.trace is None:
            return
        result = self.trace.result
        # Считаем по протоколу кадров, а не по block_statuses: на отказных
        # путях там лежат слова заголовка, и карта выдавала бы их за слова
        # кадров, которых не читала вовсе.
        frames = self.trace.frames
        total = FRAME_CODEWORDS * len(frames)
        corrected = sum(report.corrected for report in frames)
        clean = (
            total - corrected - result.erased_by_code - result.erased_by_loss
        )
        shown = min(len(frames), MAX_MAP_FRAMES)
        tail = (
            ""
            if shown == len(frames)
            else f"   •   показано {_frames(shown)} из {len(frames)}"
        )
        self.map_legend.setText(
            "Клетка — одно кодовое слово. Первая строка «заг.» — заголовок "
            "(12 слов), дальше кадры по номерам.<br>"
            + "&nbsp;&nbsp;&nbsp;".join(
                (
                    chip("fixed", "декодер правил бит"),
                    chip("erased", "код не смог восстановить"),
                    chip("lost", "кадр не принят"),
                )
            )
            + "<br>Клетка без заливки — признаков повреждения нет."
        )
        if not frames:
            # Кадры не читались: сообщать о них нечего, зато о заголовке
            # есть что сказать — его слова разбирает шаг 3.
            self.map_summary.setText(
                "Кадры не читались: разбор оборвался на заголовке. Слов "
                f"заголовка {len(result.header_blocks)}, из них не "
                f"восстановлено кодом {result.erased_in_header}."
            )
            return
        header_note = (
            "без правок"
            if not result.corrected_in_header and not result.erased_in_header
            else f"правлено {result.corrected_in_header}, не восстановлено "
            f"{result.erased_in_header}"
        )
        self.map_summary.setText(
            f"Заголовок: {len(result.header_blocks)} слов, {header_note}\n"
            f"Слов в кадрах {total}   •   целых {clean}   •   правлено "
            f"{corrected}   •   не восстановлено кодом "
            f"{result.erased_by_code}   •   осталось в непринятых кадрах "
            f"{result.erased_by_loss}{tail}"
        )

    def _explain_block(self, row: int, column: int) -> None:
        """Рассказывает об одном кодовом слове по клетке карты."""
        if self.trace is None:
            return
        if row == 0:
            self.map_detail.setText(
                _explain_header(self.trace.result, column)
            )
            return
        frames = self.trace.frames[:MAX_MAP_FRAMES]
        if not 1 <= row <= len(frames):
            return
        self.map_detail.setText(
            _explain(frames[row - 1], column, self.trace.depth)
        )


def _frames(count: int) -> str:
    """Число кадров в согласованной форме."""
    return frames_word(count)


#: Как называется состояние кодового слова в подсказках.
_STATUS_WORDS = {
    BlockStatus.OK: "признаков повреждения нет",
    BlockStatus.CORRECTED: "декодер перевернул один бит",
    BlockStatus.ERASED: "код не смог восстановить",
}


def _status_color(status: BlockStatus) -> str:
    """Цвет клетки по состоянию кодового слова."""
    if status is BlockStatus.CORRECTED:
        return theme.FILL_GOLD
    if status is BlockStatus.ERASED:
        return theme.FILL_RUST
    return theme.FIELD


def _cell_color(report: FrameReport, column: int) -> str:
    """Цвет клетки карты по состоянию кодового слова кадра."""
    if report.outcome is not FrameOutcome.CAPTURED:
        return theme.FILL_GREY
    if column >= len(report.blocks):
        return theme.FILL_GREY
    return _status_color(report.blocks[column].status)


def _cell_tip(report: FrameReport, column: int) -> str:
    """Короткая подсказка по клетке."""
    if report.outcome is not FrameOutcome.CAPTURED:
        return f"кадр {report.number}, слово {column}: кадр не принят"
    if column >= len(report.blocks):
        return f"кадр {report.number}, слово {column}: слова нет"
    status = report.blocks[column].status
    return f"кадр {report.number}, слово {column}: {_STATUS_WORDS[status]}"


def _explain_header(result: ExtractResult, column: int) -> str:
    """Подробности об одном кодовом слове заголовка.

    Заголовок не перемежается, поэтому место бита в канале считается
    прямо: начало заголовка плюс восемь бит на слово.
    """
    if column >= len(result.header_blocks):
        return (
            f"В заголовке {len(result.header_blocks)} кодовых слов, клетки "
            "правее к нему не относятся."
        )
    block = result.header_blocks[column]
    where = f"Заголовок, кодовое слово {column}. "
    if block.status is BlockStatus.OK:
        return where + "Проверки кода сошлись: признаков повреждения нет."
    if block.status is BlockStatus.ERASED:
        return (
            where + "Ошибок в слове больше одной — код его не восстановит. "
            "Поля заголовка собраны из этих же слов, поэтому дальше "
            "приёмник не идёт."
        )
    position = block.error_position
    if position is None:
        return where + "Декодер сообщил о правке, не указав позиции."
    start = 0 if result.header_at is None else result.header_at + 16
    return (
        where + f"Декодер перевернул бит № {position}; в потоке это бит "
        f"{start + column * CODEWORD_LENGTH + position}. Заголовок не "
        "перемежается, поэтому его биты лежат подряд. Поля ниже верны при "
        "условии, что ошибка была одиночной."
    )


def _explain(report: FrameReport, column: int, depth: int) -> str:
    """Подробности об одном кодовом слове — только из принятых бит."""
    where = f"Кадр {report.number}, кодовое слово {column}. "
    if report.outcome is FrameOutcome.MARKER_LOST:
        return (
            where + "Кадр не принят: маркер не найден в окне поиска. "
            "Слово помечено стёртым без участия кода — декодировать было "
            "нечего, и работе кода эта пометка не принадлежит."
        )
    if report.outcome is FrameOutcome.TRUNCATED:
        return (
            where + "Кадр не принят: поток оборвался внутри него, "
            f"принято {len(report.received)} бит из "
            f"{FRAME_CODEWORDS * CODEWORD_LENGTH}. Слово помечено стёртым "
            "без участия кода."
        )
    if column >= len(report.blocks):
        return where + "Слова с таким номером в кадре нет."

    block = report.blocks[column]
    if block.status is BlockStatus.OK:
        return (
            where + "Проверки кода сошлись: признаков повреждения нет. "
            "Целость это не доказывает — ошибка сразу в четырёх битах "
            "переводит слово в другое допустимое, и код её не видит. О "
            "сообщении в целом судит контрольная сумма."
        )
    if block.status is BlockStatus.ERASED:
        return (
            where + "Ошибок в слове больше одной — восстановить его код "
            "не может: четыре информационных бита потеряны, и в сообщении на "
            "их месте мусор. Какой бит испорчен, неизвестно, поэтому залито "
            "всё слово. Зато отказ честный: код с d* = 3 исправил бы наугад "
            "и выдал слово за верное."
        )

    position = block.error_position
    if position is None:
        return where + "Декодер сообщил о правке, не указав позиции."
    place = channel_position(depth, column, position)
    if position == CODEWORD_LENGTH - 1:
        return (
            where + "Нарушена общая чётность, и декодер отнёс это к "
            f"самому биту чётности: данных он не касался. В канале этот бит "
            f"стоял на {place}-м месте кадра. Что данные целы, отсюда не "
            "следует — ту же картину даёт ошибка в трёх битах."
        )
    return (
        where + f"Декодер перевернул бит № {position}; в канале это "
        f"место {place}-е в кадре. Позицию он вычислил по синдрому, то есть "
        "из одного принятого слова. Это его действие, а не утверждение о "
        "том, где правили документ: при ошибке в трёх битах он перевернул "
        "бы исправный бит и сказал бы то же самое."
    )
