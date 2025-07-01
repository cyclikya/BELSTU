#include <iostream>
#include <forward_list>

using namespace std;

int main()
{
    setlocale(LC_ALL, "rus");
    forward_list<int> newlist = { 6, 2, 8, 4, 5 };

    newlist.clear();

    int n;
    cout << "Введите количество элементов для добавления: ";
    cin >> n;
    cout << "Введите новые элементы: ";
    for (int i = 0; i < n; ++i) {
        int el;
        cin >> el;
        newlist.push_front(el);
    }

    cout << "Список после добавления новых элементов: ";
    for (int element : newlist) {
        cout << element << " ";
    }
    cout << endl;

    return 0;
}
