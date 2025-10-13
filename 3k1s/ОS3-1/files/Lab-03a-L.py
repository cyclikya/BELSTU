import os
import subprocess

def main():
    processes = []

    # Первый дочерний процесс — передача через аргумент командной строки
    cmd1 = ["python3", "Lab-03x.py", "5"]
    processes.append(subprocess.Popen(cmd1))

    # Второй дочерний процесс — передача через переменную окружения
    env = os.environ.copy()
    env["ITER_NUM"] = "8"
    cmd2 = ["python3", "Lab-03x.py"]
    processes.append(subprocess.Popen(cmd2, env=env))

    # Ожидание завершения обоих процессов
    for p in processes:
        p.wait()

    print("Оба дочерних процесса завершены.")

if __name__ == "__main__":
    main()
