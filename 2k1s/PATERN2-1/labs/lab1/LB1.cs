using System;
public class Student1
{
    private const int MaxAge = 100;
    public const string UniversityName = "BSTU";
    protected const double MinGrade = 2.0;

    private int age;
    public string fullName;
    protected double grade;

    private int PrivateStudentID { get; set; }
    protected int Year { get; set; }
    public string FullName { get; set; }

    public Student1()
    {
        this.PrivateStudentID = new Random().Next(0, 100);
    }
    public Student1(int age, string fullName, double grade)
    {
        this.age = age;
        this.fullName = fullName;
        this.grade = grade;

        this.PrivateStudentID = new Random().Next(0, 100);
    }
    public Student1(Student1 other)
    {
        this.age = other.age;
        this.fullName = other.fullName;
        this.grade = other.grade;
        this.PrivateStudentID = other.PrivateStudentID;
        this.Year = other.Year;

        this.PrivateStudentID = GetID();
    }

    private string GetStudentInfo()
    {
        return $"Age: {age}, Full Name: {fullName}, University name: {UniversityName}, Grade: {grade}, Private Student ID: {PrivateStudentID}";
    }

    public void DisplayInfo()
    {
        Console.WriteLine(GetStudentInfo());
    }

    protected int GetID()
    {
        return new Random().Next(0, 100);
    }
}
public interface I1
{
    string FullName { get; set; }

    void DisplayInfo();

    event EventHandler Graduated;

    string this[int index] { get; set; }
}

public class Student2 : Student1, I1
{
    private const int MaxAge = 90;
    public const string UniversityName = "BSU";

    private int age;
    public string[] subjects = new string[5];

    public event EventHandler Graduated;

    private int PrivateStudentID { get; set; }
    protected bool IsGraduated { get; set; } = false;
    public string this[int index]
    {
        get { return subjects[index]; }
        set { subjects[index] = value; }
    }
    public Student2()
    {
        this.PrivateStudentID = new Random().Next(100, 200);
    }
    public Student2(int age, string fullName, double grade) : base(age, fullName, grade)
    {
        this.PrivateStudentID = new Random().Next(100, 200);
    }
    public Student2(Student2 other) { 
        this.PrivateStudentID = new Random().Next(100, 200); 
    }
    protected void Graduate()
    {
        Console.WriteLine("The student has graduated.");
        IsGraduated = true;
    }
}

class C3
{
    private int privateField = 10;
    protected int protectedField = 20;
    public int publicField = 30; 

    public int ProtectedField => protectedField;

    public C3()
    {
        Console.WriteLine("Конструктор BaseClass вызван");
    }
    public virtual void DisplayInfo()
    {
        Console.WriteLine($"BaseClass: PrivateField = {privateField}, ProtectedField = {protectedField}, PublicField = {publicField}");
    }
}

class C4 : C3
{
    private string derivedField = "Hello from DerivedClass";

    public C4()
    {
        Console.WriteLine("Конструктор DerivedClass вызван");
    }
    public int ProtectedField => protectedField;
    public string DerivedField
    {
        get => derivedField;
        set => derivedField = value;
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"DerivedClass: DerivedField = {derivedField}");
    }
}

class LB1
{
    static void Main()
    {
        Console.WriteLine("Сlass Student1:\n");

        Student1 first = new Student1();
        first.fullName = "John Doe";
        first.DisplayInfo();

        Student1 second = new Student1(21, "Violetta", 4.5);
        second.DisplayInfo();

        Student1 third = new Student1(second);
        third.fullName = "Anna";
        third.DisplayInfo();


        I1 fourth = new Student2(21, "Violetta", 4.5);
        fourth.FullName = "Alina";
        fourth.DisplayInfo();
        fourth[0] = "Math";
        Console.WriteLine(fourth[0]);

        Console.WriteLine("\nСlass Student2:\n");

        I1 fifth = new Student2();
        fifth.FullName = "Denny";
        fifth.DisplayInfo();
        Student2 sixth = new Student2(18, "Ken", 3.3);
        sixth.DisplayInfo();
        I1 seventh = new Student2(sixth);
        seventh.FullName = "Alex";
        seventh.DisplayInfo();


        C4 derived = new C4();

        derived.publicField = 100; 
        Console.WriteLine($"PublicField после изменения: {derived.publicField}");

        Console.WriteLine($"ProtectedField: {derived.ProtectedField}");

        derived.DerivedField = "New value for DerivedField";
        Console.WriteLine($"DerivedField: {derived.DerivedField}");

        derived.DisplayInfo();
    }
}