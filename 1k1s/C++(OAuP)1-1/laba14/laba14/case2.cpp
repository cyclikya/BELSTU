#include "case2.h"
using namespace std;

int case2() {
    const int N = 50;
    int n, m, rmn = 0, rmx = 49;
    std::cout << "Введите количество строк (n) матрицы: ";
    std::cin >> n;
    std::cout << "Введите количество столбцов (m) матрицы: ";
    std::cin >> m;

    int C[N][N];

    std::cout << "Матрица:\n";

    srand(static_cast<unsigned>(time(nullptr)));
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < m; j++) {
            C[i][j] = static_cast<int>((static_cast<double>(rand()) / RAND_MAX) * (rmx - rmn) + rmn);
            std::cout << C[i][j] << " ";
        }
        std::cout << '\n';
    }

    int count = 0;
    for (int j = 0; j < m; j++) {       // i - столбец j - строка
        for (int i = 0; i < n; i++) {
            int el = C[i][j];
            int sum = 0;

            for (int str = 0; str < n; str++) {
                if (str != i) {
                    sum += C[str][j];
                }
            }

            if (el > sum) {
                count++;
            }
        }
    }

    cout << "\nКоличество элементов,, значение каждого из которых больше суммы остальных элементов своего столбца. : " << count << endl;

    return 0;
}