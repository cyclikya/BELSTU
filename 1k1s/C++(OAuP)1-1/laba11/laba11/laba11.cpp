#include <iostream>
using namespace std;

int ex1()
{
    int A;
    std::cout << "Введите целое число A: ";
    std::cin >> A;

    // Извлекаем 2 бита числа A, начиная с пятого бита справа
    int cup = (A >> 4) & 0b11;

    // Вводим целое число B
    int B;
    std::cout << "Введите целое число B: ";
    std::cin >> B;

    // Очищаем биты в числе B, начиная с пятого бита справа
    B &= ~(0b11 << 4);

    // Вставляем извлеченные биты в число B
    B |= (cup << 4);

    // Выводим результат
    std::cout << "Результат: " << B << std::endl;

    return 0;
}


unsigned int invertBits(unsigned int A, int p, int n) {
    // Создаем маску для инвертирования битов
    unsigned int mask = ((1 << n) - 1) << (p - n + 1);

    // Инвертируем биты внутри маски и применяем ее к числу A
    A ^= mask;

    return A;
}
int ex2()
{
    // Пример использования функции
    unsigned int A = 178; // Пример числа
    int p = 5;           // Позиция, с которой начинается инвертирование
    int n = 3;           // Количество бит для инвертирования

    // Выводим исходное число
    std::cout << "Исходное число A: " << A << std::endl;

    // Инвертируем биты и выводим результат
    A = invertBits(A, p, n);
    std::cout << "Число A после инвертирования: " << A << std::endl;

    return 0;
}
int main()
{
    setlocale(LC_ALL, "Russian");
    //ex1();
    ex2();
}
