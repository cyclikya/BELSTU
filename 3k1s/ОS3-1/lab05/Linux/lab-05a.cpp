#include <iostream>
#include <unistd.h>
#include <sched.h>
#include <sys/syscall.h>
#include <bitset>
#include <sys/resource.h>
#include <locale.h>

std::string affinityMaskToString(cpu_set_t* mask) {
    std::string result;
    for (int i = 0; i < 64; i++) {
        result += CPU_ISSET(i, mask) ? '1' : '0';
        if ((i + 1) % 8 == 0 && i != 63) result += ' ';
    }
    return result;
}

int main() {
    setlocale(LC_ALL, "ru_RU.UTF-8");

    pid_t processId = getpid();
    pid_t threadId = syscall(SYS_gettid);

    cpu_set_t processMask;
    CPU_ZERO(&processMask);
    if (sched_getaffinity(0, sizeof(cpu_set_t), &processMask) == -1) {
        perror("sched_getaffinity");
        return 1;
    }

    int numProcessors = sysconf(_SC_NPROCESSORS_ONLN);
    int currentProcessor = sched_getcpu();

    int niceValue = getpriority(PRIO_PROCESS, 0);

    std::cout << "Идентификатор процесса: " << processId << std::endl;
    std::cout << "Идентификатор потока: " << threadId << std::endl;
    std::cout << "Уровень любезности (nice): " << niceValue << std::endl;
    std::cout << "Маска родственности процесса: " << affinityMaskToString(&processMask) << std::endl;
    std::cout << "Доступно процессоров: " << numProcessors << std::endl;
    std::cout << "Текущий процессор: " << currentProcessor << std::endl;

    std::cout << "Нажмите Enter для выхода...";
    std::cin.get();

    return 0;
}