#include <iostream>
#include <unistd.h>
#include <sys/wait.h>
#include <sched.h>
#include <string>
#include <cstdlib>
#include <cstring>
#include <locale.h>

void printUsage() {
    std::cout << "Использование: ./Lab-05b <маска_родственности> <nice1> <nice2>" << std::endl;
    std::cout << "Маска родственности: -1 - все процессоры, 0 - CPU0, 1 - CPU1, и т.д." << std::endl;
    std::cout << "Значения nice: от -20 (высший) до 19 (низший)" << std::endl;
}

int main(int argc, char* argv[]) {
    setlocale(LC_ALL, "ru_RU.UTF-8");

    if (argc != 4) {
        printUsage();
        std::cout << "Нажмите Enter для выхода...";
        std::cin.get();
        return 1;
    }

    int cpu_mask = std::stoi(argv[1]);
    int nice1 = std::stoi(argv[2]);
    int nice2 = std::stoi(argv[3]);

    std::cout << "Маска родственности: " << cpu_mask << std::endl;
    std::cout << "Nice процесса 1: " << nice1 << std::endl;
    std::cout << "Nice процесса 2: " << nice2 << std::endl;

    pid_t pid1 = fork();
    if (pid1 == 0) {
        if (cpu_mask != -1) {
            cpu_set_t set;
            CPU_ZERO(&set);
            CPU_SET(cpu_mask, &set);
            if (sched_setaffinity(0, sizeof(cpu_set_t), &set) == -1) {
                perror("sched_setaffinity");
                exit(1);
            }
        }

        if (setpriority(PRIO_PROCESS, 0, nice1) == -1) {
            perror("setpriority");
            exit(1);
        }

        execl("./Lab-05x", "Lab-05x", NULL);
        perror("execl");
        exit(1);
    }

    pid_t pid2 = fork();
    if (pid2 == 0) {
        if (cpu_mask != -1) {
            cpu_set_t set;
            CPU_ZERO(&set);
            CPU_SET(cpu_mask, &set);
            if (sched_setaffinity(0, sizeof(cpu_set_t), &set) == -1) {
                perror("sched_setaffinity");
                exit(1);
            }
        }

        if (setpriority(PRIO_PROCESS, 0, nice2) == -1) {
            perror("setpriority");
            exit(1);
        }

        execl("./Lab-05x", "Lab-05x", NULL);
        perror("execl");
        exit(1);
    }

    std::cout << "Процесс 1 запущен с PID: " << pid1 << std::endl;
    std::cout << "Процесс 2 запущен с PID: " << pid2 << std::endl;

    std::cout << "Оба процесса запущены успешно!" << std::endl;
    std::cout << "Для мониторинга используйте команды:" << std::endl;
    std::cout << "ps -p " << pid1 << "," << pid2 << " -o pid,ni,psr,comm" << std::endl;
    std::cout << "cat /proc/" << pid1 << "/status | grep -i cpu" << std::endl;
    std::cout << "cat /proc/" << pid2 << "/status | grep -i cpu" << std::endl;

    int status1, status2;
    waitpid(pid1, &status1, 0);
    waitpid(pid2, &status2, 0);

    std::cout << "Оба процесса завершены." << std::endl;

    std::cout << "Нажмите Enter для выхода...";
    std::cin.get();

    return 0;
}