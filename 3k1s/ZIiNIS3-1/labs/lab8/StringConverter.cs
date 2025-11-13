using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LB88
{
    public class StringConverter
    {
        public static string StringToASCII(string message)
        {
            var encoding1251 = Encoding.GetEncoding(1251);
            string binString = string.Empty;
            var decString = encoding1251.GetBytes(message.ToCharArray());

            foreach (var letter in decString)
            {
                binString += Convert.ToString(letter, 2);
            }
            return binString;
        }
    }
}
