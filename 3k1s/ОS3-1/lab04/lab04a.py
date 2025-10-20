import threading
import os
import time
import getpass
import sys
import random


def Lab_04х(iterations):
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



def main():
    # Создаем дочерние потоки
        print("Создание дочерних потоков...")
        thread1 = Lab_04х(50)
        thread2 = Lab_04х(125)
        
        if thread1: threads.append(thread1)
        if thread2: threads.append(thread2)
        
        # ГЛАВНЫЙ ПОТОК выполняет свою работу напрямую
        print("\n" + "=" * 50)
        print("ГЛАВНЫЙ ПОТОК начинает работу (100 итераций)")
        print("=" * 50)
        
        # Главный поток выполняет работу САМ, а не в отдельном потоке
        username = getpass.getuser()
        if not username:
            print("[ОШИБКА в Main] Не удалось определить имя пользователя из системы.")
            return
        for i in range(100):
            letter_index = i % len(username)
            current_letter = username[letter_index]
            
            print(f"{os.getpid()} – {threading.get_ident()} - {i + 1} – {current_letter}")
            sys.stdout.flush()
            
            # Параллельно проверяем состояние дочерних потоков
            if i % 10 == 0:  # Каждые 10 итераций проверяем
                alive_threads = [t for t in threads if t and t.is_alive()]
                print(f"[Проверка] Дочерних потоков активно: {len(alive_threads)}")
            
            delay = random.uniform(0.3, 0.4)
            time.sleep(delay)



if __name__ == "__main__":
    main()
