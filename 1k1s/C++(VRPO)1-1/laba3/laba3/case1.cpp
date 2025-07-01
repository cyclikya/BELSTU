#include "case1.h"
using namespace std;

int case1()
{
    string symbols;
    cout << "Символы латинского алфавита: ";
    cin >> symbols;
    for (char symbol : symbols) {
        if (islower(symbol) || isupper(symbol)) {
            int lower = tolower(symbol);
            int upper = toupper(symbol);
            int diff = lower - upper;
            cout << endl << "Разница значений у кодов для символа " << symbol << ": " << diff << endl;
        }
        else {
            cout << endl << "Неверный ввод" << endl;
        }
    }
    return 0;
}
