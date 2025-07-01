#include <iostream>
using namespace std;
void main()
{
	cout << vr13() << endl;
	cout << vr16() << endl;
}
int vr13()
{
	float	d = 5 * pow(10, -9);
	float	c = 9;
	float	a = 1.5;
	float	t, y;
	t = d * c + a + sqrt(c - 1);
	y = 0.5 * t / d + exp(a);
	cout << "t=" << t << "\n";
	cout << "y=" << y;
}
int vr16()
{
		float	m = 6;
		float	z = 0.05 * pow(10, -5);
		float	w, y;
		y = cos(5 * m) / sin(0.4 * m) * sin(0.4 * m);
		w = 4 * z * y - 7 * exp(-2 * y);
		cout << "y=" << y << endl;
		cout << "w=" << w << endl;
}