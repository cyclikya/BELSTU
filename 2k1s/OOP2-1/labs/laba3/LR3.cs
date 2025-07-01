
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace LR3
{
    public class Stack
    {
        public List<int> list;

        public Stack()
        {
            list = new List<int>();
        }

        public static Stack operator +(Stack st, int el)
        {
            st.list.Add(el);
            return st;
        }

        public static int operator -(Stack st, int el)
        {
            if (st.isEmpty())
                Console.WriteLine("Стек пустой. Невозможно извлечь элемент.");

            int x = st.list[el];
            st.list.RemoveAt(el);
            return x;
        }

        private bool isEmpty()
        {
            if (this.list.Count() == 0)
            {
                return true;
            } else
            {
                return false;
            }

        }

        public static bool operator true(Stack st)
        {
            return st.list.Count == 0;
        }

        public static bool operator false(Stack st)
        {
            return !(st.list.Count == 0);
        }

        public static Stack operator >(Stack st1, Stack st2)
        {
            st2.list.Sort();
            st1.list = st2.list;
            return st1;
        }

        public static Stack operator <(Stack st1, Stack st2)
        {
            st2.list.Sort();
            st1.list = st2.list;
            return st1;
        }

        public override string ToString()
        {
            return string.Join(", ", list);
        }

        public class Production
        {
            public int Id { get; set; }
            public string OrganizationName { get; set; }

            public Production(int id, string orgName)
            {
                Id = id;
                OrganizationName = orgName;
            }
            public override string ToString()
            {
                return $"ID: {Id}, Организация: {OrganizationName}";
            }
        }
        public class Developer
        {
            public string FullName { get; set; }
            public int Id { get; set; }
            public string Department { get; set; }

            public Developer(string fullName, int id, string department)
            {
                FullName = fullName;
                Id = id;
                Department = department;
            }
            public override string ToString()
            {
                return $"Имя: {FullName}, ID: {Id}, Отдел: {Department}";
            }
        }

        public Production production { get; set; }
        public Developer developer { get; set; }

        public Stack(List<int> list, Production production, Developer developer)
        {
            this.list = list;
            this.production = production;
            this.developer = developer;
        }
    }

    public static class StatisticOperatin
    {
        public static int Sum(Stack st)
        {
            return st.list.Sum();
        }
        public static int diffBtiwMaxNMin(Stack st)
        {
            return st.list.Max() - st.list.Min();
        }
        public static int Count(Stack st)
        {
            return st.list.Count();
        }
    }

    public static class ExpensionsMethods
    {
        public static int GetCountOfSent(this string str)
        {
            int cup = 0;
            foreach (char c in str)
            {
                if (c == '.' || c == '!' || c == '?')
                {
                    cup++;
                }
            }
            return cup;
        }

        public static double GetMid(this Stack st)
        {
            if (st.list.Count == 0)
                return 0;

            int sum = st.list.Sum();
            return (double)sum / st.list.Count;
        }
    }

    class LR3 
    { 
        static void Main()
        {
            Stack s1 = new Stack();

            s1 = s1 + 1 + 3 + 5 + 10;
            Console.WriteLine($"{s1.ToString()}");

            int x = s1 - 2; // извлечение 2-го эл-та

            Console.WriteLine($"\n{s1.ToString()}");
            Console.WriteLine($"Извлеченный элемент: {x}");

            Console.WriteLine($"\n{s1.ToString()}");
            if (s1)
            {
                Console.WriteLine("Стек пустой");
            }
            else
            {
                Console.WriteLine("Стек полный");
            }

            Console.WriteLine($"\n{s1.ToString()}");
            var production = new Stack.Production(5, "Назваине организации");
            var developer = new Stack.Developer("Даниил", 4, "Москва");

            Stack stack = new Stack(new List<int> { 3, 2, 1 }, production, developer);
            Console.WriteLine($"Production: {stack.production}");
            Console.WriteLine($"Developer: {stack.developer}");

            Console.WriteLine($"\n{s1.ToString()}");
            Console.WriteLine($"Cумма из StatisticOperatin и s1: {StatisticOperatin.Sum(s1)}");
            Console.WriteLine($"Разница между максимальным и минимальным элементом из StatisticOperatin и s1: {StatisticOperatin.diffBtiwMaxNMin(s1)}");
            Console.WriteLine($"Количество элементов из StatisticOperatin и s1: {StatisticOperatin.Count(s1)}");

            Console.WriteLine($"\n{s1.ToString()}");

            string str = "dfgh.fsdghf, zdxf! sg?";
            Console.WriteLine(str.GetCountOfSent);
            Console.WriteLine($"Кол-во предложений из StatisticOperatin и str: {ExpensionsMethods.GetCountOfSent(str)}");
            Console.WriteLine($"Количество элементов из StatisticOperatin и s1: {ExpensionsMethods.GetMid(s1)}");
        }
    }
}