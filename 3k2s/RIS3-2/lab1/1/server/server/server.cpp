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

    double getAverageCorrection() const {
        if (corrections.empty()) return 0;
        return accumulate(corrections.begin(), corrections.end(), 0.0) / corrections.size();
    }

    int getMinCorrection() const {
        if (corrections.empty()) return 0;
        return *min_element(corrections.begin(), corrections.end());
    }

    int getMaxCorrection() const {
        if (corrections.empty()) return 0;
        return *max_element(corrections.begin(), corrections.end());
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
            lock_guard<mutex> lock(consoleMutex);
            cerr << "Error receiving data from client: " << WSAGetLastError() << endl;
            continue;
        }

        if (bytesReceived == 0) {
            continue;
        }

       
        clock_t serverTime = clock() * 1000 / CLOCKS_PER_SEC;

      
        response.correction = serverTime - request.curvalue;

     
        int bytesSent = sendto(serverSocket, (char*)&response, sizeof(response), 0,
            (struct sockaddr*)&clientAddr, clientAddrLen);

        if (bytesSent == SOCKET_ERROR) {
            lock_guard<mutex> lock(consoleMutex);
            cerr << "Error sending response to client: " << WSAGetLastError() << endl;
            continue;
        }

 
        char clientIp[INET_ADDRSTRLEN];
        inet_ntop(AF_INET, &(clientAddr.sin_addr), clientIp, INET_ADDRSTRLEN);

        
        int intervalCode = request.curvalue % 100000;

        string clientKey = string(clientIp) + ":" +
            to_string(ntohs(clientAddr.sin_port)) + ":" +
            to_string(intervalCode);

        {
            lock_guard<mutex> lock(clientStatsMutex);
            ClientStats& stats = clientStats[clientKey];
            stats.requestCount++;

           
            if (stats.firstRequestReceived) {
                stats.corrections.push_back(response.correction);
            }
            else {
                stats.firstRequestReceived = true;
            }

            stats.clientIp = clientIp;
            stats.port = ntohs(clientAddr.sin_port);
        }

        {
            lock_guard<mutex> lock(consoleMutex);
            cout << "Client: " << clientIp << ":" << ntohs(clientAddr.sin_port)
                << ", Interval: " << intervalCode << " ms"
                << ", Request #: " << clientStats[clientKey].requestCount
                << ", Correction: " << response.correction;

            if (clientStats[clientKey].requestCount == 1) {
                cout << " [SETUP REQUEST - Ignored for statistics]" << endl;
            }
            else {
                cout << ", Avg Correction: " << clientStats[clientKey].getAverageCorrection()
                    << ", Min: " << clientStats[clientKey].getMinCorrection()
                    << ", Max: " << clientStats[clientKey].getMaxCorrection() << endl;
            }
        }
    }
}

void printStatistics() {
    while (true) {
        Sleep(5000);

        lock_guard<mutex> statsLock(clientStatsMutex);
        lock_guard<mutex> consoleLock(consoleMutex);

        map<int, vector<ClientStats*>> statsByInterval;

        for (auto& pair : clientStats) {
            string key = pair.first;
            size_t lastColon = key.find_last_of(':');
            int interval = stoi(key.substr(lastColon + 1));

            statsByInterval[interval].push_back(&pair.second);
        }

       
        for (const auto& intervalPair : statsByInterval) {

            for (const ClientStats* stats : intervalPair.second) {
                if (stats->corrections.empty()) continue; 

                cout << "Client " << stats->clientIp << ":" << stats->port << ":\n";
                cout << "  Total Requests: " << stats->requestCount << " (including setup request)\n";
                cout << "  Actual Requests Used for Stats: " << stats->corrections.size() << "\n";
                cout << "  Avg Correction: " << stats->getAverageCorrection() << " ms\n";
                cout << "  Min Correction: " << stats->getMinCorrection() << " ms\n";
                cout << "  Max Correction: " << stats->getMaxCorrection() << " ms\n";
            }
        }

        cout << "==========================\n\n";
    }
}

int main(int argc, char* argv[]) {
    int port = 8888;


    if (argc > 1) {
        port = stoi(argv[1]);
    }


    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cerr << "Failed to initialize Winsock: " << WSAGetLastError() << endl;
        return 1;
    }

   
    SOCKET serverSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (serverSocket == INVALID_SOCKET) {
        cerr << "Error creating socket: " << WSAGetLastError() << endl;
        WSACleanup();
        return 1;
    }


    struct sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(port);

    if (bind(serverSocket, (struct sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
        cerr << "Bind failed with error code: " << WSAGetLastError() << endl;
        closesocket(serverSocket);
        WSACleanup();
        return 1;
    }

    cout << "UDP Server started on port " << port << endl;
    cout << "Waiting for client requests..." << endl;

    thread clientThread(handleClient, serverSocket);
    thread statsThread(printStatistics);
    clientThread.join();
    statsThread.join();
    closesocket(serverSocket);
    WSACleanup();

    return 0;
}

