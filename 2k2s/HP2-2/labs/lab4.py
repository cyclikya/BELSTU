import requests
from bs4 import BeautifulSoup
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

##1
url = "https://www.chitai-gorod.ru/?utm_medium=cpc&utm_source=yandex&utm_campaign=y_search_generic_by%7C64449418&utm_term=каталоги%20книг&utm_content=k50id%7C0100000048229215880_48229215880%7Ccid%7C64449418%7Cgid%7C5324868218%7Caid%7C15306068621%7Cadp%7Cno%7Cpos%7Cother2%7Csrc%7Csearch_none%7Cdvc%7Cdesktop%7Creg157%7Cmain&k50id=0100000048229215880_48229215880&referrer=reattribution%3D1&yclid=12930592729740345343"

response = requests.get(url)

soup = BeautifulSoup(response.content, 'html.parser')

product_items = soup.find_all('div', class_='embla__container')

products = []
for item in product_items:
    name_tag = item.find('div', class_='product-title__head')
    price_tag = item.find('div', class_='product-price__value product-price__value--discount')

    if name_tag and price_tag:
        name = name_tag.text.strip()
        price = price_tag.text.strip()
        products.append({'Name': name, 'Price': price})

df = pd.DataFrame(products)

print(type(products[1]))

csv_file_path = 'products.csv'
df.to_csv(csv_file_path, index=False)

##3
csv_file_path = 'products.csv'
df = pd.read_csv(csv_file_path)

# Очистка и конвертация цен
df['Price'] = df['Price'].replace('[^\d.]', '', regex=True).str.rstrip('.')
df['Price'] = df['Price'].astype(float)

# Сортировка по цене
df_sorted = df.sort_values(by='Price', ascending=False)

print("Первые 5 значений отсортированного DataFrame:")
print(df_sorted.head())

description = df['Price'].describe()
print("\nОсновные метрики статистики для цен:")
print(description)

# Средняя цена для одинаковых товаров
grouped_df = df.groupby('Name')['Price'].mean().reset_index()
print("\nСредняя цена для повторяющихся товаров:")
print(grouped_df)

# Построение графиков
plt.figure(figsize=(12, 6))

# Гистограмма
plt.subplot(1, 2, 1)
sns.histplot(df['Price'], bins=30, kde=True, color='skyblue', linewidth=2, edgecolor='black')
plt.title('Гистограмма распределения цен', fontsize=14)
plt.xlabel('Цена', fontsize=12)
plt.ylabel('Частота', fontsize=12)
plt.grid(True, linestyle='--', alpha=0.7)

# Диаграмма box-plot
plt.subplot(1, 2, 2)
sns.boxplot(x=df['Price'], color='lightgreen', linewidth=2)
plt.title('Диаграмма box-plot распределения цен', fontsize=14)
plt.xlabel('Цена', fontsize=12)
plt.grid(True, linestyle='--', alpha=0.7)

plt.tight_layout()
plt.show()
