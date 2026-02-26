#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <windows.h>
#include <climits>
#include <string>

#pragma comment(lib, "ws2_32.lib")

// Структуры (должны совпадать с сервером)
#pragma pack(push, 1)
struct TimeSyncRequest {
    int request_number;
    long long curvalue;
};

struct TimeSyncResponse {
    long long correction;
    long long server_time;
};
#pragma pack(pop)

void printUsage(const char* progName) {
    std::cout << "Usage: " << progName << " <server_ip> <Tc_ms> <num_requests>\n";
    std::cout << "Example: " << progName << " 192.168.1.5 1000 10\n";
    std::cout << "Tc - периодичность запросов в мс (1/1000 сек)\n";
}

int main(int argc, char* argv[]) {
    if (argc != 4) {
        printUsage(argv[0]);
        return 1;
    }

    const char* serverIP = argv[1];
    int Tc = atoi(argv[2]);
    int numRequests = atoi(argv[3]);
    const int SERVER_PORT = 8888;

    if (Tc <= 0 || numRequests <= 0) {
        std::cerr << "Error: Tc and num_requests must be positive\n";
        return 1;
    }

    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        std::cerr << "WSAStartup failed\n";
        return 1;
    }

    SOCKET clientSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (clientSocket == INVALID_SOCKET) {
        std::cerr << "Socket creation failed\n";
        WSACleanup();
        return 1;
    }

    // Таймаут для получения ответа
    DWORD timeout = 5000;
    setsockopt(clientSocket, SOL_SOCKET, SO_RCVTIMEO, (const char*)&timeout, sizeof(timeout));

    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(SERVER_PORT);
    if (inet_pton(AF_INET, serverIP, &serverAddr.sin_addr) != 1) {
        std::cerr << "Invalid server IP address\n";
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }

    long long Cc = 0;  // Счетчик времени клиента (пункт 10 - начальное значение 0)
    long long minCorrection = LLONG_MAX;
    long long maxCorrection = LLONG_MIN;
    long long sumCorrection = 0;

    std::cout << "TimeSyncClient started\n";
    std::cout << "Server: " << serverIP << ":" << SERVER_PORT << "\n";
    std::cout << "Tc = " << Tc << " ms, Requests = " << numRequests << "\n";
    std::cout << "Initial Cc = " << Cc << "\n";
    std::cout << "========================================\n";

    for (int i = 1; i <= numRequests; i++) {
        // Формируем запрос (пункт 6)
        TimeSyncRequest req;
        req.request_number = i;
        req.curvalue = Cc;  // Текущее значение счетчика (пункт 10, 13)

        // Отправляем запрос
        int sent = sendto(clientSocket, (char*)&req, sizeof(req), 0,
            (sockaddr*)&serverAddr, sizeof(serverAddr));
        if (sent != sizeof(req)) {
            std::cerr << "Failed to send request " << i << "\n";
            break;
        }

        // Получаем ответ
        TimeSyncResponse resp;
        int serverLen = sizeof(serverAddr);
        int received = recvfrom(clientSocket, (char*)&resp, sizeof(resp), 0,
            (sockaddr*)&serverAddr, &serverLen);

        if (received == sizeof(resp)) {
            // Применяем коррекцию (пункт 11)
            Cc = Cc + resp.correction;

            // Собираем статистику
            if (resp.correction < minCorrection) minCorrection = resp.correction;
            if (resp.correction > maxCorrection) maxCorrection = resp.correction;
            sumCorrection += resp.correction;

            std::cout << "Req " << i << ": correction=" << resp.correction
                << ", new Cc=" << Cc << "\n";
        }
        else {
            std::cerr << "Req " << i << ": Timeout or error (received=" << received << ")\n";
        }

        // Задержка Tc перед следующим запросом (пункт 12)
        if (i < numRequests) {
            Sleep(Tc);  // Задержка в мс

            // Увеличиваем счетчик на величину задержки (пункт 13)
            Cc += Tc;
        }
    }

    // Итоговая статистика для таблицы (пункт 16)
    double avgCorrection = (double)sumCorrection / numRequests;

    std::cout << "\n========== EXPERIMENT RESULTS ==========\n";
    std::cout << "Server: " << serverIP << "\n";
    std::cout << "Tc (ms): " << Tc << "\n";
    std::cout << "Number of requests: " << numRequests << "\n";
    std::cout << "----------------------------------------\n";
    std::cout << "Min correction: " << minCorrection << " ms\n";
    std::cout << "Max correction: " << maxCorrection << " ms\n";
    std::cout << "Avg correction: " << avgCorrection << " ms\n";
    std::cout << "Final Cc: " << Cc << "\n";
    std::cout << "========================================\n";

    closesocket(clientSocket);
    WSACleanup();
    return 0;
}