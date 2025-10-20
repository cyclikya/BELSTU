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
    try {
        setlocale(0, "Russian");

        // 1) Инициализация Winsock
        WSADATA wsaData;
        int res = WSAStartup(MAKEWORD(2, 2), &wsaData);
        if (res != 0) {
            throw std::runtime_error("WSAStartup failed");
        }
        cout << "Winsock инициализирован.\n";

        // 2) Создание UDP сокета (type = SOCK_DGRAM)
        SOCKET servSock = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
        if (servSock == INVALID_SOCKET) {
            int err = WSAGetLastError();
            WSACleanup();
            throw std::runtime_error("Не удалось создать сокет. WSA error: " + std::to_string(err));
        }
        cout << "UDP сокет создан.\n";

        // 3) Привязка сокета к порту 2000 на всех интерфейсах
        sockaddr_in servAddr{};
        servAddr.sin_family = AF_INET;
        servAddr.sin_port = htons(2000);
        servAddr.sin_addr.s_addr = INADDR_ANY;

        if (bind(servSock, (sockaddr*)&servAddr, sizeof(servAddr)) == SOCKET_ERROR) {
            int err = WSAGetLastError();
            closesocket(servSock);
            WSACleanup();
            throw std::runtime_error("Bind failed. WSA error: " + std::to_string(err));
        }
        cout << "Сокет привязан к порту 2000 и готов принимать сообщения.\n";

        // 4) Ожидание recvfrom (блокирующий вызов) — задание 2
        const int BUF_SIZE = 1024;
        char buffer[BUF_SIZE];

        cout << "Ждем входящее сообщение (recvfrom)...\n";

        sockaddr_in clientAddr{};
        int clientAddrLen = sizeof(clientAddr);
        int bytesReceived = recvfrom(servSock, buffer, BUF_SIZE - 1, 0,
            (sockaddr*)&clientAddr, &clientAddrLen);
        if (bytesReceived == SOCKET_ERROR) {
            int err = WSAGetLastError();
            closesocket(servSock);
            WSACleanup();
            throw std::runtime_error("recvfrom failed. WSA error: " + std::to_string(err));
        }

        // Нуль-терминируем и выводим
        buffer[bytesReceived] = '\0';
        char clientIp[INET_ADDRSTRLEN];
        inet_ntop(AF_INET, &clientAddr.sin_addr, clientIp, sizeof(clientIp));
        cout << "Получено " << bytesReceived << " байт от " << clientIp
            << ":" << ntohs(clientAddr.sin_port) << "\n";
        cout << "Текст: \"" << buffer << "\"\n";

        // 5) (Задание 5) - отправить обратно (echo)
        string reply = string("Echo from ServerU: ") + buffer;
        int sent = sendto(servSock, reply.c_str(), (int)reply.size(), 0,
            (sockaddr*)&clientAddr, clientAddrLen);
        if (sent == SOCKET_ERROR) {
            int err = WSAGetLastError();
            closesocket(servSock);
            WSACleanup();
            throw std::runtime_error("sendto failed. WSA error: " + std::to_string(err));
        }
        cout << "Отправлено " << sent << " байт в адрес клиента.\n";

        // Закрыть сокет и очистить Winsock
        closesocket(servSock);
        WSACleanup();
        cout << "ServerU завершил работу.\n";
        return 0;
    }
    catch (const std::exception& ex) {
        cerr << "[ОШИБКА] " << ex.what() << endl;
        return 1;
    }
}
