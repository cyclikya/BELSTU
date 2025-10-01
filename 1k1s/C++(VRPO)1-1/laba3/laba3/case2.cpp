#include "case2.h"
#include <iostream>
#include <clocale>
#include <cwctype>
using namespace std;


int case2()
{
    string symbols2;
    cout << "Символы русского алфавита: ";
    cin >> symbols2;

    for (char symbol2 : symbols2) {
        if (symbol2 >= 'А' && symbol2 <= 'я')
        {
            int lower = tolower(symbol2);
            int upper = toupper(symbol2);
            int diff = lower - upper;
            cout << "Разница в Windows-1251 для символа " << symbol2 << ": " << diff << endl;
        }
        else {
            cout << endl << "Неверный ввод" << endl;
        }
    }
    return 0;
}