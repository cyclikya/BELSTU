#include <windows.h>
#include <iostream>
#include "../OS10_HTAPI/HTAPI.h"
#include <locale>

#include <vector>

int main()
{
	HT::HTHandle* ht = nullptr;

	try
	{
		std::cout << "Creating hash table..." << std::endl;
		ht = HT::Create(1000, 3, 10, 256, L"HTspace.ht");
		if (ht) std::cout << "-- Create: success" << std::endl; else throw "-- Create: error";

		if (HT::Insert(ht, new HT::Element("key222", 7, "payload", 8)))
			std::cout << "-- Insert: success" << std::endl;
		else throw "-- insert:error";

		auto hte = HT::Get(ht, new HT::Element("key222", 7));
		if (!hte.empty()) {
			std::cout << "-- Get: found " << hte.size() << " elements" << std::endl;
			for (auto& e : hte) {
				HT::Print(e);
			}
		}
		else {
			std::cout << "-- Get: not found" << std::endl;
		}
		if (HT::Snap(ht)) std::cout << "-- Snap: success" << std::endl; else throw "-- Snap: error";

		if (!hte.empty()) {
			for (auto& e : hte) {
				if (HT::Update(ht, e, "newpayload", 11)) std::cout << "-- Update: success" << std::endl;
			}
				
		}
		else throw "-- Update:error";

		auto hte1 = HT::Get(ht, new HT::Element("key222", 7));
		if (!hte1.empty()) {
			std::cout << "-- Get: found " << hte1.size() << " elements" << std::endl;
			for (auto& e : hte1) {
				HT::Print(e);
			}
		}
		else {
			std::cout << "-- Get: not found" << std::endl;
		}
		
		if (!hte1.empty()) {
			for (auto& e : hte1) {
				HT::Delete(ht, e);

			}
			std::cout << "-- Delete: success" << std::endl;
		} else throw "-- Delete: error";
		
		if (HT::Close(ht)) std::cout << "-- Close: success" << std::endl; else throw "-- Close: error";
	}
	catch (const char* msg)
	{
		std::cout << "Error: " << msg << std::endl;
		if (ht != nullptr) std::cout << "Error details: " << HT::GetHTLastError(ht) << std::endl;
		return 1;
	}

	std::cout << "\n=== Second test: Opening existing table ===" << std::endl;


	try
	{
		std::cout << "Opening existing hash table..." << std::endl;
		ht = HT::Open(L"HTspace.ht");
		if (ht) std::cout << "-- Open: success" << std::endl; else throw "-- Open: error";

		if (HT::Insert(ht, new HT::Element("key333", 7, "payload", 8)))
			std::cout << "-- Insert: success" << std::endl;
		else throw "-- insert:error";

		auto hte = HT::Get(ht, new HT::Element("key222", 7));
		if (!hte.empty()) {
			std::cout << "-- Get: found " << hte.size() << " elements" << std::endl;
			for (auto& e : hte) {
				HT::Print(e);
			}
		}
		else {
			std::cout << "-- Get: not found" << std::endl;
		}
		if (HT::Close(ht)) std::cout << "-- Close: success" << std::endl; else throw "-- Close: error";
	}
	catch (const char* msg)
	{
		std::cout << "Error: " << msg << std::endl;
		if (ht != nullptr) std::cout << "Error details: " << HT::GetHTLastError(ht) << std::endl;
		return 1;
	}

	std::cout << "\nAll tests completed successfully!" << std::endl;
	return 0;
}
