int latchPin = 9;   // ST_CP (пин, подключенный к ST_CP первого 74HC595)
int clockPin = 10;  // SH_CP (пин, подключенный к SH_CP обоих 74HC595)
int dataPin = 11;   // DS (пин, подключенный к DS обоих 74HC595)

// Массив для отображения цифр от 0 до 9 на 7-сегментном индикаторе
byte numbers[] = { 0b00111111, 0b00000110, 0b01011011, 0b01001111, 0b01100110, 0b01101101, 0b01111101, 0b00000111, 0b01111111, 0b01101111 };

// Функция для отправки данных на два сдвиговых регистра
void shiftOutData(byte value1, byte value2) {
  digitalWrite(latchPin, LOW);                // Отключаем защелку для отправки данных
  shiftOut(dataPin, clockPin, MSBFIRST, value1);  // Отправляем данные на первый 74HC595 (единицы)
  shiftOut(dataPin, clockPin, MSBFIRST, value2);  // Отправляем данные на второй 74HC595 (десятки)
  digitalWrite(latchPin, HIGH);               // Включаем защелку для отображения результата
}

// Функция для отображения двузначного числа на двух 7-сегментных индикаторах
void displayNumber(int number) {
  int tens = number / 10;  // Извлекаем десятки
  int ones = number % 10;  // Извлекаем единицы

  // Отправляем данные на сдвиговые регистры
  shiftOutData(numbers[ones], numbers[tens]);
}

void setup() {
  pinMode(latchPin, OUTPUT);  // Настраиваем пин для защелки как выход
  pinMode(clockPin, OUTPUT);  // Настраиваем пин для тактового сигнала как выход
  pinMode(dataPin, OUTPUT);   // Настраиваем пин для данных как выход
}

void loop() {
  for (int i = 99; i >= 0; i--) {  // Обратный отсчет от 99 до 0
    displayNumber(i);              // Отображаем текущее число
    delay(100);                    // Задержка 0.1 секунда
  }
}