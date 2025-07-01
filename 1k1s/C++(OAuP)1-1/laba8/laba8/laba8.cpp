#include <iostream>
#include <cmath>
using namespace std;
void pr1()
{
	float y, d, q = 0, i = 0;
	cout << "Введите Y" << i + 1 << ": ";
	cin >> y;
	d = y;
	for (int i = 1; i < 5; i++)
	{
		cout << "Введите Y" << i + 1 << ": ";
		cin >> y;
		if (y < d)
		{
			d = y;
		}
	}
	for (int i = 0; i < 5; i++)
	{
		cout << "Ещё раз введите Y" << i + 1 << ": ";
		cin >> y;
		q += (4 * y + d);

	}
	printf("q = %.2f\n", q);
	printf("d = %.2f", d);
	cout << "\n";
}
void pr2()
{
	float x, t, y, sum = 0;
	for (float t = 0.5; t <= 3; t += 0.5)
	{
		if (t > 2)
		{
			for (int i = 0; i < 5; i++)
			{
				//while (i < 5)
				//{
					cout << "Введите X" << i + 1 << ": ";
					cin >> x;
					sum += x * x;
					
				//}
			}
			y = sum + t;
		}
		else if (t <= 2)
		{
			y = cos(t * t);
		}
		printf("y = %.2f, при t = %.2f\n", y, t);	}
}
void pr1V5()
{
	float a = 0.5, b = 7, v, w, c = 0, max;
	for (int i = 0; i < 5; i++)
	{
		cout << "Введите V" << i + 1 << ": ";
		cin >> v;
		if (v > 0)
		{
			w = a + v;
		}
		else
		{
			w = b / v;
		};
		if (c < w)
		{
			c = w;
		};
		cout << "w = " << w << ", при i = " << i << endl;
	}
	cout << "c = " << c << endl;
}
void pr2V5()
{
	float y, p = 0, q, com = 1, i = 0, n;
	cout << "Введите значение n: ";
	cin >> n;
	cout << "Введите минимально возможное значение Y: ";
	cin >> p;

	for (int i = 0; i < n; i++)
	{
		cout << "Введите Y" << i + 1 << "{3; -2; 0.9; 0.5; 1}: ";
		cin >> y;
		com *= (y - 5);
	}
	q = com + p;
	cout << "q =" << q << "\n";
	cout << "p = " << p << endl;
}
int main()
{
	setlocale(LC_ALL, "Russian");
	pr1();
	pr2();
	pr1V5();
	pr2V5();
}