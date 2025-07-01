#include <iostream>
#include <iomanip> 
#include <cmath>
using namespace std;
int Dw()
{
	float d, c, a = 2, m = 6, b = 5e3, k;
	for (; a <= 2.8; a += 0.2)
	{
		for (int n = 0; n < 3; n++)
		{
			cin >> k;
			d = sin(k / a) / cos(m * b);
			c = d / (pow(d, 2) + 1) / (1 - exp(k));
			printf("a = %5.2f\t", a);
			printf("k = %5.2f\t", k);
			printf("d = %5.2f\t", d);
			printf("c = %5.2f\n", c);
		}
		printf("\n");
	}
	return 0;
}
int For()
{
	float d, c, a = 8, m = 6, b = 5 * pow(10, 3), k;


	for (int n = 0; n < 3; n++)
	{
		cin >> k;

		d = sin(k/ a) / cos(m * b);
		c = d / (pow(d, 2) + 1) / (1 - exp(k));
		printf("k = %5.2f\t", k);
		printf("d = %5.2f\t", d);
		printf("c = %5.2f\n", c);
	}
	return 0;
}
int While()
{
	float d, c, a = 8, m = 6, b = 5 * pow(10, 3), k = a;
	do
	{
		d = sin(k / a) / cos(m * b);
		c = d / (pow(d, 2) + 1) / (1 - exp(k));
		printf("k = %5.2f\n", k);
		printf("d = %5.2f\t", d);
		printf("c = %5.2f\n", c);
		printf("\n");
		k = k - 0.5;
	} while (k >= 3);
	return 0;
}


int Dw2()
{
	float d, k, c, a = 2, m = 6, b = 5 * pow(10, 3);
	int i;
	cout << "Введите значенеи числа k [1(1.7), 2(5), 3(-2)]: ";
	cin >> i;
	for (; a <= 2.8; a += 0.2)
	{
		if (i == 1)
		{
			k = 1.7;
			d = sin(k / a) / cos(m * b);
			c = d / (pow(d, 2) + 1) / (1 - exp(k));
			printf("a = %5.2f\t", a);
			printf("k = %5.2f\t", k);
			printf("d = %5.2f\t", d);
			printf("c = %5.2f\n", c);
		}
		else if (i == 2)
		{
			k = 5;
			d = sin(k / a) / cos(m * b);
			c = d / (pow(d, 2) + 1) / (1 - exp(k));
			printf("a = %5.2f\t", a);
			printf("k = %5.2f\t", k);
			printf("d = %5.2f\t", d);
			printf("c = %5.2f\n", c);
		}
		else if (i == 3)
		{
			k = -2;
			d = sin(k / a) / cos(m * b);
			c = d / (pow(d, 2) + 1) / (1 - exp(k));
			printf("a = %5.2f\t", a);
			printf("k = %5.2f\t", k);
			printf("d = %5.2f\t", d);
			printf("c = %5.2f\n", c);
		}
		else
		{
			cout << "Ошибка";
		}
		printf("\n");
	}
	return 0;
}
int For2()
{
	float d, k, c, a = 8, m = 6, b = 5 * pow(10, 3);
	int i;
	cout << "Введите значенеи числа k [1(1.6), 2(9.1), 3(8)]: ";
	cin >> i;
	if (i == 1)
	{
		k = 1.6;
		d = sin(k / a) / cos(m * b);
		c = d / (pow(d, 2) + 1) / (1 - exp(k));
		printf("a = %5.2f\t", a);
		printf("k = %5.2f\t", k);
		printf("d = %5.2f\t", d);
		printf("c = %5.2f\n", c);
	}
	else if (i == 2)
	{
		k = 9.1;
		d = sin(k / a) / cos(m * b);
		c = d / (pow(d, 2) + 1) / (1 - exp(k));
		printf("a = %5.2f\t", a);
		printf("k = %5.2f\t", k);
		printf("d = %5.2f\t", d);
		printf("c = %5.2f\n", c);
	}
	else if (i == 3)
	{
		k = 8;
		d = sin(k / a) / cos(m * b);
		c = d / (pow(d, 2) + 1) / (1 - exp(k));
		printf("a = %5.2f\t", a);
		printf("k = %5.2f\t", k);
		printf("d = %5.2f\t", d);
		printf("c = %5.2f\n", c);
	}
	else
	{
		cout << "Ошибка";
	}
	printf("\n");
	return 0;
}


int main()
{
	setlocale(LC_ALL, "Russian");
	cout << Dw();
	//cout << Dw2();
	cout << For();
	//cout << For2();
	cout << While();
	return 0;
}