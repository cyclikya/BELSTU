#include <iomanip>
#include <iostream>
using namespace std;

int pr1()
{
	char in, backGr;
	cin >> in;
	cin >> backGr;
	cout << setw(11) << setfill(backGr) << backGr << endl;
	cout << setw(1) << setfill(backGr) << backGr << setw(3) << setfill(in) << in << setw(3) << setfill(backGr) << backGr << setw(3) << setfill(in) << in << setw(1) << setfill(backGr) << backGr << setw(1) << endl;
	cout << setw(5) << setfill(in) << in << setw(1) << setfill(backGr) << backGr << setw(5) << setfill(in) << in << endl;
	cout << setw(11) << setfill(in) << in << endl;
	cout << setw(11) << setfill(in) << in << endl;
	cout << setw(1) << setfill(backGr) << backGr << setw(9) << setfill(in) << in << setw(1) << setfill(backGr) << backGr << endl;
	cout << setw(2) << setfill(backGr) << backGr << setw(7) << setfill(in) << in << setw(2) << setfill(backGr) << backGr << endl;
	cout << setw(3) << setfill(backGr) << backGr << setw(5) << setfill(in) << in << setw(3) << setfill(backGr) << backGr << endl;
	cout << setw(4) << setfill(backGr) << backGr << setw(3) << setfill(in) << in << setw(4) << setfill(backGr) << backGr << endl;
	cout << setw(5) << setfill(backGr) << backGr << setw(1) << setfill(in) << in << setw(5) << setfill(backGr) << backGr << endl;
	cout << setw(11) << setfill(backGr) << backGr << endl;
	return 0;
}
int pr2()
{
	int N;
	double M;

	cout << "введите кол-во косилок: ";
	cin >> N;

	cout << "введите время работы первой косилки: ";
	cin >> M;

	double totalTime = M;

	for (int i = 2; i <= N; ++i) {
		M += 10.0 / 60;
		totalTime += M;
	}

	cout << "бригада работала" << totalTime << " часов(а)." << endl;
	return 0;
}
void main()
{
	setlocale(LC_ALL, "Russian");
	cout << pr1();
	cout << pr2();
}