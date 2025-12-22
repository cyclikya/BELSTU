#define _GNU_SOURCE

#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <fcntl.h>
#include <sys/stat.h>
#include <time.h>
#include <string.h>
#include <errno.h>

void PrintError(const char* msg)
{
    perror(msg);
}

void PrintFileType(mode_t mode)
{
    if (S_ISREG(mode))
        printf("Object type: regular file\n");
    else if (S_ISDIR(mode))
        printf("Object type: directory\n");
    else if (S_ISCHR(mode))
        printf("Object type: character device\n");
    else if (S_ISBLK(mode))
        printf("Object type: block device\n");
    else if (S_ISFIFO(mode))
        printf("Object type: FIFO (pipe)\n");
    else if (S_ISLNK(mode))
        printf("Object type: symbolic link\n");
    else if (S_ISSOCK(mode))
        printf("Object type: socket\n");
    else
        printf("Object type: unknown\n");
}

void PrintTime(const char* title, time_t t)
{
    struct tm* tm_info = localtime(&t);
    char buffer[64];

    strftime(buffer, sizeof(buffer), "%d.%m.%Y %H:%M:%S", tm_info);
    printf("%s: %s\n", title, buffer);
}

void PrintInfo(char* FileName)
{
    struct stat st;

    if (stat(FileName, &st) == -1)
    {
        PrintError("stat failed");
        return;
    }

    printf("File name: %s\n", FileName);

    printf("File size:\n");
    printf("  %ld bytes\n", st.st_size);
    printf("  %.2f KiB\n", st.st_size / 1024.0);
    printf("  %.2f MiB\n", st.st_size / (1024.0 * 1024.0));

    PrintFileType(st.st_mode);

    PrintTime("Metadata change time (ctime)", st.st_ctime);
    PrintTime("Last access time (atime)", st.st_atime);
    PrintTime("Last modification time (mtime)", st.st_mtime);
}

void PrintText(char* FileName)
{
    int fd = open(FileName, O_RDONLY);
    if (fd == -1)
    {
        PrintError("open failed");
        return;
    }

    char buffer[1024];
    ssize_t bytes;

    printf("\n===== FILE CONTENT =====\n");

    while ((bytes = read(fd, buffer, sizeof(buffer))) > 0)
    {
        write(STDOUT_FILENO, buffer, bytes);
    }

    if (bytes == -1)
        PrintError("read failed");

    printf("\n===== END OF FILE =====\n");

    close(fd);
}

int main(int argc, char* argv[])
{
    if (argc != 2)
    {
        printf("Usage: ./lab09a <file_path>\n");
        return 1;
    }

    PrintInfo(argv[1]);
    PrintText(argv[1]);

    return 0;
}
