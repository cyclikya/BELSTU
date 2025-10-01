// вариант 2
#include <iostream>    

# define str_len 30                               
# define size 30  

using namespace std;

void enter_new();
void del();
void change();
void out();

enum Facultet
{
	Fit = 1, Htit, Lhf, Ftov, Pim
} fac;


struct Student
{
	char FIO[80];
	char data[80];
	char spec[80];
	int gr;
	char fac;
	float sr;
};
struct Student list_of_student[size];
struct Student bad;
int current_size = 0; int choice;

int main()
{
	setlocale(LC_CTYPE, "Russian");
	cout << "Введите:" << endl;
	cout << "1-для удаления записи" << endl;
	cout << "2-для ввода новой записи" << endl;
	cout << "3-для изменения записи" << endl;
	cout << "4-для вывода записи(ей)" << endl;
	cout << "5-для выхода" << endl;
	cin >> choice;
	do
	{
		switch (choice)
		{
		case 1:  del();	break;
		case 2:  enter_new();  break;
		case 3:  change();  break;
		case 4:  out();	break;
		}
	} while (choice != 5);
}
void enter_new()
{
	cout << "Ввод информации" << endl;
	if (current_size < size)
	{
		cout << "Строка номер ";
		cout << current_size + 1;
		cout << endl << "ФИО " << endl;
		cin >> list_of_student[current_size].FIO;
		cout << "Год поступления " << endl;
		cin >> list_of_student[current_size].data;
		cout << "специальность " << endl;
		cin >> list_of_student[current_size].spec;
		cout << "группа " << endl;
		cin >> list_of_student[current_size].gr;
		cout << "факультет(ФИТ-1, ХТИТ-2, ЛХФ-3, ФТОВ-4, ПИМ-5) " << endl;
		cin >> list_of_student[current_size].fac;
		cout << "средний балл " << endl;
		cin >> list_of_student[current_size].sr;
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
	cout << "\nНомер строки, которую надо удалить (для удаления всех строк нажать 99)" << endl;  cin >> d;
	if (d != 99)
	{
		for (int de1 = (d - 1); de1 < current_size; de1++)
			list_of_student[de1] = list_of_student[de1 + 1];
		current_size = current_size - 1;
	}
	if (d == 99)
		for (int i = 0; i < size; i++)
			list_of_student[i] = bad;
	cout << "Что дальше?" << endl;
	cin >> choice;
}
void change()
{
	int n, per;
	cout << "\nВведите номер строки" << endl; 	cin >> n;
	do
	{
		cout << "Введите: " << endl;
		cout << "1-для изменения фамилии" << endl;
		cout << "2-для изменения года рождения" << endl;
		cout << "3-для изменения факультета" << endl;///////////////////////////////////////
		cout << "4-конец\n";
		cin >> per;
		switch (per)
		{
		case 1: cout << "Новые ФИО";
			cin >> list_of_student[n - 1].FIO;   break;
		case 2: cout << "Новая дата поступления";
			cin >> list_of_student[n - 1].data; break;
		case 3: cout << "Новая специальность ";
			cin >> list_of_student[n - 1].spec; break;
		case 4: cout << "Новая группа ";
			cin >> list_of_student[n - 1].gr; break;
		case 5: cout << "Новый факультет ";
			cin >> list_of_student[n - 1].fac; break;
		case 6: cout << "Новый средний балл ";
			cin >> list_of_student[n - 1].sr; break;
		}
	} while (per != 7);
	cout << "Что дальше?" << endl;
	cin >> choice;
}
void out() {
	int sw, n;
	cout << "1-вывод 1 строки" << endl;
	cout << "2-вывод всех строк" << endl;
	cin >> sw;
	if (sw == 1)
	{
		cout << "Номер студента в списке " << endl;   cin >> n;  cout << endl;
		cout << endl << "ФИО ";
		cout << list_of_student[n - 1].FIO << endl;
		cout << "Год поступления ";
		cout << list_of_student[n - 1].data << endl;
		cout << "специальность ";
		cout << list_of_student[n - 1].spec << endl;
		cout << "группа ";
		cout << list_of_student[n - 1].gr << endl;
		cout << "факультет ";
		cout << list_of_student[n - 1].fac << endl;
		cout << "средний балл  ";
		cout << list_of_student[n - 1].sr << endl;
	}
	if (sw == 2)
	{
		for (int i = 0; i < current_size; i++)
		{
			cout << endl << "ФИО ";
			cout << list_of_student[i].FIO << endl;
			cout << "Год поступления ";
			cout << list_of_student[i].data << endl;
			cout << "специальность ";
			cout << list_of_student[i].spec << endl;
			cout << "группа ";
			cout << list_of_student[i].gr << endl;
			cout << "факультет ";
			switch (list_of_student[i].fac) {
				case Fit: cout << "ФИТ"; break;
				case Htit: cout << "ХТИТ"; break;
				case Lhf: cout << "ЛХФ"; break;
				case Ftov: cout << "ФТОВ"; break;
				case Pim: cout << "ПИМ"; break;
				default: cout << "Неизвестный факультет"; // Обработка значения по умолчанию
			}
			cout << "средний балл  ";
			cout << list_of_student[i].sr << endl;
		}
	}
	cout << "Что дальше?" << endl;
	cin >> choice;
}