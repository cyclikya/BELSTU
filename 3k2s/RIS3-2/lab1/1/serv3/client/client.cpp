#define _WINSOCK_DEPRECATED_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <winsock2.h>
#include <chrono>
#include <thread>
#include <windows.h>
#include <mutex>
#include <iomanip>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

struct GETSINCHRO {
    char cmd[4];
    int curvalue;
};

struct SETSINCHRO {
    char cmd[4];
    int correction;
};

mutex timeMutex;
time_t globalServerTime = -1;
double totalAvgCorrection = 0.0;
double totalAvgCc_OStime = 0.0;
int totalRequestCount = 0;

const int intervals[] = { 1000, 3000, 6000, 8000, 10000, 12000, 14000 };
const int maxRequests = 10;
bool stopClient = false;

inline time_t GetLocalTime() {
    return time(nullptr);
}

void SetSystemTime(time_t newTime) {
    SYSTEMTIME st;
    FILETIME ft;
    ULARGE_INTEGER ui;

    ui.QuadPart = (newTime + 11644473600ULL) * 10000000ULL;
    ft.dwLowDateTime = ui.LowPart;
    ft.dwHighDateTime = ui.HighPart;

    FileTimeToSystemTime(&ft, &st);

    if (::SetSystemTime(&st)) {
        cout << "System time successfully set: "
            << put_time(localtime(&newTime), "%Y-%m-%d %H:%M:%S") << endl;
    }
    else {
        cerr << "Error setting system time!" << endl;
    }
}

void ConnectToServer(SOCKET clientSocket, sockaddr_in serverAddr) {
    for (int interval : intervals) {
        cout << "\n===== START OF EXPERIMENT =====\n";
        cout << "Request interval: " << interval << " ms" << endl;

        double avgCorrection = 0.0;
        double avgCc_OStime = 0.0;
        int requestCount = 0;

        while (requestCount < maxRequests && !stopClient) {
            time_t localTime = GetLocalTime();
            GETSINCHRO sendData;
            sendData.curvalue = static_cast<int>(localTime);
            strcpy_s(sendData.cmd, "GET");

            sendto(clientSocket, (char*)&sendData, sizeof(sendData), 0, (sockaddr*)&serverAddr, sizeof(serverAddr));

            SETSINCHRO recvData;
            int serverAddrSize = sizeof(serverAddr);
            int bytesReceived = recvfrom(clientSocket, (char*)&recvData, sizeof(recvData), 0, (sockaddr*)&serverAddr, &serverAddrSize);

            if (bytesReceived == SOCKET_ERROR) {
                cerr << "Error receiving data: " << WSAGetLastError() << endl;
                continue;
            }

            int correction = recvData.correction;
            time_t newTime = localTime + correction;
            SetSystemTime(newTime);

            if (globalServerTime == -1) {
                globalServerTime = newTime;
            }

            requestCount++;
            totalRequestCount++;

            if (requestCount > 1) {
                avgCorrection += (correction - avgCorrection) / requestCount;
                time_t localNtpDiff = (globalServerTime > 0) ? globalServerTime - localTime : 0;
                avgCc_OStime += (localNtpDiff - avgCc_OStime) / requestCount;
            }

            cout << "Request " << requestCount << "/" << maxRequests
                << " | Local time: " << put_time(localtime(&localTime), "%H:%M:%S")
                << " | Correction: " << correction << " ms" << endl;

            this_thread::sleep_for(chrono::milliseconds(interval));
        }

        totalAvgCorrection += avgCorrection;
        totalAvgCc_OStime += avgCc_OStime;

        cout << "\n===== INTERIM RESULTS =====\n";
        cout << "Average correction: " << fixed << setprecision(2) << avgCorrection << " ms" << endl;
        cout << "Average difference with NTP: " << fixed << setprecision(2) << avgCc_OStime << " ms" << endl;
        cout << "============================\n" << endl;
    }
}

int main() {
    setlocale(LC_ALL, "en_US.utf8");
    string serverIp = "192.168.224.78";
    int serverPort = 8890;

    WSAData wsaData;
    WSAStartup(MAKEWORD(2, 2), &wsaData);

    SOCKET clientSocket = socket(AF_INET, SOCK_DGRAM, 0);
    sockaddr_in serverAddr{};
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(serverPort);
    serverAddr.sin_addr.s_addr = inet_addr(serverIp.c_str());

    ConnectToServer(clientSocket, serverAddr);

    closesocket(clientSocket);
    WSACleanup();

    cout << "\n===== FINAL RESULTS =====\n";
    cout << "Overall average correction: " << fixed << setprecision(2) << (totalAvgCorrection / 7) << " ms" << endl;
    cout << "Overall average difference with NTP: " << fixed << setprecision(2) << (totalAvgCc_OStime / 7) << " ms" << endl;
    cout << "=========================\n" << endl;

    cout << "Experiment completed. Press Enter to exit..." << endl;
    cin.get();
    return 0;
}
