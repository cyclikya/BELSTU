from lab04x import Lab_04x
import threading

def main():
    t1 = threading.Thread(target=Lab_04x, args=(50,), name="Thread-1")
    t2 = threading.Thread(target=Lab_04x, args=(125,), name="Thread-2")

    t1.start()
    t2.start()

    # Главный поток
    Lab_04x(100)

    t1.join()
    t2.join()

    print("\nВсе потоки завершены.")

if __name__ == "__main__":
    main()
