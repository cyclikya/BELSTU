#define _WIN32_WINNT 0x0600
#include <windows.h>
#include <stdio.h>

#define HEAP_INITIAL   (1 * 1024 * 1024)     // 1 MiB
#define HEAP_MAX       (8 * 1024 * 1024)     // 8 MiB

#define BLOCK_COUNT    5
#define BLOCK_SIZE     (1 * 1024 * 1024)     // 1 MiB

void PrintError(const char* stage)
{
    DWORD err = GetLastError();
    printf("%s failed. GetLastError = %lu\n", stage, err);

    if (err == ERROR_NOT_ENOUGH_MEMORY)
        printf("ERROR_NOT_ENOUGH_MEMORY detected (as expected)\n");
}

int main()
{
    printf("LAB-08C-1M (Windows)\n");
    printf("Attempt to allocate 5 blocks of 1 MiB via HeapAlloc\n");
    printf("Expected result: ERROR_NOT_ENOUGH_MEMORY\n\n");

    printf("STEP 1: Create heap\n");

    HANDLE heap = HeapCreate(0, HEAP_INITIAL, HEAP_MAX);
    if (!heap)
    {
        PrintError("HeapCreate");
        return 1;
    }

    system("pause");

    printf("\nSTEP 2: Try to allocate 5 blocks of 1 MiB using HeapAlloc\n");

    void* blocks[BLOCK_COUNT] = { 0 };

    for (int i = 0; i < BLOCK_COUNT; i++)
    {
        blocks[i] = HeapAlloc(heap, 0, BLOCK_SIZE);

        if (!blocks[i])
        {
            PrintError("HeapAlloc");
            printf("Allocation failed at block %d (as expected)\n", i);
            break;
        }

        printf("Allocated block %d: %p (1 MiB)\n", i, blocks[i]);
    }

    system("pause");

    printf("\nSTEP 3: Destroy heap\n");

    if (!HeapDestroy(heap))
    {
        PrintError("HeapDestroy");
        return 1;
    }

    printf("Heap destroyed\n");
    system("pause");

    return 0;
}
