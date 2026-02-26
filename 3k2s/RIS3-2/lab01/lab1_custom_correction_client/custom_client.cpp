#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include "Winsock2.h"
#include <iostream>
#include <string>
#include <ctime>
#include <tchar.h>
#include <vector>
#include <algorithm>
#pragma comment(lib, "WS2_32.lib")

#define SIP "26.58.222.244"
#define PORT 5000

using namespace std;

struct GETSINCHRO	///запрос на синхронизацию
{
    char cmd[10];		///всегда значение SINC
    int curvalue;	///тек. значение счетчика времени
};

struct SETSINCRO		///ответ сервера
{
    char cmd[10];		///всегда значение SINCRO
    int correction;		///знач, кот надо прибавить к знач счетчика t
};

string GetErrorMsgText(int code)
{
    string msgText;
    switch (code)
    {
    case WSAEINTR:				 msgText = "Работа функции прервана\n";						  break;
    case WSAEACCES:				 msgText = "Разрешение отвергнуто\n";						  break;
    case WSAEFAULT:				 msgText = "Ошибочный адрес\n";								  break;
    case WSAEINVAL:				 msgText = "Ошибка в аргументе\n";							  break;
    case WSAEMFILE:				 msgText = "Слишком много файлов открыто\n";				  break;
    case WSAEWOULDBLOCK:		 msgText = "Ресурс временно недоступен\n";					  break;
    case WSAEINPROGRESS:		 msgText = "Операция в процессе развития\n";				  break;
    case WSAEALREADY: 			 msgText = "Операция уже выполняется\n";					  break;
    case WSAENOTSOCK:   		 msgText = "Сокет задан неправильно\n";						  break;
    case WSAEDESTADDRREQ:		 msgText = "Требуется адрес расположения\n";				  break;
    case WSAEMSGSIZE:  			 msgText = "Сообщение слишком длинное\n";				      break;
    case WSAEPROTOTYPE:			 msgText = "Неправильный тип протокола для сокета\n";		  break;
    case WSAENOPROTOOPT:		 msgText = "Ошибка в опции протокола\n";					  break;
    case WSAEPROTONOSUPPORT:	 msgText = "Протокол не поддерживается\n";					  break;
    case WSAESOCKTNOSUPPORT:	 msgText = "Тип сокета не поддерживается\n";				  break;
    case WSAEOPNOTSUPP:			 msgText = "Операция не поддерживается\n";					  break;
    case WSAEPFNOSUPPORT:		 msgText = "Тип протоколов не поддерживается\n";			  break;
    case WSAEAFNOSUPPORT:		 msgText = "Тип адресов не поддерживается протоколом\n";	  break;
    case WSAEADDRINUSE:			 msgText = "Адрес уже используется\n";						  break;
    case WSAEADDRNOTAVAIL:		 msgText = "Запрошенный адрес не может быть использован\n";	  break;
    case WSAENETDOWN:			 msgText = "Сеть отключена\n";								  break;
    case WSAENETUNREACH:		 msgText = "Сеть не достижима\n";							  break;
    case WSAENETRESET:			 msgText = "Сеть разорвала соединение\n";					  break;
    case WSAECONNABORTED:		 msgText = "Программный отказ связи\n";						  break;
    case WSAECONNRESET:			 msgText = "Связь восстановлена\n";							  break;
    case WSAENOBUFS:			 msgText = "Не хватает памяти для буферов\n";				  break;
    case WSAEISCONN:			 msgText = "Сокет уже подключен\n";							  break;
    case WSAENOTCONN:			 msgText = "Сокет не подключен\n";							  break;
    case WSAESHUTDOWN:			 msgText = "Нельзя выполнить send: сокет завершил работу\n";  break;
    case WSAETIMEDOUT:			 msgText = "Закончился отведенный интервал  времени\n";		  break;
    case WSAECONNREFUSED:		 msgText = "Соединение отклонено\n";						  break;
    case WSAEHOSTDOWN:			 msgText = "Хост в неработоспособном состоянии\n";			  break;
    case WSAEHOSTUNREACH:		 msgText = "Нет маршрута для хоста\n";						  break;
    case WSAEPROCLIM:			 msgText = "Слишком много процессов\n";						  break;
    case WSASYSNOTREADY:		 msgText = "Сеть не доступна\n";							  break;
    case WSAVERNOTSUPPORTED:	 msgText = "Данная версия недоступна\n";					  break;
    case WSANOTINITIALISED:		 msgText = "Не выполнена инициализация WS2_32.DLL\n";		  break;
    case WSAEDISCON:			 msgText = "Выполняется отключение\n";						  break;
    case WSATYPE_NOT_FOUND:		 msgText = "Класс не найден\n";								  break;
    case WSAHOST_NOT_FOUND:		 msgText = "Хост не найден\n";								  break;
    case WSATRY_AGAIN:			 msgText = "Неавторизированный хост не найден\n";			  break;
    case WSANO_RECOVERY:		 msgText = "Неопределенная ошибка\n";						  break;
    case WSANO_DATA:			 msgText = "Нет записи запрошенного типа\n";				  break;
    case WSA_INVALID_HANDLE:	 msgText = "Указанный дескриптор события  с ошибкой\n";		  break;
    case WSA_INVALID_PARAMETER:	 msgText = "Один или более параметров с ошибкой\n";			  break;
    case WSA_IO_INCOMPLETE:		 msgText = "Объект ввода-вывода не в сигнальном состоянии\n"; break;
    case WSA_IO_PENDING:		 msgText = "Операция завершится позже\n";					  break;
    case WSA_NOT_ENOUGH_MEMORY:	 msgText = "Не достаточно памяти\n";						  break;
    case WSA_OPERATION_ABORTED:	 msgText = "Операция отвергнута\n";							  break;
    case WSAEINVALIDPROCTABLE:	 msgText = "Ошибочный сервис\n";							  break;
    case WSAEINVALIDPROVIDER:	 msgText = "Ошибка в версии сервиса\n";						  break;
    case WSAEPROVIDERFAILEDINIT: msgText = "Невозможно инициализировать сервис\n";			  break;
    case WSASYSCALLFAILURE:		 msgText = "Аварийное завершение системного вызова\n";		  break;
    default:					 msgText = "Error\n";										  break;
    };
    return msgText;
}

string SetErrorMsgText(string msgText, int code)
{
    return msgText + GetErrorMsgText(code);
}

int _tmain(int argc, _TCHAR* argv[])
{
    setlocale(LC_CTYPE, "Russian");

    if (argc < 2) {
        cout << "Usage: client.exe <Tc>" << endl;
        cout << "Tc - interval in milliseconds" << endl;
        system("pause");
        return 1;
    }

    // Преобразуем _TCHAR* в int
    int Tc = _ttoi(argv[1]);

    string IP = SIP;

    SYSTEMTIME tm;
    GETSINCHRO getsincro;
    SETSINCRO setsincro;

    ZeroMemory(&setsincro, sizeof(setsincro));
    ZeroMemory(&getsincro, sizeof(getsincro));

    strcpy_s(getsincro.cmd, "SINC");
    getsincro.curvalue = 0;

    cout << "Client run with Tc = " << Tc << " ms" << endl;

    try
    {
        SOCKET cS;
        WSADATA wsaData;

        if (WSAStartup(MAKEWORD(2, 0), &wsaData) != 0)
            throw SetErrorMsgText("Startup: ", WSAGetLastError());

        if ((cS = socket(AF_INET, SOCK_DGRAM, NULL)) == INVALID_SOCKET)
            throw SetErrorMsgText("Socket: ", WSAGetLastError());

        SOCKADDR_IN serv;
        serv.sin_family = AF_INET;
        serv.sin_port = htons(PORT);
        serv.sin_addr.s_addr = inet_addr(IP.c_str());

        int maxcor = INT_MIN;
        int mincor = INT_MAX;
        int avgcorr = 0;
        int lensockaddr = sizeof(serv);

        vector<int> corrections;
        vector<long long> rttimes; // времена приема-передачи

        cout << "\nStarting synchronization with improved algorithm...\n" << endl;

        for (int i = 0; i < 10; i++)
        {
            // Время отправки запроса
            clock_t sendTime = clock();

            GetSystemTime(&tm);

            // Отправляем запрос
            sendto(cS, (char*)&getsincro, sizeof(getsincro), 0, (sockaddr*)&serv, sizeof(serv));

            // Получаем ответ
            recvfrom(cS, (char*)&setsincro, sizeof(setsincro), 0, (sockaddr*)&serv, &lensockaddr);

            // Время получения ответа
            clock_t receiveTime = clock();

            // Вычисляем время в пути (round-trip time)
            long long rtt = receiveTime - sendTime;
            rttimes.push_back(rtt);

            // Улучшенный алгоритм: учитываем половину времени в пути
            // Предполагаем, что время в пути одинаково в обе стороны
            int oneWayDelay = (int)(rtt / 2);

            // Коррекция с учетом времени в пути
            // serverTime из ответа - это время получения запроса сервером
            // К нему добавляем время в пути, чтобы получить актуальное время сервера
            int improvedCorrection = setsincro.correction + oneWayDelay;

            corrections.push_back(improvedCorrection);

            maxcor = max(maxcor, improvedCorrection);
            mincor = min(mincor, improvedCorrection);
            avgcorr += improvedCorrection;

            // Выводим детальную информацию
            cout << "--- Request " << i + 1 << " ---" << endl;
            cout << "Local time: " << tm.wHour + 3 << ":" << tm.wMinute << ":"
                << tm.wSecond << "." << tm.wMilliseconds << endl;
            cout << "Current counter: " << getsincro.curvalue << endl;
            cout << "Raw correction: " << setsincro.correction << endl;
            cout << "RTT: " << rtt << " ticks" << endl;
            cout << "One-way delay: " << oneWayDelay << " ticks" << endl;
            cout << "Improved correction: " << improvedCorrection << endl;
            cout << "Max/Min: " << maxcor << "/" << mincor << endl;
            cout << "Average so far: " << avgcorr / (i + 1) << endl << endl;

            // Обновляем значение счетчика с учетом улучшенной коррекции
            if (i == 0)
                getsincro.curvalue += improvedCorrection;
            else
                getsincro.curvalue += improvedCorrection + Tc;

            Sleep(Tc);
        }

        // Итоговая статистика
        cout << "\n========================================\n";
        cout << "FINAL STATISTICS for Tc = " << Tc << " ms\n";
        cout << "========================================\n";
        cout << "Max correction: " << maxcor << endl;
        cout << "Min correction: " << mincor << endl;
        cout << "Average correction: " << avgcorr / 10 << endl;

        // Статистика по RTT
        long long avgRTT = 0;
        for (size_t i = 0; i < rttimes.size(); i++)
            avgRTT += rttimes[i];
        cout << "Average RTT: " << avgRTT / rttimes.size() << " ticks" << endl;

        if (closesocket(cS) == SOCKET_ERROR)
            throw SetErrorMsgText("Closesocket: ", WSAGetLastError());

        if (WSACleanup() == SOCKET_ERROR)
            throw SetErrorMsgText("Cleanup: ", WSAGetLastError());
    }
    catch (string errorMsgText)
    {
        cout << endl << errorMsgText << endl;
    }

    system("pause");
    return 0;
}