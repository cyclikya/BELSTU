#include <winsock2.h>
#include <ws2tcpip.h>
#include <iostream>
#include <string>

#define _WINSOCK_DEPRECATED_NO_WARNINGS
#pragma comment(lib, "ws2_32.lib")

using namespace std;

int main() {
    WSADATA wsaData;
    SOCKET serverSocket;
    sockaddr_in server, client;
    int clientLen;
    char buffer[512];
    int recvSize;

    const int PORT = 3000;

    // Инициализация Winsock
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cerr << "WSAStartup failed" << endl;
        return 1;
    }

    // Создание UDP сокета
    serverSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (serverSocket == INVALID_SOCKET) {
        cerr << "socket failed" << endl;
        WSACleanup();
        return 1;
    }

    // Привязка сокета к адресу и порту
    server.sin_family = AF_INET;
    server.sin_port = htons(PORT);
    server.sin_addr.s_addr = htonl(INADDR_ANY);

    if (bind(serverSocket, (sockaddr*)&server, sizeof(server)) == SOCKET_ERROR) {
        cerr << "bind failed" << endl;
        closesocket(serverSocket);
        WSACleanup();
        return 1;
    }

    cout << "UDP Server listening on port: " << PORT << endl;
    cout << "Server address: 0.0.0.0" << endl;
    cout << "Family: AF_INET" << endl;

    while (true) {
        clientLen = sizeof(client);

        // Получение данных от клиента
        recvSize = recvfrom(serverSocket, buffer, sizeof(buffer) - 1, 0, (sockaddr*)&client, &clientLen);

        if (recvSize == SOCKET_ERROR) {
            cerr << "recvfrom failed" << endl;
            continue;
        }

        buffer[recvSize] = '\0';
        char clientIP[INET_ADDRSTRLEN];
        if (InetNtopA(AF_INET, &client.sin_addr, clientIP, INET_ADDRSTRLEN) == nullptr) {
            cerr << "InetNtop failed" << endl;
            continue;
        }

        cout << "Received from client: " << buffer << endl;
        cout << "Received " << recvSize << " bytes from " << clientIP << ":" << ntohs(client.sin_port) << endl;

        // Отправка ответа с префиксом ECHO:
        string response = "ECHO:";
        response += buffer;

        if (sendto(serverSocket, response.c_str(), response.length(), 0, (sockaddr*)&client, clientLen) == SOCKET_ERROR) {
            cerr << "sendto failed" << endl;
            continue;
        }

        cout << "Data sent to client" << endl;
    }

    closesocket(serverSocket);
    WSACleanup();
    return 0;
}
