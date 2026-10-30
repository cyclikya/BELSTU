"""Отрисовка битовых данных, общая для обеих панелей разбора.

Модуль собирает разметку, а не виджеты: обе панели — и разбор встраивания,
и разбор извлечения — показывают одни и те же виды данных (поток с
раскрашенными участками, таблицу полей, таблицу произвольных строк), и
держать два набора отрисовки означало бы расхождение в оформлении при
первой же правке.

Цвета берутся из палитры приложения. Поверх любой заливки текст тёмный:
заливки средней светлоты, и светлый текст на них не читается.
"""

from __future__ import annotations

from html import escape

from PySide6.QtGui import QFont

from ..core.bits import Bits
from ..trace import Span, Stage
from . import theme

#: Цвет подложки для каждого вида участка потока. Все цвета из палитры
#: приложения, поверх них текст остаётся тёмным.
COLORS = {
    "info": theme.FILL_CYAN,
    "parity": theme.FILL_GOLD,
    "overall": theme.FILL_GREEN,
    "pad": theme.FILL_GREY,
    "marker": theme.FILL_GOLD,
    "header": theme.FILL_TEAL,
    "frame": theme.FILL_CYAN,
    "cw0": theme.FILL_CYAN,
    "cw1": theme.FILL_GOLD,
    "cw2": theme.FILL_TEAL,
    "cw3": theme.FILL_GREEN,
    # Виды, которыми размечается принятый поток при разборе извлечения.
    # Они говорят о действии приёмника, а не о причине: «правлено» значит
    # «декодер перевернул этот бит».
    "fixed": theme.FILL_GOLD,
    "erased": theme.FILL_RUST,
    "lost": theme.FILL_GREY,
}

#: Сколько бит потока показывать, чтобы панель оставалась читаемой.
#: Величина выбрана по времени отрисовки: четыре тысячи бит занимают около
#: шести миллисекунд, весь поток длинного сообщения — уже десятки.
BITS_SHOWN = 4096

#: Виды участков, ради которых окно показа сдвигается: именно они
#: сообщают о повреждении, и пропустить их нельзя.
FOCUS_KINDS = frozenset({"fixed", "erased", "lost"})


def monospace() -> QFont:
    font = QFont("Consolas")
    font.setStyleHint(QFont.StyleHint.Monospace)
    font.setPointSize(10)
    return font


def chip(kind: str, caption: str) -> str:
    color = COLORS.get(kind, theme.FILL_GREY)
    return (
        f'<span style="background-color:{color}">&nbsp;&nbsp;&nbsp;</span>'
        f"&nbsp;{escape(caption)}"
    )


def _window_start(kinds: list[str], limit: int) -> int:
    """Начало окна показа: обычно ноль, но повреждение перетягивает окно.

    Длинный поток целиком не показывается, и при обычном разборе
    достаточно его начала. Если же дальше в потоке есть исправленный или
    стёртый участок, показывать начало бессмысленно: пользователь не
    увидит того единственного места, ради которого и смотрит. Окно тогда
    начинается с границы строки перед первым таким участком.
    """
    if len(kinds) <= limit:
        return 0
    first = next(
        (i for i, kind in enumerate(kinds) if kind in FOCUS_KINDS), None
    )
    if first is None or first < limit:
        return 0
    start = max(0, min(first - limit // 4, len(kinds) - limit))
    return start - start % 32


def render_bits(
    bits: Bits, spans: tuple[Span, ...], limit: int = BITS_SHOWN
) -> str:
    """Собирает разметку потока: участки раскрашены, байты разделены."""
    kinds: list[str] = [""] * len(bits)
    for span in spans:
        stop = min(span.start + span.length, len(bits))
        for index in range(span.start, stop):
            kinds[index] = span.kind

    begin = _window_start(kinds, limit)
    shown = min(len(bits), begin + limit)
    parts: list[str] = []
    buffer: list[str] = []
    current = kinds[begin] if begin < shown else ""

    def flush() -> None:
        if not buffer:
            return
        text = "".join(buffer)
        buffer.clear()
        if current:
            color = COLORS.get(current, theme.FILL_GREY)
            parts.append(
                f'<span style="background-color:{color};'
                f'color:{theme.ON_FILL}">{text}</span>'
            )
        else:
            parts.append(text)

    for index in range(begin, shown):
        if kinds[index] != current:
            flush()
            current = kinds[index]
        buffer.append(str(bits[index]))
        if index + 1 == shown:
            continue
        if (index + 1) % 32 == 0:
            flush()
            parts.append("<br>")
            current = kinds[index + 1]
        elif (index + 1) % 8 == 0:
            flush()
            parts.append("&nbsp;")
            current = kinds[index + 1]
    flush()

    body = "".join(parts)
    if shown - begin < len(bits):
        where = (
            f"показаны первые {shown}"
            if begin == 0
            else f"показаны биты с {begin}-го по {shown - 1}-й"
        )
        body += (
            f'<br><span style="color:{theme.MUTED}">… всего {len(bits)} бит, '
            f"{where}</span>"
        )
    return f'<div style="line-height:150%">{body}</div>'


def render_fields(stage: Stage) -> str:
    """Собирает таблицу «из чего складывается значение»."""
    head = (
        f'<tr style="background-color:{theme.RAISED};color:{theme.MUTED}">'
        "<th align='left'>Что</th><th align='left'>Значение</th>"
        "<th align='left'>Размер</th><th align='left'>Биты</th></tr>"
    )
    rows = []
    for item in stage.fields:
        bits = "&nbsp;".join(
            "".join(str(bit) for bit in item.bits[i:i + 8])
            for i in range(0, len(item.bits), 8)
        )
        rows.append(
            "<tr>"
            f"<td>{escape(item.name)}</td>"
            f"<td>{escape(item.value)}</td>"
            f"<td align='right'>{escape(item.width)}</td>"
            f'<td><span style="background-color:{theme.FILL_TEAL};'
            f'color:{theme.ON_FILL}">'
            f"{bits}</span></td>"

            "</tr>"
        )
    return (
        '<table cellspacing="0" cellpadding="3" width="100%">'
        + head
        + "".join(rows)
        + "</table>"
    )


def render_table(stage: Stage) -> str:
    """Собирает произвольную таблицу шага: заголовки и строки."""
    head = (
        f'<tr style="background-color:{theme.RAISED};color:{theme.MUTED}">'
        + "".join(f"<th align='left'>{escape(name)}</th>" for name in stage.headers)
        + "</tr>"
    )
    rows = "".join(
        "<tr>"
        + "".join(f"<td>{escape(cell)}</td>" for cell in row)
        + "</tr>"
        for row in stage.rows
    )
    return (
        '<table cellspacing="0" cellpadding="3" width="100%">'
        + head
        + rows
        + "</table>"
    )


def render_sections(stage: Stage) -> str:
    """Собирает данные шага, разбитые на озаглавленные куски.

    Так показываются кадры: все сразу, с подписью над каждым. Границы
    видны по заголовкам, и переключать между ними нечего.
    """
    parts: list[str] = []
    for index, section in enumerate(stage.sections):
        top = "0" if index == 0 else "12px"
        parts.append(
            f'<p style="color:{theme.ACCENT};margin:{top} 0 2px 0">'
            f"<b>{escape(section.title)}</b></p>"
        )
        if section.note:
            parts.append(
                f'<p style="color:{theme.MUTED};margin:0 0 4px 0">'
                f"{escape(section.note)}</p>"
            )
        if section.bits:
            parts.append(render_bits(section.bits, section.spans))
    return "".join(parts)


def render_empty(text: str) -> str:
    """Короткое пояснение вместо данных."""
    return f'<p style="color:{theme.MUTED}">{escape(text)}</p>'
