#include "case1.h"
using namespace std;

int case1() {
    const int N = 100;
    int n, rmn = 0, rmx = 99;
    cout << "������� ������(n) ���������� �������:";
    cin >> n;

    int C[N][N];

    cout << "�������:\n";

    srand(static_cast<unsigned>(time(NULL)));
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            C[i][j] = static_cast<int>((static_cast<double>(rand()) / RAND_MAX) * (rmx - rmn) + rmn);
            cout << C[i][j] << " ";
        }
        cout << '\n';
    }

    int el = C[0][0];
    int st;
    for (int i = 1; i < n; i++) {
        if (C[i][i] < el) {
            el = C[i][i];
            st = i;
        }
    }

    cout << "\n���������� ������� �� ������� ���������: " << el << endl;
    cout << "�������, � ������� ��������� ����������� �������: \n";
    for (int i = 0; i < n; i++) {
        cout << C[i][st] << '\n';
    }
    return 0;
}
