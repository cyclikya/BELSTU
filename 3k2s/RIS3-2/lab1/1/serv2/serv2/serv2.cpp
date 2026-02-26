#include <iostream>
#include <ctime>
#include <cstring>
#include <vector>
#include <map>
#include <thread>
#include <mutex>
#include <numeric>
#include <algorithm>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <string>

#pragma comment(lib, "ws2_32.lib")
using namespace std;

mutex consoleMutex;
mutex clientStatsMutex;

struct GETSINCHRO {
    char cmd[4];
    int curvalue;
};

struct SETSINCHRO {
    char cmd[4];
    int correction;
}; 

struct ClientStats {
    int requestCount = 0;
    vector<int> corrections;
    string clientIp;
    int port;
    bool firstRequestReceived = false; 
    int stableCorrection = 0;
    static constexpr size_t maxHistorySize = 5;
    double alpha = 0.7;

    int getStableCorrection(int newCorrection) {
       
        if (!firstRequestReceived) {
            firstRequestReceived = true;
            return newCorrection; 
        }

        corrections.push_back(newCorrection);
        if (corrections.size() > maxHistorySize) {
            corrections.erase(corrections.begin());
        }

       
        if (stableCorrection == 0) {
            stableCorrection = newCorrection;
        }
        else {
            stableCorrection = static_cast<int>(alpha * newCorrection + (1 - alpha) * stableCorrection);
        }

        return stableCorrection;
    }
};


map<string, ClientStats> clientStats;

void handleClient(SOCKET serverSocket) {
    struct sockaddr_in clientAddr;
    int clientAddrLen = sizeof(clientAddr);

    GETSINCHRO request;
    SETSINCHRO response;

    strcpy_s(response.cmd, "SET");

    while (true) {
        memset(&request, 0, sizeof(request));

        int bytesReceived = recvfrom(serverSocket, (char*)&request, sizeof(request), 0,
            (struct sockaddr*)&clientAddr, &clientAddrLen);

        if (bytesReceived == SOCKET_ERROR) {
            cerr << "Error receiving data: " << WSAGetLastError() << endl;
            continue;
        }

        clock_t serverTime = clock() * 1000 / CLOCKS_PER_SEC;
        int rawCorrection = serverTime - request.curvalue;

        char clientIp[INET_ADDRSTRLEN];
        inet_ntop(AF_INET, &(clientAddr.sin_addr), clientIp, INET_ADDRSTRLEN);
        string clientKey = string(clientIp) + ":" + to_string(ntohs(clientAddr.sin_port));

        {
            lock_guard<mutex> lock(clientStatsMutex);
            ClientStats& stats = clientStats[clientKey];
            stats.requestCount++;
            stats.clientIp = clientIp;
            stats.port = ntohs(clientAddr.sin_port);
            response.correction = stats.getStableCorrection(rawCorrection);
        }

        int bytesSent = sendto(serverSocket, (char*)&response, sizeof(response), 0,
            (struct sockaddr*)&clientAddr, clientAddrLen);

        if (bytesSent == SOCKET_ERROR) {
            cerr << "Error sending response: " << WSAGetLastError() << endl;
            continue;
        }

        {
            lock_guard<mutex> lock(consoleMutex);
            cout << "Client: " << clientIp << ":" << ntohs(clientAddr.sin_port)
                << " | Raw Correction: " << rawCorrection
                << " | Stable Correction: " << clientStats[clientKey].stableCorrection
                << endl;
        }
    }
}

int main(int argc, char* argv[]) {
    int port = 8889;

    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cerr << "Failed to initialize Winsock" << endl;
        return 1;
    }

    SOCKET serverSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (serverSocket == INVALID_SOCKET) {
        cerr << "Error creating socket" << endl;
        WSACleanup();
        return 1;
    }

    struct sockaddr_in serverAddr = {};
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(port);

    if (bind(serverSocket, (struct sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
        cerr << "Bind failed" << endl;
        closesocket(serverSocket);
        WSACleanup();
        return 1;
    }

    cout << "Server running on port " << port << endl;
    handleClient(serverSocket);

    closesocket(serverSocket);
    WSACleanup();
    return 0;
}
