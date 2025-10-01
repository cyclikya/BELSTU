#include <iostream>
using namespace std;

float f(float x)
{
    return 2 + pow(x, 3);
}
void pr1tr()
{
    float a = 8, b = 14, h, x, s = 0;
    int n = 200;
    h = (b - a) / n;
    x = a;
    do {
        s += h * (f(x) + f(x + h)) / 2;
        x = x + h;
    } while (x <= b - h);
    cout << "Площадь криволинейной трапеции( вычисленная при помощи метода трапеции): " << s << endl;
}
void pr1par()
{
    float a = 8, b = 14, h, x, s = 0, s1 = 0, s2 = 0, S;
    int n = 200, i = 1;
    h = (b - a) / (2 * n);
    x = a + 2 * h;
    do {
        s2 += f(x);
        x = x + h;
        s1 += f(x);
        x += h;
        i += 1;
    } while (i < n);
    S = (h / 3) * (f(a) + 4 * f(a + h) + 4 * s1 + 2 * s2 + f(b));
    cout << "Площадь криволинейной трапеции( вычисленная при помощи метода параболы) : " << s << endl;
}

float g(float x)
{
    return exp(x) + x - 4;
}
void pr2dich()
{
    float a = 0, b = 3, e = 1e-4, x;
    do
    {
        x = (a + b) / 2;
        if ((g(x) * g(a)) <= 0)
        {
            b = x;
        }
        else {
            a = x;
        };
    } while ((fabs(a - b)) > (2 * e)); // fabs() это модуль
    cout << "Значение корня x: " << x << endl;
}

void main()
{
    setlocale(LC_ALL, "Russian");
    //pr1tr();
    pr1par();
    //pr2dich();
}