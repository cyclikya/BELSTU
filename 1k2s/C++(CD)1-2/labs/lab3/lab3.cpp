#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <map>
#include <windows.h>

using namespace std;

struct Advertiser {
    string bank_details;
    string phone;
    string contact_person;
};

struct Ad {
    string show_name;
    string date;
    int duration;
    double cost_per_minute;
    double rating;
};

vector<Ad> ads;
map<string, Advertiser> advertisers;
int _stateMenu;

void add_ad() {
    Ad ad;
    Advertiser advertiser;

    cout << "Введите название передачи: ";
    cin >> ad.show_name;
    cout << "Введите дату (ДД-ММ-ГГГГ): ";
    cin >> ad.date;
    cout << "Введите продолжительность (минуты): ";
    cin >> ad.duration;
    cout << "Введите стоимость за минуту: ";
    cin >> ad.cost_per_minute;
    cout << "Введите рейтинг передачи: ";
    cin >> ad.rating;

    cout << "Введите банковские реквизиты: ";
    cin >> advertiser.bank_details;
    cout << "Введите телефон: ";
    cin >> advertiser.phone;
    cout << "Введите контактное лицо: ";
    cin >> advertiser.contact_person;

    ads.push_back(ad);
    advertisers[ad.show_name] = advertiser;
    cout << "Данные введены!" << endl;
}

void print_ads() {
    cout << "Список рекламных роликов: " << endl;
    for (const auto& ad : ads) {
        cout << "Передача: " << ad.show_name
            << ", Дата: " << ad.date
            << ", Продолжительность: " << ad.duration
            << ", Стоимость за минуту: " << ad.cost_per_minute
            << ", Рейтинг: " << ad.rating << endl;

        const auto& advertiser = advertisers.at(ad.show_name);
        cout << "Рекламодатель: " << advertiser.contact_person
            << ", Телефон: " << advertiser.phone
            << ", Банковские реквизиты: " << advertiser.bank_details << endl;
    }
    cout << "Данные выведены!" << endl;
}

void save_to_file(const string& fileName) {
    ofstream outFile(fileName);
    if (outFile) {
        outFile << ads.size() << endl;
        for (const auto& ad : ads) {
            outFile << ad.show_name << endl
                << ad.date << endl
                << ad.duration << endl
                << ad.cost_per_minute << endl
                << ad.rating << endl;

            const auto& advertiser = advertisers.at(ad.show_name);
            outFile << advertiser.bank_details << endl
                << advertiser.phone << endl
                << advertiser.contact_person << endl;
        }
        cout << "Данные записаны!" << endl;
    }
    else {
        cout << "Ошибка открытия файла!" << endl;
    }
}

void load_from_file(const string& fileName) {
    ifstream inFile(fileName);
    if (inFile) {
        int size;
        inFile >> size;
        ads.clear();
        advertisers.clear();
        for (int i = 0; i < size; ++i) {
            Ad ad;
            Advertiser advertiser;
            inFile >> ad.show_name
                >> ad.date
                >> ad.duration
                >> ad.cost_per_minute
                >> ad.rating;

            inFile >> advertiser.bank_details
                >> advertiser.phone
                >> advertiser.contact_person;

            ads.push_back(ad);
            advertisers[ad.show_name] = advertiser;
        }
        cout << "Данные считаны!" << endl;
    }
    else {
        cout << "Ошибка открытия файла!" << endl;
    }
}

void Menu() {
    cout << "Выберите действие:" << endl
        << "(1) Ввод данных" << endl
        << "(2) Вывод" << endl
        << "(3) Удаление" << endl
        << "(4) Сохранить в файл" << endl
        << "(5) Загрузить из файла" << endl
        << "(6) Выход" << endl
        << "Ваш выбор: ";
    cin >> _stateMenu;
}

int main() {
    setlocale(LC_ALL, "rus");

    while (true) {
        system("cls");
        Menu();
        switch (_stateMenu) {
        case 1:
            system("cls");
            add_ad();
            system("pause");
            break;
        case 2:
            system("cls");
            print_ads();
            system("pause");
            break;
        case 3:
            system("cls");
            ads.clear();
            advertisers.clear();
            cout << "Данные удалены!" << endl;
            system("pause");
            break;
        case 4:
            system("cls");
            {
                string fileName;
                cout << "Введите имя файла для сохранения: ";
                cin >> fileName;
                save_to_file(fileName);
            }
            system("pause");
            break;
        case 5:
            system("cls");
            {
                string fileName;
                cout << "Введите имя файла для загрузки: ";
                cin >> fileName;
                load_from_file(fileName);
            }
            system("pause");
            break;
        case 6:
            return 0;
        default:
            system("cls");
            cout << "Неверно введен номер действия!" << endl;
            system("pause");
            break;
        }
    }
    return 0;
}