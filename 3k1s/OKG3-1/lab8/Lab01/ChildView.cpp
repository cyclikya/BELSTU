
// ChildView.cpp: реализация класса CChildView
//

#include "stdafx.h"
#include "Lab06.h"
#include "ChildView.h"
#include <string>
#include <fstream>
#include <iostream>

using namespace std;

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CChildView

CChildView::CChildView()
{		
	Index = 0;	// Индикатор для OnPaint
	PView.resize(3);
	PView(0) = 10;
	PView(1) = 45;
	PView(2) = 45;
}

CChildView::~CChildView()
{
}

// Реализация карты сообщений
BEGIN_MESSAGE_MAP(CChildView, CWnd)
	ON_WM_PAINT()
	// сообщения меню выбора
	ON_COMMAND(ID_CPlot3D_FUNC1, &CChildView::OnCPlot3DDef)
	ON_COMMAND(ID_CPlot3D_FUNC2, &CChildView::OnCplot3dFunc1)
	ON_COMMAND(ID_CPLOT3D_FUNC3, &CChildView::OnCplot3dFunc2)
	ON_WM_KEYDOWN()
	ON_COMMAND(ID_FUNCTION_32777, &CChildView::DefaultCameraPosition)
	ON_COMMAND(ID_FUNCTION_32776, &CChildView::SetCameraPosition)
END_MESSAGE_MAP()



// Обработчики сообщений CChildView

BOOL CChildView::PreCreateWindow(CREATESTRUCT& cs) 
{
	if (!CWnd::PreCreateWindow(cs))
		return FALSE;

	cs.dwExStyle |= WS_EX_CLIENTEDGE;
	cs.style &= ~WS_BORDER;
	cs.lpszClass = AfxRegisterWndClass(CS_HREDRAW|CS_VREDRAW|CS_DBLCLKS, 
		::LoadCursor(nullptr, IDC_ARROW), reinterpret_cast<HBRUSH>(COLOR_WINDOW+1), nullptr);

	return TRUE;
}

void CChildView::OnPaint()
{
	CPaintDC dc(this); // контекст устройства для рисования

	CString ss;

	if (Index == 1)
		Graph1.Draw(dc);
	if (Index == 2)
		Graph2.Draw(dc);
	if (Index == 3)
		Graph3.Draw(dc);
	if (Index > 0)
	{
		ss.Format(L"f=%5.1f,  q=%5.1f", PView(1), PView(2));
		dc.TextOutW(5, 5, ss);
	}
}

void CChildView::OnCPlot3DDef()			// функция Function1
{
	double dx = 0.25, dy = 0.25;		// шаг для рисования
	CRectD SpaceRect(-5, 5, 5, -5);		// область для рисования
	CRect  WinRect;
	this->GetClientRect(WinRect);		// получаем координаты окна
	WinRect.SetRect(WinRect.left + 50, WinRect.top + 50, WinRect.right - 50, WinRect.bottom - 50);	// устанавливаем координаты окна
	Graph1.SetFunction(Function1, SpaceRect, dx, dy);		// устанавливаем Function1
	Graph1.SetViewPoint(PView(0), PView(1), PView(2));		// устанавливаем положение наблюдателя
	Graph1.SetWinRect(WinRect);								// устанавливаем область в окне для рисования
	Index = 1;
	this->Invalidate();
}

void CChildView::OnCplot3dFunc1()		// функция Function2
{
	double dx = 0.25, dy = 0.25;
	CRectD SpaceRect(-5, 5, 5, -5);
	CRect  WinRect;
	this->GetClientRect(WinRect);
	WinRect.SetRect(WinRect.left + 50, WinRect.top + 50, WinRect.right - 50, WinRect.bottom - 50);
	Graph2.SetFunction(Function2, SpaceRect, dx, dy);	
	Graph2.SetViewPoint(PView(0), PView(1), PView(2));
	Graph2.SetWinRect(WinRect);
	Index = 2;
	this->Invalidate();
}

void CChildView::OnCplot3dFunc2()  // Функция Function3
{
	double dx = 0.05, dy = 0.05;
	CRectD SpaceRect(-3, 3, 3, -3);
	CRect  WinRect;
	this->GetClientRect(WinRect);
	WinRect.SetRect(WinRect.left + 50, WinRect.top + 50, WinRect.right - 50, WinRect.bottom - 50);
	Graph3.SetFunction(Function3, SpaceRect, dx, dy);  // Function3 
	Graph3.SetViewPoint(PView(0), PView(1), PView(2));
	Graph3.SetWinRect(WinRect);
	Index = 3;
	this->Invalidate();
}

void CChildView::DefaultCameraPosition()
{
	PView(0) = 10;
	PView(1) = 45;
	PView(2) = 45;

	switch (Index)
	{
		case 1:
			Graph1.SetViewPoint(PView(0), PView(1), PView(2));
			break;
		case 2:
			Graph2.SetViewPoint(PView(0), PView(1), PView(2));
			break;
		case 3:
			Graph3.SetViewPoint(PView(0), PView(1), PView(2));
			break;
	}

	Invalidate();
}


void CChildView::SetCameraPosition()
{
	std::string line;

	int fi = 0;
	int teta = 0;

	std::ifstream in("D:\\лабы\\ОКГ\\8_2\\lab_08\\Debug\\camera_position.txt");

	if (in.is_open())
	{
		std::getline(in, line);
		fi = stoi(line);

		std::getline(in, line);
		teta = stoi(line);
	}
	in.close();

	PView(1) = fi;
	PView(2) = teta;

	switch (Index)
	{
	case 1:
		Graph1.SetViewPoint(PView(0), PView(1), PView(2));
		break;
	case 2:
		Graph2.SetViewPoint(PView(0), PView(1), PView(2));
		break;
	case 3:
		Graph3.SetViewPoint(PView(0), PView(1), PView(2));
		break;
	}

	Invalidate();
}






































void CChildView::OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags)
{
	return;
	CMatrix P(3);
	switch (Index)
	{
	case 1:
	{
		P = Graph1.GetViewPoint();
		break;
	}
	case 2:
	{
		P = Graph2.GetViewPoint();
		break;
	}
	case 3:
	{
		P = Graph3.GetViewPoint();
		break;
	}
	}

	PView(0) = P(0), PView(1) = P(1), PView(2) = P(2);
	double delta_fi = 10, delta_q = 10;
	switch (nChar)
	{
	case VK_UP:
	{
		double qx = PView(2) - delta_q;
		if (qx >= 0)PView(2) = qx;
		break;
	}
	case VK_DOWN:
	{
		double qx = PView(2) + delta_q;
		if (qx <= 180)PView(2) = qx;
		break;
	}
	case VK_LEFT:
	{
		double fix = PView(1) - delta_fi;
		if (fix >= 0)PView(1) = fix;
		break;
	}
	case VK_RIGHT:
	{
		double fix = PView(1) + delta_fi;
		if (fix <= 360)PView(1) = fix;
		break;
	}
	}

	switch (Index)
	{
	case 1:
	{
		Graph1.SetViewPoint(PView(0), PView(1), PView(2));
		break;
	}

	case 2:
	{
		Graph2.SetViewPoint(PView(0), PView(1), PView(2));
		break;
	}

	case 3:
	{
		Graph3.SetViewPoint(PView(0), PView(1), PView(2));
		break;
	}
	}
	CWnd::OnKeyDown(nChar, nRepCnt, nFlags);
	this->Invalidate();
}

