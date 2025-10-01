#include <iostream>
#include <cmath>
using namespace std;
int pr1()
{
    int a, b, c;
    float d, x1, x2;
    cout << "Введите a: ";
    cin >> a;
    cout << "Введите b: ";
    cin >> b;
    cout << "Введите c: ";
    cin >> c;
    d = pow(b, 2) - (4 * a * c);
    if (d < 0) {
        cout << "Нет корней" << endl;
    }
    else {
        x1 = (-b + sqrt(d)) / (2 * a);
        x2 = (-b - sqrt(d)) / (2 * a);
        cout << "x1 = " << x1 << endl;
        cout << "x2 = " << x2 << endl;
    }
    return 0;
}
int pr2()
{
    int K;
    cout << "Введите кол-во грибов:";
    cin >> K;
    switch (K % 100)
    {
    case 11:printf("We found %d gribov in the forest", K);break;
    case 12:printf("We found %d gribov in the forest", K);break;
    case 13:printf("We found %d gribov in the forest", K);break;
    case 14:printf("We found %d gribov in the forest", K);break;
    default: switch (K % 10)
    {
    case 0: printf("We found %d gribov in the forest", K);break;
    case 1: printf("We found %d grib in the forest", K);break;
    case 2: printf("We found %d griba in the forest", K);break;
    case 3: printf("We found %d griba in the forest", K);break;
    case 4: printf("We found %d griba in the forest", K);break;
    default:printf("We found %d gribov in the forest", K);break;
    }
    }
    cout << endl;
    return 0;
}
int pr3()
{
    int i;
    puts("Вы готовы сделать заказ? (1-Да, 2-Нет)");
    cin >> i;
    switch (i)
    {
    case 1:
        puts("Отлично, что бы вы хотели заказать? (1-чебурек, 2-беляш, 3-шарлотка, 4-видба)");
        cin >> i;
        switch (i)
        {
        case 1:
        case 2:
            puts("Вам нужно его подогреть? (1-да, 2-нет)");
            cin >> i;
            switch (i)
            {
            case 1:
                puts("Ваш заказ будет готов в течение 3 минут");
                break;
            case 2:
                puts("Вот ваш заказ, приятного аппетита");
                break;
            default:
                puts("Неверный вариант");
                break;
            }
            break;
        case 3:
        case 4:
            puts("Вот ваш заказ, приятного аппетита");
            break;
        default:
            puts("Неверный вариант");
            break;
        }
        break;
    case 2:
        puts("Дайте знать, когда вы захотите что-нибудь заказать");
        break;
    default:
        puts("Неверный вариант");
        break;
    }
    return 0;
}
int main()
{
    setlocale(LC_ALL, "Russian");
    pr1();
    pr2();
    pr3();
    return 0;
}