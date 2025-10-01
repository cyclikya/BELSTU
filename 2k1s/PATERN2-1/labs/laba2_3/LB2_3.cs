using System;
using System.Collections.Generic;
using System.Linq;

namespace LB2_3
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

    public class Organization<T> : IStaff
    {
        public int Id { get; private set; }
        public string Name { get; protected set; }
        public T ShortName { get; protected set; }
        public string Address { get; protected set; }
        public DateTime TimeStamp { get; protected set; }

        private List<JobVacancy> jobVacancies = new List<JobVacancy>();
        private List<Employee> employees = new List<Employee>();
        private List<JobTitle> jobTitles = new List<JobTitle>();

        public Organization() { }

        public Organization(string name, T shortName, string address)
        {
            Name = name;
            ShortName = shortName;
            Address = address;
            TimeStamp = DateTime.Now;
        }

        public Organization(Organization<T> other)
        {
            Id = other.Id;
            Name = other.Name;
            ShortName = other.ShortName;
            Address = other.Address;
            TimeStamp = other.TimeStamp;
        }

        public void SetId(int id)
        {
            if (id >= 0)
            {
                this.Id = id;
            }
            else
            {
                throw new ArgumentException("Id не должен быть отрицательным");
            }
        }

        public List<JobVacancy> GetJobVacancies() => jobVacancies;

        public List<Employee> GetEmployees() => employees;

        public List<JobTitle> GetJobTitles() => jobTitles;

        public int AddJobTitle(JobTitle jobTitle)
        {
            jobTitles.Add(jobTitle);
            return jobTitles.Count;
        }

        public string PrintJobVacancies()
        {
            return string.Join("\n", jobVacancies.Select(v => v.Title));
        }

        public bool DelJobTitle(int id)
        {
            var jobTitle = jobTitles.FirstOrDefault(j => j.Id == id);
            if (jobTitle != null)
            {
                jobTitles.Remove(jobTitle);
                return true;
            }
            return false;
        }

        public bool OpenJobVacancy(JobVacancy jobVacancy)
        {
            jobVacancies.Add(jobVacancy);
            return true;
        }

        public bool CloseJobVacancy(int id)
        {
            var jobVacancy = jobVacancies.FirstOrDefault(j => j.Id == id);
            if (jobVacancy != null)
            {
                jobVacancies.Remove(jobVacancy);
                return true;
            }
            return false;
        }

        public Employee Recruit(JobVacancy jobVacancy, Person person)
        {
            var employee = new Employee { Person = person, JobVacancy = jobVacancy };
            employees.Add(employee);
            return employee;
        }

        public bool Dismiss(int id, Reason reason)
        {
            var employee = employees.FirstOrDefault(e => e.Id == id);
            if (employee != null)
            {
                employees.Remove(employee);
                return true;
            }
            return false;
        }

        public void PrintInfo()
        {
            Console.WriteLine($"Id: {Id}, Название: {Name}({ShortName}), Адрес: {Address}, Время: {TimeStamp}");
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

        public University(string name, string shortName, string address) : base(name, shortName, address) { }

        public int AddFaculty(Faculty faculty)
        {
            faculties.Add(faculty);
            return faculties.Count;
        }

        public bool DelFaculty(int id)
        {
            var faculty = faculties.FirstOrDefault(f => f.Id == id);
            if (faculty != null)
            {
                faculties.Remove(faculty);
                return true;
            }
            return false;
        }

        public bool UpdFaculty(Faculty faculty)
        {
            var index = faculties.FindIndex(f => f.Id == faculty.Id);
            if (index >= 0)
            {
                faculties[index] = faculty;
                return true;
            }
            return false;
        }

        public List<Faculty> GetFaculties() => faculties;

        public new void PrintInfo()
        {
            Console.WriteLine($"Университет: {Name}({ShortName}), Адрес: {Address}");
        }
    }

    public class Faculty : Organization<string>
    {
        protected List<Department> departments = new List<Department>();

        public Faculty() : base() { }

        public Faculty(Faculty other) : base(other) { }

        public Faculty(string name, string shortName, string address) : base(name, shortName, address) { }

        public int AddDepartment(Department department)
        {
            departments.Add(department);
            return departments.Count;
        }

        public bool DelDepartment(int departmentId)
        {
            var department = departments.FirstOrDefault(d => d.id == departmentId);
            if (department != null)
            {
                departments.Remove(department);
                return true;
            }
            return false;
        }

        public bool UpdDepartment(Department department)
        {
            var index = departments.FindIndex(d => d.id == department.id);
            if (index >= 0)
            {
                departments[index] = department;
                return true;
            }
            return false;
        }

        public List<Department> GetDepartments() => departments;

        public new void PrintInfo()
        {
            Console.WriteLine($"Факультет: {Name}({ShortName}), Адрес: {Address}");
        }
    }

    public class JobVacancy
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public JobVacancy JobVacancy { get; set; }
        public Person Person { get; set; }
    }

    public class Department
    {
        public int id { get; set; }
        public string DepartmentName { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
    }

    public class Reason
    {
        public string Description { get; set; }
    }

    public class JobTitle
    {
        public int Id { get; set; }
        public string Title { get; set; }
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


}