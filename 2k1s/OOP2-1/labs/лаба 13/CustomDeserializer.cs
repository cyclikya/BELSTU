using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using lab13;

namespace lab13
{
    public static class CustomDeserializer
    {
        public static List<Continent> JSONDeserializer()
        {
            using (FileStream fs = File.OpenRead("Continent.json"))
            {
                byte[] array = new byte[fs.Length];
                fs.Read(array, 0, array.Length);
                string circle = Encoding.Default.GetString(array);
                return JsonSerializer.Deserialize<List<Continent>>(circle);
            }
        }
        public static List<Continent> XMLDeserializer()
        {
            XmlSerializer xml = new XmlSerializer(typeof(List<Continent>));
            using (FileStream fs = File.OpenRead("Continent.xml"))
            {
                return xml.Deserialize(fs) as List<Continent>;

            }
        }
        //public static List<Continent> BinDeserializer()
        //{
        //    BinaryFormatter formatter = new BinaryFormatter();
        //    using (FileStream fs = File.OpenRead("Continent.bin"))
        //    {
        //        return formatter.Deserialize(fs) as List<Continent>;
        //    }
        //}
    }
}