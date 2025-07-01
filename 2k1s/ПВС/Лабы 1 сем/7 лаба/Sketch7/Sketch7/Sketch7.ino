int LEDrot = 12;
int LEDgelb = 11;
int LEDgruen = 10;
int cnt = 0;
int state = 1;
char mode = '0';

void setup()
{
    pinMode(LEDrot, OUTPUT);
    pinMode(LEDgelb, OUTPUT);
    pinMode(LEDgruen, OUTPUT);
    Serial.begin(9600);
    digitalWrite(LEDrot, LOW);
    digitalWrite(LEDgelb, LOW);
    digitalWrite(LEDgruen, LOW);
}

void loop()
{
    if (Serial.available() > 0) {
        char input = Serial.read();
        switch (input) {
        case 'N':
            mode = 'N';
            Serial.println("Включён режим normal");
            break;
        case 'S':
            mode = 'S';
            Serial.println("Включён режим sleep");
            break;
        case 'R':
            mode = 'R';
            Serial.println("Включён режим red");
            break;
        case 'G':
            mode = 'G';
            Serial.println("Включён режим green");
            break;
        default:
            break;
        }
    }

    if (mode == 'N') {
        cnt++;
        if (cnt == 100) {
            cnt = 0;
            Statemaschine();
        }
        delay(10);
    }
    else if (mode == 'S') {
        digitalWrite(LEDrot, LOW);
        digitalWrite(LEDgruen, LOW);
        digitalWrite(LEDgelb, cnt % 200 < 100 ? HIGH : LOW);
        cnt++;
        delay(10);
    }
    else if (mode == 'R') {
        digitalWrite(LEDrot, HIGH);
        digitalWrite(LEDgelb, LOW);
        digitalWrite(LEDgruen, LOW);
    }
    else if (mode == 'G') {
        digitalWrite(LEDrot, LOW);
        digitalWrite(LEDgelb, LOW);
        digitalWrite(LEDgruen, HIGH);
    }
}

void Statemaschine(void)
{
    switch (state)
    {
    case 1:
        digitalWrite(LEDrot, HIGH);
        digitalWrite(LEDgelb, LOW);
        digitalWrite(LEDgruen, LOW);
        state++;
        break;

    case 2:
        digitalWrite(LEDrot, HIGH);
        digitalWrite(LEDgelb, HIGH);
        digitalWrite(LEDgruen, LOW);
        state++;
        break;

    case 3:
        digitalWrite(LEDrot, LOW);
        digitalWrite(LEDgelb, LOW);
        digitalWrite(LEDgruen, HIGH);
        state++;
        break;

    case 4:
        digitalWrite(LEDrot, LOW);
        digitalWrite(LEDgelb, HIGH);
        digitalWrite(LEDgruen, LOW);
        state = 1;
        break;
    }
}