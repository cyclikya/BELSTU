#include <iostream>
#include "MyQueue.h"
using namespace std;

// Вывод на экран с очисткой очереди
void printQueue(Queue& q)
{
    while (!(q.isEmpty()))  // Пока очередь не пуста
    {
        cout << *(int*)peekQueue(q) << " ";  // Выводим первый элемент очереди
        delQueue(q);  // Удаляем первый элемент очереди
    }
    cout << endl; 
}

// Добавление отрицательного элемента в очередь
void enqueueNegative(Queue& q, int value) {
    if (value < 0) {  
        int* elem = new int(value);  // Создаем новый элемент
        enQueue(q, elem);  // Добавляем элемент в очередь
    }
}

// Отображение минимального и максимального элементов очереди
void displayMinMax(Queue& q) {
    if (!q.isEmpty()) {  // Если очередь не пуста
        int min = *(int*)peekQueue(q);  // Первый элемент очереди как минимальный
        int max = *(int*)peekQueue(q);  // Первый элемент очереди как максимальный

        Queue temp = createQueue(q.Size);  // Создаем временную очередь
        while (!q.isEmpty()) {  
            int value = *(int*)peekQueue(q);  // Получаем первый элемент очереди
            if (value < min) min = value;  
            if (value > max) max = value;  
            enQueue(temp, delQueue(q));  // Удаляем и добавляем элементы во временную очередь
        }

        cout << "Minimum element: " << min << endl;  
        cout << "Maximum element: " << max << endl;  

        while (!temp.isEmpty()) {  // Пока временная очередь не пуста
            enQueue(q, delQueue(temp));  // Удаляем и добавляем элементы обратно в исходную очередь
        }
    }
    else {
        cout << "Queue is empty." << endl;  // Очередь пуста
    }
}

int main()
{
    int maxSize;
    cout << "Enter the maximum size of the queue: ";
    cin >> maxSize;

    Queue q1 = createQueue(maxSize);  // Создание очереди заданного размера

    char choice;
    do {
        cout << "\nMenu:\n";
        cout << "1. Enqueue\n";
        cout << "2. Display min and max elements\n";
        cout << "3. Print queue\n";
        cout << "4. Count elements in the queue\n";
        cout << "5. Exit\n";
        cout << "Enter your choice: ";
        cin >> choice;

        switch (choice) {
        case '1': {
            int value;
            cout << "Enter an integer value: ";
            cin >> value;
            enqueueNegative(q1, value);  // Добавление отрицательного элемента в очередь
            break;
        }
        case '2':
            displayMinMax(q1);  // Отображение минимального и максимального элементов очереди
            break;
        case '3':
            cout << "Queue contents: ";
            printQueue(q1);  // Вывод содержимого очереди
            break;
        case '4':
            cout << "Number of elements in the queue: " << (q1.Tail - q1.Head) << endl;  // Подсчет количества элементов в очереди
            break;
        case '5':
            cout << "Exiting...\n";  // Выход из программы
            break;
        default:
            cout << "Invalid choice. Please enter a number between 1 and 5.\n";  // Неверный выбор
        }
    } while (choice != '5');

    releaseQueue(q1);  // Освобождение ресурсов, выделенных для очереди
    return 0;
}

