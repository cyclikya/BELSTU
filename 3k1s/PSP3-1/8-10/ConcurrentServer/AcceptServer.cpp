#include "AcceptServer.h"
#include <process.h>
#include <atomic>
#include <ctime>

static std::map<int, SOCKET> g_listenSockets;
static std::atomic<int> g_clientIdCounter{ 1 };

DWORD WINAPI ClientThread(LPVOID param)
{
    int clientId = g_clientIdCounter++;
    SOCKET client = (SOCKET)param;

    std::cout << "[CLIENT " << clientId << "] Connected\n";

    char service[50] = {};
    int rc = recv(client, service, sizeof(service), 0);

    if (rc == 0)
    {
        std::cout << "[CLIENT " << clientId << "] Disconnected before service select\n";
        closesocket(client);
        return 0;
    }
    if (rc == SOCKET_ERROR)
    {
        std::cout << "[CLIENT " << clientId << "] Error recv service: "
            << WSAGetLastError() << "\n";
        closesocket(client);
        return 0;
    }

    // подтверждаем выбранный сервис
    send(client, service, strlen(service) + 1, 0);

    char buffer[50]{};

    while (true)
    {
        rc = recv(client, buffer, sizeof(buffer), 0);

        if (rc == 0, WSAGetLastError() == 10054)
        {
            std::cout << "[CLIENT " << clientId << "] Client disconnect\n";
            break;
        }

        if (rc == SOCKET_ERROR)
        {
            int err = WSAGetLastError();
            std::cout << "[CLIENT " << clientId << "] Aborted connection, error: "
                << err << "\n";
            break;
        }

        // ===== СЕРВИСЫ =====
        if (strcmp(service, "Echo") == 0)
        {
            send(client, buffer, strlen(buffer) + 1, 0);
        }
        else if (strcmp(service, "Time") == 0)
        {
            time_t t = time(nullptr);
            char tm[50];
            ctime_s(tm, sizeof(tm), &t);
            send(client, tm, strlen(tm) + 1, 0);
        }
        else if (strcmp(service, "Rand") == 0)
        {
            int r = rand() % 100;
            char msg[50];
            sprintf_s(msg, "%d", r);
            send(client, msg, strlen(msg) + 1, 0);
        }
    }

    closesocket(client);
    std::cout << "[CLIENT " << clientId << "] Thread finished\n";
    return 0;
}

DWORD WINAPI AcceptThread(LPVOID param)
{
    SOCKET listenSock = (SOCKET)param;

    while (true)
    {
        SOCKET client = accept(listenSock, nullptr, nullptr);
        if (client == INVALID_SOCKET)
            break;

        std::cout << "[ACCEPT] New client accepted\n";
        CreateThread(nullptr, 0, ClientThread, (LPVOID)client, 0, nullptr);
    }
    return 0;
}

SOCKET StartAccept(int port)
{
    if (g_listenSockets.count(port))
        return g_listenSockets[port];

    SOCKET s = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

    sockaddr_in addr{};
    addr.sin_family = AF_INET;
    addr.sin_port = htons(port);
    addr.sin_addr.s_addr = INADDR_ANY;

    bind(s, (sockaddr*)&addr, sizeof(addr));
    listen(s, SOMAXCONN);

    CreateThread(nullptr, 0, AcceptThread, (LPVOID)s, 0, nullptr);

    g_listenSockets[port] = s;
    std::cout << "[OPEN_ACCEPT] Port " << port << " opened\n";
    return s;
}

void StopAccept(int port)
{
    if (!g_listenSockets.count(port))
        return;

    closesocket(g_listenSockets[port]);
    g_listenSockets.erase(port);
    std::cout << "[CLOSE_ACCEPT] Port " << port << " closed\n";
}

void StopAllAccept()
{
    for (auto& p : g_listenSockets)
        closesocket(p.second);

    g_listenSockets.clear();
}
