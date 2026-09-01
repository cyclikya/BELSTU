import sympy as sp
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

# 1. SymPy

x = sp.Symbol('x')
f = x**2 + 1

derivative = sp.diff(f, x)
integral = sp.integrate(f, (x, 0, 1))
limit = sp.limit(1 / x**2 + 1, x, sp.oo)

print("Производная функции x^2 + 1:", derivative)
print("Интеграл функции x^2 + 1 на [0, 1]:", integral)
print("Предел функции 1/x^2 + 1 при x -> бесконечность:", limit)


# 2. NumPy

array = np.random.randint(0, 10, 20)
print("\nОдномерный массив:")
print(array)

matrix = array.reshape(4, 5)
print("\nДвумерный массив 4x5:")
print(matrix)

first_array, second_array = np.array_split(matrix, 2)
print("\nПервый массив:")
print(first_array)

print("\nВторой массив:")
print(second_array)

value = 6
found = first_array[first_array == value]
count = len(found)

print("\nЭлементы, равные 6, в первом массиве:")
print(found)
print("Количество найденных элементов:", count)

print("\nМинимум во втором массиве:", second_array.min())
print("Максимум во втором массиве:", second_array.max())
print("Среднее во втором массиве:", second_array.mean())


# 3. Pandas

series_from_array = pd.Series(array)
print("\nSeries из массива NumPy:")
print(series_from_array)

dictionary = {
    "a": 10,
    "b": 20,
    "c": 30
}

series_from_dict = pd.Series(dictionary)
print("\nSeries из словаря:")
print(series_from_dict)

print("\nSeries + 5:")
print(series_from_dict + 5)

print("\nSeries * 2:")
print(series_from_dict * 2)

dataframe_from_array = pd.DataFrame(matrix)
print("\nDataFrame из массива NumPy:")
print(dataframe_from_array)

dataframe_from_dict = pd.DataFrame({
    "Имя": ["Анна", "Иван", "Мария"],
    "Возраст": [18, 19, 20],
    "Оценка": [9, 8, 10]
})

print("\nDataFrame из словаря:")
print(dataframe_from_dict)

dataframe_from_series = pd.DataFrame(series_from_dict, columns=["Значение"])
print("\nDataFrame из Series:")
print(dataframe_from_series)


# 4. Matplotlib

x_values = np.linspace(-10, 10, 100)
y_values = x_values**2 + 1

plt.figure()
plt.plot(x_values, y_values)
plt.title("График функции f(x) = x^2 + 1")
plt.xlabel("x")
plt.ylabel("f(x)")
plt.grid()
plt.show()


# 5. График поверхности

fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')

x_surface = np.linspace(-5, 5, 50)
y_surface = np.linspace(-5, 5, 50)

X, Y = np.meshgrid(x_surface, y_surface)
Z = X**2 + 2 * Y**2 + 1

ax.plot_surface(X, Y, Z)
ax.set_title("Поверхность f(x, y) = x^2 + 2y^2 + 1")
ax.set_xlabel("x")
ax.set_ylabel("y")
ax.set_zlabel("f(x, y)")

plt.show()


# 6. Диаграммы

names = ["A", "B", "C", "D"]
values = [5, 7, 3, 9]

plt.figure()
plt.bar(names, values)
plt.title("Столбчатая диаграмма")
plt.show()

plt.figure()
plt.pie(values, labels=names)
plt.title("Круговая диаграмма")
plt.show()

plt.figure()
plt.scatter(names, values)
plt.title("Точечная диаграмма")
plt.show()