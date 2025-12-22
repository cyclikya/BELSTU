#define _GNU_SOURCE

#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <fcntl.h>
#include <string.h>
#include <errno.h>

#define MAX_BUFFER 65536
#define MAX_LINE   1024

int    g_fd = -1;       
char* g_Buffer = NULL; 
size_t g_FileSize = 0;

void PrintError(const char* msg)
{
    perror(msg);
}

int IsFileOpened()
{
    return g_fd != -1;
}

int OpenStudentFile(char* filePath)
{
    if (IsFileOpened())
    {
        printf("File already opened\n");
        return 0;
    }

    g_fd = open(filePath, O_RDWR);
    if (g_fd == -1)
    {
        PrintError("open failed");
        return 0;
    }

    g_Buffer = (char*)malloc(MAX_BUFFER);
    if (!g_Buffer)
    {
        PrintError("malloc failed");
        close(g_fd);
        g_fd = -1;
        return 0;
    }

    printf("File opened successfully\n");
    return 1;
}

int LoadFileToBuffer()
{
    lseek(g_fd, 0, SEEK_SET);
    g_FileSize = read(g_fd, g_Buffer, MAX_BUFFER - 1);
    if (g_FileSize == (size_t)-1)
        return 0;

    g_Buffer[g_FileSize] = '\0';
    return 1;
}

int SaveBufferToFile()
{
    lseek(g_fd, 0, SEEK_SET);
    ftruncate(g_fd, 0);
    write(g_fd, g_Buffer, strlen(g_Buffer));
    return 1;
}

int CountLines()
{
    int count = 0;
    for (size_t i = 0; i < g_FileSize; i++)
        if (g_Buffer[i] == '\n')
            count++;
    return count;
}

int AddRow(int fd, char* row, int pos)
{
    if (!IsFileOpened() || !row)
        return 0;

    LoadFileToBuffer();
    int lines = CountLines();
    int target;

    if (pos > 0 && pos <= lines + 1)
        target = pos - 1;
    else if (pos == 0)
        target = 0;
    else if (pos == -1)
        target = lines;
    else
        return 0;

    char newBuffer[MAX_BUFFER] = { 0 };
    int currentLine = 0;
    char* src = g_Buffer;
    char* dst = newBuffer;

    while (*src)
    {
        if (currentLine == target)
            dst += sprintf(dst, "%s\n", row);

        *dst++ = *src;
        if (*src == '\n')
            currentLine++;
        src++;
    }

    if (target == lines)
        sprintf(dst, "%s\n", row);

    strcpy(g_Buffer, newBuffer);
    SaveBufferToFile();
    return 1;
}

int RemRow(int fd, int pos)
{
    if (!IsFileOpened())
        return 0;

    LoadFileToBuffer();
    int lines = CountLines();
    int target;

    if (pos > 0 && pos <= lines)
        target = pos - 1;
    else if (pos == 0)
        target = 0;
    else if (pos == -1)
        target = lines - 1;
    else
        return 0;

    char newBuffer[MAX_BUFFER] = { 0 };
    int currentLine = 0;
    char* src = g_Buffer;
    char* dst = newBuffer;

    while (*src)
    {
        if (currentLine != target)
            *dst++ = *src;

        if (*src == '\n')
            currentLine++;
        src++;
    }

    strcpy(g_Buffer, newBuffer);
    SaveBufferToFile();
    return 1;
}

int PrintRow(int fd, int pos)
{
    if (!IsFileOpened())
        return 0;

    LoadFileToBuffer();
    int lines = CountLines();
    int target;

    if (pos > 0 && pos <= lines)
        target = pos - 1;
    else if (pos == 0)
        target = 0;
    else if (pos == -1)
        target = lines - 1;
    else
        return 0;

    int currentLine = 0;
    char line[MAX_LINE] = { 0 };
    char* p = g_Buffer;
    char* out = line;

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
    return 1;
}

int PrintRows(int fd)
{
    if (!IsFileOpened())
        return 0;

    LoadFileToBuffer();
    printf("\n===== FILE CONTENT =====\n%s\n========================\n", g_Buffer);
    return 1;
}

int CloseFile(int fd)
{
    if (!IsFileOpened())
        return 0;

    close(g_fd);
    g_fd = -1;

    free(g_Buffer);
    g_Buffer = NULL;

    printf("File closed\n");
    return 1;
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
    int cmd;
    char path[1024];
    char row[MAX_LINE];
    int pos;

    do
    {
        Menu();
        scanf("%d", &cmd);
        getchar();

        switch (cmd)
        {
        case 1:
            printf("File path: ");
            fgets(path, sizeof(path), stdin);
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
            AddRow(g_fd, row, pos);
            break;

        case 3:
            printf("Position: ");
            scanf("%d", &pos);
            getchar();
            RemRow(g_fd, pos);
            break;

        case 4:
            printf("Position: ");
            scanf("%d", &pos);
            getchar();
            PrintRow(g_fd, pos);
            break;

        case 5:
            PrintRows(g_fd);
            break;

        case 6:
            CloseFile(g_fd);
            break;
        }
    } while (cmd != 0);

    if (IsFileOpened())
        CloseFile(g_fd);

    return 0;
}
