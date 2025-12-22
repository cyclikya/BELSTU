#include "AcceptServer.h"
#include <process.h>
#include <atomic>
#include <ctime>
#include <Ws2tcpip.h>
#include "Global.h"

static std::map<int, SOCKET> g_listenSockets;
static std::atomic<int> g_clientIdCounter{ 1 };
std::atomic<DWORD> g_lastAcceptTime{ 0 };

std::atomic<bool> g_acceptEnabled{ true };
std::atomic<bool> g_serverRunning{ true };

std::atomic<int> g_totalConnections{ 0 };
std::atomic<int> g_activeConnections{ 0 };
std::atomic<int> g_timeoutConnections{ 0 };

char g_callsign[64];
int  g_udpPort;
int  g_port;
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

    /*while (true)
    {
        SOCKET client = accept(listenSock, nullptr, nullptr);

        // обработка в блокирующем и неблокирующем режимах
        if (client == INVALID_SOCKET)
            if (WSAGetLastError() == WSAEWOULDBLOCK) {
                Sleep(10);
                continue;
            }
            else {
                break;
            }

        g_lastAcceptTime = GetTickCount();

        std::cout << "[ACCEPT] New client accepted\n";
        CreateThread(nullptr, 0, ClientThread, (LPVOID)client, 0, nullptr);
    }*/

    while (g_serverRunning)
    {
        if (!g_acceptEnabled)
        {
            Sleep(100);
            continue;
        }

        SOCKET client = accept(listenSock, nullptr, nullptr);
        if (client == INVALID_SOCKET)
            continue;

        g_lastAcceptTime = GetTickCount();

        g_totalConnections++;
        g_activeConnections++;

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

DWORD WINAPI ResponseServer(LPVOID)
{
    SOCKET s = socket(AF_INET, SOCK_DGRAM, 0);

    BOOL bcast = TRUE;
    setsockopt(s, SOL_SOCKET, SO_BROADCAST, (char*)&bcast, sizeof(bcast));

    sockaddr_in addr{};
    addr.sin_family = AF_INET;
    addr.sin_port = htons(g_udpPort);
    addr.sin_addr.s_addr = INADDR_ANY;

    bind(s, (sockaddr*)&addr, sizeof(addr));

    char buf[128];
    sockaddr_in from{};
    int fromlen = sizeof(from);

    while (true)
    {
        int n = recvfrom(s, buf, sizeof(buf) - 1, 0,
            (sockaddr*)&from, &fromlen);

        if (n > 0)
        {
            buf[n] = 0;
            if (strcmp(buf, g_callsign) == 0)
            {
                char ip[INET_ADDRSTRLEN];
                InetNtopA(AF_INET, &from.sin_addr, ip, INET_ADDRSTRLEN);
                std::cout << "[UDP] Callsign request from " << ip << "\n";

                char reply[128];
                sprintf_s(reply, "HELLO %d", g_udpPort);
                sendto(s, reply, strlen(reply), 0,
                    (sockaddr*)&from, fromlen);
            }
        }
    }
}

DWORD WINAPI InactivityTimerThread(LPVOID)
{
    const DWORD TIMEOUT = 180000; // 3 минуты = 180 000 мс

    while (true)
    {
        Sleep(5000); // проверяем раз в 5 секунд

        DWORD now = GetTickCount();
        DWORD last = g_lastAcceptTime.load();

        if (now - last > TIMEOUT)
        {
            std::cout << "\n[TIMER] Нет подключений более 3 минут!\n";
            std::cout << "[TIMER] Сервер простаивает.\n\n";

            g_lastAcceptTime = now; // чтобы не спамило
        }
    }
}