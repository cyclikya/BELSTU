#include <iostream>
#include <windows.h>
#include <thread>
#include <ctime>

using namespace std;

void threadFunc() {
    DWORD pid = GetCurrentProcessId();
    DWORD tid = GetCurrentThreadId();
    HANDLE hThread = GetCurrentThread();

    for (int i = 1; i <= 1000000; i++) {
        if (i % 1000 == 0) {
            Sleep(200);
            cout << "Итерация: " << i
                << " | PID=" << pid
                << " | TID=" << tid
                << " | Приоритет=" << GetThreadPriority(hThread)
                << " | CPU=" << GetCurrentProcessorNumber() << endl;
        }
    }
}

int main(int argc, char* argv[]) {
    setlocale(LC_ALL, "Russian");

    if (argc < 5) {
        cout << "Использование: lab-05c <mask> <proc_prio> <thr1_prio> <thr2_prio>" << endl;
        return 1;
    }

    DWORD_PTR mask = atoi(argv[1]);
    int procPrio = atoi(argv[2]);
    int thr1Prio = atoi(argv[3]);
    int thr2Prio = atoi(argv[4]);

    cout << "Параметры: mask=" << mask << ", proc=" << procPrio
        << ", thr1=" << thr1Prio << ", thr2=" << thr2Prio << endl;

    SetPriorityClass(GetCurrentProcess(), procPrio);
    SetProcessAffinityMask(GetCurrentProcess(), mask);

    thread t1([]() { SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_NORMAL); threadFunc(); });
    thread t2([]() { SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_HIGHEST); threadFunc(); });

    t1.join();
    t2.join();
    return 0;
}
