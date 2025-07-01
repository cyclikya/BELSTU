#include <iostream>
#include <fstream>
#include <sstream> 
using namespace std;

bool isVowel(char c) 
{
    c = tolower(c);
    return (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u' || c == 'y' || c == 'а' || c == 'е' || c == 'ё' || c == 'и' || c == 'о' || c == 'у' || c == 'ы' || c == 'э' || c == 'ю' || c == 'я');
}

void ex1()
{
    ofstream f1("FILE1.txt");
    f1 << "три зелёных яблока \n" << "пять ребят \n" << "дуб \n" << "шесть красных карандаша ";
    f1.close();

    ofstream f2("FILE2.txt");

    char baff1[80];
    ifstream f1r("FILE1.txt");

    while (f1r.getline(baff1, 80, '\n')) 
    {
        stringstream line(baff1);
        string word;
        int n = 0;

        while (line >> word) {
            n++;
        }

        if (n > 2) {
            f2 << baff1 << endl;
            cout << baff1 << "\n";
        }
    }
    f1r.close();
    f2.close();

    char baff2[80];
    ifstream f2r("FILE2.txt");
    
    int VowelsCount = 0;
    int wordNumberWithMaxVowels = 0;
    int WordNumber = 0;

    while (f2r.getline(baff2, 80, '\n')) 
    {
        stringstream line(baff2);
        string word;
        int vowelsCount = 0;

        while (line >> word) 
        {
            WordNumber++;
            int WordVowelsCount = 0;
            for (char c : word) 
            {
                if (isVowel(c)) 
                {
                    WordVowelsCount++;
                }
            }
            if (WordVowelsCount > vowelsCount) 
            {
                vowelsCount = WordVowelsCount;
                if (vowelsCount > VowelsCount) 
                {
                    VowelsCount = vowelsCount;
                    wordNumberWithMaxVowels = WordNumber;
                }
            }
        };
    }
    cout << "Номер слова в строке, в котором больше всего гласных букв: " << wordNumberWithMaxVowels;
    f2r.close();
}

void ex2() {
    string l;
    cin >> l;

    ofstream f("FILE.txt");
    f << l;
    f.close();
    
    ifstream fread("FILE.txt");

    string line;
    getline(fread, line);
    fread.close();

    int openRound = 0;
    int closeRound = 0;
    int openSquare = 0;
    int closeSquare = 0;
    int openCurly = 0;
    int closeCurly = 0;

    for (char c : line) {
        switch (c) {
        case '(': openRound++; 
            break;
        case ')': closeRound++; 
            break;
        case '[': openSquare++; 
            break;
        case ']': closeSquare++; 
            break;
        case '{': openCurly++; 
            break;
        case '}': closeCurly++; 
            break;
        default: break;
        }
    }
    cout << "Количество скобок различного вида:" << endl;
    cout << "Открывающие круглые скобки: " << openRound << endl;
    cout << "Закрывающие круглые скобки: " << closeRound << endl;
    cout << "Открывающие квадратные скобки: " << openSquare << endl;
    cout << "Закрывающие квадратные скобки: " << closeSquare << endl;
    cout << "Открывающие фигурные скобки: " << openCurly << endl;
    cout << "Закрывающие фигурные скобки: " << closeCurly << endl;
}

void main()
{
    setlocale(LC_ALL, "rus");
    //ex1();
    ex2();
}
