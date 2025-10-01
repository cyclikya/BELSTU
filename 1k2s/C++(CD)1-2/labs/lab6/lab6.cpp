#include <iostream>
#include <iomanip>
#include <fstream>
#include <string>
#include <vector>
#include <map>

using namespace std;

// Структура для рекламного ролика
struct Ad {
    string show_name;
    string date;
    int duration;
    double cost_per_minute;
    double rating;
};

// Структура для рекламодателя
struct Advertiser {
    string bank_details;
    string phone;
    string contact_person;
};

vector<Ad> ads;                 // Вектор для хранения рекламных роликов
map<string, Advertiser> advertisers;   // Мапа для хранения рекламодателей

// Функция для чтения данных из файла
void readFromFile(const string& filename) {
    ifstream file(filename);   // Открытие файла для чтения
    if (!file.is_open()) {     // Проверка на успешное открытие файла
        cout << "Ошибка открытия файла.\n";   // Вывод сообщения об ошибке
        return;                // Возврат из функции
    }

    ads.clear();               // Очистка вектора рекламных роликов
    advertisers.clear();      // Очистка мапы рекламодателей

    Ad ad;                    // Временная переменная для рекламного ролика
    Advertiser advertiser;    // Временная переменная для рекламодателя
    // Чтение данных из файла до конца файла
    while (file >> ad.show_name >> ad.date >> ad.duration >> ad.cost_per_minute >> ad.rating
        >> advertiser.bank_details >> advertiser.phone >> advertiser.contact_person) {
        ads.push_back(ad);    // Добавление рекламного ролика в вектор
        advertisers[ad.show_name] = advertiser;   // Добавление рекламодателя в мапу
    }

    file.close();             // Закрытие файла
}

// Функция для записи данных в файл
void writeToFile(const string& filename) {
    ofstream file(filename);  // Открытие файла для записи
    if (!file.is_open()) {    // Проверка на успешное открытие файла
        cout << "Ошибка открытия файла для записи.\n";  // Вывод сообщения об ошибке
        return;                // Возврат из функции
    }

    // Запись данных в файл
    for (const auto& ad : ads) {
        const auto& advertiser = advertisers[ad.show_name];
        file << ad.show_name << " " << ad.date << " " << ad.duration << " "
            << ad.cost_per_minute << " " << ad.rating << " "
            << advertiser.bank_details << " " << advertiser.phone << " "
            << advertiser.contact_person << endl;
    }

    file.close();             // Закрытие файла
}

// Функция для добавления рекламного ролика
void add_ad() {
    Ad ad;                     // Создание объекта рекламного ролика
    Advertiser advertiser;     // Создание объекта рекламодателя

    // Ввод данных с клавиатуры
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

    ads.push_back(ad);         // Добавление рекламного ролика в вектор
    advertisers[ad.show_name] = advertiser;   // Добавление рекламодателя в мапу
    cout << "Данные введены!" << endl;
}

// Функция для вывода рекламных роликов
void print_ads(const vector<Ad>& ads, const map<string, Advertiser>& advertisers) {
    cout << left << setw(20) << "Передача"
        << setw(15) << "Дата"
        << setw(15) << "Продолжительность"
        << setw(20) << "Стоимость за минуту"
        << setw(10) << "Рейтинг"
        << setw(20) << "Рекламодатель"
        << setw(15) << "Телефон"
        << setw(30) << "Банковские реквизиты" << endl;

    for (const auto& ad : ads) {
        cout << left << setw(20) << ad.show_name
            << setw(15) << ad.date
            << setw(15) << ad.duration
            << setw(20) << ad.cost_per_minute
            << setw(10) << ad.rating;

        const auto& advertiser = advertisers.at(ad.show_name);
        cout << setw(20) << advertiser.contact_person
            << setw(15) << advertiser.phone
            << setw(30) << advertiser.bank_details << endl;
    }

    cout << "Данные выведены!" << endl;
}


// Функция для удаления рекламного ролика по названию передачи
void delete_ad(const string& show_name) {

    auto it = ads.begin();
    while (it != ads.end()) {
        if (it->show_name == show_name) {
            it = ads.erase(it); // Удаление рекламного ролика из вектора
            advertisers.erase(show_name); // Удаление рекламодателя из мапы
            cout << "Рекламный ролик '" << show_name << "' удален.\n";
            return;
        }
        else {
            ++it;
        }
    }
    cout << "Рекламный ролик с названием '" << show_name << "' не найден.\n";
}

// Функция для изменения данных рекламного ролика по названию передачи
void edit_ad(const string& show_name) {

    auto it = ads.begin();
    while (it != ads.end()) {
        if (it->show_name == show_name) {
            Ad& ad = *it; // Получение ссылки на рекламный ролик
            Advertiser& advertiser = advertisers[show_name]; // Получение ссылки на рекламодателя
            cout << "Введите новую дату (ДД-ММ-ГГГГ): ";
            cin >> ad.date;
            cout << "Введите новую продолжительность (минуты): ";
            cin >> ad.duration;
            cout << "Введите новую стоимость за минуту: ";
            cin >> ad.cost_per_minute;
            cout << "Введите новый рейтинг передачи: ";
            cin >> ad.rating;
            cout << "Введите новые банковские реквизиты: ";
            cin >> advertiser.bank_details;
            cout << "Введите новый телефон: ";
            cin >> advertiser.phone;
            cout << "Введите новое контактное лицо: ";
            cin >> advertiser.contact_person;
            cout << "Данные рекламного ролика '" << show_name << "' изменены.\n";
            return;
        }
        else {
            ++it;
        }
    }
    cout << "Рекламный ролик с названием '" << show_name << "' не найден.\n";
}

void clear_ad() {
    ads.clear();             // Очистка вектора рекламных роликов
    advertisers.clear();     // Очистка мапы рекламодателей
    cout << "Все данные удалены!" << endl;
}

// Функция для отображения меню
void Menu() {
    cout << "Выберите действие:" << endl
        << "(1) Ввод данных" << endl
        << "(2) Вывод" << endl
        << "(3) Загрузить данные из файла" << endl
        << "(4) Сохранить данные в файл" << endl
        << "(5) Удалить рекламный ролик" << endl
        << "(6) Удалить все данные" << endl
        << "(7) Изменить данные рекламного ролика" << endl
        << "(8) Выход" << endl
        << "Ваш выбор: ";
}

int main() {
    setlocale(LC_ALL, "rus");

    while (true) {
        Menu();
        int choice;
        cin >> choice;

        switch (choice) {
        case 1:
            add_ad();
            break;
        case 2:
            print_ads(ads, advertisers);
            break;
        case 3: {
            string filename;
            cout << "Введите имя файла для загрузки данных: ";
            cin >> filename;
            readFromFile(filename);
            cout << "Данные загружены из файла.\n";
            break;
        }
        case 4: {
            string filename;
            cout << "Введите имя файла для сохранения данных: ";
            cin >> filename;
            writeToFile(filename);
            cout << "Данные сохранены в файле.\n";
            break;
        }
        case 5: {
            string show_name;
            cout << "Введите название передачи для удаления: ";
            cin >> show_name;
            delete_ad(show_name);       // Вызов функции удаления рекламного ролика
            break;
        }
        case 6:
            clear_ad();
            break;
        case 7: {
            string show_name;
            cout << "Введите название передачи для изменения данных: ";
            cin >> show_name;
            edit_ad(show_name);         // Вызов функции изменения данных рекламного ролика
            break;
        }
        case 8:
            return 0;
        default:
            cout << "Неверный выбор\n";
            break;
        }
    }

    return 0;
}

