#define _GNU_SOURCE

#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/inotify.h>
#include <sys/stat.h>
#include <dirent.h>
#include <string.h>
#include <errno.h>

#define EVENT_BUF_LEN 4096

void PrintError(const char* msg)
{
    perror(msg);
}

int PrintDirectoryContent(const char* dirPath)
{
    DIR* dir = opendir(dirPath);
    if (!dir)
    {
        PrintError("opendir failed");
        return 0;
    }

    printf("Directory content:\n");

    struct dirent* entry;
    while ((entry = readdir(dir)) != NULL)
    {
        if (!strcmp(entry->d_name, ".") ||
            !strcmp(entry->d_name, ".."))
            continue;

        if (entry->d_type == DT_DIR)
            printf("[DIR ] %s\n", entry->d_name);
        else
            printf("[FILE] %s\n", entry->d_name);
    }

    closedir(dir);
    return 1;
}

void PrintEvent(uint32_t mask)
{
    if (mask & IN_CREATE)        printf("Event: Created\n");
    if (mask & IN_DELETE)        printf("Event: Deleted\n");
    if (mask & IN_MODIFY)        printf("Event: Modified\n");
    if (mask & IN_ATTRIB)        printf("Event: Metadata changed\n");
    if (mask & IN_MOVED_FROM)    printf("Event: Moved from\n");
    if (mask & IN_MOVED_TO)      printf("Event: Moved to\n");
    if (mask & IN_DELETE_SELF)   printf("Event: Directory deleted\n");
    if (mask & IN_MOVE_SELF)     printf("Event: Directory moved\n");
}

int main(int argc, char* argv[])
{
    if (argc != 2)
    {
        printf("Usage: ./lab09c <directory_path>\n");
        return 1;
    }

    const char* dirPath = argv[1];

    struct stat st;
    if (stat(dirPath, &st) == -1 || !S_ISDIR(st.st_mode))
    {
        printf("Error: directory does not exist\n");
        return 1;
    }

    if (!PrintDirectoryContent(dirPath))
        return 1;

    printf("\nMonitoring directory changes...\n");

    int inotifyFd = inotify_init();
    if (inotifyFd == -1)
    {
        PrintError("inotify_init failed");
        return 1;
    }

    int wd = inotify_add_watch(
        inotifyFd,
        dirPath,
        IN_CREATE |
        IN_DELETE |
        IN_MODIFY |
        IN_ATTRIB |
        IN_MOVED_FROM |
        IN_MOVED_TO |
        IN_DELETE_SELF |
        IN_MOVE_SELF
    );

    if (wd == -1)
    {
        PrintError("inotify_add_watch failed");
        close(inotifyFd);
        return 1;
    }

    char buffer[EVENT_BUF_LEN];

    while (1)
    {
        ssize_t length = read(inotifyFd, buffer, EVENT_BUF_LEN);
        if (length < 0)
        {
            PrintError("read failed");
            break;
        }

        ssize_t i = 0;
        while (i < length)
        {
            struct inotify_event* event =
                (struct inotify_event*)&buffer[i];

            PrintEvent(event->mask);

            if (event->len)
                printf("Object: %s\n", event->name);

            printf("\n");

            i += sizeof(struct inotify_event) + event->len;
        }
    }

    inotify_rm_watch(inotifyFd, wd);
    close(inotifyFd);
    return 0;
}
