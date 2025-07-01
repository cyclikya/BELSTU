#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>

#define moisture_sensor A0

#define SCREEN_WIDTH 128
#define SCREEN_HEIGHT 32
#define OLED_RESET -1

Adafruit_SSD1306 display(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, OLED_RESET);

static const unsigned char PROGMEM eye0[] =
{
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B01111111, B11111111, B11111111, B11111110,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B01111111, B11111111, B11111111, B11111110,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
};
static const unsigned char PROGMEM eye1[] =
{
  B00000111, B11111111, B11111111, B11100000,
  B00011111, B11111111, B11111111, B11111000,
  B00111111, B11111111, B11111111, B11111100,
  B01111111, B11111111, B11111111, B11111110,
  B01111111, B11111111, B11111111, B11111110,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B01111111, B11111111, B11111111, B11111110,
  B01111111, B11111111, B11111111, B11111110,
  B00111111, B11111111, B11111111, B11111100,
  B00011111, B11111111, B11111111, B11111000,
  B00000111, B11111111, B11111111, B11100000,
};
static const unsigned char PROGMEM eye4[] =
{
  B00000111, B11111111, B11111111, B11100000,
  B00011111, B11111111, B11111111, B11111000,
  B00111111, B11111111, B11111111, B11111100,
  B01111111, B11111111, B11111111, B11111110,
  B01111111, B11111111, B11111111, B11111110,
  B11111111, B11111111, B11111111, B11111111,
  B11111111, B11111111, B11111111, B11111111,
  B11111000, B00000000, B00000000, B00011111,
  B11100000, B00000000, B00000000, B00000111,
  B11000000, B00000000, B00000000, B00000011,
  B10000000, B00000000, B00000000, B00000001,
  B10000000, B00000000, B00000000, B00000001,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
};
static const unsigned char PROGMEM eye5[] =
{
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B01111111, B11111110, B00000000,
  B00000001, B11111111, B11111111, B10000000,
  B00000011, B11111111, B11111111, B11000000,
  B00000111, B11111111, B11111111, B11100000,
  B00000111, B11111111, B11111111, B11100000,
  B00001111, B11111111, B11111111, B11110000,
  B00001111, B10000000, B00000001, B11110000,
  B00001110, B00000000, B00000000, B01110000,
  B00001100, B00000000, B00000000, B00110000,
  B00001000, B00000000, B00000000, B00010000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
  B00000000, B00000000, B00000000, B00000000,
};

const int Touch = 4;
const int photoPin = A2;
const int wetPin = A0;
const int moistureThreshold = 700;

int very_moist_value = 0;

bool isMessageDisplayed = false;
bool isSleeping = false;
unsigned long lastPressedTime = 0;
unsigned long displayDuration = 500;

void setup() {
  pinMode(Touch, INPUT_PULLUP);

  if (!display.begin(SSD1306_SWITCHCAPVCC, 0x3C)) {
      Serial.println(F("SSD1306 allocation failed"));
      for (;;);
  }

  display.clearDisplay();
  display.display();

  display.drawBitmap(20, 0, eye4, 32, 32, WHITE);
  display.drawBitmap(76, 0, eye4, 32, 32, WHITE);
  display.display();
  delay(1000);
}

void loop() {
  static unsigned long lastMoistureCheckTime = 3600000;
  unsigned long currentMillis = millis();
  int lightLevel = analogRead(photoPin);
  int wetLevel = analogRead(wetPin);


  if (lightLevel > 200) { //если темно
    if (!isSleeping) {
      sleepMode();
    }
  } else { //если светло
    isSleeping = false; 

    //моргание
    if (!isMessageDisplayed) {
      blinkEyes();
    }

    //касание
    if (digitalRead(Touch) == HIGH && !isMessageDisplayed) {
      display.clearDisplay();
      isMessageDisplayed = true;
      for (int i = 0; i < 2; i++)  {
        joy();
      }
      isMessageDisplayed = false;
    }

    //полив
    if (wetLevel < 70 && !isMessageDisplayed && currentMillis - lastMoistureCheckTime >= 3600000) {
        lastMoistureCheckTime = currentMillis;
        display.clearDisplay();
        isMessageDisplayed = true;

        joy();

        delay(10);
        int moisture_value = analogRead(moisture_sensor);
        display.clearDisplay();
        display.setTextSize(2);
        display.setTextColor(WHITE); 
        display.setCursor(3, 10); 
        display.print("Thank you");
        display.display();
        delay(10000);
        isMessageDisplayed = false;
      }
  }
}

void closeEyes() {
  display.clearDisplay(); 
  display.drawBitmap(20, 0, eye0, 32, 32, WHITE); 
  display.drawBitmap(76, 0, eye0, 32, 32, WHITE); 
  display.display(); 
}

void sleepMode() {
    isSleeping = true; 

    isMessageDisplayed = true; 
    for (int i = 0; i < 3; i++) { 
        display.clearDisplay();
        display.drawBitmap(20, 0, eye1, 32, 32, WHITE);
        display.drawBitmap(76, 0, eye1, 32, 32, WHITE); 
        display.display();
        delay(500);

        display.clearDisplay();
        display.drawBitmap(20, 0, eye0, 32, 32, WHITE); 
        display.drawBitmap(76, 0, eye0 ,32 ,32 ,WHITE); 
        display.display();
        delay(700);
    } 
    closeEyes();

    while (analogRead(photoPin) > 200) { 
        animateZs(); 
    } 

    for (int i = 0; i < 2; i++) { 
        display.clearDisplay();
        display.drawBitmap(20, 0, eye0, 32, 32, WHITE); 
        display.drawBitmap(76, 0, eye0 ,32 ,32 ,WHITE); 
        display.display();
        delay(700);

        display.clearDisplay();
        display.drawBitmap(20, 0, eye1, 32, 32, WHITE);
        display.drawBitmap(76, 0, eye1, 32, 32, WHITE); 
        display.display();
        delay(200);
    } 

    isMessageDisplayed = false; 
}

void blinkEyes() {
  display.clearDisplay();
  display.drawBitmap(20, 0, eye1, 32, 32, WHITE); // Левый глаз
  display.drawBitmap(76, 0, eye1, 32, 32, WHITE); // Правый глаз
  display.display();
  delay(800);

  display.clearDisplay();
  display.drawBitmap(20, 0, eye0, 32, 32, WHITE); // Левый глаз
  display.drawBitmap(76, 0, eye0 ,32 ,32 ,WHITE); // Правый глаз
  display.display();
  delay(200);

}

void joy() {
  display.clearDisplay();
  display.drawBitmap(20, 0, eye4, 32, 32, WHITE); // Левый глаз
  display.drawBitmap(76, 0, eye4, 32, 32, WHITE); // Правый глаз
  display.display();
  delay(500);

  display.clearDisplay();
  display.drawBitmap(20, 0, eye5, 32, 32, WHITE); // Левый глаз
  display.drawBitmap(76, 0, eye5, 32, 32, WHITE); // Правый глаз
  display.display();
  delay(500);
}

void animateZs() {
  for (int i = 0; i < 3; i++) { 
    display.clearDisplay(); 
    
    int eyeOffset = (i % 2 == 0) ? -1 : 1; 

    display.drawBitmap(20, 0 + eyeOffset, eye0, 32, 32, WHITE); 
    display.drawBitmap(76, 0 + eyeOffset, eye0, 32, 32, WHITE); 
    
    display.setTextSize(i + 0.05); 
    display.setTextColor(WHITE); 
    display.setCursor(100 + i * 2, 5); 
    display.print("Z"); 
    
    display.display(); 
    delay(300); 
  }
}