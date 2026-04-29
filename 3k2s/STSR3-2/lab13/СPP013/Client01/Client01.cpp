#include <winsock2.h>
#include <ws2tcpip.h>
#include <iostream>
#include <string>

#define _WINSOCK_DEPRECATED_NO_WARNINGS
#pragma comment(lib, "ws2_32.lib")

using namespace std;

int main() {
    WSADATA wsaData;
    SOCKET clientSocket;
    sockaddr_in server;
    char buffer[512];
    int recvSize;

    const char* HOST = "127.0.0.1";
    int PORT = 40000;

    // Инициализация Winsock
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cerr << "WSAStartup failed" << endl;
        return 1;
    }

    // Создание сокета
    clientSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (clientSocket == INVALID_SOCKET) {
        cerr << "socket failed" << endl;
        WSACleanup();
        return 1;
    }

    // Подключение к серверу
    server.sin_family = AF_INET;
    server.sin_port = htons(PORT);
    if (InetPtonA(AF_INET, HOST, &server.sin_addr.s_addr) <= 0) {
        cerr << "InetPton failed" << endl;
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }

    if (connect(clientSocket, (sockaddr*)&server, sizeof(server)) == SOCKET_ERROR) {
        cerr << "connect failed" << endl;
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }

    char serverIP[INET_ADDRSTRLEN];
    if (InetNtopA(AF_INET, &server.sin_addr, serverIP, INET_ADDRSTRLEN) == nullptr) {
        cerr << "InetNtop failed" << endl;
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }
    cout << "Client connected: " << serverIP << ":" << ntohs(server.sin_port) << endl;

    // Отправка сообщения серверу
    string message = "Hello";
    if (send(clientSocket, message.c_str(), message.length(), 0) == SOCKET_ERROR) {
        cerr << "send failed" << endl;
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }

    // Получение ответа от сервера
    recvSize = recv(clientSocket, buffer, sizeof(buffer) - 1, 0);
    if (recvSize > 0) {
        buffer[recvSize] = '\0';
        cout << buffer << endl;
    } else {
        cerr << "recv failed" << endl;
    }

    cout << "Client closed" << endl;
    closesocket(clientSocket);
    WSACleanup();
    return 0;
}
