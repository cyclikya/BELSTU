using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using labs10_11.Data;
using labs10_11.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace labs10_11.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Department
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public partial class MainViewModel : ObservableObject
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObservableCollection<Person> Persons { get; } = new();
        public ObservableCollection<Department> Departments { get; } = new();

        [ObservableProperty]
        private string newPersonName;
        [ObservableProperty]
        private int newPersonAge;

        public MainViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            LoadData();
        }

        [RelayCommand]
        private async Task LoadData()
        {
            Persons.Clear();
            Departments.Clear();

            foreach (var p in await _unitOfWork.Persons.GetAllAsync())
                Persons.Add(p);

            foreach (var d in await _unitOfWork.Departments.GetAllAsync())
                Departments.Add(d);
        }

        [RelayCommand]
        private async Task AddPerson()
        {
            var person = new Person { Name = NewPersonName, Age = NewPersonAge };
            await _unitOfWork.Persons.AddAsync(person);
            await _unitOfWork.SaveAsync();
            Persons.Add(person);
        }
    }
}
