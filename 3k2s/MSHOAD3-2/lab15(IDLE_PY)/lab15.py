# Лабораторная работа №15
# Демонстрация структур данных Python

# Список
numbers = [1, 2, 3, 4, 5]
numbers.append(6)
print("Список:", numbers)
print("Первый элемент списка:", numbers[0])

# Кортеж
student = ("Анна", 18, "Python")
print("Кортеж:", student)
print("Имя студента:", student[0])

# Словарь
person = {
    "name": "Анна",
    "age": 18,
    "course": "Python"
}

print("Словарь:", person)
print("Имя:", person["name"])

# Изменение значения в словаре
person["age"] = 19
print("Измененный словарь:", person)
