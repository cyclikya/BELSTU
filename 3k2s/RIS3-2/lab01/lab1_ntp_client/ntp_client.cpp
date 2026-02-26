#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <chrono>
#include <thread>
#include <vector>
#include <algorithm>

#pragma comment(lib, "ws2_32.lib")

#define SERVER_IP "26.180.211.124" // IP сервера
#define SERVER_PORT 12345
#define REQUEST_INTERVAL 1000 // 10000 ms

using namespace std;
using namespace chrono;

uint64_t getOSTime() {
    return duration_cast<milliseconds>(
        system_clock::now().time_since_epoch()).count();
}

int main() {
    WSADATA wsa;
    WSAStartup(MAKEWORD(2, 2), &wsa);

    SOCKET sock = socket(AF_INET, SOCK_DGRAM, 0);

    sockaddr_in server{};
    server.sin_family = AF_INET;
    server.sin_port = htons(SERVER_PORT);
    inet_pton(AF_INET, SERVER_IP, &server.sin_addr);

    cout << "Client started...\n";
    cout << "Will send 10 requests with interval " << REQUEST_INTERVAL << " ms\n\n";

    vector<uint64_t> client_times;      // вектор дл€ хранени€ времени клиента (OStime)
    vector<int64_t> corrections;        // вектор дл€ хранени€ разницы (Cs - OStime)

    for (int i = 0; i < 10; i++) {
        char msg[] = "sync";
        sendto(sock, msg, sizeof(msg), 0,
            (sockaddr*)&server, sizeof(server));

        uint64_t Cs; // врем€ на локальном сервере
        int len = sizeof(server);

        recvfrom(sock, (char*)&Cs, sizeof(Cs), 0,
            (sockaddr*)&server, &len);

        uint64_t OStime = getOSTime(); // врем€ клиента от эпохи Unix (в миллисекундах)
        int64_t correction = (int64_t)Cs - (int64_t)OStime; // разница Cs - OStime

        client_times.push_back(OStime);
        corrections.push_back(correction);

        cout << "Request " << i + 1 << ":\n";
        cout << "  Server time (Cs): " << Cs << " ms\n";
        cout << "  Client OS time: " << OStime << " ms\n";
        cout << "  Correction (Cs - OStime): " << correction << " ms\n\n";

        this_thread::sleep_for(milliseconds(REQUEST_INTERVAL));
    }

    // ¬ычисл€ем статистику дл€ времени клиента (Cc - OStime)
    uint64_t sum_client_times = 0;
    for (uint64_t time : client_times) {
        sum_client_times += time;
    }
    double avg_client_time = static_cast<double>(sum_client_times) / client_times.size();

    // ¬ычисл€ем статистику дл€ коррекции (разницы между сервером и клиентом)
    int64_t max_correction = *max_element(corrections.begin(), corrections.end());
    int64_t min_correction = *min_element(corrections.begin(), corrections.end());

    int64_t sum_corrections = 0;
    for (int64_t corr : corrections) {
        sum_corrections += corr;
    }
    double avg_correction = static_cast<double>(sum_corrections) / corrections.size();

    // ¬ыводим результаты
    cout << "\n========================================\n";
    cout << "FINAL STATISTICS\n";
    cout << "========================================\n";
    cout << "Number of requests: " << corrections.size() << "\n";
    cout << "Average Cc - OStime: " << avg_client_time << " ms\n";
    cout << "\n--- Correction statistics ---\n";
    cout << "Max correction (Cs - OStime): " << max_correction << " ms\n";
    cout << "Min correction (Cs - OStime): " << min_correction << " ms\n";
    cout << "Average correction (Cs - OStime): " << avg_correction << " ms\n";
    cout << "========================================\n";

    closesocket(sock);
    WSACleanup();

    system("pause");
    return 0;
}