#include <Windows.h>
#include <iostream>

int main()
{
    SYSTEMTIME utcSt, localSt;
    GetSystemTime(&utcSt);      // UTC
    GetLocalTime(&localSt);     // Local time

    // Convert SYSTEMTIME → FILETIME
    FILETIME utcFt, localFt;
    SystemTimeToFileTime(&utcSt, &utcFt);
    SystemTimeToFileTime(&localSt, &localFt);

    // Convert FILETIME to 64-bit integers
    ULARGE_INTEGER utc64, local64;
    utc64.LowPart = utcFt.dwLowDateTime;
    utc64.HighPart = utcFt.dwHighDateTime;

    local64.LowPart = localFt.dwLowDateTime;
    local64.HighPart = localFt.dwHighDateTime;

    // Difference (100-ns intervals)
    LONGLONG diff = (LONGLONG)local64.QuadPart - (LONGLONG)utc64.QuadPart;

    // 1 сек = 10 000 000 интервалов.
    LONGLONG hours = diff / (10'000'000LL * 3600LL);

    // Формат вывода YYYY-MM-DDThh:mm:ss±hh
    char sign = (hours >= 0) ? '+' : '-';
    if (hours < 0) hours = -hours;

    std::cout
        << localSt.wYear << "-"
        << (localSt.wMonth < 10 ? "0" : "") << localSt.wMonth << "-"
        << (localSt.wDay < 10 ? "0" : "") << localSt.wDay << "T"
        << (localSt.wHour < 10 ? "0" : "") << localSt.wHour << ":"
        << (localSt.wMinute < 10 ? "0" : "") << localSt.wMinute << ":"
        << (localSt.wSecond < 10 ? "0" : "") << localSt.wSecond
        << sign
        << (hours < 10 ? "0" : "") << hours
        << std::endl;

    return 0;
}
