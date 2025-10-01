//вариант 13 задание 5
#include <iostream>
#include <string>
using namespace std;

// Объединение (union), которое позволяет хранить различные типы данных в одной области памяти
union CountryInfo {
    int population;
    double area;
    string capital;
    string president;
};

// Структура, представляющая информацию о государстве
struct Country {
    string name;
    CountryInfo info;
};
struct Country countries[3];

int main() {
    setlocale(LC_ALL, "Russian");

    // Создаем объект структуры для каждого государства
    countries[0].name = "Россия";
    countries[0].info.population = 144500000;
    countries[0].info.area = 17098242;
    countries[0].info.capital = "Москва";
    countries[0].info.president = "Путин";

    countries[1].name = "США";
    countries[1].info.population = 331000000;
    countries[1].info.area = 9833517;
    countries[1].info.capital = "Вашингтон";
    countries[1].info.president = "Байден";

    countries[2].name = "Китай";
    countries[2].info.population = 1402000000;
    countries[2].info.area = 9596960;
    countries[2].info.capital = "Пекин";
    countries[2].info.president = "Си Цзиньпин";

    string countryName;
    cout << "Введите название государства: ";
    getline(cin, countryName);

    // Поиск и вывод информации о государстве
    Country* selectedCountry = nullptr;
    for (int i = 0; i < 3; ++i) {
        if (countries[i].name == countryName) {
            selectedCountry = &countries[i];
            break;
        }
    }

    if (selectedCountry != nullptr) {
        cout << "Информация о государстве " << selectedCountry->name << ":" << endl;
        cout << "Столица: " << selectedCountry->info.capital << endl;
        cout << "Численность населения: " << selectedCountry->info.population << endl;
        cout << "Площадь: " << selectedCountry->info.area << endl;
        cout << "Президент: " << selectedCountry->info.president << endl;
    }
    else {
        cout << "Государство с названием '" << countryName << "' не найдено." << endl;
    }

    return 0;
}
