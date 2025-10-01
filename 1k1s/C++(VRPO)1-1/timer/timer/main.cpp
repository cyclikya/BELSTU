#include <iostream>
#include <Windows.h> //для функций system, sleep, beep
#include <iomanip> // для форматирования времени и расположения текста
#include <conio.h>
#include "displayTime.h"
#include "displayPausedTime.h"
#include "getRemainingTime.h"
#include "pauseTimer.h"
#include "stopTimer.h"
#include "anotherTimer.h"
using namespace std;

int main() {
    system("title Консольный таймер"); // изменение названия окна 
    system("mode con cols=44 lines=10"); // изменение размера окна 

    setlocale(LC_ALL, "Russian");

setTimer: //метку (label) для goto
    int remainingTime = getRemainingTime(); //===функция получения оставшегося времени
    bool isPaused = false; //для паузы
    bool isStopped = false; // для 

    while (remainingTime > 0) { 
        pauseTimer(isPaused); //===проверка нажатия пробела

        if (!isPaused) { // не останавливали
            system("cls"); //очистка консоли
            displayTime(remainingTime); //===отображение времени
            Sleep(1000); //задержка на 1000 милисекунд(секунду)
            remainingTime--; // уменьшение времемни на 1 величину (секунду)
        }
        else { //остановили
            system("cls");
            displayPausedTime(remainingTime);//===функция отображения времени на паузе
            Sleep(1000); // выход из оператора через 1000 милисекунд(секунду)
        }

        stopTimer(isStopped); //===проверка нажатия Enter

        if (isStopped) {
            remainingTime = 0;
        }
    }

    system("cls");

    cout << setw(206) << setfill(' ') << "Таймер завершен!" << endl;

    for (int i = 0; i < 2; i++) {
        Beep(800, 300);
        Beep(800, 300);
        Beep(800, 300);
        Beep(800, 300);
        Beep(800, 300);
        Sleep(1500);
    }

    system("cls");
setAnotherTimer:    
    cout << "Хотите установить ещё один таймер?\n1 - Да.\n2 - Нет.\n";
    int key = anotherTimer();
    switch (key) {
        case 1: {
            system("cls");
            goto setTimer;
        }
        case 2: {
            system("cls");
            cout << setw(28) << setfill(' ') << "До свидания!";
            return 0;
        }
        default: {
            cout << "Ошибка! Необходимо ввести цифру от 1 до 2.\n";
            goto setAnotherTimer;
        }
    }
}