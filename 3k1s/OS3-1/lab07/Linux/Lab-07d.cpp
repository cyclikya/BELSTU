#include <iostream>
#include <unistd.h>
#include <sys/wait.h>
#include <cmath>
#include <ctime>
#include <string>

// Проверка простых чисел
bool isPrime(unsigned int n)
{
    if (n < 2) return false;
    if (n % 2 == 0) return n == 2;

    unsigned int r = (unsigned int)std::sqrt((double)n);
    for (unsigned int i = 3; i <= r; i += 2)
        if (n % i == 0)
            return false;

    return true;
}

// Логика дочернего процесса
void run_child(int seconds)
{
    struct timespec ts;
    clock_gettime(CLOCK_MONOTONIC, &ts);
    double start = ts.tv_sec + ts.tv_nsec / 1e9;
    double end = start + seconds;

    unsigned long long counter = 0;
    unsigned int n = 2;

    while (true)
    {
        if (isPrime(n))
        {
            counter++;
            std::cout << counter << ": " << n << std::endl;
        }
        n++;

        clock_gettime(CLOCK_MONOTONIC, &ts);
        if ((ts.tv_sec + ts.tv_nsec / 1e9) >= end)
            break;
    }

    std::cout << "Time elapsed: "
        << (ts.tv_sec + ts.tv_nsec / 1e9) - start
        << " seconds" << std::endl;
}

// Выбор терминала
bool exists(const char* cmd)
{
    return system((std::string("which ") + cmd + " >/dev/null 2>&1").c_str()) == 0;
}

std::string build_terminal_cmd(const std::string& term, int secs)
{
    std::string childCmd = "./Lab-07d child " + std::to_string(secs);

    if (term == "gnome-terminal")
        return "gnome-terminal -- bash -c '" + childCmd + "; exec bash'";

    if (term == "xterm")
        return "xterm -hold -e \"" + childCmd + "\"";

    if (term == "konsole")
        return "konsole -e " + childCmd;

    if (term == "xfce4-terminal")
        return "xfce4-terminal -e \"" + childCmd + "\"";

    // fallback
    return "xterm -hold -e \"" + childCmd + "\"";
}

// ----------------------------
// Основная программа
// ----------------------------
int main(int argc, char* argv[])
{
    // Режим дочернего процесса
    if (argc == 3 && std::string(argv[1]) == "child")
    {
        run_child(atoi(argv[2]));
        return 0;
    }

    // Родительский процесс
    std::string term;

    if (exists("gnome-terminal")) term = "gnome-terminal";
    else if (exists("xterm"))     term = "xterm";
    else if (exists("konsole"))   term = "konsole";
    else if (exists("xfce4-terminal")) term = "xfce4-terminal";
    else term = "xterm";

    std::cout << "Using terminal: " << term << std::endl;

    // Запуск детей
    std::cout << "Starting child 1 (60 seconds)..." << std::endl;
    system(build_terminal_cmd(term, 60).c_str());

    std::cout << "Starting child 2 (120 seconds)..." << std::endl;
    system(build_terminal_cmd(term, 120).c_str());

    std::cout << "Waiting for children to finish..." << std::endl;

    // Ждать нечего — окна сами закончатся
    sleep(1);

    std::cout << "Both completed." << std::endl;
    return 0;
}
