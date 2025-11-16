#include <windows.h>
#include <iostream>
#include <ctime>
#include <string>
#include <conio.h>

std::string priorityClassToString(DWORD priorityClass) {
    switch (priorityClass) {
    case IDLE_PRIORITY_CLASS: return "IDLE";
    case BELOW_NORMAL_PRIORITY_CLASS: return "BELOW_NORMAL";
    case NORMAL_PRIORITY_CLASS: return "NORMAL";
    case ABOVE_NORMAL_PRIORITY_CLASS: return "ABOVE_NORMAL";
    case HIGH_PRIORITY_CLASS: return "HIGH";
    case REALTIME_PRIORITY_CLASS: return "REALTIME";
    default: return "UNKNOWN";
    }
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

int main() {
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    clock_t startTime = clock();
    const int TOTAL_ITERATIONS = 1000000;
    const int REPORT_INTERVAL = 1000;
    const int DELAY_MS = 200;

    DWORD processId = GetCurrentProcessId();
    DWORD threadId = GetCurrentThreadId();
    DWORD processPriorityClass = GetPriorityClass(GetCurrentProcess());
    int threadPriority = GetThreadPriority(GetCurrentThread());

    for (int i = 0; i <= TOTAL_ITERATIONS; i++) {
        if (i % REPORT_INTERVAL == 0) {
            DWORD currentProcessor = GetCurrentProcessorNumber();
            std::cout << "Шаг выполнения: " << i
                << " - ИД процесса: " << processId
                << " - ИД потока: " << threadId
                << " - Уровень процесса: " << priorityClassToString(processPriorityClass)
                << " - Уровень потока: " << threadPriorityToString(threadPriority)
                << " - Процессор: " << currentProcessor << std::endl;

            Sleep(DELAY_MS);
        }
    }

    clock_t endTime = clock();
    double elapsedTime = static_cast<double>(endTime - startTime) / CLOCKS_PER_SEC;

    std::cout << "Программа Lab-05x - Выполнение завершено" << std::endl;
    std::cout << "Общая продолжительность работы: " << elapsedTime << " секунд" << std::endl;

    std::cout << "\nНажмите любую клавишу для выхода..." << std::endl;
    _getch();

    return 0;
}
