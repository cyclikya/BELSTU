#include <winsock2.h>
#include <ws2tcpip.h>
#include <iostream>
#include <string>

#define _WINSOCK_DEPRECATED_NO_WARNINGS
#pragma comment(lib, "ws2_32.lib")

using namespace std;

int main() {
    WSADATA wsaData;
    SOCKET listeningSocket, clientSocket;
    sockaddr_in server, client;
    int clientLen;
    char buffer[512];
    int recvSize;

    const char* HOST = "0.0.0.0";
    int PORT = 40000;

    // Инициализация Winsock
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cerr << "WSAStartup failed" << endl;
        return 1;
    }

    // Создание сокета
    listeningSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (listeningSocket == INVALID_SOCKET) {
        cerr << "socket failed" << endl;
        WSACleanup();
        return 1;
    }

    // Привязка сокета к адресу и порту
    server.sin_family = AF_INET;
    server.sin_port = htons(PORT);
    if (InetPtonA(AF_INET, HOST, &server.sin_addr.s_addr) <= 0) {
        cerr << "InetPton failed" << endl;
        closesocket(listeningSocket);
        WSACleanup();
        return 1;
    }

    if (bind(listeningSocket, (sockaddr*)&server, sizeof(server)) == SOCKET_ERROR) {
        cerr << "bind failed" << endl;
        closesocket(listeningSocket);
        WSACleanup();
        return 1;
    }

    // Прослушивание входящих соединений
    if (listen(listeningSocket, SOMAXCONN) == SOCKET_ERROR) {
        cerr << "listen failed" << endl;
        closesocket(listeningSocket);
        WSACleanup();
        return 1;
    }

    cout << "TCP-server " << HOST << ":" << PORT << endl;

    while (true) {
        clientLen = sizeof(client);
        clientSocket = accept(listeningSocket, (sockaddr*)&client, &clientLen);

        if (clientSocket == INVALID_SOCKET) {
            cerr << "accept failed" << endl;
            continue;
        }

        char clientIP[INET_ADDRSTRLEN];
        if (InetNtopA(AF_INET, &client.sin_addr, clientIP, INET_ADDRSTRLEN) == nullptr) {
            cerr << "InetNtop failed" << endl;
            continue;
        }
        cout << "Server connected: " << clientIP << ":" << ntohs(client.sin_port) << endl;

        // Получение данных от клиента
        recvSize = recv(clientSocket, buffer, sizeof(buffer) - 1, 0);
        if (recvSize > 0) {
            buffer[recvSize] = '\0';
            cout << "Server data: " << clientIP << ": " << buffer << endl;

            // Отправка ответа с префиксом ECHO:
            string response = "ECHO: ";
            response += buffer;
            send(clientSocket, response.c_str(), response.length(), 0);
        }

        cout << "Server closed: " << clientIP << ":" << ntohs(client.sin_port) << endl;
        closesocket(clientSocket);
    }

    closesocket(listeningSocket);
    WSACleanup();
    return 0;
}
