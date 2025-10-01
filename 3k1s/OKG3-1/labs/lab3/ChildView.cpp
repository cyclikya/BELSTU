#include "ChildView.h"
#include "Task3_CPlot2D.h"
#include <math.h>

// ===== Функции из условия =====
double CChildView::MyF1(double x) { return sin(x); }
double CChildView::MyF2(double x) { return cos(x); }

// ===== Обработка меню =====
void CChildView::OnTestsF1() {
    CMatrix X(1, 100), Y(1, 100);
    for (int i = 0;i < 100;i++) {
        double x = -3.14 + i * 0.063;
        X(i) = x;
        Y(i) = MyF1(x);
    }

    CRect rw(50, 50, 450, 450);
    CPlot2D plot;
    plot.SetParams(X, Y, rw);

    CMyPen pline; pline.Set(PS_SOLID, 1, RGB(255, 0, 0));
    CMyPen paxis; paxis.Set(PS_SOLID, 2, RGB(0, 0, 255));

    plot.SetPenLine(pline);
    plot.SetPenAxis(paxis);

    CClientDC dc(this);
    plot.Draw(dc, 0, 99);
}

void CChildView::OnTestsF2() {
    CMatrix X(1, 100), Y(1, 100);
    for (int i = 0;i < 100;i++) {
        double x = -3.14 + i * 0.063;
        X(i) = x;
        Y(i) = MyF2(x);
    }

    CRect rw(50, 50, 450, 450);
    CPlot2D plot;
    plot.SetParams(X, Y, rw);

    CMyPen pline; pline.Set(PS_DASHDOT, 3, RGB(255, 0, 0));
    CMyPen paxis; paxis.Set(PS_SOLID, 2, RGB(0, 0, 0));

    plot.SetPenLine(pline);
    plot.SetPenAxis(paxis);

    CClientDC dc(this);
    plot.Draw(dc, 0, 99);
}

void CChildView::OnTestsF3() {
    CMatrix X(1, 9), Y(1, 9);
    double R = 10;
    for (int i = 0;i < 8;i++) {
        double angle = i * M_PI / 4;
        X(i) = R * cos(angle);
        Y(i) = R * sin(angle);
    }
    X(8) = X(0);
    Y(8) = Y(0);

    CRect rw(50, 50, 450, 450);
    CPlot2D plot;
    plot.SetParams(X, Y, rw);

    CMyPen pline; pline.Set(PS_SOLID, 3, RGB(255, 0, 0));
    CMyPen paxis; paxis.Set(PS_SOLID, 2, RGB(0, 0, 255));

    plot.SetPenLine(pline);
    plot.SetPenAxis(paxis);

    CClientDC dc(this);
    plot.Draw(dc, 0, 8);

    // окружность
    CPen circlePen(PS_SOLID, 2, RGB(0, 0, 255));
    CPen* oldPen = dc.SelectObject(&circlePen);
    int xc, yc;
    plot.GetWindowCoords(0, 0, xc, yc);
    dc.Ellipse(xc - 100, yc - 100, xc + 100, yc + 100);
    dc.SelectObject(oldPen);
}
