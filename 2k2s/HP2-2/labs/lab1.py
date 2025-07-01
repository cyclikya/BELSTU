import random
# 1
x = input("Введите ваше имя: ")
y = input("Введите вашу фамилию: ")
print(f"Ваше имя и фамилия: {x.capitalize()} {y.capitalize()}")
print(f"Ваши инициалы: {x[0].upper()}.{y[0].upper()}.")

# 2
sq = [x**2 for x in range(10, 20)]
print("Список квадратов:", sq)
print("Сумма элементов списка:", sum(sq))

sq = [x for x in sq if x % 2 != 0]
print("Список без четных элементов:", sq)
print("Количество оставшихся элементов:", len(sq))

# 3
n = int(input("Введите количество случайных чисел: "))
A = [random.randint(1, 100) for _ in range(n)]
print("Список A:", A)

B = [sum(A[:i+1]) for i in range(n)]
print("Список B:", B)