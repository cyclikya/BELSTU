#include <iostream>
#include "case1.h"
#include "case2.h"
#include "case3.h"

int main()
{
    using namespace std;
    setlocale(LC_ALL, "Russian");

    int k, num;
    cout << "Выберите код для вывода: \n "
        << "1) Символ латинского алфавита;\n "
        << "2) Символ кириллицы;\n "
        << "3) Цифра;\n";
    cin >> k;

    switch (k){
    case 1:
        case1();
        break;

    case 2:
        case2();
        break;

    case 3:
        case3();
        break;

    case 4:
        cout << "Программа завершена" << endl;
        break;
    default:
        cout << "Некорректный ввод. Пожалуйста, выберите от 1 до 3.\n";
        break;
    }
    return 0;
}
