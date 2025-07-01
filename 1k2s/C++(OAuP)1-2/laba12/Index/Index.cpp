#include <iostream>
using namespace std;

class AAA {
public:
    int x;
};

class heap {
public:
    enum CMP { EQUAL, LESS, GREAT };

    static CMP cmpAAA(void* a1, void* a2) {
#define A1 ((AAA*)a1)
#define A2 ((AAA*)a2)
        CMP rc = EQUAL;
        if (A1->x > A2->x)
            rc = GREAT;
        else if (A2->x > A1->x)
            rc = LESS;
        return rc;
#undef A2
#undef A1
    }
};

class Heap {
    int size;
    int maxSize;
    void** heapArray;
    heap::CMP(*compare)(void*, void*);

    void heapify(int index) {
        int largest = index;
        int leftChild = 2 * index + 1;
        int rightChild = 2 * index + 2;

        if (leftChild < size && compare(heapArray[leftChild], heapArray[largest]) == heap::GREAT)
            largest = leftChild;

        if (rightChild < size && compare(heapArray[rightChild], heapArray[largest]) == heap::GREAT)
            largest = rightChild;

        if (largest != index) {
            swap(heapArray[index], heapArray[largest]);
            heapify(largest);
        }
    }
public:
    static Heap create(int maxSize, heap::CMP(*cmp)(void*, void*)) {
        Heap h;
        h.size = 0;
        h.maxSize = maxSize; // Присваиваем значение maxSize
        h.heapArray = new void* [maxSize];
        h.compare = cmp;
        return h;
    }

public:
    static Heap create(int maxSize, heap::CMP(*cmp)(void*, void*)) {
        Heap h;
        h.size = 0;
        h.heapArray = new void* [maxSize];
        h.compare = cmp;
        return h;
    }

    void scan(int index) {
        if (index >= size) {
            cout << "Неверный индекс." << endl;
            return;
        }

        if (index < size) {
            cout << "x = " << ((AAA*)heapArray[index])->x << endl;
            scan(2 * index + 1); // Рекурсивно вызываем для левого поддерева
            scan(2 * index + 2); // Рекурсивно вызываем для правого поддерева
        }
    }

    void insert(AAA* a) {
        if (size == maxSize) {
            cout << "Куча полна. Невозможно добавить элемент." << endl;
            return;
        }

        int i = size;
        heapArray[size++] = a;

        while (i > 0 && compare(heapArray[i], heapArray[(i - 1) / 2]) == heap::GREAT) {
            swap(heapArray[i], heapArray[(i - 1) / 2]);
            i = (i - 1) / 2;
        }
    }

    void extractMax() {
        if (size == 0) {
            cout << "Куча пуста. Невозможно извлечь максимальный элемент." << endl;
            return;
        }

        swap(heapArray[0], heapArray[size - 1]);
        size--;
        heapify(0);
    }

    void extractMin();
    void extractI(int i);
    void unionHeap(const Heap& other);
};

void Heap::extractMin() {
    if (size == 0) {
        cout << "Куча пуста. Невозможно удалить минимальный элемент." << endl;
        return;
    }

    swap(heapArray[0], heapArray[size - 1]);
    size--;
    heapify(0);
}

void Heap::extractI(int i) {
    if (i < 0 || i >= size) {
        cout << "Неверный индекс элемента для удаления." << endl;
        return;
    }

    swap(heapArray[i], heapArray[size - 1]);
    size--;
    heapify(i);
}

void Heap::unionHeap(const Heap& other) {
    for (int i = 0; i < other.size; ++i) {
        insert((AAA*)other.heapArray[i]);
    }
}

int main() {
    setlocale(LC_ALL, "rus");
    int k, choice;
    Heap h1 = Heap::create(30, heap::cmpAAA);
    for (;;) {
        cout << "1 - вывод кучи на экран" << endl;
        cout << "2 - добавить элемент" << endl;
        cout << "3 - удалить максимальный элемент" << endl;
        cout << "4 - удалить минимальный элемент" << endl;
        cout << "5 - удалить i-ый элемент" << endl;
        cout << "6 - объединить две кучи" << endl;
        cout << "0 - выход" << endl;
        cout << "Сделайте выбор: ";
        cin >> choice;
        switch (choice) {
        case 0:
            exit(0);
        case 1:
            h1.scan(0);
            break;
        case 2: {
            AAA* a = new AAA;
            cout << "Введите ключ: ";
            cin >> k;
            a->x = k;
            h1.insert(a);
        }
              break;
        case 3:
            h1.extractMax();
            break;
        case 4:
            h1.extractMin();
            break;
        case 5: {
            int index;
            cout << "Введите индекс элемента для удаления: ";
            cin >> index;
            h1.extractI(index);
        }
              break;
        case 6: {
            int newSize;
            cout << "Введите размер второй кучи: ";
            cin >> newSize;
            Heap h2 = Heap::create(newSize, heap::cmpAAA);
            // Заполните вторую кучу h2 элементами...
            h1.unionHeap(h2);
        }
              break;
        default:
            cout << endl << "Введена неверная команда!" << endl;
        }
    }
    return 0;
}