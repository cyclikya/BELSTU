#pragma once
#define _WINSOCK_DEPRECATED_NO_WARNINGS
#pragma warning(disable : 4996)
#include "Global.h"

// Структура для передачи параметров в поток AcceptServerForPort
struct AcceptServerParams {
	int portNumber;
	TalkersCommand* command;
	bool* shouldStop;
	
	AcceptServerParams() : portNumber(0), command(NULL), shouldStop(NULL) {}
};

// Объявления функций для работы с дополнительными портами
bool OpenAcceptPort(int portNumber, TalkersCommand* command);
bool CloseAcceptPort(int portNumber);


static void WaitClients() {
	bool ListEmpty = false;
	while (!ListEmpty) {
		EnterCriticalSection(&scListContact);
		ListEmpty = Contacts.empty();
		LeaveCriticalSection(&scListContact);
		SleepEx(0, TRUE);
	}
}

//squirt явл максимальным количеством 
//итераций выполнения функции accept(в режиме без блокировки) для
//подключения клиента за один вызов функции AcceptCylcle.
bool AcceptCycle(int squirt, SOCKET* s) {
	bool rc = false;
	Contact c(Contact::ACCEPT, "EchoServer");
	c.hAcceptServer = hAcceptServer;

	// цикл проверки очереди подключений, повторяется squirt раз
	//while(squirt-- > 0 && rc == false)
	//{
	if ((c.s = accept(*s, (sockaddr*)&c.prms, &c.lprms)) == INVALID_SOCKET) {
		if (WSAGetLastError() != WSAEWOULDBLOCK) throw  SetErrorMsgText("Accept:", WSAGetLastError()); //если очередь подключений пуста.
	}
	else {
		rc = true;
		InterlockedIncrement(&Accept);
		InterlockedIncrement(&ClientServiceNumber);
		EnterCriticalSection(&scListContact);
		Contacts.push_front(c);
		LeaveCriticalSection(&scListContact);
		SetEvent(Event);
	}
	//}
	return rc;
};


void CommandsCycle(TalkersCommand& cmd, SOCKET* s)
{
	int squirt = 0; 
	while (cmd != EXIT) // цикл обработки команд консоли и подключений
	{
		switch (cmd) {	//GETCOMMAND Если команда принята функцией на обработку (готова к приему новой команды управления)
		case START:		cmd = GETCOMMAND;	// возобновить подключение клиентов
						squirt = AS_SQUIRT;
						break;

		case STOP:		cmd = GETCOMMAND;	// остановить подключение клиентов
						squirt = 0;
						break;

		case WAIT:		WaitClients();
						cmd = GETCOMMAND;
						squirt = 0;
						cmd = START;
						break;

		case SHUTDOWN:	WaitClients();
						cmd = EXIT;
						break;
		};
		if (cmd != EXIT && squirt > ClientServiceNumber) {
			if (AcceptCycle(squirt, s)) { //цикл запрос/подключение (accept)
				cmd = GETCOMMAND;
			}
			SleepEx(0, TRUE); // выполнить асинхронные процедуры
		}
	}
};

DWORD WINAPI AcceptServer(LPVOID pPrm) {
	cout << "AcceptServer работает\n";
	DWORD rc = 0;
	SOCKET  ServerSocket;
	WSADATA wsaData;

	try {
		if (WSAStartup(MAKEWORD(2, 0), &wsaData) != 0) throw  SetErrorMsgText("Startup:", WSAGetLastError());
		if ((ServerSocket = socket(AF_INET, SOCK_STREAM, NULL)) == INVALID_SOCKET) throw  SetErrorMsgText("Socket:", WSAGetLastError());

		SOCKADDR_IN Server_IN;
		Server_IN.sin_family = AF_INET;
		Server_IN.sin_port = htons(port);
		Server_IN.sin_addr.s_addr = ADDR_ANY;
		/*Server_IN.sin_addr.s_addr = inet_addr("127.0.0.1");*/

		if (bind(ServerSocket, (LPSOCKADDR)&Server_IN, sizeof(Server_IN)) == SOCKET_ERROR) throw  SetErrorMsgText("Bind:", WSAGetLastError());
		if (listen(ServerSocket, SOMAXCONN) == SOCKET_ERROR) throw  SetErrorMsgText("Listen:", WSAGetLastError());

		//позволяет избежать приостановки программы
		//выполнение accept, не приостанавливает выполнение потока, 
		//как это было прежде, а возвращает значение нового сокета, если обнаружен
		//запрос на создание канала(функция connect, выполненная клиентом), или
		//значение INVALID_SOCKET, если запроса на создание канала нет в очереди
		//запросов или возникла ошибка.
		u_long nonblk;
		if (ioctlsocket(ServerSocket, FIONBIO, &(nonblk = 1)) == SOCKET_ERROR) throw SetErrorMsgText("Ioctlsocket:", WSAGetLastError());

		TalkersCommand* command = (TalkersCommand*)pPrm;
		CommandsCycle(*((TalkersCommand*)command), &ServerSocket);

		if (closesocket(ServerSocket) == SOCKET_ERROR) throw  SetErrorMsgText("Сlosesocket:", WSAGetLastError());
		if (WSACleanup() == SOCKET_ERROR) throw  SetErrorMsgText("Cleanup:", WSAGetLastError());
	}
	catch (string errorMsgText) {
		std::cout << errorMsgText << endl;
	}
	catch (...) {
		cout << "AcceptServer error" << endl;
	}
	cout << "AcceptServer остановлен\n" << endl;
	ExitThread(rc);
}

// Функция для работы AcceptServer с конкретным портом
DWORD WINAPI AcceptServerForPort(LPVOID pPrm) {
	AcceptServerParams* params = (AcceptServerParams*)pPrm;
	int portNumber = params->portNumber;
	TalkersCommand* command = params->command;
	bool* shouldStop = params->shouldStop;
	
	char portStr[20];
	sprintf_s(portStr, "%d", portNumber);
	cout << "AcceptServer для порта " << portStr << " работает\n";
	
	DWORD rc = 0;
	SOCKET  ServerSocket;
	WSADATA wsaData;

	try {
		if (WSAStartup(MAKEWORD(2, 0), &wsaData) != 0) throw  SetErrorMsgText("Startup:", WSAGetLastError());
		if ((ServerSocket = socket(AF_INET, SOCK_STREAM, NULL)) == INVALID_SOCKET) throw  SetErrorMsgText("Socket:", WSAGetLastError());

		SOCKADDR_IN Server_IN;
		Server_IN.sin_family = AF_INET;
		Server_IN.sin_port = htons(portNumber);
		Server_IN.sin_addr.s_addr = ADDR_ANY;

		if (bind(ServerSocket, (LPSOCKADDR)&Server_IN, sizeof(Server_IN)) == SOCKET_ERROR) throw  SetErrorMsgText("Bind:", WSAGetLastError());
		if (listen(ServerSocket, SOMAXCONN) == SOCKET_ERROR) throw  SetErrorMsgText("Listen:", WSAGetLastError());

		u_long nonblk;
		if (ioctlsocket(ServerSocket, FIONBIO, &(nonblk = 1)) == SOCKET_ERROR) throw SetErrorMsgText("Ioctlsocket:", WSAGetLastError());

		// Сохраняем сокет в структуре порта
		EnterCriticalSection(&scListPorts);
		for (ListPorts::iterator p = AdditionalPorts.begin(); p != AdditionalPorts.end(); p++) {
			if (p->port == portNumber) {
				p->socket = ServerSocket;
				break;
			}
		}
		LeaveCriticalSection(&scListPorts);

		int squirt = AS_SQUIRT;
		while (*command != EXIT && !(*shouldStop)) {
			if (*command == START || *command == GETCOMMAND) {
				if (squirt > ClientServiceNumber) {
					if (AcceptCycle(squirt, &ServerSocket)) {
						*command = GETCOMMAND;
					}
				}
			}
			SleepEx(0, TRUE);
		}

		if (closesocket(ServerSocket) == SOCKET_ERROR) throw  SetErrorMsgText("Сlosesocket:", WSAGetLastError());
		// WSACleanup не вызываем здесь, так как он должен вызываться только в основном потоке
		// WSAStartup/WSACleanup должны быть сбалансированы, но в многопоточной среде лучше
		// вызывать WSACleanup только в основном потоке
	}
	catch (string errorMsgText) {
		std::cout << "AcceptServer для порта " << portNumber << ": " << errorMsgText << endl;
	}
	catch (...) {
		cout << "AcceptServer для порта " << portNumber << " error" << endl;
	}
	
	char portStr2[20];
	sprintf_s(portStr2, "%d", portNumber);
	cout << "AcceptServer для порта " << portStr2 << " остановлен\n" << endl;
	
	// Удаляем порт из списка
	EnterCriticalSection(&scListPorts);
	for (ListPorts::iterator p = AdditionalPorts.begin(); p != AdditionalPorts.end(); p++) {
		if (p->port == portNumber) {
			p->isActive = false;
			p->socket = INVALID_SOCKET;
			break;
		}
	}
	LeaveCriticalSection(&scListPorts);
	
	delete params;
	ExitThread(rc);
}

// Функция для открытия нового порта
bool OpenAcceptPort(int portNumber, TalkersCommand* command) {
	EnterCriticalSection(&scListPorts);
	
	// Проверяем, не открыт ли уже этот порт
	for (ListPorts::iterator p = AdditionalPorts.begin(); p != AdditionalPorts.end(); p++) {
		if (p->port == portNumber && p->isActive) {
			LeaveCriticalSection(&scListPorts);
			return false; // Порт уже открыт
		}
	}
	
	// Создаем новую структуру порта
	AdditionalPort newPort;
	newPort.port = portNumber;
	newPort.socket = INVALID_SOCKET;
	newPort.isActive = true;
	newPort.command = command;
	newPort.shouldStop = new bool(false);
	
	// Создаем параметры для потока
	AcceptServerParams* params = new AcceptServerParams();
	params->portNumber = portNumber;
	params->command = command;
	params->shouldStop = newPort.shouldStop;
	
	// Создаем поток
	newPort.thread = CreateThread(NULL, NULL, AcceptServerForPort, (LPVOID)params, NULL, NULL);
	if (newPort.thread == NULL) {
		delete params;
		delete newPort.shouldStop;
		LeaveCriticalSection(&scListPorts);
		return false;
	}
	
	AdditionalPorts.push_back(newPort);
	LeaveCriticalSection(&scListPorts);
	
	cout << "Порт " << portNumber << " открыт для подключений\n";
	return true;
}

// Функция для закрытия порта
bool CloseAcceptPort(int portNumber) {
	EnterCriticalSection(&scListPorts);
	
	bool found = false;
	for (ListPorts::iterator p = AdditionalPorts.begin(); p != AdditionalPorts.end(); p++) {
		if (p->port == portNumber && p->isActive) {
			found = true;
			p->isActive = false;
			
			// Устанавливаем флаг остановки
			if (p->shouldStop != NULL) {
				*(p->shouldStop) = true;
			}
			
			// Закрываем сокет
			if (p->socket != INVALID_SOCKET) {
				closesocket(p->socket);
				p->socket = INVALID_SOCKET;
			}
			
			// Ждем завершения потока
			if (p->thread != NULL) {
				WaitForSingleObject(p->thread, 5000); // Ждем максимум 5 секунд
				CloseHandle(p->thread);
				p->thread = NULL;
			}
			
			// Освобождаем память
			if (p->shouldStop != NULL) {
				delete p->shouldStop;
			}
			
			// Удаляем из списка
			AdditionalPorts.erase(p);
			break;
		}
	}
	
	LeaveCriticalSection(&scListPorts);
	
	if (found) {
		cout << "Порт " << portNumber << " закрыт\n";
		return true;
	}
	return false;
}

