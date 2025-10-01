#include <iostream>
#include <iomanip>
using namespace std;

void ex1() {
    const int N = 100;
    int n;
    double X[N];
    int rmn = -99, rmx = 99;

    cout << "Введите размер(n) массива: ";
    cin >> n;

    cout << "Массив X: ";

    srand(static_cast<unsigned>(time(nullptr)));

    for (int i = 0; i < n; i++) {
        X[i] = static_cast<double>(rand()) / RAND_MAX * (rmx - rmn) + rmn;
        cout << fixed << setprecision(2) << X[i] << " ";
    }

    double sum = 0;
    for (int i = 1; i < n; i += 2) {
        sum += X[i];
    }

    cout << "\nСумма элементов с нечетными номерами: " << sum << endl;

    double sum_between = 0;
    int first_negative = -1, last_negative = -1;

    for (int i = 0; i < n; ++i) { // поиск первого минимального
        if (X[i] < 0) {
            if (first_negative == -1) {
                first_negative = i;
            }
            else {
                last_negative = i;
            }
        }
    }

    if (first_negative != -1 && last_negative != -1) {
        for (int i = first_negative + 1; i < last_negative; ++i) {
            sum_between += X[i];
        }
    }

    cout << "Сумма элементов между первым и последним отрицательными: " << sum_between << endl;
}

void ex2() {
    const int N = 100;

    int n;
    cout << "Введите размер(n) квадратной матрицы: ";
    cin >> n;

    int C[N][N];

    cout << "Введите элементы матрицы:\n";

    // Ручной ввод матрицы
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            cin >> C[i][j];
        }
    }

    // Вывод исходной матрицы
    cout << "\nИсходная матрица:\n";
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            cout << C[i][j] << " ";
        }
        cout << '\n';
    }
       
    // Поиск и обработка первой строки с нулевыми элементами
    int firstZeroRow = -1;
    for (int i = 0; i < n; i++) {
        bool allZeros = true;
        for (int j = 0; j < n; j++) {
            if (C[i][j] != 0) {
                allZeros = false;
            }
        }
        if (allZeros) {
            firstZeroRow = i;
        }
    }

    // Если найдена строка с нулевыми элементами, уменьшаем вдвое столбец с таким же номером
    if (firstZeroRow != -1) {
        for (int i = 0; i < n; i++) {
            C[i][firstZeroRow] /= 2;
        }

        // Вывод измененной матрицы
        cout << "\nИзмененная матрица:\n";
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                cout << C[i][j] << " ";
            }
            cout << '\n';
        }
        cout << "\nСтрока с нулевыми элементами:" << firstZeroRow + 1;
    }
    else {
        cout << "\nСтрока с нулевыми элементами не найдена.\n";
    }

}

int main()
{
	setlocale(LC_ALL, "rus");
	ex1();
	//ex2();
}