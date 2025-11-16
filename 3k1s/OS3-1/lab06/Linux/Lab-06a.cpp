// Lab-06a.cpp (Linux)
// Компиляция: g++ Lab-06a.cpp -lpthread -o Lab-06a

#include <iostream>
#include <vector>
#include <string>
#include <cctype>
#include <cstdlib>
#include <pthread.h>
#include <unistd.h>     // usleep

pthread_mutex_t mutex1;

// Получение букв имени пользователя
std::vector<char> extract_letters_from_username() {
    const char* name = std::getenv("USER");
    if (!name) name = "Unknown";

    std::vector<char> letters;
    for (int i = 0; name[i]; i++) {
        if (std::isalpha((unsigned char)name[i]))
            letters.push_back(name[i]);
    }
    if (letters.empty()) letters.push_back('X');
    return letters;
}

struct ThreadParam {
    const char* name;
    std::vector<char>* letters;
};

void* thread_func(void* arg) {
    ThreadParam* p = (ThreadParam*)arg;
    const char* tname = p->name;
    auto& letters = *p->letters;
    int L = letters.size();

    for (int i = 1; i <= 90; i++) {

        if (i == 30)
            pthread_mutex_lock(&mutex1);

        char ch = letters[(i - 1) % L];
        std::cout << "[" << tname << "] Iteration "
            << i << " Char: " << ch << std::endl;

        if (i == 60)
            pthread_mutex_unlock(&mutex1);

        usleep(100000); // 100 ms
    }

    return nullptr;
}

int main() {
    pthread_mutex_init(&mutex1, nullptr);

    auto letters = extract_letters_from_username();

    ThreadParam A{ "Thread-A", &letters };
    ThreadParam B{ "Thread-B", &letters };

    pthread_t tA, tB;

    // Запуск потоков A и B
    pthread_create(&tA, nullptr, thread_func, &A);
    pthread_create(&tB, nullptr, thread_func, &B);

    int L = letters.size();

    // Главный поток
    for (int i = 1; i <= 90; i++) {

        if (i == 30)
            pthread_mutex_lock(&mutex1);

        char ch = letters[(i - 1) % L];
        std::cout << "[Main] Iteration "
            << i << " Char: " << ch << std::endl;

        if (i == 60)
            pthread_mutex_unlock(&mutex1);

        usleep(100000); // 100 ms
    }

    // Ожидание потоков
    pthread_join(tA, nullptr);
    pthread_join(tB, nullptr);

    pthread_mutex_destroy(&mutex1);
    return 0;
}
