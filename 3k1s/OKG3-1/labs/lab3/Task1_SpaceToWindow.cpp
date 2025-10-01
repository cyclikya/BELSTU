#include "Task1_SpaceToWindow.h"

// ========== ЗАДАНИЕ 1 ==========
// Реализация функции SpaceToWindow
CMatrix SpaceToWindow(CRectD& rs, CRect& rw) {
    CMatrix K(3, 3);

    double sx = (double)(rw.right - rw.left) / (rs.right - rs.left);
    double sy = (double)(rw.bottom - rw.top) / (rs.bottom - rs.top);

    K(0, 0) = sx;   K(0, 1) = 0;    K(0, 2) = rw.left - rs.left * sx;
    K(1, 0) = 0;    K(1, 1) = -sy;  K(1, 2) = rw.top + rs.bottom * sy;
    K(2, 0) = 0;    K(2, 1) = 0;    K(2, 2) = 1;

    return K;
}
