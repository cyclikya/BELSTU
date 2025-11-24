#include <Windows.h>
#include <iostream>
#include <vector>
#include <cmath>
#include <cstdlib>

bool isPrime(unsigned int n)
{
    if (n < 2) return false;
    if (n % 2 == 0) return (n == 2);

    unsigned int limit = (unsigned int)std::sqrt((double)n);
    for (unsigned int i = 3; i <= limit; i += 2)
    {
        if (n % i == 0)
            return false;
    }
    return true;
}

int main(int argc, char* argv[])
{
    int runSeconds = 0;
    if (argc > 1)
    {
        runSeconds = atoi(argv[1]);
        if (runSeconds < 0) runSeconds = 0;
    }

    LARGE_INTEGER freq, start, now;
    QueryPerformanceFrequency(&freq);
    QueryPerformanceCounter(&start);

    std::vector<unsigned int> primes;
    primes.reserve(10000000);

    unsigned int n = 2;

    while (true)
    {
        if (isPrime(n))
            primes.push_back(n);

        // Вывод СРАЗУ при нахождении
        if (isPrime(n))
            std::cout << primes.size() << ": " << n << std::endl;

        QueryPerformanceCounter(&now);
        double elapsed =
            (double)(now.QuadPart - start.QuadPart) / freq.QuadPart;

        // РЕЖИМ ПО ВРЕМЕНИ
        if (runSeconds > 0 && elapsed >= runSeconds)
            break;

        // РЕЖИМ ПО КОЛИЧЕСТВУ
        if (runSeconds == 0 && primes.size() >= 1000)
            break;

        n++;
    }

    double totalTime =
        (double)(now.QuadPart - start.QuadPart) / freq.QuadPart;

    std::cout << "Time elapsed: " << totalTime << " seconds" << std::endl;
    return 0;
}
