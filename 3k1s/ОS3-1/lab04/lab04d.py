import threading
import os

def main():
    print(f"PID процесса: {os.getpid()}")
    threads = threading.enumerate()
    for t in threads:
        print(f"Поток: {t.name}, ID: {t.ident}")

if __name__ == "__main__":
    main()
