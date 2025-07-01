#include <iostream>
void main()
{
	float	m = 6;
	float	z = 0.05 * pow(10, -5);
	float	w, y;
	y = cos(5 * m) / sin (0.4 * m) * sin(0.4 * m);
	w = 4 * z * y - 7 * exp( - 2 * y);
	std::cout << "y=" << y << std::endl;
	std::cout << "w=" << w << std::endl;
}