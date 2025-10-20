import threading
import os
import time
import getpass
import sys
import random

def Lab_04x(iterations):
    try:
        if not isinstance(iterations, int):
            raise TypeError("Параметр iterations должен быть целым числом.")
        if iterations <= 0:
            raise ValueError("Количество итераций должно быть положительным числом.")
    except Exception as e:
        print(f"[ОШИБКА в Lab_04x] неверный аргумент iterations: {e}")
        return
    
    username = getpass.getuser()
    if not username:
        print("[ОШИБКА в Lab_04x] Не удалось определить имя пользователя из системы.")
        return
        
    def worker_thread():
        try:
            for i in range(iterations):
                letter_index = i % len(username)
                current_letter = username[letter_index]
                
                print(f"{threading.get_native_id()} – {threading.get_ident()} - {i + 1} – {current_letter}")
                sys.stdout.flush()
                delay = random.uniform(0.3, 0.4)
                time.sleep(delay)      
        except Exception as e:
            print(f"Ошибка в потоке: {e}")
            
    thread = None
    try:
        thread = threading.Thread(target=worker_thread)
        thread.start()
        thread.join()     
    except Exception as e:
        print(f"Ошибка при работе с потоком: {e}")   
    finally:
        if thread and thread.is_alive():
            thread.join(timeout=1.0)


if __name__ == "__main__":
    print("Запуск Lab_04x с iterations")
    try:
        if len(sys.argv) > 1:
            Lab_04x(int(sys.argv[1]))
        else:
            Lab_04x(None) 
    except Exception as e:
        print(f"[ОШИБКА в main]: {e}")
    else:
        print("main: вызов Lab_04x завершён.")
