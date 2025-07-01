// - main  
// -- вычисления длины LCS 
#include <iostream>
#include "LCS.h"
int _tmain(int argc, char* argv[])
{
    setlocale(LC_ALL, "rus");
    char X[] = "TOUEXAZ", Y[] = "HIEHXZ";
    std::cout << std::endl << "-- вычисление длины LCS для X и Y(рекурсия)";
    std::cout << std::endl << "-- последовательность X: " << X;
    std::cout << std::endl << "-- последовательность Y: " << Y;
    int s = lcs(
        sizeof(X) - 1,  // длина   последовательности  X   
        "TOUEXAZ",       // последовательность X
        sizeof(Y) - 1,  // длина   последовательности  Y
        "HIEHXZ"       // последовательность Y
    );
    std::cout << std::endl << "-- длина LCS: " << s << std::endl;
    system("pause");
    return 0;
}
