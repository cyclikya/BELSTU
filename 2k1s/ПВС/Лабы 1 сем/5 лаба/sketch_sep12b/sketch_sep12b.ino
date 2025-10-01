#define LED_PIN 9
#define RED_PIN 11    // ШИМ-выход для красного цвета
#define GREEN_PIN 10 // ШИМ-выход для зеленого цвета
#define BLUE_PIN 9  // ШИМ-выход для синего цвета
// для работы с текстом существуют объекты-строки (англ. string)
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
  Serial.begin(9600);
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
        Serial.print("Яркость установлена на: ");
        Serial.println(value);
      }
      // Очищаем сообщение
      message = "";
    }
  }
}