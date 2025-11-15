#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include "../OS11_HTAPI/HT.h"
#include <string>
#include <Windows.h>
#pragma comment(lib, "OS11_HTAPI")

using namespace std;
using namespace HT;

int main(int argc, char* argv[])
{
    setlocale(LC_ALL, "Rus");
    srand((unsigned int)time(nullptr));

    if (argc != 2) {
        cout << "Usage: OS11_04.exe path_to_storage" << endl;
        return -1;
    }

    const size_t cSize = strlen(argv[1]) + 1;
    wchar_t* wc = new wchar_t[cSize];
    mbstowcs(wc, argv[1], cSize);

    cout << "[CLIENT 04] Starting, waiting for server..." << endl;

    while (true) {
        HANDLE hServerMutex = OpenMutex(SYNCHRONIZE, FALSE, L"HT_SERVER_MUTEX");
        if (!hServerMutex) {
            cout << "[CLIENT 04] Server not running, waiting..." << endl;
            Sleep(1000);
            continue;
        }
        CloseHandle(hServerMutex);

        HTHandle* HT = OpenExist(wc);
        if (!HT) {
            cout << "[CLIENT 04] Cannot open storage, retrying in 1 second..." << endl;
            Sleep(1000);
            continue;
        }

        cout << "[CLIENT 04] Connected to storage" << endl;

        string key;
        while (true) {
            hServerMutex = OpenMutex(SYNCHRONIZE, FALSE, L"HT_SERVER_MUTEX");
            if (!hServerMutex) {
                cout << "[CLIENT 04] Server stopped, closing storage..." << endl;
                Sleep(10000);
                continue;
            }
            CloseHandle(hServerMutex);

            Sleep(1000);

            key = to_string(rand() % 50);
            cout << "key: " << key << endl;

            HT::Element* elFromHT = Get(HT, new Element(key.c_str(), (int)key.length() + 1));
            if (!elFromHT) {
                cout << "Error: " << HT->LastErrorMessage << endl;
                continue;
            }

            Print(elFromHT);

            int newPayload = atoi((char*)elFromHT->payload) + 1;
            string newPayloadStr = to_string(newPayload);

            if (!Update(HT, elFromHT, newPayloadStr.c_str(), (int)newPayloadStr.length() + 1)) {
                cout << "Error: " << HT->LastErrorMessage << endl;
                continue;
            }

            Print(Get(HT, new Element(key.c_str(), (int)key.length() + 1)));
            cout << "Elements amount: " << HT->ElementCount << endl;
        }

        Close(HT);
        cout << "[CLIENT 04] Waiting for server restart..." << endl;
    }

    delete[] wc;
    return 0;
}
