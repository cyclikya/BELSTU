#include <windows.h>
#include <iostream>
#include <string>
#include <vector>
#include <ctime>
#include <conio.h>

struct ThreadData {
    int threadId;
    int priority;
    int iterationsCompleted;
    bool finished;
};

DWORD WINAPI ThreadFunction(LPVOID lpParam) {
    ThreadData* data = static_cast<ThreadData*>(lpParam);
    clock_t startTime = clock();

    const int TOTAL_ITERATIONS = 1000000;
    const int REPORT_INTERVAL = 1000;
    const int DELAY_MS = 200;

    DWORD processId = GetCurrentProcessId();
    DWORD threadId = GetCurrentThreadId();

    SetThreadPriority(GetCurrentThread(), data->priority);
    int actualPriority = GetThreadPriority(GetCurrentThread());

    std::cout << "Запущен поток " << data->threadId << ". Установленный уровень: " << actualPriority << std::endl;

    for (int i = 0; i <= TOTAL_ITERATIONS; i++) {
        data->iterationsCompleted = i;

        if (i % REPORT_INTERVAL == 0) {
            DWORD currentProcessor = GetCurrentProcessorNumber();
            std::cout << "Шаг выполнения: " << i
                << " - ИД процесса: " << processId
                << " - ИД потока: " << threadId
                << " - Уровень процесса: " << actualPriority
                << " - Процессор: " << currentProcessor << std::endl;

            Sleep(DELAY_MS);
        }
    }

    clock_t endTime = clock();
    double elapsedTime = static_cast<double>(endTime - startTime) / CLOCKS_PER_SEC;

    std::cout << "Поток " << data->threadId << " завершил работу. Затраченное время: " << elapsedTime << "с" << std::endl;

    data->finished = true;
    return 0;
}

std::string threadPriorityToString(int priority) {
    switch (priority) {
    case THREAD_PRIORITY_IDLE: return "IDLE";
    case THREAD_PRIORITY_LOWEST: return "LOWEST";
    case THREAD_PRIORITY_BELOW_NORMAL: return "BELOW_NORMAL";
    case THREAD_PRIORITY_NORMAL: return "NORMAL";
    case THREAD_PRIORITY_ABOVE_NORMAL: return "ABOVE_NORMAL";
    case THREAD_PRIORITY_HIGHEST: return "HIGHEST";
    case THREAD_PRIORITY_TIME_CRITICAL: return "TIME_CRITICAL";
    default: return "UNKNOWN";
    }
}

int main(int argc, char* argv[]) {
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    if (argc != 5) {
        std::cout << "Формат команды: Lab-05c.exe <маска_процессоров> <уровень_процесса> <уровень_потока1> <уровень_потока2>" << std::endl;
        std::cout << "Уровни потоков: -15=Фоновый, -2=Пониженный, 0=Обычный, 2=Повышенный, 15=Наивысший" << std::endl;
        std::cout << "\nНажмите любую клавишу для выхода..." << std::endl;
        _getch();
        return 1;
    }

    DWORD_PTR affinityMask = std::stoul(argv[1]);
    DWORD processPriority = std::stoul(argv[2]);
    int threadPriority1 = std::stoi(argv[3]);
    int threadPriority2 = std::stoi(argv[4]);

    std::cout << "Маска распределения: " << affinityMask << std::endl;
    std::cout << "Уровень основного процесса: " << processPriority << std::endl;
    std::cout << "Уровень выполнения потока 1: " << threadPriorityToString(threadPriority1) << std::endl;
    std::cout << "Уровень выполнения потока 2: " << threadPriorityToString(threadPriority2) << std::endl;

    SetPriorityClass(GetCurrentProcess(), processPriority);
    SetProcessAffinityMask(GetCurrentProcess(), affinityMask);

    ThreadData data1 = { 1, threadPriority1, 0, false };
    ThreadData data2 = { 2, threadPriority2, 0, false };

    HANDLE threads[2];
    threads[0] = CreateThread(NULL, 0, ThreadFunction, &data1, 0, NULL);
    threads[1] = CreateThread(NULL, 0, ThreadFunction, &data2, 0, NULL);

    if (threads[0] && threads[1]) {
        WaitForMultipleObjects(2, threads, TRUE, INFINITE);

        std::cout << "\nСтатистика выполнения" << std::endl;
        std::cout << "Выполнено шагов потоком 1: " << data1.iterationsCompleted << std::endl;
        std::cout << "Выполнено шагов потоком 2: " << data2.iterationsCompleted << std::endl;
        std::cout << "Разница в выполнении: " << abs(data1.iterationsCompleted - data2.iterationsCompleted) << std::endl;

        CloseHandle(threads[0]);
        CloseHandle(threads[1]);
    }
    else {
        std::cout << "Ошибка создания потоков" << std::endl;
    }

    std::cout << "\nНажмите любую клавишу для выхода..." << std::endl;
    _getch();

    return 0;
}
