import os
import subprocess

def main():
    processes = []

    # 1. Передача аргумента в командной строке
    cmd1 = ["python", "Lab-03x.py", "5"]
    processes.append(subprocess.Popen(cmd1))

    # 2. Передача имени и аргумента как одной строки (аналог второго параметра)
    cmd2 = "python Lab-03x.py 7"
    processes.append(subprocess.Popen(cmd2, shell=True))

    # 3. Использование переменной окружения
    env = os.environ.copy()
    env["ITER_NUM"] = "9"
    cmd3 = ["python", "Lab-03x.py"]
    processes.append(subprocess.Popen(cmd3, env=env))

    # Ожидание завершения всех процессов
    for p in processes:
        p.wait()

    print("Все дочерние процессы завершены.")

if __name__ == "__main__":
    main()
