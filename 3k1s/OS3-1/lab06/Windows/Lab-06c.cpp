#include <windows.h>
#include <iostream>
#include <vector>
#include <string>
#include <cctype>
#include <cstdlib>

std::vector<char> extract_letters() {
    const char* name = std::getenv("USERNAME");
    if (!name) name = "Unknown";

    std::vector<char> v;
    for (int i = 0; name[i]; i++)
        if (std::isalpha((unsigned char)name[i]))
            v.push_back(name[i]);

    if (v.empty()) v.push_back('X');
    return v;
}

int run_loop(const char* procName) {
    auto letters = extract_letters();
    int L = letters.size();

    HANDLE hSem = OpenSemaphoreA(SEMAPHORE_ALL_ACCESS, FALSE, "Global\\LAB06_SEM");
    if (!hSem)
        hSem = CreateSemaphoreA(NULL, 1, 1, "Global\\LAB06_SEM");

    for (int i = 1; i <= 90; i++) {

        if (i == 30)
            WaitForSingleObject(hSem, INFINITE);

        char c = letters[(i - 1) % L];
        std::cout << "[" << procName << "] Iteration " << i
            << " Char: " << c << std::endl;

        if (i == 60)
            ReleaseSemaphore(hSem, 1, NULL);

        Sleep(100);
    }

    return 0;
}

int main(int argc, char* argv[]) {

    if (argc > 1) {
        return run_loop(argv[1]);
    }

    STARTUPINFOA si{};
    PROCESS_INFORMATION piA{}, piB{};
    si.cb = sizeof(si);

    CreateProcessA(
        NULL,
        (LPSTR)"Lab-06c.exe A",
        NULL, NULL, FALSE,
        CREATE_NEW_CONSOLE,
        NULL, NULL,
        &si, &piA
    );

    CreateProcessA(
        NULL,
        (LPSTR)"Lab-06c.exe B",
        NULL, NULL, FALSE,
        CREATE_NEW_CONSOLE,
        NULL, NULL,
        &si, &piB
    );

    run_loop("MAIN");

    WaitForSingleObject(piA.hProcess, INFINITE);
    WaitForSingleObject(piB.hProcess, INFINITE);

    CloseHandle(piA.hProcess);
    CloseHandle(piA.hThread);
    CloseHandle(piB.hProcess);
    CloseHandle(piB.hThread);

    return 0;
}
