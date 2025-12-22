#include "PipeServer.h"
#include "AcceptServer.h"
#include "Global.h"
#include <iostream>
#include <thread>
#include <atomic>

static std::atomic<bool> g_pipeRunning{ true };

DWORD WINAPI PipeThread(LPVOID)
{
    std::string pipeName = "\\\\.\\pipe\\" PIPE_NAME;

    while (g_pipeRunning)
    {
        HANDLE hPipe = CreateNamedPipeA(
            pipeName.c_str(),
            PIPE_ACCESS_DUPLEX,
            PIPE_TYPE_MESSAGE | PIPE_READMODE_MESSAGE | PIPE_WAIT,
            PIPE_UNLIMITED_INSTANCES,
            512, 512,
            0,
            nullptr
        );

        if (hPipe == INVALID_HANDLE_VALUE)
            continue;

        std::cout << "[PIPE] Waiting for RConsole...\n";

        BOOL connected = ConnectNamedPipe(hPipe, nullptr) ?
            TRUE : (GetLastError() == ERROR_PIPE_CONNECTED);

        if (connected)
        {
            std::cout << "[PIPE] RConsole connected\n";

            char cmd[256]{};
            DWORD readBytes = 0;

            while (ReadFile(hPipe, cmd, sizeof(cmd), &readBytes, nullptr))
            {
                cmd[readBytes] = '\0';
                std::string command(cmd);

                std::cout << "[PIPE CMD] " << command << "\n";

                if (command.find("OPEN_ACCEPT") == 0)
                {
                    int port = atoi(command.c_str() + 12);
                    StartAccept(port);
                }
                else if (command.find("CLOSE_ACCEPT") == 0)
                {
                    int port = atoi(command.c_str() + 13);
                    StopAccept(port);
                }
                else if (command == "EXIT")
                {
                    g_pipeRunning = false;
                    break;
                }
            }
        }

        DisconnectNamedPipe(hPipe);
        CloseHandle(hPipe);
    }

    std::cout << "[PIPE] Server stopped\n";
    return 0;
}

void StartPipeServer()
{
    CreateThread(nullptr, 0, PipeThread, nullptr, 0, nullptr);
}

void StopPipeServer()
{
    g_pipeRunning = false;
}
