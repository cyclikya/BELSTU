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

    const char* HOST = "localhost";
    const int PORT = 3000;

    // Инициализация Winsock
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cerr << "WSAStartup failed" << endl;
        return 1;
    }

    // Создание UDP сокета
    clientSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (clientSocket == INVALID_SOCKET) {
        cerr << "socket failed" << endl;
        WSACleanup();
        return 1;
    }

    // Настройка адреса сервера
    server.sin_family = AF_INET;
    server.sin_port = htons(PORT);
    
    // Преобразование hostname в IP адрес
    addrinfo hints = {}, * result = nullptr;
    hints.ai_family = AF_INET;
    hints.ai_socktype = SOCK_DGRAM;
    hints.ai_protocol = IPPROTO_UDP;
    
    if (getaddrinfo(HOST, nullptr, &hints, &result) != 0) {
        cerr << "getaddrinfo failed" << endl;
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }
    
    sockaddr_in* addr = (sockaddr_in*)result->ai_addr;
    server.sin_addr = addr->sin_addr;
    freeaddrinfo(result);

    // Отправка сообщения серверу
    string message = "Client message";
    if (sendto(clientSocket, message.c_str(), message.length(), 0, (sockaddr*)&server, sizeof(server)) == SOCKET_ERROR) {
        cerr << "sendto failed" << endl;
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }

    cout << "Message sent to server." << endl;

    // Получение ответа от сервера
    int serverLen = sizeof(server);
    recvSize = recvfrom(clientSocket, buffer, sizeof(buffer) - 1, 0, (sockaddr*)&server, &serverLen);

    if (recvSize > 0) {
        buffer[recvSize] = '\0';
        cout << "Received message: " << buffer << " (" << recvSize << " bytes)" << endl;
    } else if (recvSize == SOCKET_ERROR) {
        cerr << "recvfrom failed" << endl;
    }

    cout << "Closed" << endl;
    closesocket(clientSocket);
    WSACleanup();
    return 0;
}
