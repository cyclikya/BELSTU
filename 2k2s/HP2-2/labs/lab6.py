import sqlite3
from datetime import datetime, timedelta
import telebot
from telebot import types
import matplotlib.pyplot as plt
import pandas as pd
from io import BytesIO

# Токен бота
BOT_TOKEN = "7384071048:AAGtuQ0NjS5k8ZF3t-n4jvBTzzT-DLq2OSs"
bot = telebot.TeleBot(BOT_TOKEN)

# Инициализация базы данных
def init_db():
    conn = sqlite3.connect('mood_diary.db')
    cursor = conn.cursor()
    cursor.execute('''
    CREATE TABLE IF NOT EXISTS moods (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        user_id INTEGER NOT NULL,
        mood_score INTEGER NOT NULL,
        note TEXT,
        timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
    )
    ''')
    conn.commit()
    conn.close()

init_db()

# Команда /start и /help
@bot.message_handler(commands=['start', 'help'])
def send_welcome(message):
    help_text = """
🌟 <b>Дневник настроения</b> 🌟

Этот бот поможет вам отслеживать ваше эмоциональное состояние.

<b>Основные команды:</b>
/start или /help - показать это сообщение
/add - добавить запись о настроении
/stats - показать статистику за неделю
/history - показать историю записей за неделю
/export - экспортировать данные в CSV
"""
    markup = types.ReplyKeyboardMarkup(resize_keyboard=True)
    markup.add(
        types.KeyboardButton('📊 Статистика'),
        types.KeyboardButton('➕ Добавить запись'),
        types.KeyboardButton('📜 История'),
        types.KeyboardButton('📤 Экспорт данных')
    )
    bot.send_message(message.chat.id, help_text, parse_mode='HTML', reply_markup=markup)

@bot.message_handler(content_types=['sticker'])
def handle_sticker(message):
    bot.send_message(message.chat.id, "😊 Классный стикер!")

# Обработка текстовых сообщений
@bot.message_handler(func=lambda message: True)
def handle_text(message):
    
    if message.text == '📊 Статистика' or message.text == '/stats':
        show_stats_week(message)
    elif message.text == '➕ Добавить запись' or message.text == '/add':
        ask_mood_score(message)
    elif message.text == '📜 История' or message.text == '/history':
        show_history_week(message)
    elif message.text == '📤 Экспорт данных' or message.text == '/export':
        export_data(message)
    else:
        bot.reply_to(message, "Извините, я не понимаю эту команду. Попробуйте /help для списка доступных команд.")

# Запрос оценки настроения
def ask_mood_score(message):
    markup = types.InlineKeyboardMarkup()
    for i in range(1, 11):
        markup.add(types.InlineKeyboardButton(str(i), callback_data=f"mood_{i}"))
    bot.send_message(message.chat.id, "Как вы оцениваете свое настроение сегодня от 1 до 10?", reply_markup=markup)

# Callback-обработчик
@bot.callback_query_handler(func=lambda call: True)
def callback_handler(call):
    if call.data.startswith('mood_'):
        mood_score = int(call.data.split('_')[1])
        msg = bot.send_message(call.message.chat.id, f"Вы выбрали {mood_score}. Хотите добавить заметку? Напишите её или 'нет'.")
        bot.register_next_step_handler(msg, lambda m: save_mood_with_note(m, call.from_user.id, mood_score))
    elif call.data.startswith('delete_'):
        record_id = int(call.data.split('_')[1])
        delete_record(call.message, record_id)

# Сохранение настроения
def save_mood_with_note(message, user_id, mood_score):
    note = message.text if message.text.lower() != 'нет' else None
    conn = sqlite3.connect('mood_diary.db')
    cursor = conn.cursor()
    cursor.execute('INSERT INTO moods (user_id, mood_score, note) VALUES (?, ?, ?)', (user_id, mood_score, note))
    conn.commit()
    conn.close()
    bot.send_message(message.chat.id, f"✅ Запись сохранена: настроение {mood_score}" + (f"\nЗаметка: {note}" if note else ""))

# Статистика за неделю
def show_stats_week(message):
    user_id = message.from_user.id
    conn = sqlite3.connect('mood_diary.db')
    cursor = conn.cursor()
    since = datetime.now() - timedelta(days=7)
    cursor.execute('SELECT mood_score, timestamp FROM moods WHERE user_id = ? AND timestamp >= ? ORDER BY timestamp', (user_id, since))
    rows = cursor.fetchall()
    conn.close()

    if rows:
        df = pd.DataFrame(rows, columns=['mood_score', 'timestamp'])
        df['timestamp'] = pd.to_datetime(df['timestamp'])
        avg_score = df['mood_score'].mean()
        min_score = df['mood_score'].min()
        max_score = df['mood_score'].max()
        count = len(df)

        plt.figure(figsize=(10, 5))
        plt.plot(df['timestamp'], df['mood_score'], marker='o')
        plt.title('Настроение за последнюю неделю')
        plt.xlabel('Дата')
        plt.ylabel('Оценка')
        plt.ylim(0, 10)
        plt.grid(True)

        buf = BytesIO()
        plt.savefig(buf, format='png')
        buf.seek(0)
        plt.close()

        caption = f"""
📊 <b>Статистика за неделю</b>

Среднее: {avg_score:.2f}
Минимум: {min_score}
Максимум: {max_score}
Записей: {count}
"""
        bot.send_photo(message.chat.id, buf, caption=caption, parse_mode='HTML')
    else:
        bot.send_message(message.chat.id, "😕 За последнюю неделю записей нет.")

# История за неделю
def show_history_week(message):
    user_id = message.from_user.id
    conn = sqlite3.connect('mood_diary.db')
    cursor = conn.cursor()
    since = datetime.now() - timedelta(days=7)
    cursor.execute('SELECT id, mood_score, note, timestamp FROM moods WHERE user_id = ? AND timestamp >= ? ORDER BY timestamp DESC', (user_id, since))
    rows = cursor.fetchall()
    conn.close()

    if rows:
        bot.send_message(message.chat.id, "<b>📜 История записей за неделю:</b>", parse_mode='HTML')
        for idx, (record_id, score, note, timestamp) in enumerate(rows, 1):
            timestamp = datetime.strptime(timestamp, '%Y-%m-%d %H:%M:%S').strftime('%d.%m.%Y %H:%M')
            text = f"{idx}. <b>{timestamp}</b> — Настроение: {score}/10"
            if note:
                text += f"\n   Заметка: {note}"
            markup = types.InlineKeyboardMarkup()
            markup.add(types.InlineKeyboardButton("❌ Удалить", callback_data=f"delete_{record_id}"))
            bot.send_message(message.chat.id, text, parse_mode='HTML', reply_markup=markup)
    else:
        bot.send_message(message.chat.id, "😕 Нет записей за последнюю неделю.")

# Удаление записи
def delete_record(message, record_id):
    user_id = message.from_user.id
    conn = sqlite3.connect('mood_diary.db') 
    cursor = conn.cursor()
    cursor.execute('SELECT 1 FROM moods WHERE id = ? AND user_id = ?', (record_id, user_id))
    if cursor.fetchone():
        cursor.execute('DELETE FROM moods WHERE id = ?', (record_id,))
        conn.commit()
        bot.send_message(message.chat.id, "✅ Запись удалена.")
    else:
        bot.send_message(message.chat.id, "⚠️ Запись не найдена или не принадлежит вам.")
    conn.close()

# Экспорт в CSV
def export_data(message):
    user_id = message.from_user.id
    conn = sqlite3.connect('mood_diary.db')
    df = pd.read_sql_query('SELECT mood_score, note, timestamp FROM moods WHERE user_id = ? ORDER BY timestamp', conn, params=(user_id,))
    conn.close()

    if not df.empty:
        buf = BytesIO()
        df.to_csv(buf, index=False, encoding='utf-8')
        buf.seek(0)
        bot.send_document(message.chat.id, ('mood_history.csv', buf), caption="📤 Ваши данные экспортированы в CSV")
    else:
        bot.send_message(message.chat.id, "😕 Нет данных для экспорта.")

# Запуск бота
if __name__ == '__main__':
    print("Бот запущен...")
    bot.infinity_polling()
