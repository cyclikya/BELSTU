import time
import functools
import logging
from memory_profiler import memory_usage

# 1. Декоратор @timer для замера времени выполнения функции
def timer(func):
    @functools.wraps(func)
    def wrapper_timer(*args, **kwargs):
        start_time = time.perf_counter()
        result = func(*args, **kwargs)
        end_time = time.perf_counter()
        elapsed_time = end_time - start_time
        print(f"[TIMER] Функция '{func.__name__}' выполнена за {elapsed_time:.6f} секунд")
        return result
    return wrapper_timer

# 2. Декоратор @log для логирования вызовов функций в файл
def log(func):
    logger = logging.getLogger(func.__name__)
    logger.setLevel(logging.INFO)
    # Создаем обработчик записи в файл
    fh = logging.FileHandler('function_calls.log')
    fh.setLevel(logging.INFO) 
    formatter = logging.Formatter('%(asctime)s - %(message)s')
    fh.setFormatter(formatter)
    # Добавляем обработчик к логгеру
    if not logger.hasHandlers():
        logger.addHandler(fh)

    @functools.wraps(func)
    def wrapper_log(*args, **kwargs):
        result = func(*args, **kwargs)
        logger.info(f"Вызов: {func.__name__}("
                    f"args={args}, kwargs={kwargs}) -> {result}")
        print(f"[LOG] Функция '{func.__name__}' записала лог в function_calls.log")
        return result
    return wrapper_log

# 3. Декоратор @memory_usage для замера потребления памяти функцией
def memory_usage_decorator(func):
    @functools.wraps(func)
    def wrapper_memory(*args, **kwargs):
        mem_before = memory_usage(-1, interval=0.1, timeout=1)
        result = func(*args, **kwargs)
        mem_after = memory_usage(-1, interval=0.1, timeout=1)
        mem_diff = max(mem_after) - max(mem_before)
        print(f"[MEMORY] Функция '{func.__name__}' использовала примерно {mem_diff:.2f} MiB памяти")
        return result
    return wrapper_memory

# Пример использования всех трех декораторов
@timer
@log
@memory_usage_decorator
def some_function(n):
    return sum(i * i for i in range(n))

if __name__ == "__main__":
    some_function(10**6)
