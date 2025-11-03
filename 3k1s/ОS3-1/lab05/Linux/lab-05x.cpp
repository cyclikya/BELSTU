// lab-05x.cpp
#include <iostream>
#include <locale>
#include <clocale>
#include <ctime>
#include <unistd.h>
#include <sys/syscall.h>
#include <pthread.h>
#include <sched.h>
#include <sys/resource.h>

using namespace std;

pid_t gettid_wrap() { return (pid_t)syscall(SYS_gettid); }

int main() {
    setlocale(LC_ALL, "ru_RU.UTF-8");
    if (!setlocale(LC_ALL, "ru_RU.UTF-8")) setlocale(LC_ALL, "");

    clock_t start = clock();
    pid_t pid = getpid();
    pid_t tid = gettid_wrap();

    for (int i = 1; i <= 1000000; ++i) {
        if (i % 1000 == 0) {
            // задержка 200 ms
            usleep(200000);
            int nicev = getpriority(PRIO_PROCESS, 0);
            int cpu = sched_getcpu();
            cout << "Итерация: " << i
                << " | PID=" << pid
                << " | TID=" << tid
                << " | nice=" << nicev
                << " | CPU=" << cpu
                << endl;
            cout.flush();
        }
    }

    clock_t end = clock();
    double elapsed = double(end - start) / CLOCKS_PER_SEC;
    cout << "Время выполнения: " << elapsed << " сек." << endl;
    return 0;
}
