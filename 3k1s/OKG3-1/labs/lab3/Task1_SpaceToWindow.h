#pragma once
#include "CMatrix.h"
#include "CRectD.h"
#include <afxwin.h>

// ========== ЗАДАНИЕ 1 ==========
// Функция пересчёта координат из мировой системы в оконную
CMatrix SpaceToWindow(CRectD& rs, CRect& rw);
