#include <iostream>
#include <vector>
#include <string>
#include <cctype>
#include <cstdlib>
#include <unistd.h>
#include <semaphore.h>
#include <fcntl.h>
#include <sys/wait.h>
#include <sys/types.h>
#include <errno.h>

static constexpr const char* SEM_NAME = "/lab06_sem";

std::vector<char> extract_letters() {
    const char* name = std::getenv("USER");
    if (!name) name = "Unknown";
    std::vector<char> letters;
    for (size_t i = 0; name[i]; ++i)
        if (std::isalpha(static_cast<unsigned char>(name[i])))
            letters.push_back(name[i]);
    if (letters.empty()) letters.push_back('X');
    return letters;
}

int run_loop(const char* procName) {
    auto letters = extract_letters();
    int L = (int)letters.size();

    sem_t* sem = sem_open(SEM_NAME, O_CREAT, 0666, 1);
    if (sem == SEM_FAILED) {
        std::perror("sem_open");
        return 1;
    }

    for (int i = 1; i <= 90; ++i) {
        if (i == 30) {
            if (sem_wait(sem) == -1) {
                std::perror("sem_wait");
                sem_close(sem);
                return 1;
            }
        }

        char c = letters[(i - 1) % L];
        std::cout << "[" << procName << "] Iteration " << i
            << " Char: " << c << std::endl;
        std::cout.flush();

        if (i == 60) {
            if (sem_post(sem) == -1) {
                std::perror("sem_post");
                sem_close(sem);
                return 1;
            }
        }

        usleep(100000); 
    }

    sem_close(sem);
    return 0;
}

[[noreturn]] void exec_terminal(const std::vector<std::string>& argv_vec) {
    std::vector<char*> argv;
    argv.reserve(argv_vec.size() + 1);
    for (const auto& s : argv_vec) argv.push_back(const_cast<char*>(s.c_str()));
    argv.push_back(nullptr);
    execvp(argv[0], argv.data());
    std::perror("execvp");
    _exit(127);
}

pid_t launch_in_terminal(const char* procName) {

    auto exists_in_path = [](const char* name)->bool {
        const char* path = std::getenv("PATH");
        if (!path) return false;
        std::string p(path);
        size_t start = 0;
        while (true) {
            size_t pos = p.find(':', start);
            std::string dir = (pos == std::string::npos) ? p.substr(start) : p.substr(start, pos - start);
            if (!dir.empty()) {
                std::string full = dir + "/" + name;
                if (access(full.c_str(), X_OK) == 0) return true;
            }
            if (pos == std::string::npos) break;
            start = pos + 1;
        }
        return false;
        };

    pid_t pid = fork();
    if (pid < 0) {
        std::perror("fork");
        return -1;
    }
    if (pid == 0) {
        std::string cmd = "./Lab-06b ";
        cmd += procName;

        if (exists_in_path("gnome-terminal")) {
            std::vector<std::string> args = {
                "gnome-terminal",
                "--",
                "bash", "-lc",
                cmd + "; exec bash"
            };
            exec_terminal(args);
        }
        else if (exists_in_path("konsole")) {
            std::vector<std::string> args = {
                "konsole",
                "-e",
                "bash", "-lc",
                cmd + "; exec bash"
            };
            exec_terminal(args);
        }
        else if (exists_in_path("xterm")) {
            std::vector<std::string> args = {
                "xterm",
                "-hold",
                "-e",
                "./Lab-06b",
                std::string(procName)
            };
            exec_terminal(args);
        }
        else {
            std::cerr << "No supported terminal found (gnome-terminal/konsole/xterm)\n";
            _exit(2);
        }
    }

    return pid;
}

int main(int argc, char* argv[]) {
    if (argc > 1) {
        return run_loop(argv[1]);
    }

    sem_unlink(SEM_NAME);
    sem_t* sem = sem_open(SEM_NAME, O_CREAT | O_EXCL, 0666, 1);
    if (sem == SEM_FAILED) {
        sem = sem_open(SEM_NAME, 0);
        if (sem == SEM_FAILED) {
            std::perror("sem_open");
            return 1;
        }
    }
    else {
        sem_close(sem); 
    }

    pid_t pidA = launch_in_terminal("A");
    if (pidA <= 0) {
        std::cerr << "Failed to launch terminal for A\n";
    }
    else {
        usleep(200000);
    }

    pid_t pidB = launch_in_terminal("B");
    if (pidB <= 0) {
        std::cerr << "Failed to launch terminal for B\n";
    }
    else {
        usleep(200000);
    }

    run_loop("MAIN");

    if (pidA > 0) {
        int status;
        waitpid(pidA, &status, 0);
    }
    if (pidB > 0) {
        int status;
        waitpid(pidB, &status, 0);
    }

    sem_unlink(SEM_NAME);
    return 0;
}
