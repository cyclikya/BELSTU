#include <iostream>
#include <fstream>
#include <vector>

// Рекурсивная функция для генерации всех чисел из цифр, не превышающих A, с длиной, равной A
void generateNumbers(std::ofstream& outFile, std::vector<int>& number, int A, int length) {
    if (length == 0) {
        // Печатаем число в файл
        for (int digit : number) {
            outFile << digit;
        }
        outFile << std::endl;
        return;
    }

    for (int i = 1; i <= A; ++i) {
        number.push_back(i); // Добавляем цифру к числу
        generateNumbers(outFile, number, A, length - 1);
        number.pop_back(); // Удаляем последнюю добавленную цифру
    }
}

int main() {
    int A;
    std::cout << "Введите цифру A: ";
    std::cin >> A;

    std::ofstream outFile("numbers.txt"); // Открываем файл для записи

    std::vector<int> number; // Вектор для хранения текущего числа

    // Генерируем все числа и записываем их в файл
    generateNumbers(outFile, number, A, A);

    outFile.close(); // Закрываем файл

    std::cout << "Числа успешно записаны в файл numbers.txt" << std::endl;

    return 0;
}
