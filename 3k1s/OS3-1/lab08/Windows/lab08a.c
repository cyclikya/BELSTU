#include <stdio.h>
#include <windows.h>

// Глобальные переменные
int g_init = 10;
int g_uninit;

// Глобальные static
static int gs_init = 20;
static int gs_uninit;

// Любая функция
void demo_function() {
    printf("Inside demo_function\n");
}

int main(int argc, char* argv[])
{
    // Локальные
    int l_init = 123;
    int l_uninit;

    // Локальные static
    static int ls_init = 777;
    static int ls_uninit;

    printf("=== FUNCTIONS ===\n");
    printf("demo_function: %p\n", demo_function);
    printf("main:          %p\n", main);

    printf("\n=== GLOBALS ===\n");
    printf("g_init:        %p\n", &g_init);
    printf("g_uninit:      %p\n", &g_uninit);
    printf("gs_init:       %p\n", &gs_init);
    printf("gs_uninit:     %p\n", &gs_uninit);

    printf("\n=== LOCALS ===\n");
    printf("l_init:        %p\n", &l_init);
    printf("l_uninit:      %p\n", &l_uninit);

    printf("\n=== LOCAL STATICS ===\n");
    printf("ls_init:       %p\n", &ls_init);
    printf("ls_uninit:     %p\n", &ls_uninit);

    printf("\n=== ARGS ===\n");
    printf("&argc:         %p\n", &argc);
    printf("argv:          %p\n", argv);
    printf("&argv:         %p\n", &argv);

    printf("\nPress ENTER to exit...");
    getchar();
}
