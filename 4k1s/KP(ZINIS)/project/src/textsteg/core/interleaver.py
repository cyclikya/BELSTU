r"""Блочный (матричный) перемежитель.

Перемежение решает задачу, с которой помехоустойчивый код не справляется
самостоятельно: борьбу с пакетными ошибками. Редактирование текста
повреждает не разрозненные биты, а связные участки — удаление одного слова
уничтожает сразу несколько подряд идущих бит. Код Хэмминга исправляет одну
ошибку в кодовом слове, поэтому пакет из нескольких ошибок, целиком попавший
в одно слово, делает его невосстановимым.

Перемежитель переставляет биты так, что соседние в контейнере биты
принадлежат разным кодовым словам. Тогда пакет ошибок распределяется по
нескольким словам, оставляя в каждом не более одной ошибки.

Принцип работы: d кодовых слов длиной n записываются в матрицу d x n
построчно, а считываются по столбцам.

    запись по строкам              чтение по столбцам
    +---------------------+
    | a1 a2 a3 ... an |   слово 1      a1 b1 c1 ... | a2 b2 c2 ... | ...
    | b1 b2 b3 ... bn |   слово 2      \___________/   \___________/
    | c1 c2 c3 ... cn |   слово 3        столбец 1       столбец 2
    |       ...       |
    +---------------------+
      d строк = глубина перемежения

Пакет длиной B оставляет в каждом кодовом слове не более ceil(B / d) ошибок.
Отсюда гарантированная оценка максимального исправляемого пакета:

    B_max = d * t

где t — число ошибок, исправляемых кодом в одном блоке. Для расширенного
кода Хэмминга (8,4) t = 1, поэтому B_max = d.

Избыточности перемежитель не добавляет: скорость кода не меняется, биты
только переставляются. Платой служит буферизация — прежде чем начать
встраивание, необходимо накопить целый блок из d * n бит.
"""

from __future__ import annotations

import numpy as np

from .bits import Bits
from .hamming import N as CODEWORD_LENGTH
from .hamming import T as CORRECTABLE_ERRORS

#: Глубины перемежения, исследуемые в работе. Значение 1 соответствует
#: отсутствию перемежения и служит базой для сравнения.
STUDIED_DEPTHS: tuple[int, ...] = (1, 4, 8, 16, 32)


class BlockInterleaver:
    """Блочный перемежитель заданной глубины.

    Объект работает с одним блоком фиксированного размера. Разбиение потока
    на блоки и дополнение последнего блока выполняет вызывающий код
    (модуль framing), поскольку именно он расставляет синхромаркеры на
    границах блоков.
    """

    def __init__(self, depth: int, word_length: int = CODEWORD_LENGTH) -> None:
        if depth < 1:
            raise ValueError("глубина перемежения должна быть не меньше 1")
        if word_length < 1:
            raise ValueError("длина кодового слова должна быть положительной")

        self.depth = depth
        self.word_length = word_length

    @property
    def block_size(self) -> int:
        """Размер блока перемежителя в битах: d * n."""
        return self.depth * self.word_length

    @property
    def max_burst(self) -> int:
        """Гарантированно исправляемая длина пакета ошибок: B_max = d * t."""
        return self.depth * CORRECTABLE_ERRORS

    @property
    def is_identity(self) -> bool:
        """Глубина 1 означает отсутствие перемежения."""
        return self.depth == 1

    def interleave(self, block: Bits) -> Bits:
        """Перемежение блока: запись по строкам, чтение по столбцам."""
        self._check_size(block)
        if self.is_identity:
            return list(block)

        matrix = np.array(block, dtype=np.uint8).reshape(
            self.depth, self.word_length
        )
        return [int(b) for b in matrix.T.reshape(-1)]

    def deinterleave(self, block: Bits) -> Bits:
        """Обратная перестановка: восстанавливает исходный порядок бит.

        Принятая последовательность заполняет матрицу по столбцам, после
        чего считывается по строкам — операция, обратная перемежению.
        """
        self._check_size(block)
        if self.is_identity:
            return list(block)

        matrix = np.array(block, dtype=np.uint8).reshape(
            self.word_length, self.depth
        )
        return [int(b) for b in matrix.T.reshape(-1)]

    def _check_size(self, block: Bits) -> None:
        if len(block) != self.block_size:
            raise ValueError(
                f"блок перемежителя должен содержать {self.block_size} бит "
                f"(d={self.depth} x n={self.word_length}), получено {len(block)}"
            )

    def __repr__(self) -> str:
        return (
            f"BlockInterleaver(depth={self.depth}, "
            f"word_length={self.word_length}, block_size={self.block_size})"
        )
