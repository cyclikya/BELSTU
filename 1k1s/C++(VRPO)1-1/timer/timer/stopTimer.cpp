#include <conio.h>
#include "stopTimer.h"
using namespace std;

void stopTimer(bool& isStopped) {
    if (_kbhit()) { // Проверяет, была ли нажата клавиша
        char key = _getch(); // Считывание кода нажатой клавиши без его вывода в консоль
        if (key == 13) { //если нажата клавиша с кодом 13(Entrer) isPaused меняет значение на true
            isStopped = !isStopped;
        }
    }
}