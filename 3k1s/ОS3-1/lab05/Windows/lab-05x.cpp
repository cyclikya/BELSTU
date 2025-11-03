#include <iostream>
#include <windows.h>
#include <ctime>

using namespace std;

int main() {
    setlocale(LC_ALL, "Russian");

    clock_t start = clock();
    DWORD pid = GetCurrentProcessId();
    DWORD tid = GetCurrentThreadId();
    HANDLE hProcess = GetCurrentProcess();
    HANDLE hThread = GetCurrentThread();

    for (int i = 1; i <= 1000000; i++) {
        if (i % 1000 == 0) {
            Sleep(200);
            cout << "Итерация: " << i
                << " | PID=" << pid
                << " | TID=" << tid
                << " | Класс=" << GetPriorityClass(hProcess)
                << " | Приоритет=" << GetThreadPriority(hThread)
                << " | CPU=" << GetCurrentProcessorNumber() << endl;
        }
    }

    clock_t end = clock();
    double elapsed = double(end - start) / CLOCKS_PER_SEC;
    cout << "Время выполнения: " << elapsed << " сек." << endl;
    return 0;
}
