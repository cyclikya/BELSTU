using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using labs10_11.Data;
using labs10_11.Models;
using System.Collections.ObjectModel;

namespace labs10_11.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IUnitOfWork _unitOfWork;

        public ObservableCollection<Person> Persons { get; } = new();
        public ObservableCollection<Department> Departments { get; } = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddPersonCommand))]
        private string newPersonName;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddPersonCommand))]
        private int newPersonAge;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DeletePersonCommand))]
        private Person selectedPerson;

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

        [RelayCommand(CanExecute = nameof(CanAddPerson))]
        private async Task AddPerson()
        {
            var person = new Person { Name = NewPersonName, Age = NewPersonAge };
            await _unitOfWork.Persons.AddAsync(person);
            await _unitOfWork.SaveAsync();
            Persons.Add(person);

            NewPersonName = string.Empty;
            NewPersonAge = 0;
        }

        private bool CanAddPerson()
        {
            return !string.IsNullOrWhiteSpace(NewPersonName) && NewPersonAge > 0;
        }

        [RelayCommand(CanExecute = nameof(CanDeletePerson))]
        private async Task DeletePerson()
        {
            if (SelectedPerson == null) return;
            _unitOfWork.Persons.Remove(SelectedPerson);
            await _unitOfWork.SaveAsync();
            Persons.Remove(SelectedPerson);
            SelectedPerson = null;
        }

        private bool CanDeletePerson()
        {
            return SelectedPerson != null;
        }
    }
}
