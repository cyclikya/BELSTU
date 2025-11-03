//cl /EHsc /O2 lab-05a.cpp /link /OUT:lab-05a.exe
#include <iostream>
#include <windows.h>
#include <bitset>

using namespace std;

int main() {
    setlocale(LC_ALL, "Russian");

    DWORD processId = GetCurrentProcessId();
    DWORD threadId = GetCurrentThreadId();

    HANDLE hProcess = GetCurrentProcess();
    HANDLE hThread = GetCurrentThread();

    DWORD processPriorityClass = GetPriorityClass(hProcess);
    int threadPriority = GetThreadPriority(hThread);

    DWORD_PTR processAffinityMask, systemAffinityMask;
    GetProcessAffinityMask(hProcess, &processAffinityMask, &systemAffinityMask);

    SYSTEM_INFO sysInfo;
    GetSystemInfo(&sysInfo);
    DWORD cpuCount = sysInfo.dwNumberOfProcessors;
    DWORD cpuNumber = GetCurrentProcessorNumber();

    cout << "Идентификатор процесса: " << processId << endl;
    cout << "Идентификатор потока: " << threadId << endl;
    cout << "Класс приоритета процесса: " << processPriorityClass << endl;
    cout << "Приоритет потока: " << threadPriority << endl;
    cout << "Маска родственности процесса: " << bitset<8>(processAffinityMask) << endl;
    cout << "Системная маска родственности: " << bitset<8>(systemAffinityMask) << endl;
    cout << "Доступно процессоров: " << cpuCount << endl;
    cout << "Номер процессора для потока: " << cpuNumber << endl;

    return 0;
}
