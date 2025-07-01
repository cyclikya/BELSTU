#include <iostream>

// Рекурсивная функция для подсчета количества делителей числа n, меньших или равных x
int dnx(int n, int x) {
    if (x == 1) {
        return 1;
    }
    else if (n % x == 0) {
        return dnx(n, x - 1) + 1;
    }
    else {
        return dnx(n, x - 1);
    }
}

// Функция для подсчета количества всех положительных делителей числа n
int dn(int n) {
    return dnx(n, n);
}

int main() {
    int number;
    std::cout << "Введите натуральное число: ";
    std::cin >> number;
    std::cout << "Количество всех положительных делителей числа " << number << " равно " << dn(number) << std::endl;
    return 0;
}
