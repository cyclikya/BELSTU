#pragma once
#include "CMatrix.h"
#include "CRectD.h"
#include "Task1_SpaceToWindow.h"
#include "Task2_MyPen.h"
#include <afxwin.h>

// ========== ЗАДАНИЕ 3 ==========
// Класс для отображения графиков и фигур
class CPlot2D {
    CMatrix X;
    CMatrix Y;
    CMatrix K;
    CRect RW;
    CRectD RS;
    CMyPen PenLine;
    CMyPen PenAxis;

public:
    CPlot2D() { K.RedimMatrix(3, 3); }

    void SetParams(CMatrix& XX, CMatrix& YY, CRect& RWX);
    void SetWindowRect(CRect& RWX);
    void GetWindowCoords(double xs, double ys, int& xw, int& yw);

    void SetPenLine(CMyPen& PLine) { PenLine = PLine; }
    void SetPenAxis(CMyPen& PAxis) { PenAxis = PAxis; }

    void Draw(CDC& dc, int Ind1, int Ind2);
};
