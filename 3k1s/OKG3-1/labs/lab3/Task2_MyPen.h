#pragma once
#include <afxwin.h>

// ========== ЗАДАНИЕ 2 ==========
// Структура пера для рисования
struct CMyPen {
    int PenStyle;
    int PenWidth;
    COLORREF PenColor;

    CMyPen() {
        PenStyle = PS_SOLID;
        PenWidth = 1;
        PenColor = RGB(0, 0, 0);
    }

    void Set(int PS, int PW, COLORREF PC) {
        PenStyle = PS;
        PenWidth = PW;
        PenColor = PC;
    }
};
