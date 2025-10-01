#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include "../OS11_HTAPI/HT.h"
#include <string>
#pragma comment(lib, "OS11_HTAPI")

using namespace std;
using namespace HT;

int main(int argc, char* argv[])																													 // D:\University\осисп\labs\OS11\x64\Debug>OS11_03 ./files/os11_create.ht
{
	setlocale(LC_ALL, "Rus");
	srand(time(nullptr));

	try
	{
		if (argc != 2)
		{
			throw "Enter two arguments (name.exe path_to_storage)";
		}

		const size_t cSize = strlen(argv[1]) + 1;
		wchar_t* wc = new wchar_t[cSize];
		mbstowcs(wc, argv[1], cSize);

		HTHandle* HT = OpenExist(wc);
		if (HT == NULL)
		{
			throw "Storage not created";
		}

		string key;

		while (true)
		{
			Sleep(1000);

			key = to_string(rand() % 50);
			cout << "key: " << key << endl;

			HT::Element* elFromHT = Get(HT, new Element(key.c_str(), (int)key.length() + 1));
			if (!elFromHT)
			{
				cout << "Error: " << HT->LastErrorMessage << endl;
				continue;
			}

			if (!Delete(HT, elFromHT))
			{
				cout << "Error: " << HT->LastErrorMessage << endl;
				continue;
			}
			cout << "Elements amount: " << HT->ElementCount << endl;
		}

		Close(HT);
	}
	catch (const char* err)
	{
		cout << err << endl;
		return -1;
	}

	return 0;
}