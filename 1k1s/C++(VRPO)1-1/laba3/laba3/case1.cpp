#include "case1.h"
using namespace std;

int case1()
{
    string symbols;
    cout << "������� ���������� ��������: ";
    cin >> symbols;
    for (char symbol : symbols) {
        if (islower(symbol) || isupper(symbol)) {
            int lower = tolower(symbol);
            int upper = toupper(symbol);
            int diff = lower - upper;
            cout << endl << "������� �������� � ����� ��� ������� " << symbol << ": " << diff << endl;
        }
        else {
            cout << endl << "�������� ����" << endl;
        }
    }
    return 0;
}
