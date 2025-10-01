#include <iostream>
#include <ctime>
#include <iomanip>
#include "Levenshtein.h"

int main() {
    setlocale(LC_ALL, "rus");

    char x[] = "лот", y[] = "полет";
    int lx = strlen(x), ly = strlen(y);

    std::cout << std::endl << "-- расстояние Левенштейна -----" << std::endl;
    std::cout << std::endl << "--длина --- рекурсия (итеративно) -- дин.програм. ---" << std::endl;

    clock_t t1, t2, t3, t4;

    for (int i = 1; i <= std::min(lx, ly); i++) {
        t1 = clock();
        int rec = levenshtein_iterative(i, x, i, y);
        t2 = clock();

        t3 = clock();
        int dp = levenshtein(i, x, i, y);
        t4 = clock();

        std::cout << std::right << std::setw(2) << i << "        "
            << std::left << std::setw(10) << (t2 - t1) << "   "
            << std::setw(10) << (t4 - t3) << std::endl;
    }

    system("pause");
    return 0;
}
