#define _WINSOCK_DEPRECATED_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <winsock2.h>
#include <ctime>
#include <thread>
#include <mutex>

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

struct NTPPacket {
    uint8_t li_vn_mode;
    uint8_t stratum;
    uint8_t poll;
    uint8_t precision;
    uint32_t rootDelay;
    uint32_t rootDispersion;
    uint32_t refId;
    uint32_t refTm_s;
    uint32_t refTm_f;
    uint32_t origTm_s;
    uint32_t origTm_f;
    uint32_t rxTm_s;
    uint32_t rxTm_f;
    uint32_t txTm_s;
    uint32_t txTm_f;
};


mutex timeMutex;
time_t globalServerTime = 0;
const char* ntpServer = "ntp4.ntp-servers.net";

time_t getNTPTime() {
    SOCKET sock;
    sockaddr_in ntpAddr;
    NTPPacket packet = {};
    packet.li_vn_mode = 0x1B;

    WSAData wsaData;
    WSAStartup(MAKEWORD(2, 2), &wsaData);

    sock = socket(AF_INET, SOCK_DGRAM, 0);
    hostent* host = gethostbyname(ntpServer);
    if (!host) {
        closesocket(sock);
        WSACleanup();
        return 0;
    }

    ntpAddr.sin_family = AF_INET;
    ntpAddr.sin_port = htons(123);
    ntpAddr.sin_addr.s_addr = *(u_long*)host->h_addr;

    sendto(sock, (char*)&packet, sizeof(NTPPacket), 0, (sockaddr*)&ntpAddr, sizeof(ntpAddr));
    int addrSize = sizeof(ntpAddr);
    recvfrom(sock, (char*)&packet, sizeof(NTPPacket), 0, (sockaddr*)&ntpAddr, &addrSize);

    closesocket(sock);
    WSACleanup();

    unsigned long ntpTime = ntohl(packet.txTm_s) - 2208988800U;

    return (time_t)ntpTime;
}

void updateTime() { 
    while (true) {
        time_t newTime = getNTPTime();
        {
            lock_guard<mutex> lock(timeMutex);
            globalServerTime = newTime;
        }
        cout << "Global NTP time: " << globalServerTime << endl;
        this_thread::sleep_for(chrono::seconds(10));
    }
}

int main() {
    setlocale(LC_ALL, "Russian");
    WSADATA wsaData;
    SOCKET Ss;
    sockaddr_in serverAddr, clientAddr;
    int clientAddrSize = sizeof(clientAddr);
    GETSINCHRO recvData;
    SETSINCHRO sendData;

    WSAStartup(MAKEWORD(2, 2), &wsaData);
    Ss = socket(AF_INET, SOCK_DGRAM, 0);

    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(8890);

    bind(Ss, (sockaddr*)&serverAddr, sizeof(serverAddr));

    thread timeUpdater(updateTime);
    timeUpdater.detach();

    while (true) {
        int recvSize = recvfrom(Ss, (char*)&recvData, sizeof(recvData), 0, (sockaddr*)&clientAddr, &clientAddrSize);
        if (recvSize > 0) {
            int correction;
            {
                lock_guard<mutex> lock(timeMutex);
                correction = globalServerTime - recvData.curvalue;
            }
            cout << "========================================" << endl;
            cout << "Client: " << inet_ntoa(clientAddr.sin_addr) << endl;
            cout << "Client time: " << recvData.curvalue << endl;
            cout << "Time now: " << globalServerTime << endl;
            cout << "Correction: " << correction << " ms" << endl;
            cout << "========================================" << endl;

            strcpy(sendData.cmd, "SET");
            sendData.correction = correction;
            sendto(Ss, (char*)&sendData, sizeof(sendData), 0, (sockaddr*)&clientAddr, clientAddrSize);
        }
    }

    closesocket(Ss);
    WSACleanup();
    return 0;
}
