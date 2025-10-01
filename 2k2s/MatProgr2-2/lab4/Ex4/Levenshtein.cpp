#include <iostream>
#include <iomanip>
#include <algorithm>
#include <vector>
#include <unordered_map>
#include "Levenshtein.h"

#define DD(i,j) d[(i)*(ly+1)+(j)] 

int min3(int x1, int x2, int x3) {
    return std::min({ x1, x2, x3 });
}

// Динамическое программирование
int levenshtein(int lx, const char x[], int ly, const char y[]) {
    std::vector<int> d((lx + 1) * (ly + 1));

    for (int i = 0; i <= lx; i++) DD(i, 0) = i;
    for (int j = 0; j <= ly; j++) DD(0, j) = j;

    for (int i = 1; i <= lx; i++) {
        for (int j = 1; j <= ly; j++) {
            DD(i, j) = min3(
                DD(i - 1, j) + 1,
                DD(i, j - 1) + 1,
                DD(i - 1, j - 1) + (x[i - 1] == y[j - 1] ? 0 : 1)
            );
        }
    }
    return DD(lx, ly);
}

// Итеративная версия вместо глубокой рекурсии
int levenshtein_iterative(int lx, const char x[], int ly, const char y[]) {
    std::vector<std::vector<int>> dp(lx + 1, std::vector<int>(ly + 1, 0));

    for (int i = 0; i <= lx; i++) dp[i][0] = i;
    for (int j = 0; j <= ly; j++) dp[0][j] = j;

    for (int i = 1; i <= lx; i++) {
        for (int j = 1; j <= ly; j++) {
            dp[i][j] = min3(
                dp[i - 1][j] + 1,
                dp[i][j - 1] + 1,
                dp[i - 1][j - 1] + (x[i - 1] == y[j - 1] ? 0 : 1)
            );
        }
    }
    return dp[lx][ly];
}
