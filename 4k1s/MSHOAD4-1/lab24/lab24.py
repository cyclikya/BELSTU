import os
import mglearn
import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
from sklearn.datasets import load_breast_cancer
from sklearn.linear_model import LinearRegression
from sklearn.model_selection import train_test_split
from sklearn.tree import DecisionTreeClassifier
from sklearn.tree import DecisionTreeRegressor
from sklearn.tree import export_graphviz
from sklearn.tree import plot_tree

X = np.array([[0, 1, 0, 1],
              [1, 0, 1, 1],
              [0, 0, 0, 1],
              [1, 0, 1, 0]])
y = np.array([0, 1, 0, 1])

counts = {}
for label in np.unique(y):
    counts[label] = X[y == label].sum(axis=0)
print("Частоты признаков:\n{}".format(counts))

# Последовательное построение дерева на наборе two_moons:
# каждое следующее разбиение уточняет границу принятия решений
mglearn.plots.plot_tree_progressive()
plt.show()

# Дерево решений на наборе Breast Cancer без ограничений:
# растет, пока все листья не станут чистыми
cancer = load_breast_cancer()
X_train, X_test, y_train, y_test = train_test_split(
    cancer.data, cancer.target, stratify=cancer.target, random_state=42
)

tree = DecisionTreeClassifier(random_state=0)
tree.fit(X_train, y_train)
print("Правильность на обучающем наборе: {:.3f}".format(tree.score(X_train, y_train)))
print("Правильность на тестовом наборе: {:.3f}".format(tree.score(X_test, y_test)))

# Предварительная обрезка: ограничиваем глубину четырьмя вопросами
tree = DecisionTreeClassifier(max_depth=4, random_state=0)
tree.fit(X_train, y_train)
print("Правильность на обучающем наборе: {:.3f}".format(tree.score(X_train, y_train)))
print("Правильность на тестовом наборе: {:.3f}".format(tree.score(X_test, y_test)))

# Описание дерева в текстовом формате .dot
export_graphviz(tree, out_file="tree.dot", class_names=["malignant", "benign"],
                feature_names=cancer.feature_names, impurity=False, filled=True)

# Визуализация дерева средствами scikit-learn (замена graphviz)
plt.figure(figsize=(20, 10))
plot_tree(tree, feature_names=list(cancer.feature_names),
          class_names=["malignant", "benign"], impurity=False, filled=True, fontsize=7)
plt.show()

# Важности признаков: насколько каждый признак участвовал в принятии решений
tree = DecisionTreeClassifier(random_state=0).fit(X_train, y_train)
print("Важности признаков:\n{}".format(tree.feature_importances_))

for name, score in zip(cancer["feature_names"], tree.feature_importances_):
    print(name, score)


def plot_feature_cancer(model):
    n_features = cancer.data.shape[1]
    plt.barh(range(n_features), model.feature_importances_, align='center')
    plt.yticks(np.arange(n_features), cancer.feature_names)
    plt.xlabel("Важность признака")
    plt.ylabel("Признак")
    plt.show()


plot_feature_cancer(tree)

# Исторические цены на оперативную память.
# Файл ram_price.csv лежит рядом со скриптом
ram_prices = pd.read_csv(os.path.join(os.path.dirname(__file__), "ram_price.csv"))
plt.semilogy(ram_prices.date, ram_prices.price)
plt.xlabel("Год")
plt.ylabel("Цена $/Мбайт")
plt.show()

# Обучаем на данных до 2000 года, прогнозируем период после него
data_train = ram_prices[ram_prices.date < 2000]
data_test = ram_prices[ram_prices.date >= 2000]

# Логарифмируем цены, чтобы зависимость стала близкой к линейной
y_train = np.log(data_train.price)
X_train = data_train.date.to_numpy().reshape(-1, 1)

print("X:\n{}".format(X_train))
print("y:\n{}".format(y_train))

tree = DecisionTreeRegressor().fit(X_train, y_train)
linear_reg = LinearRegression().fit(X_train, y_train)

X_all = ram_prices.date.to_numpy().reshape(-1, 1)

pred_tree = tree.predict(X_all)
pred_lr = linear_reg.predict(X_all)

# Возвращаемся от логарифма к обычным ценам
price_tree = np.exp(pred_tree)
price_lr = np.exp(pred_lr)

plt.semilogy(data_train.date, data_train.price, label="Обучающие данные")
plt.semilogy(data_test.date, data_test.price, label="Тестовые данные")
plt.semilogy(ram_prices.date, price_tree, label="Прогнозы дерева")
plt.semilogy(ram_prices.date, price_lr, label="Прогнозы линейной регрессии")
plt.legend()
plt.show()
