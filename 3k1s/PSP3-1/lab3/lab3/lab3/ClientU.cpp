#include <winsock2.h>
#include <ws2tcpip.h>
#include <iostream>
#include <string>
#include <stdexcept>

#pragma comment(lib, "ws2_32.lib")

using std::cout;
using std::cerr;
using std::endl;
using std::string;

void ThrowIfWSAError(int code, const char* msg) {
    if (code != 0) {
        int err = WSAGetLastError();
        char buf[256];
        snprintf(buf, sizeof(buf), "%s (WSA error %d)", msg, err);
        throw std::runtime_error(buf);
    }
}

int main() {
    setlocale(0, "Russian");

    try {
        WSADATA wsaData;
        if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
            throw std::runtime_error("WSAStartup failed");
        }
        cout << "Winsock инициализирован.\n";

        // Создаем UDP сокет
        SOCKET sock = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
        if (sock == INVALID_SOCKET) {
            int err = WSAGetLastError();
            WSACleanup();
            throw std::runtime_error("Не удалось создать сокет. WSA error: " + std::to_string(err));
        }
        cout << "UDP сокет создан.\n";

        // Адрес сервера: 127.0.0.1:2000 (как в задании 3)
        sockaddr_in servAddr{};
            servAddr.sin_family = AF_INET;
            servAddr.sin_port = htons(2000);
            servAddr.sin_addr.s_addr = inet_addr("127.0.0.1");

        string message = "Hello from ClientU";

        int sent = sendto(sock, message.c_str(), (int)message.size(), 0,
            (sockaddr*)&servAddr, sizeof(servAddr));
        if (sent == SOCKET_ERROR) {
            int err = WSAGetLastError();
            cerr << "sendto вернул SOCKET_ERROR. WSAGetLastError() = " << err << "\n";
        }
        else {
            cout << "Отправлено " << sent << " байт серверу 127.0.0.1:2000\n";
        }

        // Попробуем получить ответ (recvfrom). Установим таймаут, чтобы клиент не блокировался бесконечно.
        int timeoutMs = 5000; // 5 секунд
        setsockopt(sock, SOL_SOCKET, SO_RCVTIMEO, (const char*)&timeoutMs, sizeof(timeoutMs));

        const int BUF_SIZE = 1024;
        char buffer[BUF_SIZE];
        sockaddr_in fromAddr{};
        int fromLen = sizeof(fromAddr);

        int bytesReceived = recvfrom(sock, buffer, BUF_SIZE - 1, 0,
            (sockaddr*)&fromAddr, &fromLen);
        if (bytesReceived == SOCKET_ERROR) {
            int err = WSAGetLastError();
            if (err == WSAETIMEDOUT) {
                cout << "recvfrom таймаут (сервер не ответил в течение " << timeoutMs << " мс).\n";
            }
            else {
                cerr << "recvfrom вернул SOCKET_ERROR. WSAGetLastError() = " << err << "\n";
            }
        }
        else {
            buffer[bytesReceived] = '\0';
            char fromIp[INET_ADDRSTRLEN];
            inet_ntop(AF_INET, &fromAddr.sin_addr, fromIp, sizeof(fromIp));
            cout << "Получен ответ от " << fromIp << ":" << ntohs(fromAddr.sin_port) << "\n";
            cout << "Текст: \"" << buffer << "\"\n";
        }

        // Закрытие
        closesocket(sock);
        WSACleanup();
        return 0;
    }
    catch (const std::exception& ex) {
        cerr << "[ОШИБКА] " << ex.what() << endl;
        return 1;
    }
}
