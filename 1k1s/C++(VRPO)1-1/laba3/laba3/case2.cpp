#include "case2.h"
#include <iostream>
#include <clocale>
#include <cwctype>
using namespace std;


int case2()
{
    string symbols2;
    cout << "������� �������� ��������: ";
    cin >> symbols2;

    for (char symbol2 : symbols2) {
        if (symbol2 >= '�' && symbol2 <= '�')
        {
            int lower = tolower(symbol2);
            int upper = toupper(symbol2);
            int diff = lower - upper;
            cout << "������� � Windows-1251 ��� ������� " << symbol2 << ": " << diff << endl;
        }
        else {
            cout << endl << "�������� ����" << endl;
        }
    }
    return 0;
}