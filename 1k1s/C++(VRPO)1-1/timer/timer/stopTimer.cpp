#include <conio.h>
#include "stopTimer.h"
using namespace std;

void stopTimer(bool& isStopped) {
    if (_kbhit()) { // ���������, ���� �� ������ �������
        char key = _getch(); // ���������� ���� ������� ������� ��� ��� ������ � �������
        if (key == 13) { //���� ������ ������� � ����� 13(Entrer) isPaused ������ �������� �� true
            isStopped = !isStopped;
        }
    }
}