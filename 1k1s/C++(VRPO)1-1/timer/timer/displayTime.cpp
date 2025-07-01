#include <iostream>
#include <iomanip>
#include "displayTime.h"
using namespace std;

void displayTime(int remainingTime) {
    int hours = remainingTime / 3600;
    int minutes = (remainingTime % 3600) / 60;
    int seconds = remainingTime % 60;

    cout << setw(203) << setfill(' ') << "�������� �������: ";
    cout << setw(2) << setfill('0') << hours << ":" << setw(2) << setfill('0') << minutes << ":" << setw(2) << setfill('0') << seconds << endl;
}