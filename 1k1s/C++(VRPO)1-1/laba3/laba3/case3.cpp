#include "case3.h"
using namespace std;

int case3()
{
    string numbs;
    cout << "������� �����: ";
    cin >> numbs;

    for (char numb : numbs) {
        if (isdigit(numb)) {
            int dgtNumb = numb;
            cout << "��� ������� ��� ����� " << numb << ": " << dgtNumb << endl;
        }
        else {
            cout << "�������� ����" << endl;
        }
    }
    return 0;
}