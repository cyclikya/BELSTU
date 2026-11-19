import matplotlib.pyplot as plt
import mglearn
import numpy as np
from sklearn.datasets import load_breast_cancer, make_blobs
from sklearn.model_selection import train_test_split
from sklearn.svm import LinearSVC, SVC


# Линейные модели и нелинейные признаки
X, y = make_blobs(centers=4, random_state=8)
y = y % 2

mglearn.discrete_scatter(X[:, 0], X[:, 1], y)
plt.xlabel("Признак 0")
plt.ylabel("Признак 1")
plt.show()


# Граница принятия решений линейного SVM
linear_svm = LinearSVC(random_state=0, max_iter=10000).fit(X, y)
mglearn.plots.plot_2d_separator(linear_svm, X)
mglearn.discrete_scatter(X[:, 0], X[:, 1], y)
plt.xlabel("Признак 0")
plt.ylabel("Признак 1")
plt.show()


# Добавляем квадрат второго признака
X_new = np.hstack([X, X[:, 1:] ** 2])

figure = plt.figure()
ax = figure.add_subplot(111, projection="3d", elev=-152, azim=-26)
mask = y == 0
ax.scatter(
    X_new[mask, 0],
    X_new[mask, 1],
    X_new[mask, 2],
    c="b",
    cmap=mglearn.cm2,
    s=60,
)
ax.scatter(
    X_new[~mask, 0],
    X_new[~mask, 1],
    X_new[~mask, 2],
    c="r",
    marker="^",
    cmap=mglearn.cm2,
    s=60,
)
ax.set_xlabel("признак0")
ax.set_ylabel("признак1")
ax.set_zlabel("признак1 ** 2")
plt.show()


# Разделяющая плоскость линейного SVM в расширенном пространстве признаков
linear_svm_3d = LinearSVC(random_state=0, max_iter=10000).fit(X_new, y)
coef = linear_svm_3d.coef_.ravel()
intercept = linear_svm_3d.intercept_

figure = plt.figure()
ax = figure.add_subplot(111, projection="3d", elev=-152, azim=-26)
xx = np.linspace(X_new[:, 0].min() - 2, X_new[:, 0].max() + 2, 50)
yy = np.linspace(X_new[:, 1].min() - 2, X_new[:, 1].max() + 2, 50)
XX, YY = np.meshgrid(xx, yy)
ZZ = (coef[0] * XX + coef[1] * YY + intercept[0]) / -coef[2]

ax.plot_surface(XX, YY, ZZ, rstride=8, cstride=8, alpha=0.3)
ax.scatter(
    X_new[mask, 0],
    X_new[mask, 1],
    X_new[mask, 2],
    c="b",
    cmap=mglearn.cm2,
    s=60,
)
ax.scatter(
    X_new[~mask, 0],
    X_new[~mask, 1],
    X_new[~mask, 2],
    c="r",
    marker="^",
    cmap=mglearn.cm2,
    s=60,
)
ax.set_xlabel("признак0")
ax.set_ylabel("признак1")
ax.set_zlabel("признак1 ** 2")
plt.show()


# Та же граница принятия решений как функция двух исходных признаков
ZZ = YY**2
dec = linear_svm_3d.decision_function(
    np.c_[XX.ravel(), YY.ravel(), ZZ.ravel()]
)
plt.contourf(
    XX,
    YY,
    dec.reshape(XX.shape),
    levels=[dec.min(), 0, dec.max()],
    cmap=mglearn.cm2,
    alpha=0.5,
)
mglearn.discrete_scatter(X[:, 0], X[:, 1], y)
plt.xlabel("Признак 0")
plt.ylabel("Признак 1")
plt.show()


# Ядерный SVM с RBF-ядром и его опорные векторы
X, y = mglearn.tools.make_handcrafted_dataset()
svm = SVC(kernel="rbf", C=10, gamma=0.1).fit(X, y)
mglearn.plots.plot_2d_separator(svm, X, eps=0.5)
mglearn.discrete_scatter(X[:, 0], X[:, 1], y)

sv = svm.support_vectors_
sv_labels = svm.dual_coef_.ravel() > 0
mglearn.discrete_scatter(
    sv[:, 0],
    sv[:, 1],
    sv_labels,
    s=15,
    markeredgewidth=3,
)
plt.xlabel("Признак 0")
plt.ylabel("Признак 1")
plt.show()


# Влияние параметров C и gamma на границу принятия решений
fig, axes = plt.subplots(3, 3, figsize=(15, 10))
for ax, C in zip(axes, [-1, 0, 3]):
    for a, gamma in zip(ax, range(-1, 2)):
        mglearn.plots.plot_svm(log_C=C, log_gamma=gamma, ax=a)

axes[0, 0].legend(
    ["class 0", "class 1", "sv class 0", "sv class 1"],
    ncol=4,
    loc=(0.9, 1.2),
)
plt.show()


# Применение SVM с RBF-ядром к набору данных Breast Cancer
cancer = load_breast_cancer()
X_train, X_test, y_train, y_test = train_test_split(
    cancer.data,
    cancer.target,
    random_state=0,
)

# В версии scikit-learn из методички gamma по умолчанию равнялась 1 / n_features.
# Явно задаем gamma="auto", чтобы воспроизвести приведенные в ней результаты.
svc = SVC(gamma="auto")
svc.fit(X_train, y_train)
print(
    "Правильность на обучающем наборе: {:.2f}".format(
        svc.score(X_train, y_train)
    )
)
print(
    "Правильность на тестовом наборе: {:.2f}".format(
        svc.score(X_test, y_test)
    )
)


# Диапазоны значений признаков набора Breast Cancer
plt.plot(X_train.min(axis=0), "o", label="min")
plt.plot(X_train.max(axis=0), "^", label="max")
plt.legend(loc=4)
plt.xlabel("Индекс признака")
plt.ylabel("Величина признака")
plt.yscale("log")
plt.show()


# Масштабирование признаков обучающего набора в диапазон от 0 до 1
min_on_training = X_train.min(axis=0)
range_on_training = (X_train - min_on_training).max(axis=0)
X_train_scaled = (X_train - min_on_training) / range_on_training

print(
    "Минимальное значение для каждого признака\n{}".format(
        X_train_scaled.min(axis=0)
    )
)
print(
    "Максимальное значение для каждого признака\n{}".format(
        X_train_scaled.max(axis=0)
    )
)


# Для тестового набора используем преобразование, вычисленное на обучающих данных
X_test_scaled = (X_test - min_on_training) / range_on_training

svc = SVC(gamma="auto")
svc.fit(X_train_scaled, y_train)
print(
    "Правильность на обучающем наборе: {:.3f}".format(
        svc.score(X_train_scaled, y_train)
    )
)
print(
    "Правильность на тестовом наборе: {:.3f}".format(
        svc.score(X_test_scaled, y_test)
    )
)


# Увеличиваем C, чтобы получить более сложную модель
svc = SVC(C=1000, gamma="auto")
svc.fit(X_train_scaled, y_train)
print(
    "Правильность на обучающем наборе: {:.3f}".format(
        svc.score(X_train_scaled, y_train)
    )
)
print(
    "Правильность на тестовом наборе: {:.3f}".format(
        svc.score(X_test_scaled, y_test)
    )
)
