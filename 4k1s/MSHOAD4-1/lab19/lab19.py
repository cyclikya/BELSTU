import mglearn
import matplotlib.pyplot as plt
import numpy as np
from mglearn.datasets import load_boston
from sklearn.datasets import load_breast_cancer
from sklearn.model_selection import train_test_split
from sklearn.neighbors import KNeighborsClassifier
from sklearn.neighbors import KNeighborsRegressor

# Синтетический набор forge для задачи бинарной классификации
X, y = mglearn.datasets.make_forge()
mglearn.discrete_scatter(X[:, 0], X[:, 1], y)
plt.legend(["Класс 0", "Класс 1"], loc=4)
plt.xlabel("Первый признак")
plt.ylabel("Второй признак")
print("форма массива X: {}".format(X.shape))
plt.show()

# Синтетический набор wave для задачи регрессии
X, y = mglearn.datasets.make_wave(n_samples=40)
plt.plot(X, y, 'o')
plt.ylim(-3, 3)
plt.xlabel("Признак")
plt.ylabel("Целевая переменная")
plt.show()

# Реальный набор данных по раку молочной железы
cancer = load_breast_cancer()
print("Ключи cancer(): \n{}".format(cancer.keys()))
print("Форма массива data для набора cancer: {}".format(cancer.data.shape))
print("Количество примеров для каждого класса:\n{}".format(
    {n: v for n, v in zip(cancer.target_names, np.bincount(cancer.target))}
))
print("Имена признаков:\n{}".format(cancer.feature_names))

# Реальный набор данных Boston Housing для задачи регрессии
boston = load_boston()
print("форма массива data для набора boston: {}".format(boston.data.shape))

# Тот же набор с производными признаками (конструирование признаков)
X, y = mglearn.datasets.load_extended_boston()
print("форма массива X: {}".format(X.shape))

# Прогнозы модели одного ближайшего соседа
mglearn.plots.plot_knn_classification(n_neighbors=1)
plt.show()

# Прогнозы модели трёх ближайших соседей (голосование)
mglearn.plots.plot_knn_classification(n_neighbors=3)
plt.show()

# Классификация набора forge методом k ближайших соседей
X, y = mglearn.datasets.make_forge()
X_train, X_test, y_train, y_test = train_test_split(X, y, random_state=0)

clf = KNeighborsClassifier(n_neighbors=3)
clf.fit(X_train, y_train)

print("Прогнозы на тестовом наборе: {}".format(clf.predict(X_test)))
print("Правильность на тестовом наборе: {:.2f}".format(clf.score(X_test, y_test)))

# Границы принятия решений для 1, 3 и 9 соседей
fig, axes = plt.subplots(1, 3, figsize=(10, 3))

for n_neighbors, ax in zip([1, 3, 9], axes):
    # создаем объект-классификатор и подгоняем в одной строке
    clf = KNeighborsClassifier(n_neighbors=n_neighbors).fit(X, y)
    mglearn.plots.plot_2d_separator(clf, X, fill=True, eps=0.5, ax=ax, alpha=.4)
    mglearn.discrete_scatter(X[:, 0], X[:, 1], y, ax=ax)
    ax.set_title("количество соседей:{}".format(n_neighbors))
    ax.set_xlabel("признак 0")
    ax.set_ylabel("признак 1")

axes[0].legend(loc=3)
plt.show()

# Зависимость правильности от количества соседей на наборе cancer
X_train, X_test, y_train, y_test = train_test_split(
    cancer.data, cancer.target, stratify=cancer.target, random_state=66
)

training_accuracy = []
test_accuracy = []

# пробуем n_neighbors от 1 до 10
neighbors_settings = range(1, 11)

for n_neighbors in neighbors_settings:
    # строим модель
    clf = KNeighborsClassifier(n_neighbors=n_neighbors)
    clf.fit(X_train, y_train)
    # записываем правильность на обучающем наборе
    training_accuracy.append(clf.score(X_train, y_train))
    # записываем правильность на тестовом наборе
    test_accuracy.append(clf.score(X_test, y_test))

plt.plot(neighbors_settings, training_accuracy, label="правильность на обучающем наборе")
plt.plot(neighbors_settings, test_accuracy, label="правильность на тестовом наборе")
plt.ylabel("Правильность")
plt.xlabel("количество соседей")
plt.legend()
plt.show()

# Регрессия одного ближайшего соседа на наборе wave
mglearn.plots.plot_knn_regression(n_neighbors=1)
plt.show()

# Регрессия трёх ближайших соседей (усреднение ответов)
mglearn.plots.plot_knn_regression(n_neighbors=3)
plt.show()

# Регрессия k ближайших соседей на наборе wave
X, y = mglearn.datasets.make_wave(n_samples=40)
X_train, X_test, y_train, y_test = train_test_split(X, y, random_state=0)

# создаем экземпляр модели и устанавливаем количество соседей равным 3
reg = KNeighborsRegressor(n_neighbors=3)
# подгоняем модель с использованием обучающих данных и обучающих ответов
reg.fit(X_train, y_train)

# получим прогнозы для тестового набора
print("Прогнозы для тестового набора:\n{}".format(reg.predict(X_test)))
print("R^2 на тестовом наборе: {:.2f}".format(reg.score(X_test, y_test)))

# Прогнозы регрессии для 1, 3 и 9 соседей
fig, axes = plt.subplots(1, 3, figsize=(15, 4))
line = np.linspace(-3, 3, 1000).reshape(-1, 1)

for n_neighbors, ax in zip([1, 3, 9], axes):
    reg = KNeighborsRegressor(n_neighbors=n_neighbors)
    reg.fit(X_train, y_train)
    ax.plot(line, reg.predict(line))
    ax.plot(X_train, y_train, '^', c=mglearn.cm2(0), markersize=8)
    ax.plot(X_test, y_test, 'v', c=mglearn.cm2(1), markersize=8)
    ax.set_title(
        "{} neighbor(s)\n train score: {:.2f} test score: {:.2f}".format(
            n_neighbors, reg.score(X_train, y_train), reg.score(X_test, y_test)
        )
    )
    ax.set_xlabel("Признак")
    ax.set_ylabel("Целевая переменная")

axes[0].legend(["Прогнозы модели", "Обучающие данные/ответы", "Тестовые данные/ответы"],
               loc="best")
plt.show()
