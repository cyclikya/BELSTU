#include <iostream>
#include <string> // ������ �� ��������
#include "anotherTimer.h"
using namespace std;

bool isNum(const string& str) //�������� �������� �� ��� ������
{
    for (char const& c : str) { // �������� ���� �������� � str
        if (!isdigit(c)) // isdigit �������� �������� �� "c" ������
            return false;
    }
    return true;
}

int anotherTimer() {
    string key;
    while (true) {
        cout << "������� ����� (�� 1 �� 2): ";
        cin >> key;
        if (isNum(key)) {
            break;
        }
        else {
            system("cls");
            cout << "������! ���������� ������� ������ �����. ���������� �����.\n";
            cout << "������ ���������� ��� ���� ������?\n1 - ��.\n2 - ���.\n";
        }
    }
    return stoi(key); //stoi ������� ������ �� ������ � �����
}