#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include "../OS11_HTAPI/HT.h"
#include <string>
#pragma comment(lib, "OS11_HTAPI")

using namespace std;
using namespace HT;

bool checkValue(char* input);

int main(int argc, char* argv[])																														// OS11_CREATE ./files/os11_create.ht 10 5 20 10              OS11_CREATE ./files/os11_04.ht 2000 3 4 4
{ 
	try
	{
		if (argc != 6)
		{
			throw "Enter six arguments!";
		}
		if (!checkValue(argv[2]) || !checkValue(argv[3]) || !checkValue(argv[4]) || !checkValue(argv[5]))
		{
			throw "Enter correct value!";
		}
		const size_t cSize = strlen(argv[1]) + 1;
		wchar_t* wc = new wchar_t[cSize];
		mbstowcs(wc, argv[1], cSize);

		if (GetFileAttributes(wc) != INVALID_FILE_ATTRIBUTES)
		{
			throw "File already exist";
		}

		int capacity = stoi(argv[2]);
		int snapshotinterval = stoi(argv[3]);
		int maxKeyLength = stoi(argv[4]);
		int maxPayloadLength = stoi(argv[5]);

		HTHandle* HT = Create(capacity, snapshotinterval, maxKeyLength, maxPayloadLength, wc);
		if (HT == NULL)
		{
			throw "Storage not created";
		}

		cout << "Storage Created!" << endl;
		wcout << "filename = " << wc << endl;
		cout << "snapshotinterval = " << snapshotinterval << endl;
		cout << "capacity = " << capacity << endl;
		cout << "maxkeylength = " << maxKeyLength << endl;
		cout << "maxdatalength = " << maxPayloadLength << endl;

		Close(HT);
	}
	catch (const char* err)
	{
		cout << err << endl;
		return -1;
	}

	return 0;
}

bool checkValue(char* input)
{
	int i = 0;
	while (input[i] != '\0')
	{
		if (input[i] < '0' || input[i] > '9')
			return false;
		i++;
	}
	return true;
}