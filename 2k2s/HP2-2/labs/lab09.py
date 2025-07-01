import string
import numpy as np
import matplotlib.pyplot as plt
from PIL import Image
from wordcloud import WordCloud
import nltk

# Скачиваем стоп-слова для русского языка
nltk.download('stopwords')
from nltk.corpus import stopwords

with open(r'C:\Users\vugor\OneDrive\Рабочий стол\2k2s\HP2-2\labs\input.txt', 'r', encoding='utf-8') as f:
    text = f.read()

# === 2. Предобработка текста ===
text = text.lower()
translator = str.maketrans('', '', string.punctuation + '«»-…“”’‘')
text = text.translate(translator)
words = text.split()
russian_stopwords = set(stopwords.words('russian'))
custom_stopwords = {'это', 'как', 'так', 'вот', 'ещё', 'быть', 'который', 'мочь', 'еще'}
all_stopwords = russian_stopwords.union(custom_stopwords)
filtered_words = [word for word in words if word not in all_stopwords and word.isalpha()]
processed_text = ' '.join(filtered_words)

# === 3. Загрузка маски из jpg-картинки ===
mask_image = np.array(Image.open(r"C:\Users\vugor\OneDrive\Рабочий стол\2k2s\HP2-2\labs\butterfly.jpg").convert("L"))

# === 4. Генерация облака слов ===
wordcloud = WordCloud(
    width=mask_image.shape[1],
    height=mask_image.shape[0],
    background_color='white',
    mask=mask_image,
    stopwords=all_stopwords,
    collocations=False,
).generate(processed_text)

# === 5. Визуализация ===
plt.figure(figsize=(10, 10))
plt.imshow(wordcloud, interpolation='bilinear')
plt.axis('off')
plt.show()
