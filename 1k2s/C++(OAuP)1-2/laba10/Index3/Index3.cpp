#include <iostream>
#include <vector>
#include <algorithm>

// ������� ��� ������ �������
void printVector(const std::vector<int>& vec) {
    for (int num : vec) {
        std::cout << num << " ";
    }
    std::cout << std::endl;
}

// ����������� ������� ��� ��������� ���� ������������
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
    const int n = 5; // ���������� ����������� �����
    std::vector<int> numbers = { 1, 2, 3, 4, 5 }; // ����������� �����

    std::cout << "��� ������������ ���� �����:" << std::endl;
    generatePermutations(numbers, 0, n - 1);

    return 0;
}
