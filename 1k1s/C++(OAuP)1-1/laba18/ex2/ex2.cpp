#include <iostream>

using namespace std;

// Функция для вычисления суммы элементов ниже главной диагонали в квадратной матрице
int sumBelowDiagonal(int(*matrix)[3], int size) {
    int sum = 0;
    for (int i = 1; i < size; ++i) {
        for (int j = 0; j < i; ++j) {
            sum += matrix[i][j];
        }
    }
    return sum;
}

// Функция для вывода матрицы на экран
void printMatrix(int(*matrix)[3], int size) {
    for (int i = 0; i < size; ++i) {
        for (int j = 0; j < size; ++j) {
            cout << matrix[i][j] << " ";
        }
        cout << endl;
    }
}

int main() {
    setlocale(LC_ALL, "rus");

    // Размер квадратной матрицы
    const int size = 3;

    // Инициализация матрицы 1
    int matrix1[size][size] = {
        {1, 2, 3},
        {4, 5, 6},
        {7, 8, 9}
    };

    // Инициализация матрицы 2
    int matrix2[size][size] = {
        {9, 8, 7},
        {6, 5, 4},
        {3, 2, 1}
    };

    // Вывод матриц на экран
    cout << "Матрица 1:" << endl;
    printMatrix(matrix1, size);

    cout << "Матрица 2:" << endl;
    printMatrix(matrix2, size);

    int (*sumFunction)(int(*)[3], int) = sumBelowDiagonal;

    // Вычисление суммы элементов ниже главной диагонали для матрицы 1
    int sum1 = sumFunction(matrix1, size);
    cout << "Сумма элементов ниже главной диагонали в матрице 1: " << sum1 << endl;

    // Вычисление суммы элементов ниже главной диагонали для матрицы 2
    int sum2 = sumFunction(matrix2, size);
    cout << "Сумма элементов ниже главной диагонали в матрице 2: " << sum2 << endl;

    return 0;
}
