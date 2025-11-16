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
    for (int i = 0; name[i]; ++i)
        if (std::isalpha(static_cast<unsigned char>(name[i])))
            v.push_back(name[i]);

    if (v.empty()) v.push_back('X');
    return v;
}

int run_cycle(const char* pname, HANDLE hEvent, bool isChild) {
    auto letters = extract_letters();
    int L = (int)letters.size();

    if (isChild) {
        WaitForSingleObject(hEvent, INFINITE);
    }

    for (int i = 1; i <= 90; ++i) {
        char c = letters[(i - 1) % L];
        std::cout << "[" << pname << "] Iteration " << i
            << " Char: " << c << std::endl;
        Sleep(100);
    }

    if (hEvent) CloseHandle(hEvent);
    return 0;
}

void print_last_error(const char* where) {
    DWORD err = GetLastError();
    std::cerr << where << " failed, GetLastError() = " << err << "\n";
}

int main(int argc, char* argv[]) {
    bool isChild = false;
    std::string pname = "MAIN";
    if (argc > 1) {
        isChild = true;
        pname = argv[1];
    }

    HANDLE hEvent = OpenEventA(EVENT_MODIFY_STATE | SYNCHRONIZE, FALSE, "Global\\LAB06_EVENT");
    if (!hEvent) {
        hEvent = CreateEventA(NULL, TRUE, FALSE, "Global\\LAB06_EVENT");
        if (!hEvent) {
            print_last_error("CreateEventA");
            return 1;
        }
    }

    if (isChild) {
        return run_cycle(pname.c_str(), hEvent, true);
    }

    STARTUPINFOA si{};
    si.cb = sizeof(si);
    PROCESS_INFORMATION piA{};
    PROCESS_INFORMATION piB{};
    BOOL ok;

    std::string cmdA_str = "Lab-06d.exe A";
    std::string cmdB_str = "Lab-06d.exe B";
    std::vector<char> cmdA(cmdA_str.begin(), cmdA_str.end()); cmdA.push_back('\0');
    std::vector<char> cmdB(cmdB_str.begin(), cmdB_str.end()); cmdB.push_back('\0');

    ok = CreateProcessA(
        NULL,
        cmdA.data(),
        NULL, NULL, FALSE,
        CREATE_NEW_CONSOLE,
        NULL, NULL,
        &si, &piA
    );
    if (!ok) {
        print_last_error("CreateProcessA (A)");
        piA.hProcess = NULL;
        piA.hThread = NULL;
    }

    ok = CreateProcessA(
        NULL,
        cmdB.data(),
        NULL, NULL, FALSE,
        CREATE_NEW_CONSOLE,
        NULL, NULL,
        &si, &piB
    );
    if (!ok) {
        print_last_error("CreateProcessA (B)");
        piB.hProcess = NULL;
        piB.hThread = NULL;
    }

    auto letters = extract_letters();
    int L = (int)letters.size();
    for (int i = 1; i <= 15; ++i) {
        char c = letters[(i - 1) % L];
        std::cout << "[MAIN] Iteration " << i << " Char: " << c << std::endl;
        Sleep(100);
    }

    std::cout << "[MAIN] Sending START signal to A and B..." << std::endl;
    if (!SetEvent(hEvent)) {
        print_last_error("SetEvent");
    }

    for (int i = 16; i <= 90; ++i) {
        char c = letters[(i - 1) % L];
        std::cout << "[MAIN] Iteration " << i << " Char: " << c << std::endl;
        Sleep(100);
    }

    if (piA.hProcess) {
        WaitForSingleObject(piA.hProcess, INFINITE);
        CloseHandle(piA.hProcess);
        CloseHandle(piA.hThread);
    }
    else {
        std::cerr << "Process A was not created.\n";
    }

    if (piB.hProcess) {
        WaitForSingleObject(piB.hProcess, INFINITE);
        CloseHandle(piB.hProcess);
        CloseHandle(piB.hThread);
    }
    else {
        std::cerr << "Process B was not created.\n";
    }

    if (hEvent) CloseHandle(hEvent);

    return 0;
}
