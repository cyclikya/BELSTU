#define _CRT_SECURE_NO_WARNINGS

#include <windows.h>
#include <stdio.h>

#define MAX_BUFFER 65536
#define MAX_LINE   1024

HANDLE g_hFile = INVALID_HANDLE_VALUE;
CHAR* g_Buffer = NULL;
DWORD  g_FileSize = 0;

void PrintError(const char* msg)
{
    printf("Error: %s (code %lu)\n", msg, GetLastError());
}

BOOL IsFileOpened()
{
    return g_hFile != INVALID_HANDLE_VALUE;
}

BOOL OpenStudentFile(LPSTR filePath)
{
    if (IsFileOpened())
    {
        printf("File already opened\n");
        return FALSE;
    }

    g_hFile = CreateFileA(
        filePath,
        GENERIC_READ | GENERIC_WRITE,
        FILE_SHARE_READ,
        NULL,
        OPEN_EXISTING,
        FILE_ATTRIBUTE_NORMAL,
        NULL
    );

    if (g_hFile == INVALID_HANDLE_VALUE)
    {
        PrintError("Failed to open file");
        return FALSE;
    }

    g_FileSize = GetFileSize(g_hFile, NULL);

    g_Buffer = (CHAR*)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, MAX_BUFFER);
    if (!g_Buffer)
    {
        PrintError("Failed to allocate buffer");
        CloseHandle(g_hFile);
        g_hFile = INVALID_HANDLE_VALUE;
        return FALSE;
    }

    printf("File opened successfully\n");
    return TRUE;
}

BOOL LoadFileToBuffer()
{
    SetFilePointer(g_hFile, 0, NULL, FILE_BEGIN);
    return ReadFile(g_hFile, g_Buffer, MAX_BUFFER - 1, &g_FileSize, NULL);
}

BOOL SaveBufferToFile()
{
    DWORD written;
    SetFilePointer(g_hFile, 0, NULL, FILE_BEGIN);
    SetEndOfFile(g_hFile);
    return WriteFile(g_hFile, g_Buffer, strlen(g_Buffer), &written, NULL);
}

INT CountLines()
{
    INT count = 0;
    for (DWORD i = 0; i < g_FileSize; i++)
        if (g_Buffer[i] == '\n')
            count++;
    return count;
}

BOOL AddRow(HANDLE hFile, LPSTR row, INT pos)
{
    if (!IsFileOpened() || !row)
        return FALSE;

    LoadFileToBuffer();
    INT lines = CountLines();
    INT target;

    if (pos > 0 && pos <= lines + 1)
        target = pos - 1;
    else if (pos == 0)
        target = 0;
    else if (pos == -1)
        target = lines;
    else
        return FALSE;

    CHAR newBuffer[MAX_BUFFER] = { 0 };
    INT currentLine = 0;
    CHAR* src = g_Buffer;
    CHAR* dst = newBuffer;

    while (*src)
    {
        if (currentLine == target)
        {
            dst += sprintf(dst, "%s\n", row);
        }
        *dst++ = *src;
        if (*src == '\n')
            currentLine++;
        src++;
    }

    if (target == lines)
        sprintf(dst, "%s\n", row);

    strcpy(g_Buffer, newBuffer);
    SaveBufferToFile();
    return TRUE;
}

BOOL RemRow(HANDLE hFile, INT pos)
{
    if (!IsFileOpened())
        return FALSE;

    LoadFileToBuffer();
    INT lines = CountLines();
    INT target;

    if (pos > 0 && pos <= lines)
        target = pos - 1;
    else if (pos == 0)
        target = 0;
    else if (pos == -1)
        target = lines - 1;
    else
        return FALSE;

    CHAR newBuffer[MAX_BUFFER] = { 0 };
    INT currentLine = 0;
    CHAR* src = g_Buffer;
    CHAR* dst = newBuffer;

    while (*src)
    {
        if (currentLine != target)
        {
            *dst++ = *src;
        }
        if (*src == '\n')
            currentLine++;
        src++;
    }

    strcpy(g_Buffer, newBuffer);
    SaveBufferToFile();
    return TRUE;
}

BOOL PrintRow(HANDLE hFile, INT pos)
{
    if (!IsFileOpened())
        return FALSE;

    LoadFileToBuffer();
    INT lines = CountLines();
    INT target;

    if (pos > 0 && pos <= lines)
        target = pos - 1;
    else if (pos == 0)
        target = 0;
    else if (pos == -1)
        target = lines - 1;
    else
        return FALSE;

    INT currentLine = 0;
    CHAR line[MAX_LINE] = { 0 };
    CHAR* p = g_Buffer;
    CHAR* out = line;

    while (*p)
    {
        if (currentLine == target)
        {
            if (*p == '\n')
                break;
            *out++ = *p;
        }
        if (*p == '\n')
            currentLine++;
        p++;
    }

    printf("Row: %s\n", line);
    return TRUE;
}

BOOL PrintRows(HANDLE hFile)
{
    if (!IsFileOpened())
        return FALSE;

    LoadFileToBuffer();
    printf("\n===== FILE CONTENT =====\n%s\n========================\n", g_Buffer);
    return TRUE;
}

BOOL CloseFile(HANDLE hFile)
{
    if (!IsFileOpened())
        return FALSE;

    CloseHandle(g_hFile);
    g_hFile = INVALID_HANDLE_VALUE;

    if (g_Buffer)
    {
        HeapFree(GetProcessHeap(), 0, g_Buffer);
        g_Buffer = NULL;
    }

    printf("File closed\n");
    return TRUE;
}

void Menu()
{
    printf("\nChoose operation:\n");
    printf("1. Open file\n");
    printf("2. Insert row\n");
    printf("3. Remove row\n");
    printf("4. Print row\n");
    printf("5. Print file\n");
    printf("6. Close file\n");
    printf("0. Exit\n");
}

int main()
{
    INT cmd;
    CHAR path[MAX_PATH];
    CHAR row[MAX_LINE];
    INT pos;

    do
    {
        Menu();
        scanf("%d", &cmd);
        getchar();

        switch (cmd)
        {
        case 1:
            printf("File path: ");
            fgets(path, MAX_PATH, stdin);
            path[strcspn(path, "\n")] = 0;
            OpenStudentFile(path);
            break;

        case 2:
            printf("Row text: ");
            fgets(row, MAX_LINE, stdin);
            row[strcspn(row, "\n")] = 0;
            printf("Position: ");
            scanf("%d", &pos);
            getchar();
            AddRow(g_hFile, row, pos);
            break;

        case 3:
            printf("Position: ");
            scanf("%d", &pos);
            getchar();
            RemRow(g_hFile, pos);
            break;

        case 4:
            printf("Position: ");
            scanf("%d", &pos);
            getchar();
            PrintRow(g_hFile, pos);
            break;

        case 5:
            PrintRows(g_hFile);
            break;

        case 6:
            CloseFile(g_hFile);
            break;
        }
    } while (cmd != 0);

    if (IsFileOpened())
        CloseFile(g_hFile);

    return 0;
}
