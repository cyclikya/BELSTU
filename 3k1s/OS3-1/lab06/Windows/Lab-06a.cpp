#include <iostream>
#include <string>
#include <vector>
#include <cctype>
#include <cstdlib>
#include <windows.h>

CRITICAL_SECTION cs;

std::vector<char> extract_letters_from_username() {
    const char* name = std::getenv("USERNAME");
    if (!name) name = std::getenv("USER");
    if (!name) name = "Unknown";

    std::vector<char> letters;
    for (int i = 0; name[i] != '\0'; ++i) {
        if (std::isalpha(static_cast<unsigned char>(name[i]))) {
            letters.push_back(name[i]);
        }
    }
    if (letters.empty()) letters.push_back('X');
    return letters;
}

struct ThreadParam {
    const char* name;
    std::vector<char>* letters;
};

DWORD WINAPI ThreadFunc(LPVOID lpParam) {
    ThreadParam* p = reinterpret_cast<ThreadParam*>(lpParam);
    const char* tname = p->name;
    std::vector<char>& letters = *p->letters;

    int count = (int)letters.size();

    for (int i = 1; i <= 90; ++i) {
        if (i == 30) EnterCriticalSection(&cs);

        char ch = letters[(i - 1) % count];
        std::cout << "[" << tname << "] Iteration " << i
            << "  Char: " << ch << std::endl;

        if (i == 60) LeaveCriticalSection(&cs);

        Sleep(100);
    }
    return 0;
}

int main() {
    InitializeCriticalSection(&cs);

    auto letters = extract_letters_from_username();

    ThreadParam A{ "Thread-A", &letters };
    ThreadParam B{ "Thread-B", &letters };

    HANDLE hA = CreateThread(nullptr, 0, ThreadFunc, &A, 0, nullptr);
    HANDLE hB = CreateThread(nullptr, 0, ThreadFunc, &B, 0, nullptr);

    int count = (int)letters.size();

    for (int i = 1; i <= 90; ++i) {

        if (i == 30) EnterCriticalSection(&cs);

        char ch = letters[(i - 1) % count];
        std::cout << "[Main] Iteration " << i
            << "  Char: " << ch << std::endl;

        if (i == 60) LeaveCriticalSection(&cs);

        Sleep(100);
    }

    WaitForSingleObject(hA, INFINITE);
    WaitForSingleObject(hB, INFINITE);

    CloseHandle(hA);
    CloseHandle(hB);
    DeleteCriticalSection(&cs);
    return 0;
}
