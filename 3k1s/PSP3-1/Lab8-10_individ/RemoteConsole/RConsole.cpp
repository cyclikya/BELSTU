#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <tchar.h>
#include "iostream"
#include "Windows.h"                
#include "ctime"
#include <string>
#include <cstring>
using std::string;
using namespace std;

string GetErrorMsgText(int code) // cформировать текст ошибки
{
	char buff[50];
	DWORD bufflen = sizeof(buff);
	DWORD errorMessageID = ::GetLastError();
	if (errorMessageID == 0) {
		return std::string(); //No error message has been recorded
	}
	else {
		return std::system_category().message(errorMessageID);
	}
};
string SetPipeError(string msgText, int code)
{
	return msgText + to_string(code) + ". " + GetErrorMsgText(code);
};


int _tmain(int argc, _TCHAR* argv[]) {
	setlocale(LC_ALL, "Russian");

	char ReadBuf[200] = "";
	char WriteBuf[200] = "";
	DWORD nBytesRead;
	DWORD nBytesWrite;

	#pragma region SecurityAttributes
		SECURITY_DESCRIPTOR* m_pSecDesc = (SECURITY_DESCRIPTOR*)LocalAlloc(LPTR, SECURITY_DESCRIPTOR_MIN_LENGTH);
		SECURITY_ATTRIBUTES m_pSecAttrib = { sizeof(SECURITY_ATTRIBUTES), m_pSecDesc, TRUE };
		InitializeSecurityDescriptor(m_pSecDesc, SECURITY_DESCRIPTOR_REVISION);
		SetSecurityDescriptorDacl(m_pSecDesc, TRUE, (PACL)NULL, FALSE);
	#pragma endregion


	int Code = 0;

	char serverName[256];
	char PipeName[512];
	bool result;

	try 
	{
		printf_s("\n ---------- Доступные команды ---------- \n");
		printf_s("1 - start  \t (разрешить подключение клиентов к серверу)\n");
		printf_s("2 - stop  \t (запретить подключение клиентов к серверу)\n");
		printf_s("3 - exit  \t (завершить работу сервера)\n");
		printf_s("4 - statistics\t (вывод статистики)\n");
		printf_s("5 - wait  \t (приостанавливает подключение клиентов)\n");
		printf_s("6 - shutdown  \t (wait + exit)\n");
		printf_s("7 - OPEN_ACCEPT XXXX  \t (открыть порт XXXX для подключений)\n");
		printf_s("8 - CLOSE_ACCEPT XXXX  \t (закрыть порт XXXX)\n");
		printf_s("0 - закрыть\n");
		printf_s("\n ---------- ---------- ---------- ---------- \n");

		cout << "Введите имя севера: ";
		cin >> serverName;
		result = sprintf_s(PipeName, "\\\\%s\\pipe\\cpipe", serverName);


		HANDLE hNamedPipe = CreateFile(PipeName, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, &m_pSecAttrib);

		do {
			printf_s("Команда: ");
			char input[200];
			cin.getline(input, sizeof(input));
			
			if (strlen(input) == 0) continue;
			
			// Парсим ввод: может быть "7 8080" или просто "7"
			char* token = strtok(input, " \t");
			if (token == NULL) continue;
			
			int Code = atoi(token);
			
			if (Code == 7) {
				// OPEN_ACCEPT XXXX - проверяем, есть ли номер порта в строке
				int portNum = 0;
				token = strtok(NULL, " \t");
				if (token != NULL) {
					portNum = atoi(token);
				}
				else {
					// Если порт не указан, запрашиваем отдельно
					printf_s("Введите номер порта: ");
					if (scanf_s("%d", &portNum) != 1) {
						cout << "Ошибка: неверный номер порта" << endl;
						cin.ignore(1000, '\n');
						continue;
					}
					cin.ignore(1000, '\n');
				}
				
				if (portNum > 0 && portNum <= 65535) {
					sprintf_s(WriteBuf, "OPEN_ACCEPT %d", portNum);
					if (!WriteFile(hNamedPipe, WriteBuf, strlen(WriteBuf) + 1, &nBytesWrite, NULL)) throw "WriteFile: Ошибка ";
					if (!ReadFile(hNamedPipe, ReadBuf, sizeof(ReadBuf), &nBytesRead, NULL)) throw "ReadFile: Ошибка ";
					cout << ReadBuf << endl;
				}
				else {
					cout << "Ошибка: неверный номер порта (должен быть от 1 до 65535)" << endl;
				}
			}
			else if (Code == 8) {
				// CLOSE_ACCEPT XXXX - проверяем, есть ли номер порта в строке
				int portNum = 0;
				token = strtok(NULL, " \t");
				if (token != NULL) {
					portNum = atoi(token);
				}
				else {
					// Если порт не указан, запрашиваем отдельно
					printf_s("Введите номер порта: ");
					if (scanf_s("%d", &portNum) != 1) {
						cout << "Ошибка: неверный номер порта" << endl;
						cin.ignore(1000, '\n');
						continue;
					}
					cin.ignore(1000, '\n');
				}
				
				if (portNum > 0 && portNum <= 65535) {
					sprintf_s(WriteBuf, "CLOSE_ACCEPT %d", portNum);
					if (!WriteFile(hNamedPipe, WriteBuf, strlen(WriteBuf) + 1, &nBytesWrite, NULL)) throw "WriteFile: Ошибка ";
					if (!ReadFile(hNamedPipe, ReadBuf, sizeof(ReadBuf), &nBytesRead, NULL)) throw "ReadFile: Ошибка ";
					cout << ReadBuf << endl;
				}
				else {
					cout << "Ошибка: неверный номер порта (должен быть от 1 до 65535)" << endl;
				}
			}
			else if (Code > 0 && Code < 7) {
				sprintf_s(WriteBuf, "%d", Code - 1);
				if (!WriteFile(hNamedPipe, WriteBuf, strlen(WriteBuf) + 1, &nBytesWrite, NULL)) throw "WriteFile: Ошибка ";
				if (!ReadFile(hNamedPipe, ReadBuf, sizeof(ReadBuf), &nBytesRead, NULL)) throw "ReadFile: Ошибка ";
				cout << ReadBuf << endl;
			}
			if (Code == 0) break;
		}
		while (true); 

		if (!CloseHandle(hNamedPipe)) throw SetPipeError("CloseHandle: ", GetLastError());
	}
	catch (string ErrorPipeText)
	{
		cout << endl << ErrorPipeText;
	}
	cout << "RConsole остановлена\n\n";
	return 0;
}
