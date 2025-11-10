#include <iostream>
#include <unistd.h>
#include <sys/syscall.h>
#include <sched.h>
#include <ctime>
#include <string>
#include <locale.h>

int main() {
    setlocale(LC_ALL, "ru_RU.UTF-8");

    clock_t startTime = clock();
    const int TOTAL_ITERATIONS = 1000000;
    const int REPORT_INTERVAL = 1000;
    const int DELAY_US = 200000;

    pid_t processId = getpid();
    pid_t threadId = syscall(SYS_gettid);
    int niceValue = getpriority(PRIO_PROCESS, 0);

    for (int i = 0; i <= TOTAL_ITERATIONS; i++) {
        if (i % REPORT_INTERVAL == 0) {
            int currentProcessor = sched_getcpu();
            std::cout << "Итерация: " << i
                << " - ИД процесса: " << processId
                << " - ИД потока: " << threadId
                << " - Уровень любезности (nice): " << niceValue
                << " - Текущий процессор: " << currentProcessor << std::endl;

            usleep(DELAY_US);
        }
    }

    clock_t endTime = clock();
    double elapsedTime = static_cast<double>(endTime - startTime) / CLOCKS_PER_SEC;

    std::cout << "Общее время выполнения: " << elapsedTime << " секунд" << std::endl;

    std::cout << "Нажмите Enter для выхода...";
    std::cin.get();

    return 0;
}