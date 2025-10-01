#include <stdio.h>
#include <ctype.h>

int main() {
    FILE* F1, * F2;
    char line[100];

    // Открываем файлы для чтения и записи
    F1 = fopen_s("F1.txt", "r");
    F2 = fopen_s("F2.txt", "w");

    if (F1 == NULL || F2 == NULL) {
        printf("Ошибка открытия файлов.\n");
        return 1;
    }

    // Считываем строки из F1 и копируем в F2 те, которые начинаются с цифры
    while (fgets(line, sizeof(line), F1) != NULL) {
        if (isdigit(line[0])) {
            fputs(line, F2);
        }
    }

    // Закрываем файлы
    fclose(F1);
    fclose(F2);

    printf("Программа успешно выполнена.\n");

    return 0;
}
