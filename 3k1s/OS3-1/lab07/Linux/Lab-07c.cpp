#include <iostream>
#include <ctime>

static double getTimeSec()
{
    struct timespec ts;
    clock_gettime(CLOCK_MONOTONIC, &ts);
    return ts.tv_sec + ts.tv_nsec / 1e9;
}

int main()
{
    volatile unsigned long long counter = 0;

    double start = getTimeSec();
    double nextCheckpoint = 3.0;

    // Вывод на 3, 6, 9, 12 сек
    while (nextCheckpoint <= 12.0)
    {
        double intervalStart = getTimeSec();

        // крутим 3 секунды
        while (getTimeSec() - intervalStart < 3.0)
        {
            counter++;
        }

        std::cout << "Iterations after "
            << (int)nextCheckpoint
            << " seconds: "
            << counter << std::endl;

        nextCheckpoint += 3.0;
    }

    // последние 3 секунды до 15
    double finalInterval = getTimeSec();
    while (getTimeSec() - finalInterval < 3.0)
        counter++;

    std::cout << "Final iterations after 15 seconds: "
        << counter << std::endl;

    return 0;
}
