#include <windows.h>
#include <iostream>
#include "HT.h"

int main()
{
	HT::HTHandle* ht = nullptr;
	SetConsoleCP(1251);
	SetConsoleOutputCP(1251);
	try
	{
		std::cout << "Creating hash-table..." << std::endl;
		ht = HT::Create(1000, 3, 10, 256, L"HTspace.ht");
		if (ht) std::cout << "-- Create: success" << std::endl; else throw "-- Create: error";

		if (HT::Insert(ht, new HT::Element("key222", 7, "payload", 8)))
			std::cout << "-- Insert: success" << std::endl;
		else throw "-- insert:error";

		HT::Element* hte = HT::Get(ht, new HT::Element("key222", 7));
		if (hte) std::cout << "-- Get: success" << std::endl; else throw "-- Get: error";

		HT::Print(hte);

		if (HT::Snap(ht)) std::cout << "-- Snap: success" << std::endl; else throw "-- Snap: error";

		if (HT::Update(ht, hte, "newpayload", 11))
			std::cout << "-- Update: success" << std::endl;
		else throw "-- Update:error";

		HT::Element* hte1 = HT::Get(ht, new HT::Element("key222", 7));
		if (hte1) std::cout << "-- Get: success" << std::endl; else throw "-- Get: error";

		HT::Print(hte1);

		if (HT::Delete(ht, hte1)) std::cout << "-- Delete: success" << std::endl; else throw "-- Delete: error";

		if (HT::Close(ht)) std::cout << "-- Close: success" << std::endl; else throw "-- Close: error";
	}
	catch (const char* msg)
	{
		std::cout << "Error: " << msg << std::endl;
		if (ht != nullptr) std::cout << "Error details: " << HT::GetHTLastError(ht) << std::endl;
		return 1;
	}

	std::cout << "\n=== Second test: opening an existing hash table ===" << std::endl;

	try
	{
		std::cout << "Opening an existing hash table..." << std::endl;
		ht = HT::Open(L"HTspace.ht");
		if (ht) std::cout << "-- Open: success" << std::endl; else throw "-- Open: error";

		if (HT::Insert(ht, new HT::Element("key333", 7, "payload", 8)))
			std::cout << "-- Insert: success" << std::endl;
		else throw "-- insert:error";

		HT::Element* hte = HT::Get(ht, new HT::Element("key222", 7));
		if (hte) std::cout << "-- Get: success" << std::endl; else throw "-- Get: error";

		if (HT::Close(ht)) std::cout << "-- Close: success" << std::endl; else throw "-- Close: error";
	}
	catch (const char* msg)
	{
		std::cout << "Error: " << msg << std::endl;
		if (ht != nullptr) std::cout << "Error details: " << HT::GetHTLastError(ht) << std::endl;
		return 1;
	}

	std::cout << "\nAll tests are successful!" << std::endl;
	return 0;
}