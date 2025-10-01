#include <iostream>
#include <vector>
#include <algorithm>

// Функция для печати вектора
void printVector(const std::vector<int>& vec) {
    for (int num : vec) {
        std::cout << num << " ";
    }
    std::cout << std::endl;
}

// Рекурсивная функция для генерации всех перестановок
void generatePermutations(std::vector<int>& nums, int start, int end) {
    if (start == end) {
        printVector(nums);
        return;
    }

    for (int i = start; i <= end; ++i) {
        std::swap(nums[start], nums[i]);
        generatePermutations(nums, start + 1, end);
        std::swap(nums[start], nums[i]);
    }
}

int main() {
    const int n = 5; // Количество натуральных чисел
    std::vector<int> numbers = { 1, 2, 3, 4, 5 }; // Натуральные числа

    std::cout << "Все перестановки этих чисел:" << std::endl;
    generatePermutations(numbers, 0, n - 1);

    return 0;
}
