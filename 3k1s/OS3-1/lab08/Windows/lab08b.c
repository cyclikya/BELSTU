#include <windows.h>
#include <stdio.h>

int main()
{
    SYSTEM_INFO si;
    GetSystemInfo(&si);

    DWORD pageSize = si.dwPageSize;
    printf("Page size = %u bytes\n", pageSize);

    SIZE_T totalPages = 256;
    SIZE_T reserveSize = totalPages * pageSize;

    printf("STEP 1: Reserving 256 pages...\n");

    void* region = VirtualAlloc(
        NULL,
        reserveSize,
        MEM_RESERVE,
        PAGE_NOACCESS
    );

    printf("Reserved region = %p\n", region);
    getchar(); // STEP 1 pause

    printf("STEP 2: Commit 128 pages (2nd half)...\n");

    void* commitAddr = (char*)region + 128 * pageSize;

    BOOL ok = VirtualAlloc(
        commitAddr,
        128 * pageSize,
        MEM_COMMIT,
        PAGE_READWRITE
    );

    printf("Committed 2nd half = %p\n", commitAddr);
    getchar(); // STEP 2 pause

    printf("STEP 3: Filling committed memory...\n");
    int* arr = (int*)commitAddr;

    for (int i = 0; i < (128 * pageSize / sizeof(int)); i++)
        arr[i] = i;

    printf("Filled %u integers\n", 128 * pageSize / 4);
    getchar(); // STEP 3 pause

    printf("STEP 4: Make pages READONLY...\n");

    DWORD oldProt;
    VirtualProtect(
        commitAddr,
        128 * pageSize,
        PAGE_READONLY,
        &oldProt
    );

    printf("Protection changed to PAGE_READONLY\n");
    getchar(); // STEP 4 pause

    printf("STEP 5: Decommit (free physical memory)...\n");

    VirtualFree(
        commitAddr,
        128 * pageSize,
        MEM_DECOMMIT
    );

    printf("Decommitted 2nd half\n");
    getchar(); // STEP 5 pause

    printf("STEP 6: Release virtual memory...\n");

    VirtualFree(
        region,
        0,
        MEM_RELEASE
    );

    printf("Released region\n");
    getchar(); // STEP 6 pause

    return 0;
}
