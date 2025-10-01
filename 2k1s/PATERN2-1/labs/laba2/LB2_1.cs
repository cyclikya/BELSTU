using System;

namespace LB2_1
{
    public interface IStaff
    {
        List<JobVacancy> GetJobVacancies();
        List<Employee> GetEmployees();
        List<JobTitle> GetJobTitles();
        int AddJobTitle(JobTitle jobTitle);
        string PrintJobVacancies();
        bool DelJobTitle(int id);
        bool OpenJobVacancy(JobVacancy jobVacancy);
        bool CloseJobVacancy(int id);
        Employee Recruit(JobVacancy jobVacancy, Person person);
        bool Dismiss(int id, Reason reason);
    }
    public class Organization<T>
    {

        public int Id { get; private set; }
        public string Name { get; protected set; }
        public T ShortName { get; protected set; }
        public string Adress { get; protected set; }
        public DateTime TimeStamp { get; protected set; }
        public Organization() { }
        public Organization(Organization<T> other)
        {
            Id = new Random().Next(0, 100);

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

        public void PrintInfo()
        {
            Console.WriteLine($"Id: {Id}, Название: {Name}({ShortName}), Адрес: {Adress}, Время: {TimeStamp}");
        }
    }

    public class University<T>
    {
        private List<Faculty> faculties = new List<Faculty>();
        public int Id { get; private set; }
        public string Name { get; protected set; }
        public T ShortName { get; protected set; }
        public string Adress { get; protected set; }
        public DateTime TimeStamp { get; protected set; }


        public University() { }

        public University(University<T> other)
        {
            Name = other.Name;
            ShortName = other.ShortName;
            Adress = other.Adress;
            faculties = new List<Faculty>(other.faculties);
        }

        public University(string name, T shortName, string adress)
        {
            Name = name;
            ShortName = shortName;
            Adress = adress;
        }
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


        public List<Faculty> Getfaculties()
        {
            return faculties;
        }

        public void PrintInfo()
        {
         }
        public List<JobTitle> GetJobTitles()
        {
            return new List<JobTitle>();
        }



        public int AddJobTitle(JobTitle jobTitle)
        {

            return jobTitle.Id;
        }

        public List<JobVacancy> GetJobVacancies()
        {
            return new List<JobVacancy>();
        }


        public bool DelJobTitle(int id)
        {

            return true;
        }
        public bool OpenJobVacancy(JobVacancy jobVacancy)
        {
            Console.WriteLine($"Открыта вакансия: {jobVacancy.Title}");
            return true;
        }



        public bool CloseJobVacancy(int id)
        {

            return true;
        }

        public Employee Recruit(JobVacancy jobVacancy, Person employee)
        {

            return new Employee();
        }


        public bool Dismiss(int id, Reason reason)
        {

            return true;
        }



    }
    public class Faculty
    {
        public int Id { get; set; }

        public string FacultyName { get; set; }
        public string ShortName { get; set; }
        public string Address { get; set; }

        protected List<Department> departments = new List<Department>();

        public Faculty(string facultyName, string shortName, string address)
        {
            FacultyName = facultyName;
            ShortName = shortName;
            Address = address;
        }

        public Faculty() { }

        public Faculty(Faculty other)
        {
            Id = other.Id;
            FacultyName = other.FacultyName;
            ShortName = other.ShortName;
            Address = other.Address;
            departments = new List<Department>(other.departments);
        }

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
        public void PrintInfo()
        {
            Console.WriteLine($"Факультет '{FacultyName}' ({ShortName}); {Address}; Количество офисов: {departments.Count}");
        }
        public List<JobVacancy> GetJobVacancies()
        {
            return new List<JobVacancy>();
        }
        public int AddJobTitle(JobTitle jobTitle)
        {
            return jobTitle.Id;
        }
        public bool DelJobTitle(int id)
        {
            return true;
        }
        public bool OpenJobVacancy(JobVacancy jobVacancy)
        {
            Console.WriteLine($"Открыта вакансия: {jobVacancy.Title}");
            return true;
        }

        public bool CloseJobVacancy(int id)
        {
            return true;
        }

        public Employee Recruit(JobVacancy jobVacancy, Person employee)
        {

            return new Employee();
        }


        public bool Dismiss(int id, Reason reason)
        {

            return true;
        }
    }
    public class JobVacancy
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
    public class JobTitle
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
    public class Department
    {
        public string DepartmentName { get; set; }
    }
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Person
    {
        public string Name { get; set; }
    }
    public class Reason
    {
        public string Description { get; set; }
    }


    public class LB2_1
    {
        public static void Main(string[] args)
        {
            Organization<string> org1 = new Organization<string>("Техно Волна", "ТВ", "Ул. Инноваций, 123");

            org1.PrintInfo();

            Organization<string> copiedOrg = new Organization<string>(org1);
            copiedOrg.PrintInfo();

            Organization<string> org2 = new Organization<string>("Пик Вершина", "ПВ", "Ул. Горная, 456");

            org2.PrintInfo();

            University<string> uni1 = new University<string>("Университет Будущего", "УБ", "Прсп. Знаний, 789");
            uni1.PrintInfo();

            University<string> copiedUni = new University<string>(uni1);
            copiedUni.PrintInfo();

            Faculty faculty1 = new Faculty("Информатика и ИИ", "ИИИ", "Технопарк, 101");
            int facultyCount = uni1.AddFaculty(faculty1);
            Console.WriteLine($"Количество факультетов после добавления - {facultyCount}");

            Faculty copiedFaculty = new Faculty(faculty1);
            Console.WriteLine($"Копия факультета: {copiedFaculty.FacultyName}, Короткое имя: {copiedFaculty.ShortName}, Адрес: {copiedFaculty.Address}");

            bool updated = uni1.UpdFaculty(faculty1);
            Console.WriteLine($"Факультет обновлен - {updated}");

            bool removed = uni1.DelFaculty(faculty1.Id);
            Console.WriteLine($"Факультет удален - {removed}");

            Faculty faculty2 = new Faculty("Механика", "МЕХ", "Ул. Индустрии, 202");

            Department department1 = new Department { DepartmentName = "Робототехника" };
            int departmentCount = faculty2.AddDepartment(department1);
            Console.WriteLine($"Количество кафедр после добавления - {departmentCount}");

            JobVacancy vacancy1 = new JobVacancy { Id = 1, Title = "Преподаватель робототехники" };
            uni1.OpenJobVacancy(vacancy1);
            Console.WriteLine($"Открыта вакансия с ID - {vacancy1.Id}");

            Person candidate = new Person { Name = "Иван Иванов" };
            bool vacancyClosed = uni1.CloseJobVacancy(vacancy1.Id);
            Console.WriteLine($"Вакансия закрыта - {vacancyClosed}");
        }
    }

}