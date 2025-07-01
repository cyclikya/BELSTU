#include <iostream>
#include "case1.h"
#include "case2.h"
using namespace std;

int main()
{
	setlocale(LC_ALL, "Russian");
	int k;
	cout << "выберите упражнение(1/2):";
	cin >> k;

	switch (k) 
	{
		case 1: {
			case1();
			break;
		}
		case 2: {
			case2();
			break;
		}
		default: {
			cout << "Введено не коректное значение.";
			break;
		}
	}
	return 0;
}
