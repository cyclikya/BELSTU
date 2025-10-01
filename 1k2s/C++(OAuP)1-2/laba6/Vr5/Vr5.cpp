#include <iostream>
#include <fstream>

using namespace std;

struct list
{
    int value;
    list* next; //указатель на следующий элемент
};

void input(list*& p, int value);
void printList(list* p);
void toFile(list*& p);
void fromFile(list*& p);
void menu();
int sum(list* p);

int main()
{
    setlocale(LC_CTYPE, "Russian");

    list* first = nullptr; //указатель на первый элемент списка
    int choice, value;

    menu();
    cout << " ? ";
    cin >> choice;

    while (choice != 6)
    {
        switch (choice)
        {
        case 1:
            cout << "Введите целое число: ";
            cin >> value;
            input(first, value);
            cout << "лист теперь: " << endl;
            printList(first);
            break;
        case 2:
            toFile(first);
            break;
        case 3:
            fromFile(first);
            break;
        case 4:
            cout << "Сумма положительных двузначных элементов: " << sum(first) << endl;
            break;
        case 5:
            printList(first);
            break;
        default:
            cout << "Неправильный выбор" << endl;
            menu();
            break;
        }
        cout << "?  ";
        cin >> choice;
    }
    return 0;
}

void input(list*& p, int value)
{
    list* newP = new list; //создние нового элемента списка
    if (newP != NULL)
    {
        newP->value = value;//присвоение значения новому элементу
        newP->next = p;     // назначение следующего элемента - текущего первого элемента
        p = newP;           // Перемещение указателя на начало списка на новый элемент
    }
    else
        cout << "Операция добавления не выполнена" << endl;
}

void printList(list* p)
{
    if (p == NULL)
        cout << "Список пуст" << endl;
    else
    {
        cout << "Список:" << endl;
        while (p != NULL)
        {
            cout << " --> " << p->value;
            p = p->next;
        }
        cout << " -->NULL " << endl;
    }
}

void toFile(list*& p)
{
    list* temp = p; //указатель для обхода списка
    list buf;
    ofstream frm("mList.dat"); 
    if (frm.fail())
    {
        cout << "\n Ошибка открытия файла";
        exit(1);
    }
    while (temp)
    {
        buf = *temp;
        frm.write((char*)&buf, sizeof(list)); //записывает в файл элемента
        temp = temp->next; //переход к следующему элементу списка
    }
    frm.close();
    cout << "Список записан в файл mList.dat\n";
}

void fromFile(list*& p)
{
    list buf;
    list* first = nullptr; //указатель для временного элемента
    ifstream frm("mList.dat");
    if (frm.fail())
    {
        cout << "\n Ошибка открытия файла";
        exit(1);
    }
    frm.read((char*)&buf, sizeof(list));
    while (!frm.eof())
    {
        input(first, buf.value);
        cout << "-->" << buf.value;
        frm.read((char*)&buf, sizeof(list));
    }
    cout << "-->NULL" << endl;
    frm.close();
    p = first;
    cout << "\nСписок считан из файла mList.dat\n";

}

void menu()
{
    cout << "Сделайте выбор:" << endl;
    cout << " 1 - Ввод элемента" << endl;
    cout << " 2 - Запись списка в файл" << endl;
    cout << " 3 - Чтение списка из файла" << endl;
    cout << " 4 - Найти сумму положительных двузначных элементов" << endl;
    cout << " 5 - Вывод списка" << endl;
    cout << " 6 - Выход" << endl;
}

int sum(list* p)
{
    int sum = 0;
    while (p != nullptr)
    {
        if (p->value > 0 && p->value >= -10)
            sum += p->value;
        p = p->next;
    }
    return sum;
}
