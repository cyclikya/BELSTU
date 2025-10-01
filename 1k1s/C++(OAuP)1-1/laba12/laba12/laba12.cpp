#include <iostream>
#include <cstdlib>
#include <ctime>
using namespace std;

void ex1() {
	const int N = 100;
	int n, X[N], rmn = 0, rmx = 99;
	cout << "Введите размер(n) массива:";
	cin >> n;
	cout << "Массив X:";

	srand((unsigned)time(NULL));
	for (int i = 0; i < n; i++)
	{
		*(X + i) = (int)(((double)rand() / (double)RAND_MAX) * (rmx - rmn) + rmn);
		cout << *(X + i) << " ";
	}

	int difr = n;
	for (int i = 0; i < n; i++) {
		for (int j = 0; j < n; j++) {
			if (*(X + i) == *(X + j) && i != j) {
				difr--;
			}
		}
	}
	cout << "\nКоличество различных элементов: " << difr << endl;
}

void ex2() {
	const int N = 100;
	int n, X[N], rmn = 0, rmx = 99;
	cout << "Введите размер(n) массива:";
	cin >> n;
	cout << "Массив X:";
	srand((unsigned)time(NULL));
	for (int i = 0; i < n; i++)
	{
		*(X + i) = (int)(((double)rand() / (double)RAND_MAX) * (rmx - rmn) + rmn);
		cout << *(X + i) << " ";
	}

	int sum = 0;
	for (int i = 0; i < n; i++) {
		sum += *(X + i);
	}
	int sr = sum / n;
	cout << "\n\nСреднее значение: " << sr;

	cout << "\n\nЭлементы массива меньше среднего: ";
	for (int i = 0; i < n; i++) {
		if (*(X + i) < sr) {
			cout << *(X + i) << " ";
		}
	}

	cout << "\nЭлементы массива больше среднего: ";
	for (int i = 0; i < n; i++) {
		if (*(X + i) > sr) {
			cout << *(X + i) << " ";
		}
	}

}

int main(){
    setlocale(LC_ALL, "Russian");
    ex1();
    //ex2();
}
