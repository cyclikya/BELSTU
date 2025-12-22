#include "Global.h"
#include "AcceptServer.h"
#include "PipeServer.h"

int main(int argc, char* argv[])
{
    int port = (argc > 1) ? atoi(argv[1]) : DEFAULT_PORT;

    WSADATA wsa;
    WSAStartup(MAKEWORD(2, 2), &wsa);

    std::cout << "Concurrent Server started\n";
    std::cout << "Server name: " << SERVER_NAME << "\n";
    std::cout << "Callsign: " << SERVER_CALLSIGN << "\n";
    std::cout << "Pipe: " << PIPE_NAME << "\n";
    std::cout << "Port: " << port << "\n";

    StartAccept(port);
    StartPipeServer();

    std::cout << "Server ready. Use RConsole.\n";

    while (true)
        Sleep(1000);

    StopAllAccept();
    StopPipeServer();
    WSACleanup();
    return 0;
}
