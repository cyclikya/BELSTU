#include <math.h>

int minute = 1;
int row_excel = 0; // количество строк для PLX-DAQ

// Параметр конкретного типа термистора (из datasheet):
#define TERMIST_B 4300
#define VIN 5.0

void setup() {d:\Study\3 семестр\ПВС\Лабы 1 сем\10 лаба\sketch_oct3a\sketch_oct3a.ino
  Serial.begin(9600); // скорость передачи данных через последовательный порт

  // Очищаем лист Excel и задаём заголовки столбцов
  Serial.println("CLEARDATA"); // очистка листа excel
  Serial.println("LABEL,10 sec,Temperature"); // заголовки столбцов
}

void loop() {
  // Считываем напряжение с аналогового порта
  float voltage = analogRead(A0) * VIN / 1024.0;

  // Рассчитываем сопротивление термистора
  float r1 = voltage / (VIN - voltage);

  // Рассчитываем температуру в градусах Цельсия
  float temperature = 1. / (1. / TERMIST_B * log(r1) + 1. / (25. + 273.)) - 273;

  // Записываем данные в Excel через PLX-DAQ
  row_excel++; // номер строки + 1
  Serial.print("DATA,TIME,"); // запись в excel текущего времени

  // Передача минуты и температуры
  Serial.print(minute);
  Serial.print(",");
  Serial.println(temperature); // добавляем символ новой строки

  // Если строк больше 50, то начинаем заполнять строки заново
  if (row_excel > 50) {
    row_excel = 0;
    Serial.println("ROW,SET,2"); // сбрасываем позицию на вторую строку
  }

  // Задержка 10 секунд (10000 мс)
  delay(10000);

  // Увеличиваем значение минуты на 1
  ++minute;
}

