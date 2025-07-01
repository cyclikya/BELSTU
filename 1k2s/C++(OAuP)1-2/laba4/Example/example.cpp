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
struct Train bad; // ��� ���������

int current_size = 0; 
int choice;

int main()
{
	setlocale(LC_CTYPE, "Russian");
	cout << "�������:" << endl;
	cout << "1-��� �������� ������" << endl;
	cout << "2-��� ����� ����� ������" << endl;
	cout << "4-��� ������ ������(��)" << endl;
	cout << "5-��� ������" << endl;
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
	cout << "���� ����������" << endl;
	if (current_size < size)
	{
		cout << "����� ������: ";
		cout << current_size + 1;
		cout << endl << "����� ���������� " << endl;
		cin >> list_of_trains[current_size].fTown;
		cout << "����� �������� " << endl;
		cin >> list_of_trains[current_size].time;
		current_size++;
	}
	else
		cout << "������� ������������ ���-�� �����";
	cout << "��� ������?" << endl;
	cin >> choice;
}
void del()
{
	int d;
	cout << "\n����� ������, ������� � ������� ���� ������� (��� �������� ���� ������� ������ 99)" << endl;  
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
	cout << "��� ������?" << endl;
	cin >> choice;
}
void out()
{
	int sw, n;
	cout << "1-����� 1 ������" << endl;
	cout << "2-����� ���� �����" << endl;
	cin >> sw;
	if (sw == 1)
	{
		cout << "����� ��������� ������ " << endl;   cin >> n;  cout << endl;
		cout << "����� ��� ������� " << n << ": " << endl;
		cout << "����� �������� ";
		cout << list_of_trains[n - 1].fTown << endl;
		cout << "����� �������� ";
		cout << list_of_trains[n - 1].time << endl;
	}
	if (sw == 2)
	{
		for (int i = 0; i < current_size; i++)
		{
			cout << "����� ��� ������� " << i + 1 << ": " << endl;

			cout << list_of_trains[i].fTown << endl;
			cout << "����� �������� ";
			cout << list_of_trains[i].time << endl;
		}
	}
	cout << "��� ������?" << endl;
	cin >> choice;
}
