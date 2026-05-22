#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <stdio.h>
#include <WinSock2.h>

#pragma comment(lib, "WS2_32.lib")

int main(int argc, char* argv[])
{
    if (argc < 2)
    {
        printf("Usage: %s <AGENT_IP>\n", argv[0]);
        return -1;
    }

    WSAData wsadata;
    WSAStartup(MAKEWORD(2, 2), &wsadata);

    // Создаём UDP-сокет клиента
    SOCKET s = socket(AF_INET, SOCK_DGRAM, 0);

    DWORD timeout = 3000;

    setsockopt(
        s,
        SOL_SOCKET,
        SO_RCVTIMEO,
        (const char*)&timeout,
        sizeof(timeout)
    );

    // Адрес посредника, которому клиент будет отправлять запросы
    sockaddr_in addr = { 0 };

    addr.sin_family = AF_INET;
    addr.sin_port = htons(5555);
    addr.sin_addr.s_addr = inet_addr(argv[1]);

    char buffer[256];

    while (true)
    {
        const char* msg = "gettime";

        // Отправляем запрос времени посреднику
        sendto(
            s,
            msg,
            (int)strlen(msg),
            0,
            (sockaddr*)&addr,
            sizeof(addr)
        );

        int size = sizeof(addr);

        // Ждём ответ от посредника
        int r = recvfrom(
            s,
            buffer,
            sizeof(buffer) - 1,
            0,
            (sockaddr*)&addr,
            &size
        );

        if (r == SOCKET_ERROR)
        {
            printf("[Client] Timeout waiting for response\n");
            Sleep(2000);
            continue;
        }

        buffer[r] = '\0';

        printf("[Client] Server time: %s\n", buffer);

        Sleep(5000);
    }

    closesocket(s);

    WSACleanup();

    return 0;
}