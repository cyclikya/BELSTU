#define LED_PIN 9
#define RED_PIN 11    // ШИМ-выход для красного цвета
#define GREEN_PIN 10  // ШИМ-выход для зеленого цвета
#define BLUE_PIN 9    // ШИМ-выход для синего цвета

String message;
String messageOn;
String messageRgb;
bool ledState = false; // Переменная для хранения состояния светодиода

void setup()
{
  pinMode(RED_PIN, OUTPUT);
  pinMode(GREEN_PIN, OUTPUT);
  pinMode(BLUE_PIN, OUTPUT);
  Serial.begin(9600);
  pinMode(LED_PIN, OUTPUT);
}

void loop()
{
  // передаваемые с компьютера данные поставляются байт за байтом
  while (Serial.available()) {
    // считываем пришедший символ в переменную
    char incomingChar = Serial.read();

    // Проверяем, является ли это командой (on/off)
    if (incomingChar == '\n') {
      // Приводим команду к нижнему регистру для удобства сравнения
      messageOn.toLowerCase();
      if (messageOn == "on") {
        digitalWrite(LED_PIN, HIGH); // Включаем светодиод
        Serial.println("LED is ON");
        ledState = true; // Обновляем состояние светодиода
      } else if (messageOn == "off") {
        digitalWrite(LED_PIN, LOW); // Выключаем светодиод
        Serial.println("LED is OFF");
        ledState = false; // Обновляем состояние светодиода
      } else if (ledState) {
        // Если включено RGB-управление
        processMessage(messageOn); // Обрабатываем введенные RGB-значения
      }
      // Очищаем строку сообщения для новой команды
      messageOn = "";
    } else {
      // Добавляем символ к сообщению
      messageOn += incomingChar;
    }

    // Проверяем на ввод числовых значений для регулирования яркости
    if (incomingChar >= '0' && incomingChar <= '9' && ledState) {
      // Если пришёл символ-цифра, добавляем его к сообщению
      message += incomingChar;
    } else if (incomingChar == '\n' && ledState) {
      // Преобразуем накопленные символы в число
      int value = message.toInt();

      // Проверяем, превышает ли значение допустимый диапазон
      if (value > 255) {
        Serial.println("Ошибка! Значение превышает допустимое значение (255)");
      } else {
        analogWrite(LED_PIN, value); // Регулируем яркость светодиода
      }
      // Очищаем сообщение
      message = "";
    }
  }
}

// Функция для обработки сообщения RGB
void processMessage(String msg) {
  msg.trim();  // Убираем лишние пробелы и символы перевода строки

  // Находим запятые и извлекаем числа
  int firstComma = msg.indexOf(',');    // Первая запятая
  int secondComma = msg.indexOf(',', firstComma + 1);  // Вторая запятая

  if (firstComma == -1 || secondComma == -1) {
    Serial.println("Ошибка: Введите три числа, разделенные запятыми.");
    return;
  }

  // Извлекаем значения для красного, зеленого и синего
  int redValue = msg.substring(0, firstComma).toInt();
  int greenValue = msg.substring(firstComma + 1, secondComma).toInt();
  int blueValue = msg.substring(secondComma + 1).toInt();

  // Проверяем диапазон значений
  if (redValue < 0 || redValue > 255 || greenValue < 0 || greenValue > 255 || blueValue < 0 || blueValue > 255) {
    Serial.println("Ошибка! RGB значения должны быть в диапазоне 0-255.");
    return;
  }

  // Устанавливаем яркость каждого цвета через ШИМ
  analogWrite(RED_PIN, redValue);
  analogWrite(GREEN_PIN, greenValue);
  analogWrite(BLUE_PIN, blueValue);

  Serial.print("Установлен цвет: ");
  Serial.print(redValue);
  Serial.print(", ");
  Serial.print(greenValue);
  Serial.print(", ");
  Serial.println(blueValue);
}

