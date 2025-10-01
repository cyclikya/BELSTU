#include <iostream>
using namespace std;

void ex1() {
    const int N = 20, n_max = 10;
    double A[N] = { 1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16, 17, -18, 19, -20 };

    cout << "Массив A: ";
    for (int i = 0; i < N; ++i) {
        cout << *(A + i) << " ";
    }
    cout << endl;

    double B[n_max];

    for (int i = 0, k = 0; i < N; i += 2) {
        if (*(A + i) > 0) {
            *(B + k) = *(A + i);
            k++;
        }
    }

    cout << "Массив B: ";
    for (int i = 0; i < n_max; ++i) {
        cout << *(B + i) << " "; 
    }
    cout << endl;

    double sum = 0;
    for (int i = 0; i < n_max; ++i) {
        sum += pow(*(B + i), 2);
    }

    cout << "Сумма квадратов элементов массива B: " << sum << endl;
}

void ex2() {
    const int N = 100;
    int n, rmn = -99, rmx = 99;
    cout << "Введите размер (n) квадратной матрицы: ";
    cin >> n;

    int C[N][N];

    cout << "Матрица:\n";

    srand(static_cast<unsigned>(time(NULL)));
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            C[i][j] = static_cast<int>((static_cast<double>(rand()) / RAND_MAX) * (rmx - rmn) + rmn);
            cout << C[i][j] << " ";
        }
        cout << endl;
    }

    int min = C[0][0];
    int minI = 0, minJ = 0;

    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < n; ++j) {
            if (C[i][j] < min) {
                min = C[i][j];
                minI = i;
                minJ = j;
            }
        }
    }

    if (minI == minJ) {
        cout << "Минимальное значение находится на главной диагонали и равно: " << min << endl;
    }
    else {
        cout << "Минимальное значение находится не на главной диагонали." << endl;
    }
    
}

void main()
{
	setlocale(LC_ALL, "rus");
	//ex1();
	//ex2();
}