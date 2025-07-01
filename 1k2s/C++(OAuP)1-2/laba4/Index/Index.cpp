#include <iostream>
#include <cstring>
using namespace std;

const int MAX_TRAINS = 100; 

typedef struct {
    int train_number;
    char destination[50];
    char departure_time[6];
} Train;

// Прототипы функций
void input(int size);
void output();
void find(char destination[]);

Train trains[MAX_TRAINS];
int train_count = 0; 

int main()
{
    setlocale(LC_ALL, "Russian");
    int choice;
    char time[50];
    int number;
    do
    {
        cout << "\n1. Ввод данных о поездах\n";
        cout << "2. Вывод данных о поездах\n";
        cout << "3. Поиск по пункту назначения\n";
        cout << "0. Выход из программы\n\n";
        cout << "Введите номер операции: ";
        cin >> choice;
        switch (choice)
        {
        case 1:
            cout << "Введите количество поездов: ";
            cin >> number;
            input(number);
            break;
        case 2:
            output();
            break;
        case 3:
            cout << "Введите время: ";
            cin.ignore();
            cin.getline(time, 50);
            find(time);
            break;
        case 0:
            exit(0);
            break;
        default:
            cout << "Неверный ввод. Пожалуйста, введите существующий номер операции.\n";
        }
    } while (choice != 0);
    return 0;
}

void input(int size)
{
    if (train_count + size > MAX_TRAINS)
    {
        cout << "Превышено максимальное количество поездов.\n";
        return;
    }

    for (int i = 0; i < size; ++i)
    {
        cout << "Введите номер поезда: ";
        cin >> trains[train_count].train_number;
        cout << "Введите пункт назначения: ";
        cin.ignore(); 
        cin.getline(trains[train_count].destination, 50);
        cout << "Введите время отправления (в формате чч:мм): ";
        cin >> trains[train_count].departure_time;
        train_count++;
    }
}

void output()
{
    cout << "\nНомер   Пункт назначения    Отправление\n";
    for (int i = 0; i < train_count; ++i)
    {
        cout << trains[i].train_number << "\t" << trains[i].destination << "\t\t";
        cout << "\t" << trains[i].departure_time << endl;
    }
}

void find(char time[])
{
    bool found = false;
    cout << "\nНомер   Пункт назначения    Отправление\n";
    for (int i = 0; i < train_count; ++i)
    {
        int train_hours, train_minutes;
        sscanf_s(trains[i].departure_time, "%d:%d", &train_hours, &train_minutes);

        int search_hours, search_minutes;
        sscanf_s(time, "%d:%d", &search_hours, &search_minutes);

        if (train_hours > search_hours || (train_hours == search_hours && train_minutes > search_minutes))
        {
            cout << trains[i].train_number << "\t" << trains[i].destination << "\t\t";
            cout << "\t" << trains[i].departure_time << endl;
            found = true;
        }
    }
    if (!found)
    {
        cout << "Поезда после " << time << " не найдены.\n";
    }
}
