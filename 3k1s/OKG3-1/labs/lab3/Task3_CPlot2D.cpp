#include "Task3_CPlot2D.h"

// Установка параметров графика
void CPlot2D::SetParams(CMatrix& XX, CMatrix& YY, CRect& RWX) {
    X = XX;
    Y = YY;
    RW = RWX;

    RS.left = XX.Min();
    RS.right = XX.Max();
    RS.top = YY.Max();
    RS.bottom = YY.Min();

    K = SpaceToWindow(RS, RW);
}

void CPlot2D::SetWindowRect(CRect& RWX) {
    RW = RWX;
    K = SpaceToWindow(RS, RW);
}

void CPlot2D::GetWindowCoords(double xs, double ys, int& xw, int& yw) {
    double x = K(0, 0) * xs + K(0, 1) * ys + K(0, 2);
    double y = K(1, 0) * xs + K(1, 1) * ys + K(1, 2);
    xw = (int)x;
    yw = (int)y;
}

// Рисование графика
void CPlot2D::Draw(CDC& dc, int Ind1, int Ind2) {
    // оси координат
    CPen penAxis(PenAxis.PenStyle, PenAxis.PenWidth, PenAxis.PenColor);
    CPen* oldPen = dc.SelectObject(&penAxis);

    int x0, y0, x1, y1;
    GetWindowCoords(RS.left, 0, x0, y0);
    GetWindowCoords(RS.right, 0, x1, y1);
    dc.MoveTo(x0, y0); dc.LineTo(x1, y1);

    GetWindowCoords(0, RS.top, x0, y0);
    GetWindowCoords(0, RS.bottom, x1, y1);
    dc.MoveTo(x0, y0); dc.LineTo(x1, y1);

    dc.SelectObject(oldPen);

    // сама функция
    CPen penLine(PenLine.PenStyle, PenLine.PenWidth, PenLine.PenColor);
    oldPen = dc.SelectObject(&penLine);

    for (int i = Ind1; i <= Ind2; i++) {
        int xw, yw;
        GetWindowCoords(X(i), Y(i), xw, yw);
        if (i == Ind1) dc.MoveTo(xw, yw);
        else dc.LineTo(xw, yw);
    }

    dc.SelectObject(oldPen);
}
