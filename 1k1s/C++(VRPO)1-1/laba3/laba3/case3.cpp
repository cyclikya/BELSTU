#include "case3.h"
using namespace std;

int case3()
{
    string numbs;
    cout << "Введите цифры: ";
    cin >> numbs;

    for (char numb : numbs) {
        if (isdigit(numb)) {
            int dgtNumb = numb;
            cout << "Код символа для цифры " << numb << ": " << dgtNumb << endl;
        }
        else {
            cout << "Неверный ввод" << endl;
        }
    }
    return 0;
}