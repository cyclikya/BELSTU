// --- main 
#include <iostream>
#include <iomanip> 
#include "Salesman.h"
#define N 5

int main()
{
    setlocale(LC_ALL, "rus");
    int d[N][N] = {      
                    { INF, 22, 32, INF, 11 },
                    { 11, INF, 26, 57, 73 },
                    { 13, 33, INF, 86, 60 },
                    { 28, 47, 44, INF, 33 },
                    { 82, 77, 52, 24, INF } }; 
    int r[N];                  
    int s = salesman(
        N,          // [in]  количество городов 
        (int*)d,          // [in]  массив [n*n] расстояний 
        r           // [out] массив [n] маршрут 0 x x x x  
    );

    std::cout << std::endl << "-- Задача коммивояжера -- ";
    std::cout << std::endl << "-- количество  городов: " << N;
    std::cout << std::endl << "-- матрица расстояний : ";
    for (int i = 0; i < N; i++)
    {
        std::cout << std::endl;
        for (int j = 0; j < N; j++)
            if (d[i][j] != INF) std::cout << std::setw(3) << d[i][j] << " ";
            else std::cout << std::setw(3) << "INF" << " ";
    }
    std::cout << std::endl << "-- оптимальный маршрут: ";
    for (int i = 0; i < N; i++) std::cout << r[i] + 1  << "-->"; std::cout << 1;
    std::cout << std::endl << "-- длина маршрута     : " << s;
    std::cout << std::endl;
    system("pause");
    return 0;
}