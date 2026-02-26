#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <chrono>
#include <thread>
#include <atomic>

#pragma comment(lib, "ws2_32.lib")

#define SERVER_PORT 12345
#define NTP_SERVER "216.239.35.0" // time.google.com
#define NTP_PORT 123
#define NTP_TIMESTAMP_DELTA 2208988800ull
#define UPDATE_INTERVAL 10000 // 10000 ms

using namespace std;
using namespace chrono;

atomic<uint64_t> CsBase(0);
atomic<uint64_t> lastSyncOSTime(0);

struct NTPPacket {
    uint8_t li_vn_mode;
    uint8_t stratum;
    uint8_t poll;
    uint8_t precision;
    uint32_t root_delay;
    uint32_t root_dispersion;
    uint32_t reference_id;
    uint32_t reference_timestamp[2];
    uint32_t originate_timestamp[2];
    uint32_t receive_timestamp[2];
    uint32_t transmit_timestamp[2];
};

uint64_t getOSTime() {
    return duration_cast<milliseconds>(
        system_clock::now().time_since_epoch()).count();
}

uint64_t getNTPTime() {
    SOCKET sock = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (sock == INVALID_SOCKET) return 0;

    sockaddr_in serverAddr{};
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(NTP_PORT);
    inet_pton(AF_INET, NTP_SERVER, &serverAddr.sin_addr);

    NTPPacket packet{};
    packet.li_vn_mode = (0 << 6) | (4 << 3) | 3;

    sendto(sock, (char*)&packet, sizeof(packet), 0,
        (sockaddr*)&serverAddr, sizeof(serverAddr));

    socklen_t len = sizeof(serverAddr);
    int recvLen = recvfrom(sock, (char*)&packet, sizeof(packet), 0,
        (sockaddr*)&serverAddr, &len);

    closesocket(sock);

    if (recvLen <= 0) return 0;

    uint64_t seconds = ntohl(packet.transmit_timestamp[0]) - NTP_TIMESTAMP_DELTA;
    uint64_t fraction = ntohl(packet.transmit_timestamp[1]);

    uint64_t ms = seconds * 1000 + (fraction * 1000 / 0xFFFFFFFF);

    return ms;
}

uint64_t getServerTime() {
    uint64_t base = CsBase.load();
    uint64_t lastSync = lastSyncOSTime.load();
    uint64_t now = getOSTime();

    return base + (now - lastSync);
}

void updateTime() {
    while (true) {
        uint64_t ntp = getNTPTime();
        if (ntp > 0) {
            CsBase.store(ntp);
            lastSyncOSTime.store(getOSTime());
            cout << "[SYNC] Updated from NTP: " << ntp << " ms\n";
        }
        else {
            cout << "[ERROR] NTP sync failed\n";
        }

        this_thread::sleep_for(milliseconds(UPDATE_INTERVAL));
    }
}

void runServer() {
    WSADATA wsa;
    WSAStartup(MAKEWORD(2, 2), &wsa);

    SOCKET serverSock = socket(AF_INET, SOCK_DGRAM, 0);

    sockaddr_in serverAddr{};
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(SERVER_PORT);

    bind(serverSock, (sockaddr*)&serverAddr, sizeof(serverAddr));

    cout << "Time Server started...\n";

    while (true) {
        sockaddr_in client{};
        int len = sizeof(client);

        char buffer[16];
        recvfrom(serverSock, buffer, sizeof(buffer), 0,
            (sockaddr*)&client, &len);

        uint64_t Cs = getServerTime();

        sendto(serverSock, (char*)&Cs, sizeof(Cs), 0,
            (sockaddr*)&client, len);

        cout << "[SERVER] Sent Cs = " << Cs << endl;
    }
}

int main() {
    WSADATA wsa;
    WSAStartup(MAKEWORD(2, 2), &wsa);

    thread syncThread(updateTime);
    runServer();

    syncThread.join();
    WSACleanup();
    return 0;
}