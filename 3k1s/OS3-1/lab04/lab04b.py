from lab04x import Lab_04x
import threading
import time
import os

pause_event_1 = threading.Event()
pause_event_2 = threading.Event()
resume_event_1 = threading.Event()
resume_event_2 = threading.Event()

def lab_thread_1():
    for i in range(1, 51):
        if i == 20:
            print("[Thread-1] Приостановлен")
            pause_event_1.set()
            resume_event_1.wait()
            resume_event_1.clear()
            print("[Thread-1] Возобновлён")
        Lab_04x(1)

def lab_thread_2():
    for i in range(1, 126):
        if i == 40:
            print("[Thread-2] Приостановлен")
            pause_event_2.set()
            resume_event_2.wait()
            resume_event_2.clear()
            print("[Thread-2] Возобновлён")
        Lab_04x(1)

def main():
    t1 = threading.Thread(target=lab_thread_1, name="Thread-1")
    t2 = threading.Thread(target=lab_thread_2, name="Thread-2")

    t1.start()
    t2.start()

    # Главный цикл
    for i in range(1, 101):
        print(f"[Main] Итерация {i}")
        time.sleep(0.35)

        if i == 60 and pause_event_1.is_set():
            resume_event_1.set()
        if i == 100 and pause_event_2.is_set():
            resume_event_2.set()

    t1.join()
    t2.join()
    print("\nВсе потоки завершены.")

if __name__ == "__main__":
    main()
