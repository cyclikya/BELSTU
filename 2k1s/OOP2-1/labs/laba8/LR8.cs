using System.Runtime.InteropServices;
using System.Text.Json;
using System.Xml.Serialization;
using static LR8.Program;

//Создать класс  Программист с событиями Переименовать  и  
//Новое свойство. В main создать некоторое количество
//объектов (языков программирования).
//Подпишите объекты на события произвольным образом.
//Реакция на события  может быть следующая: изменение имени/версии,
//добавление новых операций,
//технологий или понятий.
//Проверить результаты изменения состояния объектов
//после наступления событий,
//возможно не однократном 


namespace LR8
{
    public class Program
    {
        public class ProgramLanguage
        {
            public delegate void RenameEventHandler(string oldName, string newName);

            public event RenameEventHandler OnRename;
            public event Action<string> OnNewProperty;

            public string Name { get; set; }
            public string Version { get; set; }
            public List<string> Operations { get; set; }

            public ProgramLanguage(string name, string version)
            {
                Name = name;
                Version = version;
                Operations = new List<string>();
            }

            public void Rename(string newName) => OnRename?.Invoke(Name, newName);

            public void AddNewProperty(string property)
            {
                Operations.Add(property);
                OnNewProperty?.Invoke(property);
            }
        }

        public class Operations
        {
            private readonly Func<string, string>[] processingSteps;

            public Operations()
            {
                processingSteps = new Func<string, string>[]
                {
                    str => str.Trim(),
                    str => str.Replace(",", ""),
                    str => str.Replace("!", ""),
                    str => str.ToUpper(),
                    str => string.Join(" ", str.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                };
            }
            public string Process(string input)
            {
                foreach (var step in processingSteps)
                {
                    input = step(input);
                }
                return input;
            }
        }
        class LR8
        {
            static void Main()
            {
                ProgramLanguage cSharp = new ProgramLanguage("C#", "9.0");
                ProgramLanguage java = new ProgramLanguage("Java", "17");

                cSharp.OnRename += (oldName, newName) => Console.WriteLine($"Язык {oldName} был переименован в {newName}.");
                java.OnRename += (oldName, newName) => Console.WriteLine($"Язык {oldName} был переименован в {newName}.");

                cSharp.OnNewProperty += (property) => Console.WriteLine($"Язык C# получил новую технологию: {property}");
                java.OnNewProperty += (property) => Console.WriteLine($"Язык Java получил новое понятие: {property}");

                cSharp.Rename("CSharp");
                java.Rename("Java SE");
                cSharp.AddNewProperty("LINQ");
                java.AddNewProperty("Streams API");

                cSharp.AddNewProperty("Async/Await");
                java.AddNewProperty("Lambda Expressions");

                Console.WriteLine("\nСписок технологий для C#:");
                foreach (var operation in cSharp.Operations)
                {
                    Console.WriteLine($"- {operation}");
                }
                Console.WriteLine("\nСписок технологий для Java:");
                foreach (var operation in java.Operations)
                {
                    Console.WriteLine($"- {operation}");
                }



                Operations st = new Operations();

                string input = "  Строка   строка, строка строка СТРОКА!  ";
                string processed = st.Process(input);
                Console.WriteLine($"Исходная строка: {input}");
                Console.WriteLine($"Обработанная строка: {processed}");

            }
        }
    }
}