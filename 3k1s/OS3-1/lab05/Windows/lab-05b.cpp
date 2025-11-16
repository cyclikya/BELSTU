#include <windows.h>
#include <iostream>
#include <string>
#include <sstream>
#include <conio.h>

DWORD parsePriorityClass(int priority) {
    switch (priority) {
    case 32: return NORMAL_PRIORITY_CLASS;
    case 64: return BELOW_NORMAL_PRIORITY_CLASS;
    case 128: return ABOVE_NORMAL_PRIORITY_CLASS;
    case 256: return HIGH_PRIORITY_CLASS;
    case 16384: return REALTIME_PRIORITY_CLASS;
    default: return NORMAL_PRIORITY_CLASS;
    }
}

std::string priorityClassToString(DWORD priorityClass) {
    switch (priorityClass) {
    case NORMAL_PRIORITY_CLASS: return "NORMAL";
    case BELOW_NORMAL_PRIORITY_CLASS: return "BELOW_NORMAL";
    case ABOVE_NORMAL_PRIORITY_CLASS: return "ABOVE_NORMAL";
    case HIGH_PRIORITY_CLASS: return "HIGH";
    case REALTIME_PRIORITY_CLASS: return "REALTIME";
    default: return "UNKNOWN";
    }
}

int main(int argc, char* argv[]) {
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    if (argc != 4) {
        std::cout << "Формат команды: Lab-05b.exe <маска_процессоров> <уровень1> <уровень2>" << std::endl;
        std::cout << "Доступные уровни: 32=Обычный, 64=Ниже обычного, 256=Высокий" << std::endl;

        _getch();
        return 1;
    }

    DWORD_PTR affinityMask = std::stoul(argv[1]);
    DWORD priorityClass1 = parsePriorityClass(std::stoi(argv[2]));
    DWORD priorityClass2 = parsePriorityClass(std::stoi(argv[3]));

    std::cout << "Установленная маска: " << affinityMask << std::endl;
    std::cout << "Уровень доступа процесса 1: " << priorityClassToString(priorityClass1) << std::endl;
    std::cout << "Уровень доступа процесса 2: " << priorityClassToString(priorityClass2) << std::endl;

    // Получаем системную информацию для определения маски всех процессоров
    SYSTEM_INFO sysInfo;
    GetSystemInfo(&sysInfo);
    DWORD_PTR allProcessorsMask = (1 << sysInfo.dwNumberOfProcessors) - 1;

    // Корректируем маску если указаны "все процессоры"
    if (affinityMask == 0 || affinityMask > allProcessorsMask) {
        affinityMask = allProcessorsMask;
    }

    // Создание первого дочернего процесса
    STARTUPINFOA si1 = { sizeof(si1) };
    PROCESS_INFORMATION pi1;
    std::string cmdLine1 = "Lab-05x.exe";

    if (CreateProcessA(NULL, &cmdLine1[0], NULL, NULL, FALSE,
        CREATE_NEW_CONSOLE | priorityClass1, NULL, NULL, &si1, &pi1)) {
        SetProcessAffinityMask(pi1.hProcess, affinityMask);
        CloseHandle(pi1.hThread);
        CloseHandle(pi1.hProcess);
        std::cout << "Процесс 1 запущен с PID: " << pi1.dwProcessId << std::endl;
    }
    else {
        std::cout << "Ошибка запуска процесса 1. Код: " << GetLastError() << std::endl;
    }

    // Создание второго дочернего процесса
    STARTUPINFOA si2 = { sizeof(si2) };
    PROCESS_INFORMATION pi2;
    std::string cmdLine2 = "Lab-05x.exe";

    if (CreateProcessA(NULL, &cmdLine2[0], NULL, NULL, FALSE,
        CREATE_NEW_CONSOLE | priorityClass2, NULL, NULL, &si2, &pi2)) {
        SetProcessAffinityMask(pi2.hProcess, affinityMask);
        CloseHandle(pi2.hThread);
        CloseHandle(pi2.hProcess);
        std::cout << "Процесс 2 запущен с PID: " << pi2.dwProcessId << std::endl;
    }
    else {
        std::cout << "Ошибка запуска процесса 2. Код: " << GetLastError() << std::endl;
    }

    std::cout << "Оба процесса запущены. Нажмите любую клавишу для выхода..." << std::endl;
    _getch();

    return 0;
}
