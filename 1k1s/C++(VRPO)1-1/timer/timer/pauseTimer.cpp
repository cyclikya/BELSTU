#include <conio.h> // ��� ������������� _kbhit
#include "pauseTimer.h"
using namespace std;

void pauseTimer(bool& isPaused) {
    if (_kbhit()) { // ���������, ���� �� ������ �������
        char key = _getch(); // ���������� ���� ������� ������� ��� ��� ������ � �������
        if (key == 32) { //���� ������ ������� � ����� 32(������) isPaused ������ �������� �� false
            isPaused = !isPaused;
        }
    }
}