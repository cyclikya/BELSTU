#include <iostream>
#include <string>
using namespace std;

# define size 7  
# define str_len 30  

void enter_new();
void del();
void change();
void out();

struct Train
{
	char fTown[str_len];
	char time[str_len];
};

struct Train list_of_trains[size];
struct Train bad; // для обнуления

int current_size = 0; 
int choice;

int main()
{
	setlocale(LC_CTYPE, "Russian");
	cout << "Введите:" << endl;
	cout << "1-для удаления записи" << endl;
	cout << "2-для ввода новой записи" << endl;
	cout << "4-для вывода записи(ей)" << endl;
	cout << "5-для выхода" << endl;
	cin >> choice;
	do
	{
		switch (choice)
		{
		case 1:  del();	break;
		case 2:  enter_new();  break;
		case 3:  out();	break;
		}
	} while (choice != 4);
}
void enter_new()
{
	cout << "Ввод информации" << endl;
	if (current_size < size)
	{
		cout << "номер поезда: ";
		cout << current_size + 1;
		cout << endl << "Пункт назначения " << endl;
		cin >> list_of_trains[current_size].fTown;
		cout << "Время прибытия " << endl;
		cin >> list_of_trains[current_size].time;
		current_size++;
	}
	else
		cout << "Введено максимальное кол-во строк";
	cout << "Что дальше?" << endl;
	cin >> choice;
}
void del()
{
	int d;
	cout << "\nНомер поезда, записть о котором надо удалить (для удаления всех записей нажать 99)" << endl;  
	cin >> d;
	if (d != 99)
	{
		for (int de1 = (d - 1); de1 < current_size; de1++)
			list_of_trains[de1] = list_of_trains[de1 + 1];

		current_size = current_size - 1;
	}
	if (d == 99)
		for (int i = 0; i < size; i++)
			list_of_trains[i] = bad;
	cout << "Что дальше?" << endl;
	cin >> choice;
}
void out()
{
	int sw, n;
	cout << "1-вывод 1 строки" << endl;
	cout << "2-вывод всех строк" << endl;
	cin >> sw;
	if (sw == 1)
	{
		cout << "Номер выводимой строки " << endl;   cin >> n;  cout << endl;
		cout << "Поезд под номером " << n << ": " << endl;
		cout << "Пункт прибытия ";
		cout << list_of_trains[n - 1].fTown << endl;
		cout << "Время прибытия ";
		cout << list_of_trains[n - 1].time << endl;
	}
	if (sw == 2)
	{
		for (int i = 0; i < current_size; i++)
		{
			cout << "Поезд под номером " << i + 1 << ": " << endl;

			cout << list_of_trains[i].fTown << endl;
			cout << "Время прибытия ";
			cout << list_of_trains[i].time << endl;
		}
	}
	cout << "Что дальше?" << endl;
	cin >> choice;
}
