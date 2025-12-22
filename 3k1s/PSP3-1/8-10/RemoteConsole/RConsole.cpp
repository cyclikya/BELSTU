#define _CRT_SECURE_NO_WARNINGS
#include <windows.h>
#include <iostream>
#include <string>

using namespace std;

int main()
{
    setlocale(LC_ALL, "Russian");

    cout << "RConsole started\n";

    string serverName;
    cout << "Введите имя сервера: ";
    cin >> serverName;
    cin.ignore();

    string pipePath = "\\\\" + serverName + "\\pipe\\cpipe";

    HANDLE hPipe = CreateFileA(
        pipePath.c_str(),
        GENERIC_READ | GENERIC_WRITE,
        0,
        nullptr,
        OPEN_EXISTING,
        0,
        nullptr
    );

    if (hPipe == INVALID_HANDLE_VALUE)
    {
        cout << "Ошибка подключения к серверу. Код: "
            << GetLastError() << endl;
        return 1;
    }

    cout << "Подключено к серверу\n";
    cout << "Команды:\n";
    cout << "  OPEN_ACCEPT <port>\n";
    cout << "  CLOSE_ACCEPT <port>\n";
    cout << "  EXIT\n\n";

    while (true)
    {
        string cmd;
        cout << "> ";
        getline(cin, cmd);

        if (cmd.empty())
            continue;

        DWORD written = 0;
        if (!WriteFile(
            hPipe,
            cmd.c_str(),
            (DWORD)cmd.size() + 1,
            &written,
            nullptr))
        {
            cout << "Ошибка отправки команды\n";
            break;
        }

        if (cmd == "EXIT")
            break;
    }

    CloseHandle(hPipe);
    cout << "RConsole завершена\n";
    return 0;
}
