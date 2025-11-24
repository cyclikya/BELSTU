#include <Windows.h>
#include <iostream>

bool StartChildProcess(const char* exeName, const char* args, PROCESS_INFORMATION& pi)
{
    STARTUPINFOA si;
    ZeroMemory(&si, sizeof(si));
    si.cb = sizeof(si);

    ZeroMemory(&pi, sizeof(pi));

    char cmd[256];
    sprintf_s(cmd, "%s %s", exeName, args);

    BOOL ok = CreateProcessA(
        NULL,
        cmd,
        NULL, NULL,
        FALSE,
        CREATE_NEW_CONSOLE, // <<----- отдельное окно консоли
        NULL,
        NULL,
        &si,
        &pi
    );

    return ok;
}

int main()
{
    PROCESS_INFORMATION p1, p2;

    std::cout << "Starting child 1 (60 seconds)..." << std::endl;
    if (!StartChildProcess("Lab-07x.exe", "60", p1))
    {
        std::cout << "Failed to start first child." << std::endl;
        return -1;
    }

    std::cout << "Starting child 2 (120 seconds)..." << std::endl;
    if (!StartChildProcess("Lab-07x.exe", "120", p2))
    {
        std::cout << "Failed to start second child." << std::endl;
        return -1;
    }

    HANDLE children[2] = { p1.hProcess, p2.hProcess };

    std::cout << "\nWaiting for both child processes to finish...\n";

    WaitForMultipleObjects(2, children, TRUE, INFINITE);

    std::cout << "\nBoth child processes finished!" << std::endl;

    CloseHandle(p1.hProcess);
    CloseHandle(p1.hThread);
    CloseHandle(p2.hProcess);
    CloseHandle(p2.hThread);

    return 0;
}
