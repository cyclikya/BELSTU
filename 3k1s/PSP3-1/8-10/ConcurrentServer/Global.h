#pragma once
#define WIN32_LEAN_AND_MEAN

#include <winsock2.h>
#include <windows.h>
#include <iostream>
#include <string>

#pragma comment(lib, "Ws2_32.lib")

// значения по методичке ЛР 8–10
#define SERVER_NAME     "Vi"
#define SERVER_CALLSIGN "HELLO"
#define PIPE_NAME       "cpipe"
#define DEFAULT_PORT    2000
