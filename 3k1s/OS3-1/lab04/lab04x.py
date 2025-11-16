import os
import threading
import time
import sys
import random
from datetime import datetime
import getpass

def Lab_04x(iterations):
    """
    Выполняет N итераций и выводит:
    PID – TID – №Итерации – Буква из имени пользователя.
    Между итерациями задержка 300–400 мс.
    Добавлен вывод начала и окончания процесса по PID – TID – с меткой времени.
    """
    pid = os.getpid()
    tid = threading.get_ident()
    thread_name = threading.current_thread().name
    
    # Получаем имя пользователя из системы
    username = getpass.getuser()

    try:
        if not isinstance(iterations, int):
            raise TypeError("Параметр iterations должен быть целым числом.")
        if iterations <= 0:
            raise ValueError("Количество итераций должно быть положительным числом.")

        print(f"[{datetime.now().strftime('%H:%M:%S.%f')[:-3]}] "
              f"PID={pid:<6}  TID={tid:<6}  ({thread_name}) ---> НАЧАЛО выполнения")
        print(f"Пользователь системы: {username}")

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