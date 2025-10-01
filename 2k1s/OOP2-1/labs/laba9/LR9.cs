using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Service : IOrderedDictionary
{
    private readonly ConcurrentDictionary<int, object> _services = new();
    private int _orderCounter = 0;

    public object this[int index] { get => GetByIndex(index); set => UpdateAtIndex(index, value); }
    public object this[object key] { get => _services[(int)key]; set => _services[(int)key] = value; }
    public ICollection Keys => (ICollection)_services.Keys;
    public ICollection Values => (ICollection)_services.Values;
    public bool IsReadOnly => false;
    public bool IsFixedSize => false;
    public int Count => _services.Count;
    public object SyncRoot => this;
    public bool IsSynchronized => false;

    public int Id { get; set; }
    public string Name { get; set; }

    public void Add(object key, object value)
    {
        _services.TryAdd((int)key, value);
    }
    public void Clear()
    {
        _services.Clear();
    }
    public bool Contains(object key)
    {
        return _services.ContainsKey((int)key);
    }
    public IDictionaryEnumerator GetEnumerator()
    {
        return _services.GetEnumerator() as IDictionaryEnumerator;
    }
    public void Insert(int index, object key, object value)
    {
        if (_services.TryAdd((int)key, value))
            _orderCounter++;
    }
    public void Remove(object key)
    {
        _services.TryRemove((int)key, out _);
    }
    public void RemoveAt(int index)
    {
        int key = GetKeyByIndex(index);
        _services.TryRemove(key, out _);
    }
    private int GetKeyByIndex(int index)
    {
        int counter = 0;
        foreach (var key in _services.Keys)
        {
            if (counter == index) return key;
            counter++;
        }
        throw new IndexOutOfRangeException();
    }
    private object GetByIndex(int index)
    {
        int key = GetKeyByIndex(index);
        return _services[key];
    }
    private void UpdateAtIndex(int index, object value)
    {
        int key = GetKeyByIndex(index);
        _services[key] = value;
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _services.GetEnumerator();
    }
    public void CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }
}

public class Program
{
    public static void Main()
    {
        var serviceCollection = new Service();
        serviceCollection.Add(1, "Уборка");
        serviceCollection.Add(2, "Ремонт");
        serviceCollection.Add(3, "Доставка");

        Console.WriteLine("Коллекция Услуг:");
        foreach (var key in serviceCollection.Keys)
            Console.WriteLine($"Ключ: {key}, Значение: {serviceCollection[key]}");

        serviceCollection.Remove(2);
        Console.WriteLine("\nПосле удаления ключа 2:");
        foreach (var key in serviceCollection.Keys)
            Console.WriteLine($"Ключ: {key}, Значение: {serviceCollection[key]}\n");




        var queue = new Queue<string>();
        queue.Enqueue("Первая услуга");
        queue.Enqueue("Вторая услуга");
        queue.Enqueue("Третья услуга");
        queue.Enqueue("Четвертая услуга");

        Console.WriteLine("\nОчередь услуг:");
        foreach (var item in queue)
            Console.WriteLine(item);

        queue.Dequeue();
        queue.Dequeue();
        Console.WriteLine("\nПосле удаления элемента:");
        foreach (var item in queue)
            Console.WriteLine(item);

        queue.Enqueue("Пятая услуга");
        Console.WriteLine("\nПосле добавления элемента:");
        foreach (var item in queue)
            Console.WriteLine(item);

        var queueDictionary = new Dictionary<int, string>();
        int index = 1;
        foreach (var item in queue)
        {
            queueDictionary.Add(index, item);
            index++;
        }

        Console.WriteLine("\nСловарь услуг из очереди:");
        foreach (var line in queueDictionary)
            Console.WriteLine($"Ключ: {line.Key}, Значение: {line.Value}");

        string searchValue = "Пятая услуга";
        bool found = queueDictionary.ContainsValue(searchValue);
         Console.WriteLine($"\nЗначение '{searchValue}' найдено: {found}\n\n");


        ObservableCollection<Service> services = new ObservableCollection<Service>();

        services.CollectionChanged += OnCollectionChanged;

        services.Add(new Service { Id = 1, Name = "Уборка" });
        services.Add(new Service { Id = 2, Name = "Ремонт" });
        services.Add(new Service { Id = 3, Name = "Доставка" });

        Console.WriteLine("\nКоллекция услуг:");
        foreach (var service in services)
            Console.WriteLine(service);

        services.Remove(services[1]);
        Console.WriteLine("\nПосле удаления услуги с Id = 2:");
        foreach (var service in services)
            Console.WriteLine(service);

        Console.WriteLine("\nОчищаем коллекцию:");
        services.Clear();
    }

    public static void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                Console.WriteLine("Добавлены элементы:");
                foreach (Service newItem in e.NewItems)
                {
                    Console.WriteLine($"{newItem}: {newItem.Id} - {newItem.Name}"); 
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                Console.WriteLine("Удалены элементы:");
                foreach (Service oldItem in e.OldItems)
                {
                    Console.WriteLine($"{oldItem}: {oldItem.Id} - {oldItem.Name}");
                }
                break;

            case NotifyCollectionChangedAction.Reset:
                Console.WriteLine("Коллекция была очищена.");
                break;

            default:
                Console.WriteLine("Произошло другое изменение.");
                break;
        }
    }
}
