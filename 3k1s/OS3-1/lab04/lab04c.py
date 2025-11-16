from lab04x import Lab_04x
import threading
import time

stop_thread_2 = threading.Event()

def lab_thread_1():
    Lab_04x(50)

def lab_thread_2():
    for i in range(1, 126):
        if stop_thread_2.is_set():
            print("[Thread-2] Завершён досрочно.")
            return
        Lab_04x(1)

def main():
    t1 = threading.Thread(target=lab_thread_1)
    t2 = threading.Thread(target=lab_thread_2)

    t1.start()
    t2.start()

    for i in range(1, 101):
        print(f"[Main] Итерация {i}")
        time.sleep(0.35)
        if i == 40:
            stop_thread_2.set()
            print("[Main] Завершение потока 2")

    t1.join()
    t2.join()
    print("\nВсе потоки завершены.")

if __name__ == "__main__":
    main()
