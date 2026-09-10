import sympy as sp
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D

x = sp.Symbol('x')
f = x**2 + 1

# 2.1 производная
derivative = sp.diff(f, x)

# 2.2 определённый интеграл на отрезке [0, 1]
integral = sp.integrate(f, (x, 0, 1))

# 2.3 предел при x -> бесконечность
limit_1 = sp.limit(1 / x**2 + 1, x, sp.oo)      # 1/x^2 + 1   -> 1

print("Производная функции x^2 + 1:", derivative)
print("Интеграл функции x^2 + 1 на [0, 1]:", integral)
print("Предел 1/x^2 + 1 при x -> oo:", limit_1)


# 3.1 одномерный массив из 20 случайных целых чисел от 0 до 9
array = np.random.randint(0, 10, 20)
print("\nОдномерный массив:")
print(array)

# 3.2 преобразование в двумерный массив 4x5
matrix = array.reshape(4, 5)
print("\nДвумерный массив 4x5:")
print(matrix)

# 3.3 деление на 2 массива (по строкам: 2x5 и 2x5)
first_array, second_array = np.array_split(matrix, 2)
print("\nПервый массив:")
print(first_array)
print("\nВторой массив:")
print(second_array)

# 3.4 поиск всех заданных значений в первом массиве
value = 6
mask = (first_array == value)          # булева маска True/False того же размера
found = first_array[mask]              # сами значения
positions = np.argwhere(mask)          # координаты [строка, столбец]

print(f"\nЭлементы, равные {value}, в первом массиве:", found)
print("Их позиции [строка, столбец]:")
print(positions)

# 3.5 количество найденных элементов
count = np.count_nonzero(mask)
print("Количество найденных элементов:", count)

# 3.6 мин, макс, среднее во втором массиве
print("\nМинимум во втором массиве:", second_array.min())
print("Максимум во втором массиве:", second_array.max())
print("Среднее во втором массиве:", second_array.mean())


# 4.2 Series из массива NumPy
series_from_array = pd.Series(array)
print("\nSeries из массива NumPy:")
print(series_from_array)

# 4.2 Series из словаря (ключи становятся индексом)
dictionary = {"a": 10, "b": 20, "c": 30}
series_from_dict = pd.Series(dictionary)
print("\nSeries из словаря:")
print(series_from_dict)

# 4.3 математические операции над Series
print("\nSeries + 5:")
print(series_from_dict + 5)

print("\nSeries * 2:")
print(series_from_dict * 2)

print("\nSeries ** 2:")
print(series_from_dict ** 2)

print("\nSeries / 10:")
print(series_from_dict / 10)

# операция между двумя Series — сложение идёт по совпадающим индексам
other_series = pd.Series({"a": 1, "b": 2, "d": 100})
print("\nСложение двух Series (по индексам, 'c' и 'd' дают NaN):")
print(series_from_dict + other_series)

# агрегатные функции и сводная статистика
print("\nСумма:", series_from_dict.sum())
print("Среднее:", series_from_dict.mean())
print("\nОписательная статистика (describe):")
print(series_from_dict.describe())

# 4.4 DataFrame из массива NumPy
dataframe_from_array = pd.DataFrame(matrix, columns=["c1", "c2", "c3", "c4", "c5"])
print("\nDataFrame из массива NumPy:")
print(dataframe_from_array)

# 4.4 DataFrame из словаря
dataframe_from_dict = pd.DataFrame({
    "Имя": ["Анна", "Иван", "Мария"],
    "Возраст": [18, 19, 20],
    "Оценка": [9, 8, 10]
})
print("\nDataFrame из словаря:")
print(dataframe_from_dict)

# 4.4 DataFrame из объекта Series
dataframe_from_series = pd.DataFrame(series_from_dict, columns=["Значение"])
print("\nDataFrame из Series:")
print(dataframe_from_series)


# 5.1 график функции f(x) = x^2 + 1
x_values = np.linspace(-10, 10, 100)
y_values = x_values**2 + 1

plt.figure()
plt.plot(x_values, y_values, color="blue")
plt.title("График функции f(x) = x^2 + 1")
plt.xlabel("x")
plt.ylabel("f(x)")
plt.grid()
plt.show()

# 5.2 график поверхности f(x, y) = x^2 + 2y^2 + 1
fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')

x_surface = np.linspace(-5, 5, 50)
y_surface = np.linspace(-5, 5, 50)
X, Y = np.meshgrid(x_surface, y_surface)   # сетка координат
Z = X**2 + 2 * Y**2 + 1                    # значение функции в каждом узле сетки

surface = ax.plot_surface(X, Y, Z, cmap="viridis")
fig.colorbar(surface)
ax.set_title("Поверхность f(x, y) = x^2 + 2y^2 + 1")
ax.set_xlabel("x")
ax.set_ylabel("y")
ax.set_zlabel("f(x, y)")
plt.show()

# 5.3 несколько видов диаграмм
names = ["A", "B", "C", "D"]
values = [5, 7, 3, 9]

plt.figure()
plt.bar(names, values)
plt.title("Столбчатая диаграмма")
plt.show()

plt.figure()
plt.pie(values, labels=names, autopct="%1.1f%%")
plt.title("Круговая диаграмма")
plt.show()

# точечная диаграмма строится по числовым данным
plt.figure()
plt.scatter(array, np.arange(len(array)))
plt.title("Точечная диаграмма")
plt.xlabel("значение элемента массива")
plt.ylabel("индекс элемента")
plt.grid()
plt.show()