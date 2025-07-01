using System;

namespace laba8
{
    internal class Program
    {
        public class C
        {
            public delegate string DEL(string str);

            public event DEL EV;

            public C() { }

            public void Method(string str)
            {
                EV(str);
            } 
        }

        static void Main()
        {
            C c = new C();

            c.EV += (string str) =>
            {
                Console.WriteLine(str + '!');
                return str + '!';
            };

            c.Method("Hi");

        }
    }
}
