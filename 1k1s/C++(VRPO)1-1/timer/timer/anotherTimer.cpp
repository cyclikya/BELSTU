#include <iostream>
#include <string> // работа со строками
#include "anotherTimer.h"
using namespace std;

bool isNum(const string& str) //проверка символов на тип данных
{
    for (char const& c : str) { // Проверка всех символов в str
        if (!isdigit(c)) // isdigit проверка является ли "c" числом
            return false;
    }
    return true;
}

int anotherTimer() {
    string key;
    while (true) {
        cout << "Введите цифру (от 1 до 2): ";
        cin >> key;
        if (isNum(key)) {
            break;
        }
        else {
            system("cls");
            cout << "Ошибка! Необходимо вводить только цифры. Попробуйте снова.\n";
            cout << "Хотите установить ещё один таймер?\n1 - Да.\n2 - Нет.\n";
        }
    }
    return stoi(key); //stoi перевод данных из строки в число
}