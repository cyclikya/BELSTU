#include <iostream>
#include <string> 
#include <iomanip>
#include "getRemainingTime.h"
using namespace std;

bool isNumber(const string& str)
{
    for (char const& ch : str) { // �������� ���� �������� � str
        if (!isdigit(ch))
            return false;
    }
    return true;
}

int getRemainingTime() {
    string hours, minutes, seconds;
    while (true) {
        cout << "������� ���������� �����: ";
        cin >> hours;
        if (isNumber(hours)) {
            break;
        }
        else {
            system("cls");
            cout << "������! ���������� ������� ������ �����.\n���������� �����.\n";
        }
    }
    while (true) {
        cout << "������� ���������� �����: ";
        cin >> minutes;
        if (isNumber(minutes)) {
            break;
        }
        else {
            system("cls");
            cout << "������� ���������� �����: " << hours << endl;
            cout << "������! ���������� ������� ������ �����.\n���������� �����.\n";
        }
    }
    while (true) {
        cout << "������� ���������� ������: ";
        cin >> seconds;
        if (isNumber(seconds)) {
            break;
        }
        else {
            system("cls");
            cout << "������� ���������� �����: " << hours << endl;
            cout << "������� ���������� �����: " << minutes << endl;
            cout << "������! ���������� ������� ������ �����.\n���������� �����.\n";
        }
    }

    return stoi(hours) * 3600 + stoi(minutes) * 60 + stoi(seconds); // stoi - string to integer
}