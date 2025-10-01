#include <stdio.h>;
#include <cmath>;
void pr1()
{
	float t, x, b = 3, i = 8, c = 6 * pow(10, -4), a = 6;
	while (i <= 24)
	{
		t = (a * i) / (pow(a, 2) - b) * exp(-a);
		if (t > 5 * c)
			x = 4.8 * pow(10, -3) + i * a;
		else if (t <= 5 * c)
			x = a + pow(i, 2) * t;
		printf("t=%5.2f\t", t);
		printf("x=%5.2f\n", x);
		i += 4;
	 }
 }
void pr2()
{
	int n = 1, i = 1;
	while (i <= 6)
	{
		if (n % 3 == 0 && n % 5 == 0)
		{
			printf("%d\n", n);
			i++;
		}
		n++;
	}
}
void main()
{
	pr1();
	pr2();
}