#include <windows.h>
#include <iostream>
#include "../OS10_HTAPI/HTAPI.h"
#include <locale>

int main()
{
    HT::HTHandle* ht1 = nullptr;
    HT::HTHandle* ht2 = nullptr;

    try
    {
        std::cout << "Creating hash table1..." << std::endl;
        ht1 = HT::Create(1000, 3, 10, 256, L"HTspace01.ht");
        if (ht1) std::cout << "-- Create: success" << std::endl; else throw "-- Create: error";

        if (HT::Insert(ht1, new HT::Element("key222", 7, "payload", 8)))
            std::cout << "-- Insert: success" << std::endl;
        else throw "-- insert:error";

        if (HT::Insert(ht1, new HT::Element("key333", 7, "oooohoo", 8)))
            std::cout << "-- Insert: success" << std::endl;
        else throw "-- insert:error";

        auto list = HT::Get(ht1, new HT::Element("key222", 7));
        if (!list.empty()) {
            std::cout << "-- Get: found " << list.size() << " elements" << std::endl;
            for (auto& e : list) {
                HT::Print(e);
            }
        }
        else {
            std::cout << "-- Get: not found" << std::endl;
        }
        if (!list.empty()) {
            for (auto& e : list) {
                HT::Delete(ht1, e);
            }
        }
        list = HT::Get(ht1, new HT::Element("key222", 7));
        if (!list.empty()) {
            std::cout << "-- Get: found " << list.size() << " elements" << std::endl;
            for (auto& e : list) {
                HT::Print(e);
            }
        }
        else {
            std::cout << "-- Get: not found" << std::endl;
        }
        std::cout << "Creating hash table2..." << std::endl;
        ht2 = HT::Create(1000, 3, 10, 256, L"HTspace02.ht");
        if (ht2) std::cout << "-- Create: success" << std::endl; else throw "-- Create: error";

        if (HT::Insert(ht2, new HT::Element("key555", 7, "payload2", 9)))
            std::cout << "-- Insert: success" << std::endl;
        else throw "-- insert:error";

        auto list2= HT::Get(ht2, new HT::Element("key555", 6));
        if (!list2.empty()) {
            std::cout << "-- Get: found " << list2.size() << " elements" << std::endl;
            for (auto& e : list2) {
                HT::Print(e);
            }
        }
        else {
            std::cout << "-- Get: not found" << std::endl;
        }

       if (HT::Close(ht1)) std::cout << "-- Close ht1: success" << std::endl; else throw "-- Close ht1: error";

        if (HT::Close(ht2)) std::cout << "-- Close ht2: success" << std::endl; else throw "-- Close ht2: error";
    }
    catch (const char* msg)
    {
        std::cout << "Error: " << msg << std::endl;
        if (ht1 != nullptr) std::cout << "Error details ht1: " << HT::GetHTLastError(ht1) << std::endl;
        if (ht2 != nullptr) std::cout << "Error details ht2: " << HT::GetHTLastError(ht2) << std::endl;
        if (ht1 != nullptr) HT::Close(ht1);
        if (ht2 != nullptr) HT::Close(ht2);
        return 1;
    }

    std::cout << "\nAll tests completed successfully!" << std::endl;
    return 0;
}
