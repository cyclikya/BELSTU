import os
import subprocess

def main():
    processes = []

    # Глобальная переменная ITER_NUM
    os.environ["ITER_NUM"] = "4"

    # 1. Первый процесс — только имя файла
    cmd1 = ["python", "Lab-03x.py"]
    processes.append(subprocess.Popen(cmd1))

    # 2. Второй процесс — обычный аргумент
    cmd2 = ["python", "Lab-03x.py", "6"]
    processes.append(subprocess.Popen(cmd2))

    # 3. Третий процесс — локальная переменная окружения
    env = os.environ.copy()
    env["ITER_NUM"] = "8"
    cmd3 = ["python", "Lab-03x.py"]
    processes.append(subprocess.Popen(cmd3, env=env))

    # Ожидание завершения всех процессов
    for p in processes:
        p.wait()

    print("Все дочерние процессы завершены.")

if __name__ == "__main__":
    main()
