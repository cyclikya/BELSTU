#pragma once
#include "Global.h"
#include "AcceptServer.h"

DWORD WINAPI ConsolePipe(LPVOID pPrm) {
	cout << "ConsolePipe работает\n";
	DWORD rc = 0;
	HANDLE hPipe;
	try 
	{
		char rnpname[50];
		strcpy_s(rnpname, "\\\\.\\pipe\\");
		strcat_s(rnpname, npname);
		if ((hPipe = CreateNamedPipe(rnpname, PIPE_ACCESS_DUPLEX, PIPE_TYPE_MESSAGE | PIPE_WAIT, 1, NULL, NULL, INFINITE, NULL)) == INVALID_HANDLE_VALUE) throw SetErrorMsgText("Create:", GetLastError());

		while (*((TalkersCommand*)pPrm) != EXIT) {
			if (!ConnectNamedPipe(hPipe, NULL)) throw SetErrorMsgText("Connect:", GetLastError());
			char ReadBuf[200], WriteBuf[200];
			DWORD nBytesRead = 0, nBytesWrite = 0;
			TalkersCommand SetCommand;
			bool serverCommand;

			//циклически  проверяет общую область памяти потоков
			while (*((TalkersCommand*)pPrm) != EXIT) {
				//после получения команды  getcommand запрашивает следующую команду управления сервером
				if (*((TalkersCommand*)pPrm) == GETCOMMAND) {

					if (!ReadFile(hPipe, ReadBuf, sizeof(ReadBuf), &nBytesRead, NULL)) break;
					if (nBytesRead > 0) {
						serverCommand = true;
						ReadBuf[nBytesRead] = '\0'; // Убеждаемся, что строка завершена
						
						// Проверяем, является ли команда строковой (OPEN_ACCEPT или CLOSE_ACCEPT)
						if (strncmp(ReadBuf, "OPEN_ACCEPT", 11) == 0 && (ReadBuf[11] == ' ' || ReadBuf[11] == '\t' || ReadBuf[11] == '\0')) {
							// Пропускаем "OPEN_ACCEPT" и пробелы
							char* portStr = ReadBuf + 11;
							while (*portStr == ' ' || *portStr == '\t') portStr++;
							
							int portNum = atoi(portStr);
							if (portNum > 0 && portNum <= 65535) {
								if (OpenAcceptPort(portNum, (TalkersCommand*)pPrm)) {
									sprintf_s(WriteBuf, "Порт %d открыт", portNum);
								}
								else {
									sprintf_s(WriteBuf, "Ошибка: порт %d уже открыт или не удалось открыть", portNum);
								}
							}
							else {
								sprintf_s(WriteBuf, "Ошибка: неверный номер порта (должен быть от 1 до 65535)");
							}
							serverCommand = false;
						}
						else if (strncmp(ReadBuf, "CLOSE_ACCEPT", 12) == 0 && (ReadBuf[12] == ' ' || ReadBuf[12] == '\t' || ReadBuf[12] == '\0')) {
							// Пропускаем "CLOSE_ACCEPT" и пробелы
							char* portStr = ReadBuf + 12;
							while (*portStr == ' ' || *portStr == '\t') portStr++;
							
							int portNum = atoi(portStr);
							if (portNum > 0 && portNum <= 65535) {
								if (CloseAcceptPort(portNum)) {
									sprintf_s(WriteBuf, "Порт %d закрыт", portNum);
								}
								else {
									sprintf_s(WriteBuf, "Ошибка: порт %d не найден или не открыт", portNum);
								}
							}
							else {
								sprintf_s(WriteBuf, "Ошибка: неверный номер порта (должен быть от 1 до 65535)");
							}
							serverCommand = false;
						}
						else {
							// Старая логика для числовых команд
							int n = atoi(ReadBuf);
							
							switch (n) {
								case 0:
									sprintf_s(WriteBuf, "%s", "START");
									SetCommand = TalkersCommand::START;
									break;
								case 1:
									sprintf_s(WriteBuf, "%s", "STOP");
									SetCommand = TalkersCommand::STOP;
									break;
								case 2:
									sprintf_s(WriteBuf, "%s", "EXIT");
									SetCommand = TalkersCommand::EXIT;
									break;
								case 3:
									sprintf_s(WriteBuf, "\nАктивных: \t%i\nОтказов: \t%i\nЗакрытых: \t%i\n", Accept, Fail, Finished);
									serverCommand = false;
									break;
								case 4:
									sprintf_s(WriteBuf, "%s", "WAIT");
									SetCommand = TalkersCommand::WAIT;
									break;
								case 5:
									sprintf_s(WriteBuf, "%s", "SHUTDOWN");
									SetCommand = TalkersCommand::SHUTDOWN;
									break;
								default:
									sprintf_s(WriteBuf, "%s", "nocmd");
									serverCommand = false;
									break;
							}
						}
						
						if (serverCommand == true) {
							*((TalkersCommand*)pPrm) = SetCommand;
							printf_s("ConsolePipe: команда %s\n", WriteBuf);
						}
						else {
							printf_s("ConsolePipe: %s\n", WriteBuf);
						}
						if (!WriteFile(hPipe, WriteBuf, sizeof(WriteBuf), &nBytesRead, NULL)) throw new string("CP WRITE ERROR");
					}
				}
				else Sleep(1000);
			}
			if (!DisconnectNamedPipe(hPipe)) throw SetErrorMsgText("disconnect:", GetLastError());
		}
		DisconnectNamedPipe(hPipe);
		CloseHandle(hPipe);
		cout << "ConsolePipe остановлен" << endl;
	}
	catch (string ErrorPipeText) {
		cout << ErrorPipeText << endl;
	}
	catch (...) {
		cout << "Error ConsolePipe" << endl;
	}
	ExitThread(rc);
}