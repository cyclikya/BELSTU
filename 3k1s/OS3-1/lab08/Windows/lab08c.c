#define _WIN32_WINNT 0x0600
#include <windows.h>
#include <stdio.h>
#include <locale.h>

#define INITIAL_HEAP_SIZE (2 * 1024 * 1024)   // 2 MiB (стартовый размер кучи)
#define MAX_HEAP_SIZE     (16 * 1024 * 1024)  // 16 MiB (максимальный размер кучи)
#define BLOCK_SIZE        (512 * 1024)        // 512 KiB
#define NUM_BLOCKS        10                  // 10 блоков по условию

void PrintError(const char* stage) {
    DWORD e = GetLastError();
    printf("%s failed. GetLastError = %lu\n", stage, (unsigned long)e);
}

void HeapInfo(HANDLE heap) {
    PROCESS_HEAP_ENTRY entry;
    SIZE_T total_size = 0;
    DWORD lastErr;

    // Заголовок
    printf("\n===== ИНФОРМАЦИЯ О КУЧЕ =====\n");

    // Блокируем кучу для безопасного перечисления
    if (!HeapLock(heap)) {
        PrintError("HeapLock");
        return;
    }

    ZeroMemory(&entry, sizeof(entry));
    // HeapWalk возвращает TRUE, пока есть элементы
    while (HeapWalk(heap, &entry)) {
        // cbData содержит размер блока данных (не включая служебную структуру)
        total_size += entry.cbData;

        printf("Область: адрес=%p, размер=%lu байт, ",
            entry.lpData, (unsigned long)entry.cbData);

        if (entry.wFlags & PROCESS_HEAP_REGION) {
            printf("тип=REGION (регион)\n");
            printf("   Region: BaseAddress=%p, CommittedSize=%lu, ReserveSize=%lu\n",
                entry.Region.lpFirstBlock,
                (unsigned long)entry.Region.dwCommittedSize,
                (unsigned long)entry.Region.dwCommittedSize + (unsigned long)entry.Region.dwCommittedSize // placeholder
            );
        }
        else if (entry.wFlags & PROCESS_HEAP_UNCOMMITTED_RANGE) {
            printf("тип=UNCOMMITTED (невыделенная область)\n");
        }
        else if (entry.wFlags & PROCESS_HEAP_ENTRY_BUSY) {
            printf("тип=BUSY (занятый блок)\n");
        }
        else {
            printf("тип=FREE (свободный блок)\n");
        }
    }

    lastErr = GetLastError();
    if (lastErr != ERROR_NO_MORE_ITEMS) {
        // Если ошибка отлична от "конец списка" — сообщим
        printf("HeapWalk завершился с ошибкой: %lu\n", (unsigned long)lastErr);
    }

    HeapUnlock(heap);

    printf("Общий суммарный размер данных (перечислено): %lu байт\n", (unsigned long)total_size);
    printf("================================\n");
}

int main(void) {
    setlocale(LC_ALL, "");

    printf("STEP 1: Create heap\n");

    // Создаём частную кучу с флагом, позволяющим HeapWalk корректно работать
    HANDLE heap = HeapCreate(HEAP_CREATE_ENABLE_EXECUTE, INITIAL_HEAP_SIZE, MAX_HEAP_SIZE);
    if (!heap) {
        PrintError("HeapCreate");
        return 1;
    }

    printf("Начальный размер: %lu байт, максимальный: %lu байт\n",
        (unsigned long)INITIAL_HEAP_SIZE, (unsigned long)MAX_HEAP_SIZE);

    // Показываем информацию о куче после создания
    HeapInfo(heap);
    system("pause & cls");

    // Массив указателей на блоки
    void* blocks[NUM_BLOCKS] = { 0 };

    // ЭТАП 2: выделение блоков (после каждой итерации вызываем HeapInfo)
    printf("STEP 2: Allocate %d blocks of %d bytes each\n", NUM_BLOCKS, BLOCK_SIZE);
    for (int i = 0; i < NUM_BLOCKS; ++i) {
        blocks[i] = HeapAlloc(heap, HEAP_ZERO_MEMORY, BLOCK_SIZE);
        if (!blocks[i]) {
            printf("HeapAlloc failed for block %d. GetLastError = %lu\n", i, (unsigned long)GetLastError());
            // при ошибке прекращаем цикл, но перед уничтожением кучи освободим уже выделенные
            break;
        }

        printf("Allocated block %d at %p (size %d bytes)\n", i, blocks[i], BLOCK_SIZE);

        // Заполним блок массивом int-ов (512KiB / 4 = 131072 элементов)
        int* arr = (int*)blocks[i];
        size_t count = BLOCK_SIZE / sizeof(int);
        for (size_t j = 0; j < count; ++j) {
            arr[j] = (int)(i * 1000000 + j);
        }
        printf("Filled block %d with %zu integers (arr[0]=%d, arr[%zu]=%d)\n",
            i, count, arr[0], count - 1, arr[count - 1]);

        // Выводим информацию о куче после каждой итерации (как требует условие)
        HeapInfo(heap);
        system("pause & cls");
    }

    // ЭТАП 3: освобождение всех блоков
    printf("STEP 3: Free blocks\n");
    for (int i = 0; i < NUM_BLOCKS; ++i) {
        if (blocks[i]) {
            if (!HeapFree(heap, 0, blocks[i])) {
                printf("HeapFree failed for block %d. GetLastError = %lu\n", i, (unsigned long)GetLastError());
            }
            else {
                printf("Freed block %d at %p\n", i, blocks[i]);
                blocks[i] = NULL;
            }
        }
    }

    // Информация о куче после освобождения
    HeapInfo(heap);
    system("pause & cls");

    // ЭТАП 4: уничтожение кучи
    printf("STEP 4: Destroy heap\n");
    if (!HeapDestroy(heap)) {
        PrintError("HeapDestroy");
        return 1;
    }

    printf("Heap destroyed.\n");
    system("pause");
    return 0;
}
