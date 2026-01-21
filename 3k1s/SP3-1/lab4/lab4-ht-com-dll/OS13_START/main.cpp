#pragma warning(disable : 4996)

#include <iostream>
#include <windows.h>
#include <conio.h>

#include "../OS13_HTCOM_LIB/pch.h"
#include "../OS13_HTCOM_LIB/OS13_HTCOM_LIB.h"

#ifdef _WIN64
#pragma comment(lib, "../x64/Debug/OS13_HTCOM_LIB.lib")
#else
#pragma comment(lib, "../Debug/OS13_HTCOM_LIB.lib")
#endif

using namespace std;

wchar_t* getWC(const char* c);

int main(int argc, char* argv[])
{
	HANDLE hStopEvent = OpenEvent(EVENT_MODIFY_STATE | SYNCHRONIZE, FALSE, L"Stop");
	HANDLE hResumeEvent = OpenEvent(EVENT_MODIFY_STATE | SYNCHRONIZE, FALSE, L"Resume");

	if (!hStopEvent)
		hStopEvent = CreateEvent(NULL, TRUE, FALSE, L"Stop");

	if (!hResumeEvent)
		hResumeEvent = CreateEvent(NULL, TRUE, TRUE, L"Resume"); 

	ResetEvent(hStopEvent);  
	SetEvent(hResumeEvent);  	setlocale(LC_ALL, "Russian");

	try
	{
		cout << "Инициализация компонента:" << endl;
		OS13_HTCOM_HANDEL h = OS13_HTCOM::Init();

		ht::HtHandle* ht = nullptr;
		wchar_t* fileName = getWC(argv[1]);
		ht = OS13_HTCOM::HT::open(h, fileName, false);
		if (ht)
		{
			cout << "HT-Storage Start" << endl;
			wcout << "filename: " << ht->fileName << endl;
			cout << "secSnapshotInterval: " << ht->secSnapshotInterval << endl;
			cout << "capacity: " << ht->capacity << endl;
			cout << "maxKeyLength: " << ht->maxKeyLength << endl;
			cout << "maxPayloadLength: " << ht->maxPayloadLength << endl;

			
		}
		else
			cout << "-- open: error" << endl;

		cout << "[START] Ожидание stop.exe ...\n";

		WaitForSingleObject(hStopEvent, INFINITE);

		cout << "[START] stop получен -> закрытие хранилища\n";

		OS13_HTCOM::HT::snap(h, ht);
		OS13_HTCOM::HT::close(h, ht);


		cout << endl << "Удалить компонент и выгрузить dll, если можно:" << endl;
		OS13_HTCOM::Dispose(h);
	}
	catch (const char* e) { cout << e << endl; }
	catch (int e) { cout << "HRESULT: " << e << endl; }

}

wchar_t* getWC(const char* c)
{
	wchar_t* wc = new wchar_t[strlen(c) + 1];
	mbstowcs(wc, c, strlen(c) + 1);

	return wc;
}
