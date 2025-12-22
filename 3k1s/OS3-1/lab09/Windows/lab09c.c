#define _CRT_SECURE_NO_WARNINGS

#include <windows.h>
#include <stdio.h>

#define BUFFER_SIZE 4096

void PrintError(const char* msg)
{
    printf("Error: %s (code %lu)\n", msg, GetLastError());
}

BOOL PrintDirectoryContent(LPCWSTR dirPath)
{
    WIN32_FIND_DATAW ffd;
    WCHAR searchPath[MAX_PATH];

    swprintf(searchPath, MAX_PATH, L"%s\\*", dirPath);

    HANDLE hFind = FindFirstFileW(searchPath, &ffd);
    if (hFind == INVALID_HANDLE_VALUE)
    {
        PrintError("Failed to read directory");
        return FALSE;
    }

    printf("Directory content:\n");

    do
    {
        if (wcscmp(ffd.cFileName, L".") == 0 ||
            wcscmp(ffd.cFileName, L"..") == 0)
            continue;

        if (ffd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
            wprintf(L"[DIR ] %s\n", ffd.cFileName);
        else
            wprintf(L"[FILE] %s\n", ffd.cFileName);

    } while (FindNextFileW(hFind, &ffd));

    FindClose(hFind);
    return TRUE;
}

void PrintAction(DWORD action)
{
    switch (action)
    {
    case FILE_ACTION_ADDED:
        printf("Event: File added\n");
        break;
    case FILE_ACTION_REMOVED:
        printf("Event: File removed\n");
        break;
    case FILE_ACTION_MODIFIED:
        printf("Event: File modified\n");
        break;
    case FILE_ACTION_RENAMED_OLD_NAME:
        printf("Event: File renamed (old name)\n");
        break;
    case FILE_ACTION_RENAMED_NEW_NAME:
        printf("Event: File renamed (new name)\n");
        break;
    default:
        printf("Event: Unknown\n");
    }
}

int wmain(int argc, wchar_t* argv[])
{
    if (argc != 2)
    {
        printf("Usage: lab09c.exe <directory_path>\n");
        return 1;
    }

    LPCWSTR dirPath = argv[1];

    HANDLE hDir = CreateFileW(
        dirPath,
        FILE_LIST_DIRECTORY,
        FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE,
        NULL,
        OPEN_EXISTING,
        FILE_FLAG_BACKUP_SEMANTICS,
        NULL
    );

    if (hDir == INVALID_HANDLE_VALUE)
    {
        PrintError("Directory does not exist or cannot be opened");
        return 1;
    }

    if (!PrintDirectoryContent(dirPath))
    {
        CloseHandle(hDir);
        return 1;
    }

    printf("\nMonitoring directory changes...\n");

    BYTE buffer[BUFFER_SIZE];
    DWORD bytesReturned;

    while (TRUE)
    {
        if (!ReadDirectoryChangesW(
            hDir,
            buffer,
            BUFFER_SIZE,
            FALSE, 
            FILE_NOTIFY_CHANGE_FILE_NAME |
            FILE_NOTIFY_CHANGE_DIR_NAME |
            FILE_NOTIFY_CHANGE_ATTRIBUTES |
            FILE_NOTIFY_CHANGE_SIZE |
            FILE_NOTIFY_CHANGE_LAST_WRITE |
            FILE_NOTIFY_CHANGE_CREATION,
            &bytesReturned,
            NULL,
            NULL))
        {
            PrintError("ReadDirectoryChangesW failed");
            break;
        }

        FILE_NOTIFY_INFORMATION* fni =
            (FILE_NOTIFY_INFORMATION*)buffer;

        do
        {
            PrintAction(fni->Action);

            wprintf(L"Object: %.*s\n",
                fni->FileNameLength / sizeof(WCHAR),
                fni->FileName);

            if (fni->NextEntryOffset == 0)
                break;

            fni = (FILE_NOTIFY_INFORMATION*)
                ((LPBYTE)fni + fni->NextEntryOffset);

        } while (TRUE);
    }

    CloseHandle(hDir);
    return 0;
}
