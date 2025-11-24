#include <iostream>
#include <iomanip>
#include <ctime>
#include <unistd.h>
#include <sys/time.h>
#include <time.h>

int main()
{
    // Получаем текущее время в секундах через POSIX CLOCK_REALTIME
    struct timespec ts;
    clock_gettime(CLOCK_REALTIME, &ts);

    // Конвертируем в локальное время вручную
    struct tm localTm;
    localtime_r(&ts.tv_sec, &localTm);

    // Конвертируем в UTC
    struct tm utcTm;
    gmtime_r(&ts.tv_sec, &utcTm);

    // Вычисляем смещение часового пояса в часах
    time_t localSec = mktime(&localTm);
    time_t utcSec = mktime(&utcTm);
    long offsetSec = difftime(localSec, utcSec);

    int offsetHours = offsetSec / 3600;

    // Вывод в формате YYYY-MM-DDThh:mm:ss±hh
    std::cout << std::setfill('0')
        << (localTm.tm_year + 1900) << "-"
        << std::setw(2) << (localTm.tm_mon + 1) << "-"
        << std::setw(2) << localTm.tm_mday << "T"
        << std::setw(2) << localTm.tm_hour << ":"
        << std::setw(2) << localTm.tm_min << ":"
        << std::setw(2) << localTm.tm_sec;

    if (offsetHours >= 0)
        std::cout << "+" << std::setw(2) << offsetHours;
    else
        std::cout << "-" << std::setw(2) << (-offsetHours);

    std::cout << std::endl;
    return 0;
}
