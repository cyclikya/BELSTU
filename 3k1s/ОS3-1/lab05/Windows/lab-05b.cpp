#include <iostream>
#include <windows.h>

using namespace std;

DWORD parsePriorityClass(const char* name) {
    if (_stricmp(name, "IDLE_PRIORITY_CLASS") == 0) return IDLE_PRIORITY_CLASS;
    if (_stricmp(name, "BELOW_NORMAL_PRIORITY_CLASS") == 0) return BELOW_NORMAL_PRIORITY_CLASS;
    if (_stricmp(name, "NORMAL_PRIORITY_CLASS") == 0) return NORMAL_PRIORITY_CLASS;
    if (_stricmp(name, "ABOVE_NORMAL_PRIORITY_CLASS") == 0) return ABOVE_NORMAL_PRIORITY_CLASS;
    if (_stricmp(name, "HIGH_PRIORITY_CLASS") == 0) return HIGH_PRIORITY_CLASS;
    if (_stricmp(name, "REALTIME_PRIORITY_CLASS") == 0) return REALTIME_PRIORITY_CLASS;
    return NORMAL_PRIORITY_CLASS;
}

int main(int argc, char* argv[]) {
    setlocale(LC_ALL, "Russian");

    if (argc < 4) {
        cout << "Использование: lab-05b <mask> <prio1> <prio2>" << endl;
        return 1;
    }

    DWORD_PTR mask = atoi(argv[1]);
    DWORD prio1 = parsePriorityClass(argv[2]);
    DWORD prio2 = parsePriorityClass(argv[3]);

    cout << "Маска: " << mask << " | Приоритеты: " << prio1 << ", " << prio2 << endl;

    STARTUPINFOA si1 = { sizeof(si1) }, si2 = { sizeof(si2) };
    PROCESS_INFORMATION pi1, pi2;

    CreateProcessA("lab-05x.exe", NULL, NULL, NULL, FALSE, CREATE_NEW_CONSOLE, NULL, NULL, &si1, &pi1);
    SetPriorityClass(pi1.hProcess, prio1);
    SetProcessAffinityMask(pi1.hProcess, mask);

    CreateProcessA("lab-05x.exe", NULL, NULL, NULL, FALSE, CREATE_NEW_CONSOLE, NULL, NULL, &si2, &pi2);
    SetPriorityClass(pi2.hProcess, prio2);
    SetProcessAffinityMask(pi2.hProcess, mask);

    WaitForSingleObject(pi1.hProcess, INFINITE);
    WaitForSingleObject(pi2.hProcess, INFINITE);
    return 0;
}
