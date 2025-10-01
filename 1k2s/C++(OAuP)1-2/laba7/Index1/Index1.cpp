#include <iostream>
#include <fstream>

struct Stack
{
    char data;          // Информационный элемент
    Stack* next;        // Указатель на следующий элемент
};


void show(Stack* myStk)
{
    Stack* e = myStk;
    if (e == NULL)
    {
        std::cout << "Стек пуст!" << std::endl;
        return;
    }

    while (e != NULL)
    {
        std::cout << e->data << " ";
        e = e->next;
    }
    std::cout << std::endl;
}

char pop(Stack*& myStk);
void push(char x, Stack*& myStk);
void toFile(Stack*& myStk);
void fromFile(Stack*& myStk);
int countDuplicates(Stack* myStk, char element);

int main()
{
    setlocale(LC_ALL, "Rus");
    int choice;
    char x;
    Stack* myStk = nullptr;

    for (;;)
    {
        std::cout << "Выберите команду:" << std::endl;
        std::cout << "1 - Добавление элемента в стек" << std::endl;
        std::cout << "2 - Извлечение элемента из стека" << std::endl;
        std::cout << "3 - Запись в файл" << std::endl;
        std::cout << "4 - Чтение из файла" << std::endl;
        std::cout << "5 - Вывод стека" << std::endl;
        std::cout << "6 - Подсчет повторяющихся элементов в стеке" << std::endl;
        std::cout << "7 - Выход" << std::endl;
        std::cin >> choice;

        switch (choice)
        {
        case 1:
            std::cout << "Введите элемент: ";
            std::cin >> x;
            push(x, myStk);
            break;
        case 2:
            x = pop(myStk);
            if (x != -1)
                std::cout << "Извлеченный элемент: " << x << std::endl;
            break;
        case 3:
            toFile(myStk);
            break;
        case 4:
            fromFile(myStk);
            break;
        case 5:
            std::cout << "Весь стек: ";
            show(myStk);
            break;
        case 6:
            char element;
            int duplicates;
            std::cout << "Введите элемент для подсчета повторений: ";
            std::cin >> element;
            duplicates = countDuplicates(myStk, element); // Используем переменную после ее инициализации
            std::cout << "Количество повторений элемента " << element << " в стеке: " << duplicates << std::endl;
            break;
        case 7:
            return 0;
            break;
        }
    }

    return 0;
}

int countDuplicates(Stack* myStk, char element) {
    int count = 0;
    Stack* temp = myStk; // Создаем временный указатель для прохода по стеку

    // Проходим по всем элементам стека
    while (temp != nullptr) {
        // Если текущий элемент равен искомому элементу, увеличиваем счетчик
        if (temp->data == element) {
            count++;
        }
        temp = temp->next; // Переходим к следующему элементу
    }

    return count;
}

void push(char x, Stack*& myStk)
{
    Stack* e = new Stack;    // Выделение памяти для нового элемента
    e->data = x;             // Запись элемента x в поле data
    e->next = myStk;         // Перенос вершины на следующий элемент
    myStk = e;               // Сдвиг вершины на позицию вперед
}

char pop(Stack*& myStk)
{
    if (myStk == NULL)
    {
        std::cout << "Стек пуст!" << std::endl;
        return -1;            // Если стек пуст - возврат (-1)
    }
    else
    {
        Stack* e = myStk;     // Е-переменная для хранения адреса элемента
        char x = myStk->data; // Запись элемента из поля data в переменную x
        myStk = myStk->next;  // Перенос вершины
        delete e;
        return x;
    }
}

void toFile(Stack*& myStk)
{
    Stack* e = myStk;
    std::ofstream frm("mStack.dat");
    if (frm.fail())
    {
        std::cout << "\nОшибка открытия файла" << std::endl;
        exit(1);
    }
    while (e)
    {
        frm << e->data << std::endl;
        e = e->next;
    }
    frm.close();
    std::cout << "Стек записан в файл mStack.dat" << std::endl;
}

void fromFile(Stack*& myStk)
{
    char x;
    std::ifstream frm("mStack.dat");
    if (frm.fail())
    {
        std::cout << "\nОшибка открытия файла" << std::endl;
        exit(1);
    }
    while (frm >> x)
    {
        push(x, myStk);
    }
    frm.close();
    std::cout << "\nСтек считан из файла mStack.dat" << std::endl;
}

