#include <windows.h>
#include <iostream>
#include <vector>
#include <string>
#include <cctype>
#include <cstdlib>

std::vector<char> extract_letters() {
    const char* name = std::getenv("USERNAME");
    if (!name) name = "Unknown";

    std::vector<char> out;
    for (int i = 0; name[i]; i++)
        if (std::isalpha((unsigned char)name[i]))
            out.push_back(name[i]);

    if (out.empty()) out.push_back('X');
    return out;
}

int run_loop(const char* pname) {
    auto letters = extract_letters();
    int L = letters.size();

    HANDLE hMutex = OpenMutexA(MUTEX_ALL_ACCESS, FALSE, "Global\\LAB06_MUTEX");
    if (!hMutex)
        hMutex = CreateMutexA(NULL, FALSE, "Global\\LAB06_MUTEX");

    for (int i = 1; i <= 90; i++) {

        if (i == 30)
            WaitForSingleObject(hMutex, INFINITE);

        char c = letters[(i - 1) % L];
        std::cout << "[" << pname << "] Iteration " << i
            << " Char: " << c << std::endl;

        if (i == 60)
            ReleaseMutex(hMutex);

        Sleep(100);
    }

    return 0;
}

int main(int argc, char* argv[]) {

    if (argc > 1) {
        return run_loop(argv[1]);
    }

    STARTUPINFOA si{};
    PROCESS_INFORMATION piA{};
    PROCESS_INFORMATION piB{};
    si.cb = sizeof(si);

    CreateProcessA(
        NULL,
        (LPSTR)"Lab-06b.exe A",
        NULL, NULL, FALSE,
        CREATE_NEW_CONSOLE,
        NULL, NULL,
        &si, &piA
    );

    CreateProcessA(
        NULL,
        (LPSTR)"Lab-06b.exe B",
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
