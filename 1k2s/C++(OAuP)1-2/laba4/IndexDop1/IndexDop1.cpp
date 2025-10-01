#include <iostream>
#include <cstring>
using namespace std;

typedef struct {
    char lastName[50];
    char name[50]; 
    int countExans; 
    int marks[5]; 
} Student;

// Прототипы функций
void input(int size); 
void find(char lastName[]);
void check(char stud[]);

int main()
{
    setlocale(LC_ALL, "Russian");
    int choice;
    char stud[50];
    int number;
    do
    {
        cout << "\n1. Ввод данных о студентах\n";
        cout << "2. Проверка на успешную сдачу всех экзаменов\n";
        cout << "3. Обработка успееваемости всех студентов\n";
        cout << "0. Выход из программы\n\n";
        cout << "Введите номер операции: ";
        cin >> choice;
        switch (choice)
        {
        case 1:
            cout << "Введите количество студентов: ";
            cin >> number;
            input(number);
            break;
        case 2:
            cout << "Введите фамилию: ";
            cin.ignore(); // Очистка буфера перед вводом строки
            cin.getline(stud, 50);
            find(stud);
            break;
        case 3:
            cout << "Обработка успееваемости всех студентов: ";
            break;
        case 0:
            exit(0);
            break;
        default:
            cout << "Неверный ввод. Пожалуйста, введите существующий номер операции.\n";
        }
    } while (choice != 0);
    return 0;
}

// Функция ввода данных
void input(int size)
{
    Student student;
    FILE* file;
    if (!fopen_s(&file, "student.bin", "ab"))
    {
        for (int i = 0; i < size; ++i)
        {
            cout << "Введите фамилию: ";
            cin >> student.lastName;
            cout << "Введите имя: ";
            cin >> student.name;
            cout << "Введите кол-во экзаменов: ";
            cin >> student.countExans;
            cout << "Введите оценки(0-5): ";
            for (int j = 0; j < student.countExans; ++j)
            {
                cout << "Введите оценку " << j + 1 << ": ";
                cin >> student.marks[j];
            }
            fwrite(&student, sizeof(student), 1, file);
        }
        fclose(file);
    }
    else
    {
        cout << "Ошибка открытия файла";
        return;
    }
}

void find(char lastName[16])
{
    Student student;
    FILE* file;
    bool found = false; // Добавляем флаг для отслеживания наличия студента с заданной фамилией

    if (!fopen_s(&file, "student.bin", "rb"))
    {
        while (fread(&student, sizeof(student), 1, file))
        {
            if (strcmp(student.lastName, lastName) == 0) // Проверяем совпадение фамилии
            {
                found = true; // Устанавливаем флаг в true, если студент найден
                bool passedAll = true;
                for (int i = 0; i < student.countExans; i++) {
                    if (student.marks[i] < 4) {
                        passedAll = false;
                        break; // Мы уже знаем, что не все экзамены сданы на 4 и 5, поэтому можно выйти из цикла
                    }
                }
                if (passedAll) {
                    cout << "\nСтудент " << student.lastName << " сдал все экзамены на 4 и 5";
                }
                else {
                    cout << "\nСтудент " << student.lastName << " не сдал все экзамены на 4 и 5";
                }
            }
        }
        fclose(file); // Закрываем файл только после завершения всех итераций
    }
    else
    {
        cout << "Ошибка открытия файла";
        return;
    }

    if (!found) {
        cout << "Студент с фамилией " << lastName << " не найден в файле";
    }
}

