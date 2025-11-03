// lab-05b.cpp
#include <iostream>
#include <unistd.h>
#include <sys/wait.h>
#include <sys/types.h>
#include <sched.h>
#include <sys/resource.h>
#include <cstring>
#include <cstdlib>

using namespace std;

void set_affinity_mask(pid_t pid, unsigned long mask) {
    cpu_set_t cpus;
    CPU_ZERO(&cpus);
    for (int i = 0; i < (int)sizeof(unsigned long) * 8; ++i) {
        if (mask & (1UL << i)) CPU_SET(i, &cpus);
    }
    if (sched_setaffinity(pid, sizeof(cpus), &cpus) != 0) {
        perror("sched_setaffinity");
    }
}

int main(int argc, char* argv[]) {
    setlocale(LC_ALL, "ru_RU.UTF-8");
    if (argc < 4) {
        cerr << "Использование: lab-05b <mask> <prio_child1> <prio_child2>\n";
        cerr << "mask: целое (битовая маска CPU, например 1 или 3). 0 означает 'все CPU'.\n";
        return 1;
    }

    unsigned long mask = strtoul(argv[1], nullptr, 0);
    int pr1 = atoi(argv[2]); // nice value for child1
    int pr2 = atoi(argv[3]); // nice value for child2

    cout << "Параметры: mask=" << mask << " pr1=" << pr1 << " pr2=" << pr2 << endl;

    if (mask == 0) {
        // 0 => keep default (all CPUs) - do nothing
    }

    pid_t p1 = fork();
    if (p1 == 0) {
        // child 1
        if (mask != 0) set_affinity_mask(0, mask);
        // set nice (may require permissions for negative values)
        if (setpriority(PRIO_PROCESS, 0, pr1) != 0) perror("setpriority child1");
        execl("./lab-05x", "lab-05x", (char*)NULL);
        perror("execl child1");
        _exit(1);
    }

    pid_t p2 = fork();
    if (p2 == 0) {
        // child 2
        if (mask != 0) set_affinity_mask(0, mask);
        if (setpriority(PRIO_PROCESS, 0, pr2) != 0) perror("setpriority child2");
        execl("./lab-05x", "lab-05x", (char*)NULL);
        perror("execl child2");
        _exit(1);
    }

    // parent: optionally set parent's affinity too (not required)
    // wait children
    int status;
    waitpid(p1, &status, 0);
    cout << "Дочерний процесс 1 завершился, код: " << WEXITSTATUS(status) << endl;
    waitpid(p2, &status, 0);
    cout << "Дочерний процесс 2 завершился, код: " << WEXITSTATUS(status) << endl;
    return 0;
}
