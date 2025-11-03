// lab-05c.cpp
#include <iostream>
#include <locale>
#include <clocale>
#include <pthread.h>
#include <unistd.h>
#include <sys/syscall.h>
#include <sys/resource.h>
#include <sched.h>
#include <cstring>

using namespace std;
pid_t gettid_wrap() { return (pid_t)syscall(SYS_gettid); }

struct ThreadArg {
    int thread_prio;
    const char* name;
};

void* thread_func(void* arg) {
    ThreadArg* ta = (ThreadArg*)arg;
    pid_t tid = gettid_wrap();

    // setpriority for THIS thread (tid)
    if (setpriority(PRIO_PROCESS, tid, ta->thread_prio) != 0) {
        perror("setpriority (thread)");
    }

    pid_t pid = getpid();
    for (int i = 1; i <= 1000000; ++i) {
        if (i % 1000 == 0) {
            usleep(200000);
            int nicev = getpriority(PRIO_PROCESS, tid); // may return priority for thread
            int cpu = sched_getcpu();
            cout << "[" << ta->name << "] Итерация: " << i
                << " | PID=" << pid << " | TID=" << tid
                << " | nice=" << nicev
                << " | CPU=" << cpu << endl;
            cout.flush();
        }
    }

    return nullptr;
}

int main(int argc, char* argv[]) {
    setlocale(LC_ALL, "ru_RU.UTF-8");
    if (argc < 5) {
        cerr << "Использование: lab-05c <mask> <proc_prio> <thr1_prio> <thr2_prio>\n";
        cerr << "mask: 0 - все CPU, либо битовая маска (например 1)\n";
        return 1;
    }

    unsigned long mask = strtoul(argv[1], nullptr, 0);
    int proc_prio = atoi(argv[2]);
    int thr1_prio = atoi(argv[3]);
    int thr2_prio = atoi(argv[4]);

    cout << "Параметры: mask=" << mask << " proc_prio=" << proc_prio
        << " thr1=" << thr1_prio << " thr2=" << thr2_prio << endl;

    // set process priority
    if (setpriority(PRIO_PROCESS, 0, proc_prio) != 0) perror("setpriority(proc)");

    // set process affinity if mask != 0
    if (mask != 0) {
        cpu_set_t cpus;
        CPU_ZERO(&cpus);
        for (int i = 0; i < (int)sizeof(unsigned long) * 8; ++i)
            if (mask & (1UL << i)) CPU_SET(i, &cpus);
        if (sched_setaffinity(0, sizeof(cpus), &cpus) != 0) perror("sched_setaffinity");
    }

    pthread_t t1, t2;
    ThreadArg a1{ thr1_prio, "T1" };
    ThreadArg a2{ thr2_prio, "T2" };

    pthread_create(&t1, nullptr, thread_func, &a1);
    pthread_create(&t2, nullptr, thread_func, &a2);

    pthread_join(t1, nullptr);
    pthread_join(t2, nullptr);

    return 0;
}
