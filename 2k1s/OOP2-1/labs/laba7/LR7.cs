using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;

namespace LR7
{ 
    public interface IGen<T>
    {
        void Add(T item);
        bool Remove(T item);
        void ShowAll();
    }

    public class Stack<T> : IGen<T> // where T : struct
    {
        public List<T> items;

        public Stack()
        {
            items = new List<T>();
        }

        public static Stack<T> operator +(Stack<T> st, T el)
        {
            st.items.Add(el);
            return st;
        }
        public static T operator -(Stack<T> st, int i)
        {
            T x = st.items[i];
            st.items.RemoveAt(i);
            return x;
        }
        public static bool operator true(Stack<T> st)
        {
            return st.items.Count == 0;
        }
        public static bool operator false(Stack<T> st)
        {
            return !(st.items.Count == 0);
        }
        public void Add(T item)
        {
            try
            {
                items.Add(item);
                Console.WriteLine($"{item} добавлено.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Метод Add завершен.");
            }
        }
        public bool Remove(T item)
        {
            try
            {
                if (items.Remove(item))
                {
                    Console.WriteLine($"{item} элемент удален.");
                    return true;
                }
                else
                {
                    Console.WriteLine($"{item} элемент не найден.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении: {ex.Message}");
                return false;
            }
            finally
            {
                Console.WriteLine("Метод Remove завершен.");
            }
        }
        public void ShowAll()
        {
            if (items.Count == 0)
            {
                Console.WriteLine("Коллекция пуста.");
            }
            else
            {
                Console.WriteLine("Содержимое коллекции:");
                foreach (var item in items)
                {
                    Console.WriteLine(item.ToString());
                }
            }
        }
        public T Find(Func<T, bool> predicate)
        {
            foreach (var item in items)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            throw new InvalidOperationException("Элемент, удовлетворяющий условию, не найден.");
        }

        public static class StatisticOperation
        {
            public static int Sum(Stack<int> list)
            {
                return list.items.Sum();
            }

            public static int CountEl(Stack<int> list)
            {
                return list.items.Count;
            }

            public static int Diff(Stack<int> list)
            {
                return list.items.Max() - list.items.Min();
            }
        }

        public void SaveToFile(string filePath, string format)
        {
            if (format.ToLower() == "json")
            {
                string json = JsonSerializer.Serialize(items);
                File.WriteAllText(filePath, json);
                Console.WriteLine("Данные сохранены в JSON файл.");
            }
            else if (format.ToLower() == "xml")
            {
                XmlSerializer xmlSerializer = new XmlSerializer(items.GetType());
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    xmlSerializer.Serialize(writer, items);
                }
                Console.WriteLine("Данные сохранены в XML файл.");
            }
            else
            {
                Console.WriteLine("Неизвестный формат.");
            }
        }

        public void LoadFromFile(string filePath, string format)
        {
            if (File.Exists(filePath))
            {
                if (format.ToLower() == "json")
                {
                    string json = File.ReadAllText(filePath);
                    items = JsonSerializer.Deserialize<List<T>>(json);
                    Console.WriteLine("Данные загружены из JSON файла.");
                }
                else if (format.ToLower() == "xml")
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(items.GetType());
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        items = (List<T>)xmlSerializer.Deserialize(reader);
                    }
                    Console.WriteLine("Данные загружены из XML файла.");
                }
                else
                {
                    Console.WriteLine("Неизвестный формат.");
                }
            }
            else
            {
                Console.WriteLine("Файл не найден.");
            }
        }

    }

    public class Land
    {
        public string Name;

        public Land(string name)
        {
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
    }

    class LR7
    {
        static void Main()
        {
            Console.WriteLine("Пример с целыми числами:");
            Stack<int> intColl = new Stack<int>();
            intColl.Add(10);
            intColl.Add(20);
            intColl.ShowAll();
            intColl.Remove(10);
            intColl.ShowAll();

            Console.WriteLine("\nПример с вещественными числами:");
            Stack<double> doubleColl = new Stack<double>();
            doubleColl.Add(10.5);
            doubleColl.Add(20.5);
            doubleColl.ShowAll();
            doubleColl.Remove(10.5);
            doubleColl.ShowAll();

            Land europe = new Land("Европа");
            Land asia = new Land("Азия");
            Console.WriteLine("\nПример с пользовательским классом:");
            Stack<Land> landColl = new Stack<Land>();
            landColl.Add(europe);
            landColl.Add(asia);
            landColl.ShowAll();
            landColl.Remove(asia);
            landColl.ShowAll();

            Console.WriteLine("\nПоиск в landColl:");
            try
            {
                Land foundLand = landColl.Find(land => land.Name == "Европа");
                Console.WriteLine($"Найден элемент: {foundLand}\n");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }

            string filePathJson = "data.json";
            intColl.SaveToFile(filePathJson, "json");
            intColl.LoadFromFile(filePathJson, "json");

            string filePathXml = "data.xml";
            intColl.SaveToFile(filePathXml, "xml");
            intColl.LoadFromFile(filePathXml, "xml");
        }
    }
}