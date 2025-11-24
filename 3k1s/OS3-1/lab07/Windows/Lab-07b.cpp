#include <Windows.h>
#include <iostream>

int main()
{
    LARGE_INTEGER freq;
    LARGE_INTEGER start, now;

    QueryPerformanceFrequency(&freq);  // тиков в секунду
    QueryPerformanceCounter(&start);   // стартовое время

    unsigned long long counter = 0;

    bool printed5 = false;
    bool printed10 = false;

    while (true)
    {
        counter++;  // инкремент итераций

        QueryPerformanceCounter(&now);

        // прошедшее время в секундах (double для удобства)
        double elapsed = (double)(now.QuadPart - start.QuadPart) / freq.QuadPart;

        if (!printed5 && elapsed >= 5.0)
        {
            std::cout << "Iterations after 5 seconds: " << counter << std::endl;
            printed5 = true;
        }

        if (!printed10 && elapsed >= 10.0)
        {
            std::cout << "Iterations after 10 seconds: " << counter << std::endl;
            printed10 = true;
        }

        if (elapsed >= 15.0)
        {
            std::cout << "Final iterations after 15 seconds: " << counter << std::endl;
            break;
        }
    }

    return 0;
}
