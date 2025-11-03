// lab-05a.cpp
#include <iostream>
#include <locale>
#include <clocale>
#include <unistd.h>
#include <sys/types.h>
#include <sys/syscall.h>
#include <pthread.h>
#include <sched.h>
#include <sys/time.h>
#include <sys/resource.h>
#include <vector>
#include <string>
#include <cstring>

using namespace std;

pid_t gettid_wrap() {
    return (pid_t)syscall(SYS_gettid);
}

string affinity_mask_to_string(cpu_set_t& set, int maxcpus) {
    string s;
    for (int i = 0; i < maxcpus; ++i) s += CPU_ISSET(i, &set) ? '1' : '0';
    return s;
}

int main() {
    setlocale(LC_ALL, "ru_RU.UTF-8");
    // fallback
    if (!std::setlocale(LC_ALL, "ru_RU.UTF-8")) setlocale(LC_ALL, "");

    pid_t pid = getpid();
    pid_t tid = gettid_wrap();
    pthread_t self = pthread_self();

    // nice / priority
    int nicev = getpriority(PRIO_PROCESS, 0);

    // CPU count
    int cpuCount = sysconf(_SC_NPROCESSORS_ONLN);
    // cpu where thread runs
    int cpu = sched_getcpu();

    // affinity
    cpu_set_t set;
    CPU_ZERO(&set);
    if (sched_getaffinity(0, sizeof(set), &set) != 0) {
        perror("sched_getaffinity");
    }

    cout << "Идентификатор процесса (PID): " << pid << endl;
    cout << "Идентификатор потока (TID): " << tid << endl;
    cout << "Идентификатор pthread (hex): " << (void*)self << endl;
    cout << "Уровень любезности (nice): " << nicev << endl;
    cout << "Доступно процессоров: " << cpuCount << endl;
    cout << "Номер процессора для текущего потока: " << cpu << endl;
    cout << "Маска родственности (двойная, CPU 0..N-1): " << affinity_mask_to_string(set, cpuCount) << endl;

    return 0;
}
