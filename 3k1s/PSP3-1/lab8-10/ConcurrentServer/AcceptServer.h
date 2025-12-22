#pragma once
#include "Global.h"
#include <map>

SOCKET StartAccept(int port);
void StopAccept(int port);
void StopAllAccept();

DWORD WINAPI ResponseServer(LPVOID);
DWORD WINAPI InactivityTimerThread(LPVOID);