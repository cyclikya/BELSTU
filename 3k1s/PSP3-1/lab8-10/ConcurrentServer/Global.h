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
#define DEFAULT_UDP_PORT    3000

extern char g_callsign[64];
extern int  g_udpPort;
extern int g_port;

extern std::atomic<DWORD> g_lastAcceptTime;

extern std::atomic<bool> g_acceptEnabled;   // start / stop / wait
extern std::atomic<bool> g_serverRunning;   // shutdown

extern std::atomic<int> g_totalConnections;
extern std::atomic<int> g_activeConnections;
extern std::atomic<int> g_timeoutConnections;