#include <Windows.h>
#include <iostream>

double getTime()
{
    LARGE_INTEGER freq, t;
    QueryPerformanceFrequency(&freq);
    QueryPerformanceCounter(&t);
    return (double)t.QuadPart / freq.QuadPart;
}

int main()
{
    volatile unsigned long long counter = 0;

    double start = getTime();
    double checkpoint = 3.0;

    // Печатаем каждые 3 секунды: 3,6,9,12
    while (checkpoint <= 12.0)
    {
        double intervalStart = getTime();

        // крутим 3 секунды
        while (getTime() - intervalStart < 3.0)
            counter++;

        std::cout << "Iterations after " << (int)checkpoint
            << " seconds: " << counter << std::endl;

        checkpoint += 3.0;
    }

    // оставшиеся 3 секунды до 15
    double lastStart = getTime();
    while (getTime() - lastStart < 3.0)
        counter++;

    std::cout << "Final iterations after 15 seconds: "
        << counter << std::endl;

    return 0;
}
