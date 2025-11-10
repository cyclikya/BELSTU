#include <windows.h>
#include <iostream>
#include <bitset>
#include <string>
#include <conio.h>

std::string affinityMaskToString(DWORD_PTR mask) {
    std::string result;
    for (int i = sizeof(DWORD_PTR) * 8 - 1; i >= 0; i--) {
        result += (mask & (1ULL << i)) ? '1' : '0';
        if (i % 8 == 0 && i != 0) result += ' ';
    }
    return result;
}

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

    DWORD processId = GetCurrentProcessId();
    DWORD threadId = GetCurrentThreadId();
    DWORD processPriorityClass = GetPriorityClass(GetCurrentProcess());
    int threadPriority = GetThreadPriority(GetCurrentThread());

    DWORD_PTR processAffinity, systemAffinity;
    GetProcessAffinityMask(GetCurrentProcess(), &processAffinity, &systemAffinity);

    SYSTEM_INFO systemInfo;
    GetSystemInfo(&systemInfo);

    DWORD currentProcessor = GetCurrentProcessorNumber();

    std::cout << "Идентификатор процесса: " << processId << std::endl;
    std::cout << "Идентификатор потока: " << threadId << std::endl;
    std::cout << "Уровень процесса: " << priorityClassToString(processPriorityClass) << std::endl;
    std::cout << "Уровень потока: " << threadPriorityToString(threadPriority) << std::endl;
    std::cout << "Маска родственности процесса: " << affinityMaskToString(processAffinity) << std::endl;
    std::cout << "Системная маска родственности: " << affinityMaskToString(systemAffinity) << std::endl;
    std::cout << "Доступно процессоров: " << systemInfo.dwNumberOfProcessors << std::endl;
    std::cout << "Текущий процессор: " << currentProcessor << std::endl;

    std::cout << "\nНажмите любую клавишу для выхода..." << std::endl;
    _getch(); return 0;
}
