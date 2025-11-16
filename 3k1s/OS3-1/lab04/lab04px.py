#!/usr/bin/env python3
import os
import threading
import time
import sys
import random
from datetime import datetime

# --- Общая функция (идентична версии для Windows) ---
def Lab_04x(iterations, username="User-d3hc25h"):
    """
    Выполняет N итераций и выводит:
    PID – TID – №Итерации – Буква из имени пользователя.
    Между итерациями задержка 300–400 мс.
    Добавлен вывод начала и окончания процесса по PID – TID – с меткой времени.
    """
    pid = os.getpid()
    tid = threading.get_ident()
    thread_name = threading.current_thread().name

    try:
        if not isinstance(iterations, int):
            raise TypeError("Параметр iterations должен быть целым числом.")
        if iterations <= 0:
            raise ValueError("Количество итераций должно быть положительным числом.")

        print(f"[{datetime.now().strftime('%H:%M:%S.%f')[:-3]}] "
              f"PID={pid:<6}  TID={tid:<6}  ({thread_name}) ---> НАЧАЛО выполнения")

        name_letters = [c for c in username if c.isalnum()]
        n_letters = len(name_letters)

        for i in range(1, iterations + 1):
            letter = name_letters[(i - 1) % n_letters]
            print(f"PID={pid:<6}  TID={tid:<6}  Итерация={i:<3}  Буква={letter}")
            time.sleep(random.uniform(0.3, 0.4))

    except Exception as e:
        print(f"[ОШИБКА в Lab_04x]: {e}", file=sys.stderr)
    finally:
        print(f"[{datetime.now().strftime('%H:%M:%S.%f')[:-3]}] "
              f"PID={pid:<6}  TID={tid:<6}  ({thread_name}) ---> ОКОНЧАНИЕ выполнения")
        sys.stdout.flush()

# --- Основное приложение Lab-04px ---
def main():
    print(f"=== Запуск программы Lab-04px (PID={os.getpid()}) ===")

    t1 = threading.Thread(target=Lab_04x, args=(50,), name="Thread-1")
    t2 = threading.Thread(target=Lab_04x, args=(125,), name="Thread-2")

    t1.start()
    t2.start()

    # Главный поток выполняет свою функцию
    Lab_04x(100)

    # Ожидание завершения дочерних потоков
    t1.join()
    t2.join()

    print("\nВсе потоки завершены.")
    print(f"=== Завершение программы Lab-04px (PID={os.getpid()}) ===")

if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        print("Программа прервана пользователем.")
    except Exception as e:
        print(f"[ОШИБКА]: {e}")
    finally:
        sys.stdout.flush()
