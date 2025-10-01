#include <iostream>
#include <ctime>
using namespace std;

void ex1()
{
	const int N = 100;
	int size, cup, A[N], rmn = 0, rmx = 99, sum = 0;
	cout << "Введите размер массива:";
	cin >> size;
	cout << "Массив А:";

	srand((unsigned)time(NULL));
	for (int i = 0; i < size; i++)
	{
		A[i] = (int)(((double)rand() / (double)RAND_MAX) * (rmx - rmn) + rmn);
		cout << A[i] << " ";
	}

	sum = A[0];
	cout << endl << "Новый массив А:";

	for (int i = 1; i < size; i++) {
		cup = A[i];
		A[i] = sum;
		sum += cup;
		cout << A[i] << " ";
	}
}

void ex2()
{
	char lat[] = { 'a', 'b', 'c', 'd', 'e','f', 'g', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
	const int N = 25;
	int size;
	char A[N];

	cout << "Введите размер массива:";
	cin >> size;

	cout << "Массив А:";
	srand(static_cast<unsigned>(time(NULL)));

	for (int i = 0; i < size; i++) {
		int randomIndex = rand() % 25; 
		A[i] = lat[randomIndex];
		cout << A[i] << " ";
	}
	cout << "\nЭлементы которые повторяются 1 раз: ";
	for (int i = 0; i < size; i++) {
        int duplic = 0;
        for (int k = 0; k < size; k++) {
            if (i != k && A[i] == A[k]) {
                duplic = 1;
            }
        }
        if (duplic == 0) {
            std::cout << A[i] << " ";
        }
    }
}

int main()
{
	setlocale(LC_ALL, "Russian");
	//ex1();
	ex2();
}