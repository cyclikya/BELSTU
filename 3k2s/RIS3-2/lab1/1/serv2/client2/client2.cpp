#include <iostream>
#include <ctime>
#include <cstring>
#include <string>
#include <vector>
#include <map>
#include <thread>
#include <chrono>
#include <numeric>
#include <cmath>
#include <iomanip>
#include <winsock2.h>
#include <ws2tcpip.h>

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


struct ExperimentResult {
    int interval;
    vector<int> corrections;

    double getAverage() const {
        if (corrections.empty()) return 0;
        return accumulate(corrections.begin(), corrections.end(), 0.0) / corrections.size();
    }

    int getMin() const {
        if (corrections.empty()) return 0;
        return *min_element(corrections.begin(), corrections.end());
    }

    int getMax() const {
        if (corrections.empty()) return 0;
        return *max_element(corrections.begin(), corrections.end());
    }
};

vector<ExperimentResult> allResults;

int runExperiment(const string& serverIp, int serverPort, int interval) {

    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cerr << "Failed to initialize Winsock: " << WSAGetLastError() << endl;
        return -1;
    }

    SOCKET clientSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (clientSocket == INVALID_SOCKET) {
        cerr << "Error creating socket: " << WSAGetLastError() << endl;
        WSACleanup();
        return -1;
    }

   
    struct sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(serverPort);


    if (inet_pton(AF_INET, serverIp.c_str(), &serverAddr.sin_addr) <= 0) {
        cerr << "Invalid address / Address not supported" << endl;
        closesocket(clientSocket);
        WSACleanup();
        return -1;
    }

   
    GETSINCHRO request;
    SETSINCHRO response;
    strcpy_s(request.cmd, "GET");


    int clientCounter = 0;
    ExperimentResult result;
    result.interval = interval;

  
    int intervalCode = interval;


    DWORD timeout = 2000; 
    setsockopt(clientSocket, SOL_SOCKET, SO_RCVTIMEO, (const char*)&timeout, sizeof(timeout));

    cout << "\n==== STARTING EXPERIMENT WITH INTERVAL: " << interval << "ms ====" << endl;

    
    bool setupRequestCompleted = false;
    int requestCount = 0;
    const int TOTAL_REQUESTS = 11;

    while (requestCount < TOTAL_REQUESTS) {
        request.curvalue = clientCounter + intervalCode;

       
        int bytesSent = sendto(clientSocket, (char*)&request, sizeof(request), 0,
            (struct sockaddr*)&serverAddr, sizeof(serverAddr));

        if (bytesSent == SOCKET_ERROR) {
            cerr << "Error sending request to server: " << WSAGetLastError() << endl;
            break;
        }

  
        int serverAddrLen = sizeof(serverAddr);
        int bytesReceived = recvfrom(clientSocket, (char*)&response, sizeof(response), 0,
            (struct sockaddr*)&serverAddr, &serverAddrLen);

        if (bytesReceived == SOCKET_ERROR) {
            cerr << "Error receiving response from server or timeout: " << WSAGetLastError() << endl;
            continue; 
        }
        else {
            requestCount++;
            clientCounter += response.correction + interval;

          
            if (!setupRequestCompleted) {
                setupRequestCompleted = true;
                cout << "Setup request completed. Correction = " << response.correction
                    << " ms [IGNORED FOR STATISTICS]" << endl;
            }
            else {
                result.corrections.push_back(response.correction);
                cout << "Request " << requestCount - 1 << "/10: Correction = "
                    << response.correction << " ms" << endl;
            }
        }


        Sleep(interval);
    }


    cout << "\n==== EXPERIMENT RESULTS ====" << endl;
    cout << "Interval: " << interval << " ms" << endl;
    cout << "Requests processed: " << (requestCount - 1) << " (excluding setup request)" << endl;
    cout << "Average correction: " << result.getAverage() << " ms" << endl;
    cout << "Min correction: " << result.getMin() << " ms" << endl;
    cout << "Max correction: " << result.getMax() << " ms" << endl;
    cout << "==========================\n" << endl;

    allResults.push_back(result);
    closesocket(clientSocket);
    WSACleanup();

    return 0;
}

void printFinalReport() {
    cout << "\n\n=========== FINAL REPORT ===========" << endl;
    cout << setw(10) << "Interval" << setw(10) << "Avg" << setw(10) << "Min"
        << setw(10) << "Max" << endl;
    cout << "------------------------------------" << endl;

    for (const auto& result : allResults) {
        cout << setw(8) << result.interval << " ms"
            << setw(10) << fixed << setprecision(2) << result.getAverage()
            << setw(10) << result.getMin()
            << setw(10) << result.getMax() << endl;
    }

    cout << "=====================================" << endl;
}

int main(int argc, char* argv[]) {
    string serverIp = "192.168.224.78";
    int serverPort = 8889;

    if (argc > 1) {
        serverIp = argv[1];
    }

    if (argc > 2) {
        serverPort = stoi(argv[2]);
    }

    cout << "UDP Client - Synchronization Experiment" << endl;
    cout << "Server IP: " << serverIp << endl;
    cout << "Server Port: " << serverPort << endl;
    cout << "Each experiment: 1 setup request + 10 measurement requests" << endl;


    vector<int> intervals = { 1000, 3000, 6000, 8000, 10000, 12000, 14000 };

    for (int interval : intervals) {
        runExperiment(serverIp, serverPort, interval);


        Sleep(1000);
    }

    printFinalReport();

    cout << "\nAll experiments completed." << endl;
    cout << "Press Enter to exit..." << endl;
    cin.get();

    return 0;
}