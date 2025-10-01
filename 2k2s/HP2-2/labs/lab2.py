from collections import Counter
from datetime import datetime, timedelta
import re

def ex_1():
    text = input("Введите текст: ")
    words = re.findall(r'\b\w+\b', text.lower())
    print(words)
    print(dict(Counter(words)))

def ex_2():
    logs = [
        ("192.168.1.1", "200 OK", 1543),
        ("192.168.1.2", "404 Not Found", 234),
        ("192.168.1.1", "500 Internal Server Error", 542),
        ("192.168.1.3", "200 OK", 876),
        ("192.168.1.2", "200 OK", 1324),
    ]
    # подсчет количества запросов от каждого IP
    ip_count = dict(Counter(ip for ip, _, _ in logs))
    # подсчет количества каждого HTTP-статуса
    status_count = Counter(status for _, status, _ in logs)
    # определение самого частого HTTP-статуса
    most_common_status = status_count.most_common(1)[0]
    # подсчет общего объема переданных данных
    total_data = sum(data for _, _, data in logs)

    # Вывод результатов
    print("Количество запросов от каждого IP:", ip_count)
    print("Самый частый HTTP-статус:", most_common_status)
    print("Общий объем переданных данных:", total_data)


def ex_3():
    service_a = {"Анна", "Иван", "Мария", "Сергей", "Алексей"}
    service_b = {"Мария", "Иван", "Дмитрий", "Ольга", "Светлана"}
    service_c = {"Сергей", "Ольга", "Александр", "Иван", "Анна"}

    # пользователи, зарегистрированные во всех трех сервисах
    all_three = service_a & service_b & service_c
    # пользователи, присутствующие только в одном из сервисов
    only_a = service_a - (service_b | service_c)
    only_b = service_b - (service_a | service_c)
    only_c = service_c - (service_a | service_b)
    unique_users = only_a | only_b | only_c
    # определение сервиса с самой большой уникальной базой пользователей
    unique_a = service_a - (service_b | service_c)
    unique_b = service_b - (service_a | service_c)
    unique_c = service_c - (service_a | service_b)
    max_unique_service = max(
        ("Service A", len(unique_a)),
        ("Service B", len(unique_b)),
        ("Service C", len(unique_c)),
        key=lambda x: x[1]
    )

    # Вывод результатов
    print("Пользователи, зарегистрированные во всех трех сервисах:", all_three)
    print("Пользователи, присутствующие только в одном из сервисов:", unique_users)
    print("Сервис с самой большой уникальной базой пользователей:", max_unique_service[0])

def ex_4():
    tasks = {
        "Купить продукты": "2024-02-10",
        "Сдать отчет": "2025-02-15",
        "Прочитать книгу": "2024-12-01",
        "Записаться к врачу": "2025-02-20",
        "Закончить проект": "2025-02-17",
        "Позвонить клиенту": "2025-02-19",
        "Оплатить счета": "2025-02-12",
        "Подготовить презентацию": "2025-02-14",
        "Отправить посылку": "2024-11-30",
        "Разобрать почту": "2025-02-18"
    }
    today = datetime.today().date()

    # найти задачи с истекшим дедлайном
    expired_tasks = [task for task, deadline in tasks.items() if datetime.strptime(deadline, "%Y-%m-%d").date() < today]
    # найти задачи, до которых осталось меньше 3 дней
    upcoming_tasks = [task for task, deadline in tasks.items() if 0 <= (datetime.strptime(deadline, "%Y-%m-%d").date() - today).days < 3]
    # функция для добавления новой задачи
    def add_task():
        new_task = input("Введите название новой задачи: ")
        if new_task in tasks:
            print("Ошибка: такая задача уже существует!")
            return
        
        new_deadline = input("Введите дедлайн (в формате ГГГГ-ММ-ДД): ")
        try:
            deadline_date = datetime.strptime(new_deadline, "%Y-%m-%d").date()
            if deadline_date < today:
                print("Ошибка: дедлайн не может быть в прошлом!")
                return
            tasks[new_task] = new_deadline
            print(f"Задача '{new_task}' добавлена с дедлайном {new_deadline}")
        except ValueError:
            print("Ошибка: неверный формат даты!")

    # Вывод результатов
    print("\nЗадачи с истекшим дедлайном:")
    print(expired_tasks if expired_tasks else "Нет просроченных задач")
    print("\nЗадачи, до дедлайна которых < 3 дней:")
    print(upcoming_tasks if upcoming_tasks else "Нет срочных задач")

    add_task()


def main():
    ex_1()
    ex_2()
    ex_3()
    ex_4()


if __name__ == "__main__":
    main()
