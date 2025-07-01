using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laba8
{
    internal class FileName
    {
        public delegate string DEL(string str);

        public static event DEL EV;

        public string Method(string str)
        {
            return str + '!';
        }

        void Main() 
        {
            EV += Method;

            string x = EV?.Invoke("Hi");

        }
    }
}
