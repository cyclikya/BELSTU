#include "Global.h"
#include "AcceptServer.h"
#include "PipeServer.h"
int main(int argc, char* argv[])
{
    setlocale(LC_ALL, "Russian");
    int port = DEFAULT_PORT;
    char callsign[64] = SERVER_CALLSIGN;
    int udpPort = DEFAULT_UDP_PORT;
    char serverName[64] = SERVER_NAME;
    char pipeName[64] = PIPE_NAME;

    // Обработка аргументов командной строки
    for (int i = 1; i < argc; i++) {
        if (strcmp(argv[i], "-p") == 0 && i + 1 < argc) {
            port = atoi(argv[++i]);
        }
        else if (strcmp(argv[i], "-c") == 0 && i + 1 < argc) {
            strcpy_s(callsign, argv[++i]);
        }
        else if (strcmp(argv[i], "-up") == 0 && i + 1 < argc) {
            udpPort = atoi(argv[++i]);
        }
        else if (strcmp(argv[i], "-n") == 0 && i + 1 < argc) {
            strcpy_s(serverName, argv[++i]);
        }
        else if (strcmp(argv[i], "-h") == 0 || strcmp(argv[i], "--help") == 0) {
            std::cout << "Использование:\n"
                << "  Server.exe [-p <tcp_порт>] [-c <позывной>] [-up <udp_порт>] [-n <имя_сервера>]\n"
                << "Примеры:\n"
                << "  Server.exe -p 2000 -c HELLO -up 2000\n"
                << "  Server.exe -p 3000 -c TEST\n";
            return 0;
        }
    }

    WSADATA wsa;
    WSAStartup(MAKEWORD(2, 2), &wsa);

    std::cout << "Concurrent Server started\n";
    std::cout << "Server name: " << serverName << "\n";
    std::cout << "Callsign: " << callsign << "\n";
    std::cout << "Pipe: " << pipeName << "\n";
    std::cout << "TCP Port: " << port << "\n";
    std::cout << "UDP Search Port: " << udpPort << "\n";

    strcpy_s(g_callsign, callsign);
    g_udpPort = udpPort;
    g_port = port;

    g_lastAcceptTime = GetTickCount();

    StartAccept(port); // tcp server запуск
    StartPipeServer(); // udp server запуск

    CreateThread(nullptr, 0, ResponseServer, nullptr, 0, nullptr); // удп поиск по позывному

    CreateThread(nullptr, 0, InactivityTimerThread, nullptr, 0, nullptr); // удп поиск по позывному

    std::cout << "\nСервер запущен. Состояние:\n";
    std::cout << "  • TCP сервер слушает порт " << port << "\n";
    std::cout << "  • UDP поиск по позывному '" << callsign << "' на порту " << udpPort << "\n";
    std::cout << "  • Именованный канал: \\\\.\\pipe\\" << pipeName << "\n";
    std::cout << "  • Используйте RConsole для управления\n\n";

    while (true) {
        Sleep(1000);
    }

    StopAllAccept();
    StopPipeServer();
    WSACleanup();
    return 0;
}