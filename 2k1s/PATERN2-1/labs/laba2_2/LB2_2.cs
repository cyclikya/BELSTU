using System;
using System.Net;

public class Organization<T>
{
    public int Id { get; private set; }
    public string Name { get; protected set; }
    public T ShortName { get; protected set; }
    public string Adress { get; protected set; }
    public DateTime TimeStamp { get; protected set; }
    public Organization()
    {

    }
    public Organization(Organization<T> other)
    {
        Id = other.Id;
        Name = other.Name;
        ShortName = other.ShortName;
        Adress = other.Adress;
        TimeStamp = other.TimeStamp;
    }
    public Organization(string name, T shortName, string adress)
    {
        Name = name;
        ShortName = shortName;
        Adress = adress;
        TimeStamp = DateTime.Now;
    }

    public void SetId(int Id)
    {
        if (Id >= 0)
        {
            this.Id = Id;
        }
        else
        {
            throw new ArgumentException("Id не может быть отрицательным.");
        }
    }

    public void PrintInfo()
    {
        Console.WriteLine($"Id: {Id}, Имя: {Name}, Короткое имя: {ShortName}, Адресс: {Adress}, Время: {TimeStamp}");
    }
}
public class University : Organization<string>
{
    protected List<Faculty> faculties = new List<Faculty>();
    public University() : base() { }

    public University(University other) : base(other)
    {
        faculties = new List<Faculty>(other.faculties);
    }
    public University(string name, string shortName, string adress) : base(name, shortName, adress) { }
    public int AddFaculty(Faculty faculty)
    {
        faculties.Add(faculty);
        return faculties.Count;
    }

    public bool DelFaculty(int id)
    {
        var faculty = faculties.Find(f => f.Id == id);
        if (faculty != null)
        {
            faculties.Remove(faculty);
            return true;
        }
        return false;
    }

    public bool UpdFaculty(Faculty faculty)
    {
        if (VerFaculty(faculty.Id))
        {
            var index = faculties.FindIndex(f => f.Id == faculty.Id);
            faculties[index] = faculty;
            return true;
        }
        return false;
    }

    private bool VerFaculty(int id)
    {
        return faculties.Exists(f => f.Id == id);
    }


    public List<Faculty> GetFaculties()
    {
        return faculties;
    }


    public void PrintInfo()
    {
        Console.WriteLine($"Университет: {Name}({ShortName}), Адрес:{Adress}");
    }

}
public class Faculty : Organization<string>
{
    protected List<Department> departments = new List<Department>();
    public Faculty() : base() { }

    public Faculty(Faculty other) : base(other)
    {
        departments = new List<Department>(other.departments);
    }
    public Faculty(string name, string shortName, string adress) : base(name, shortName, adress) { }
    public int AddDepartment(Department department)
    {
        departments.Add(department);
        return departments.Count;
    }

    public bool DelDepartment(int departmentId)
    {

        if (VerDepartment(departmentId))
        {
            departments.RemoveAt(departmentId);
            return true;
        }
        return false;
    }

    public bool UpdDepartment(Department department)
    {
        var index = departments.FindIndex(d => d.DepartmentName == department.DepartmentName);
        if (index >= 0)
        {
            departments[index] = department;
            return true;
        }
        return false;
    }


    private bool VerDepartment(int departmentId)
    {

        return departmentId >= 0 && departmentId < departments.Count;
    }

    public List<Department> GetDepartments()
    {
        return departments;
    }

    public new void PrintInfo()
    {
        Console.WriteLine($"Факультет: {Name}({ShortName}), Адрес: {Adress}");
    }
}
public class Department
{
    public int id { get; set; }
    public string DepartmentName { get; set; }
}
public class Program
{
    public static void Main(string[] args)
    {
        Organization<string> org1 = new Organization<string>("Бел Гос Технологический Университет", "БГТУ", "Свердлова 13");
        org1.SetId(1);
        org1.PrintInfo();

        University uni1 = new University("Бел Гос Университет", "БГУ", "прсп. Независимости 4");

        uni1.PrintInfo();

        Faculty faculty1 = new Faculty("Информационные технологии", "ФИТ", "каб. 101-4");
        faculty1.SetId(3);
        faculty1.PrintInfo();

        uni1.AddFaculty(faculty1);
        Console.WriteLine($"Количество факультетов в {uni1.Name}: {uni1.GetFaculties().Count}");

        Department dep1 = new Department { id = 5, DepartmentName = "Северный" };
        faculty1.AddDepartment(dep1);
        faculty1.PrintInfo();

        Organization<string> copiedOrg = new Organization<string>(org1);
        copiedOrg.PrintInfo();

        University copiedUni = new University(uni1);
        copiedUni.PrintInfo();

        Faculty copiedFaculty = new Faculty(faculty1);
        copiedFaculty.PrintInfo();
    }
}
