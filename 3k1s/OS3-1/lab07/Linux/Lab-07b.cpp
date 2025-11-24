#include <iostream>
#include <ctime>

// Получение CPU-времени процесса
static double getCpuTimeSec()
{
    struct timespec ts;
    clock_gettime(CLOCK_PROCESS_CPUTIME_ID, &ts);
    return ts.tv_sec + ts.tv_nsec / 1e9;
}

// Получение реального времени
static double getRealTimeSec()
{
    struct timespec ts;
    clock_gettime(CLOCK_MONOTONIC, &ts);
    return ts.tv_sec + ts.tv_nsec / 1e9;
}

int main()
{
    volatile unsigned long long counter = 0;

    double cpuStart = getCpuTimeSec();
    double realStart = getRealTimeSec();

    double cpuT5 = cpuStart + 5.0;
    double cpuT10 = cpuStart + 10.0;
    double cpuT15 = cpuStart + 15.0;

    bool printed5 = false;
    bool printed10 = false;

    while (true)
    {
        counter++;

        double cpuNow = getCpuTimeSec();

        if (!printed5 && cpuNow >= cpuT5)
        {
            std::cout << "Iterations after 5 seconds (CPU time): "
                << counter << std::endl;
            printed5 = true;
        }

        if (!printed10 && cpuNow >= cpuT10)
        {
            std::cout << "Iterations after 10 seconds (CPU time): "
                << counter << std::endl;
            printed10 = true;
        }

        if (cpuNow >= cpuT15)
        {
            double realEnd = getRealTimeSec();
            std::cout << "Final iterations after 15 seconds (CPU time): "
                << counter << std::endl;

            std::cout << "Real elapsed time: "
                << (realEnd - realStart)
                << " seconds" << std::endl;

            break;
        }
    }

    return 0;
}
