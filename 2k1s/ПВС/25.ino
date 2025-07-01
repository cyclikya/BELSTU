#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <IRremote.hpp>

// Настройки для ИК-приемника
#define IR_RECEIVE_PIN 2 // Пин, к которому подключен ИК-приемник

// Инициализация LCD-дисплея (I2C адрес 0x27 или 0x3F)
LiquidCrystal_I2C lcd(0x27, 16, 2); // Укажите ваш I2C адрес: 0x27 или 0x3F

// Таблица соответствия кодов и сообщений
const unsigned long BUTTON_1 = 0xBA45FF00; // Код кнопки "CH-"
const unsigned long BUTTON_2 = 0xB946FF00; // Код кнопки "CH"
const unsigned long BUTTON_3 = 0xB847FF00; // Код кнопки "CH+"
const unsigned long BUTTON_4 = 0xBB44FF00; // Код кнопки "<<"
const unsigned long BUTTON_5 = 0xBF40FF00; // Код кнопки ">>"
const unsigned long BUTTON_6 = 0xBC43FF00; // Код кнопки ">="
const unsigned long BUTTON_7 = 0xF807FF00; // Код кнопки "-"
const unsigned long BUTTON_8 = 0xEA15FF00; // Код кнопки "+"
const unsigned long BUTTON_9 = 0xF609FF00; // Код кнопки "EQ"
const unsigned long BUTTON_10 = 0xE916FF00; // Код кнопки "0"
const unsigned long BUTTON_11= 0xF20DFF00; // Код кнопки "FOL-"
const unsigned long BUTTON_12 = 0xE619FF00; // Код кнопки "FOL+"
const unsigned long BUTTON_13 = 0xF30CFF00; // Код кнопки "1"
const unsigned long BUTTON_14 = 0xE718FF00; // Код кнопки "2"
const unsigned long BUTTON_15 = 0xA15EFF00; // Код кнопки "3"
const unsigned long BUTTON_16 = 0xF708FF00; // Код кнопки "4"
const unsigned long BUTTON_17 = 0xE31CFF00; // Код кнопки "5"
const unsigned long BUTTON_18 = 0xA55AFF00; // Код кнопки "6"
const unsigned long BUTTON_19 = 0xF807FF00; // Код кнопки "7"
const unsigned long BUTTON_20 = 0xBD42FF00; // Код кнопки "8"
const unsigned long BUTTON_21 = 0xAD52FF00; // Код кнопки "9"

void setup() {
  // Инициализация Serial Monitor
  Serial.begin(9600);

  // Инициализация ИК-приемника
  IrReceiver.begin(IR_RECEIVE_PIN, ENABLE_LED_FEEDBACK);

  // Инициализация LCD-дисплея
  lcd.init();         // Инициализация LCD
  lcd.backlight();    // Включаем подсветку
  lcd.print("Waiting for IR");
  delay(2000);
  lcd.clear();
}

void loop() {
  // Проверяем, получен ли сигнал
  if (IrReceiver.decode()) {
    // Выводим код кнопки в Serial Monitor для проверки
    Serial.print("Получен код: ");
    Serial.println(IrReceiver.decodedIRData.decodedRawData, HEX);
    lcd.clear(); // Очищаем дисплей
    
    // Проверяем код кнопки и выводим сообщение
    if (IrReceiver.decodedIRData.decodedRawData == BUTTON_1) {
      lcd.print("Button CH-");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_2) {
      lcd.print("Button CH");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_3) {
      lcd.print("Button CH+");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_4) {
      lcd.print("Button <<");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_5) {
      lcd.print("Button >>");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_6) {
      lcd.print("Button >||");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_7) {
      lcd.print("Button -");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_8) {
      lcd.print("Button +");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_9) {
      lcd.print("Button EQ");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_10) {
      lcd.print("Button 0");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_11) {
      lcd.print("Button FOL-");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_12) {
      lcd.print("Button FOL+");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_13) {
      lcd.print("Button 1");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_14) {
      lcd.print("Button 2");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_15) {
      lcd.print("Button 3"); 
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_16) {
      lcd.print("Button 4");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_17) {
      lcd.print("Button 5");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_18) {
      lcd.print("Button 6");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_19) {
      lcd.print("Button 7");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_20) {
      lcd.print("Button 8");
    } else if (IrReceiver.decodedIRData.decodedRawData == BUTTON_21) {
      lcd.print("Button 9");
    } else {
      lcd.print("Unknown Button");
    }

    // Возобновляем прием ИК-сигнала
    IrReceiver.resume();
  }
}