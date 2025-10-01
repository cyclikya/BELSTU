#include <conio.h> // для использования _kbhit
#include "pauseTimer.h"
using namespace std;

void pauseTimer(bool& isPaused) {
    if (_kbhit()) { // Проверяет, была ли нажата клавиша
        char key = _getch(); // Считывание кода нажатой клавиши без его вывода в консоль
        if (key == 32) { //если нажата клавиша с кодом 32(пробел) isPaused меняет значение на false
            isPaused = !isPaused;
        }
    }
}