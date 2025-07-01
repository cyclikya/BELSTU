#include <iostream>
#include <cmath>

using namespace std;

// Тип указателя на функцию
typedef float (*EquationFunction)(float);

// Функция 1: 5x - 1 + x^3
float equation1(float x) {
    return 5 * x - 1 + pow(x, 3);
}

// Функция 2: x^2 + 1/x
float equation2(float x) {
    return pow(x, 2) + 1 / x;
}

// Метод дихотомии
float bisectionMethod(EquationFunction equation, float a, float b, float e) {
    if (equation(a) * equation(b) > 0) {
        cerr << "Ошибка: Начальные значения не обеспечивают смену знаков функции." << endl;
        return NAN; // Возвращаем NaN в случае ошибки
    }

    float x;
    while ((b - a) >= e) {
        x = (a + b) / 2;

        if (equation(x) <= 0 + e && equation(x) >= 0 - e) {
            break; // Найден точный корень
        }
        else if (equation(x) * equation(a) < 0) {
            b = x;
        }
        else {
            a = x;
        }
    }
    return x;
}

int main() {
    setlocale(LC_ALL, "rus");
    float a, b, e = 0.001;

    // Ввод исходных данных
    cout << "Введите начальное значение a: ";
    cin >> a;
    cout << "Введите начальное значение b: ";
    cin >> b;

    EquationFunction equation1Ptr = equation1;
    EquationFunction equation2Ptr = equation2;

    // Использование метода дихотомии для уравнения 1
    float root1 = bisectionMethod(equation1Ptr, a, b, e);
    if (!isnan(root1)) {
        cout << "Корень уравнения 1: " << root1 << endl;
    }

    // Использование метода дихотомии для уравнения 2
    float root2 = bisectionMethod(equation2Ptr, a, b, e);
    if (!isnan(root2)) {
        cout << "Корень уравнения 2: " << root2 << endl;
    }

    return 0;
}
