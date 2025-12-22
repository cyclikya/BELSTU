#define _CRT_SECURE_NO_WARNINGS

#include <windows.h>
#include <stdio.h>

void PrintError(const char* msg)
{
    DWORD err = GetLastError();
    printf("Error: %s (code %lu)\n", msg, err);
}

BOOL IsTextFile(HANDLE hFile)
{
    DWORD fileSize = GetFileSize(hFile, NULL);

    if (fileSize == 0)
        return TRUE;

    BYTE buffer[512];
    DWORD bytesRead = 0;

    SetFilePointer(hFile, 0, NULL, FILE_BEGIN);

    if (!ReadFile(hFile, buffer, sizeof(buffer), &bytesRead, NULL))
        return FALSE;

    for (DWORD i = 0; i < bytesRead; i++)
    {
        if (buffer[i] == 0)
            return FALSE;
    }

    return TRUE;
}

void PrintFileType(DWORD attr)
{
    if (attr & FILE_ATTRIBUTE_DIRECTORY)
        printf("Object type: directory\n");
    else if (attr & FILE_ATTRIBUTE_DEVICE)
        printf("Object type: device\n");
    else
        printf("Object type: regular file\n");
}

void PrintTime(const char* title, FILETIME ft)
{
    SYSTEMTIME stUTC, stLocal;
    FileTimeToSystemTime(&ft, &stUTC);
    SystemTimeToTzSpecificLocalTime(NULL, &stUTC, &stLocal);

    printf("%s: %02d.%02d.%04d %02d:%02d:%02d\n",
        title,
        stLocal.wDay,
        stLocal.wMonth,
        stLocal.wYear,
        stLocal.wHour,
        stLocal.wMinute,
        stLocal.wSecond);
}

void PrintInfo(LPSTR FileName)
{
    WIN32_FILE_ATTRIBUTE_DATA fad;

    if (!GetFileAttributesExA(FileName, GetFileExInfoStandard, &fad))
    {
        PrintError("Failed to get file attributes");
        return;
    }

    LARGE_INTEGER size;
    size.HighPart = fad.nFileSizeHigh;
    size.LowPart = fad.nFileSizeLow;

    printf("File name: %s\n", FileName);
    printf("File size:\n");
    printf("  %lld bytes\n", size.QuadPart);
    printf("  %.2f KiB\n", size.QuadPart / 1024.0);
    printf("  %.2f MiB\n", size.QuadPart / (1024.0 * 1024.0));

    PrintFileType(fad.dwFileAttributes);

    PrintTime("Creation time", fad.ftCreationTime);
    PrintTime("Last access time", fad.ftLastAccessTime);
    PrintTime("Last write time", fad.ftLastWriteTime);

    HANDLE hFile = CreateFileA(
        FileName,
        GENERIC_READ,
        FILE_SHARE_READ,
        NULL,
        OPEN_EXISTING,
        FILE_ATTRIBUTE_NORMAL,
        NULL
    );

    if (hFile == INVALID_HANDLE_VALUE)
    {
        PrintError("Failed to open file");
        return;
    }

    if (IsTextFile(hFile))
        printf("File type: text file\n");
    else
        printf("File type: binary file\n");

    CloseHandle(hFile);
}

void PrintText(LPSTR FileName)
{
    HANDLE hFile = CreateFileA(
        FileName,
        GENERIC_READ,
        FILE_SHARE_READ,
        NULL,
        OPEN_EXISTING,
        FILE_ATTRIBUTE_NORMAL,
        NULL
    );

    if (hFile == INVALID_HANDLE_VALUE)
    {
        PrintError("Failed to open file");
        return;
    }

    if (!IsTextFile(hFile))
    {
        printf("The specified file is not a text file\n");
        CloseHandle(hFile);
        return;
    }

    SetFilePointer(hFile, 0, NULL, FILE_BEGIN);

    CHAR buffer[1024];
    DWORD bytesRead;

    printf("\n===== FILE CONTENT =====\n\n");

    while (ReadFile(hFile, buffer, sizeof(buffer) - 1, &bytesRead, NULL) && bytesRead)
    {
        buffer[bytesRead] = '\0';
        printf("%s", buffer);
    }

    printf("\n\n===== END OF FILE =====\n");

    CloseHandle(hFile);
}

int main(int argc, char* argv[])
{
    if (argc != 2)
    {
        printf("Usage: Lab-09a.exe <file_path>\n");
        return 1;
    }

    PrintInfo(argv[1]);
    PrintText(argv[1]);

    return 0;
}
