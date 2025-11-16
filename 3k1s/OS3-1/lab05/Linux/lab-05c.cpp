#include <iostream>
#include <stdlib.h>
#include <unistd.h>
#include <pthread.h>
#include <sys/syscall.h>
#include <sched.h>
#include <time.h>
#include <sys/resource.h>
#include <string>
#include <locale.h>

typedef struct {
    int threadId;
    int niceValue;
    int iterationsCompleted;
    int finished;
} ThreadData;

void* ThreadFunction(void* lpParam) {
    ThreadData* data = (ThreadData*)lpParam;
    clock_t startTime = clock();

    const int TOTAL_ITERATIONS = 1000000;
    const int REPORT_INTERVAL = 1000;
    const int DELAY_US = 200000;

    pid_t processId = getpid();
    pid_t threadId = syscall(SYS_gettid);

    if (setpriority(PRIO_PROCESS, 0, data->niceValue) == -1) {
        perror("setpriority");
    }
    int actualNice = getpriority(PRIO_PROCESS, 0);

    std::cout << "Поток " << data->threadId << " запущен. Nice: " << actualNice << std::endl;

    for (int i = 0; i <= TOTAL_ITERATIONS; i++) {
        data->iterationsCompleted = i;

        if (i % REPORT_INTERVAL == 0) {
            int currentProcessor = sched_getcpu();
            std::cout << "Поток " << data->threadId << " | Итерация: " << i
                << " | PID: " << processId << " | TID: " << threadId
                << " | Nice: " << actualNice << " | CPU: " << currentProcessor << std::endl;

            usleep(DELAY_US);
        }
    }

    clock_t endTime = clock();
    double elapsedTime = (double)(endTime - startTime) / CLOCKS_PER_SEC;

    std::cout << "Поток " << data->threadId << " завершен. Время: " << elapsedTime << "с" << std::endl;

    data->finished = 1;
    return NULL;
}

int main(int argc, char* argv[]) {
    setlocale(LC_ALL, "ru_RU.UTF-8");

    if (argc != 4) {
        std::cout << "Использование: ./Lab-05c <маска_родственности> <nice1> <nice2>" << std::endl;
        std::cout << "Маска родственности: -1 - все процессоры, 0 - CPU0, 1 - CPU1, и т.д." << std::endl;
        std::cout << "Значения nice: от -20 (высший) до 19 (низший)" << std::endl;
        std::cout << "Нажмите Enter для выхода...";
        std::cin.get();
        return 1;
    }

    int cpu_mask = atoi(argv[1]);
    int nice1 = atoi(argv[2]);
    int nice2 = atoi(argv[3]);

    std::cout << "Маска родственности: " << cpu_mask << std::endl;
    std::cout << "Nice потока 1: " << nice1 << std::endl;
    std::cout << "Nice потока 2: " << nice2 << std::endl;

    if (cpu_mask != -1) {
        cpu_set_t set;
        CPU_ZERO(&set);
        CPU_SET(cpu_mask, &set);
        if (sched_setaffinity(0, sizeof(cpu_set_t), &set) == -1) {
            perror("sched_setaffinity");
            return 1;
        }
    }

    ThreadData data1 = { 1, nice1, 0, 0 };
    ThreadData data2 = { 2, nice2, 0, 0 };

    pthread_t threads[2];
    pthread_create(&threads[0], NULL, ThreadFunction, &data1);
    pthread_create(&threads[1], NULL, ThreadFunction, &data2);

    pthread_join(threads[0], NULL);
    pthread_join(threads[1], NULL);

    std::cout << "Итерации потока 1: " << data1.iterationsCompleted << std::endl;
    std::cout << "Итерации потока 2: " << data2.iterationsCompleted << std::endl;
    std::cout << "Разница в итерациях: " << abs(data1.iterationsCompleted - data2.iterationsCompleted) << std::endl;

    std::cout << "Нажмите Enter для выхода...";
    std::cin.get();

    return 0;
}