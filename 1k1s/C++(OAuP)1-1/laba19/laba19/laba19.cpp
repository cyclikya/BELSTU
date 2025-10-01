#include <stdio.h>

int main() {
    FILE* fileA, * fileB;
    int num;
    int count[1000] = { 0 }; // Предполагаем, что числа в файле не превышают 1000

    // Открываем файлы для чтения и записи
    fileA = fopen_s("fileA.txt", "r");
    fileB = fopen_s("fileB.txt", "w");

    if (fileA == NULL || fileB == NULL) {
        printf("Ошибка открытия файлов.\n");
        return 1;
    }

    // Считываем числа из fileA и подсчитываем их повторения
    while (fscanf(fileA, "%d", &num) == 1) {
        count[num]++;
    }

    // Переходим в начало файла fileA
    rewind(fileA);

    // Записываем в fileB числа, которые встречаются более двух раз
    while (fscanf(fileA, "%d", &num) == 1) {
        if (count[num] > 2) {
            fprintf(fileB, "%d ", num);
        }
    }

    // Закрываем файлы
    fclose(fileA);
    fclose(fileB);

    printf("Программа успешно выполнена.\n");

    return 0;
}
