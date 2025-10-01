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
    static class CustomSerializer
    {
        public static void XMLSerializer(List<Continent> candy)
        {
            XmlSerializer xml = new XmlSerializer(typeof(List<Continent>));
            using (FileStream fs = new FileStream("Continent.xml", FileMode.OpenOrCreate))
            {
                xml.Serialize(fs, candy);
            }
            Console.WriteLine("Файл Continent.xml успешно создан.");
        }

        //public static void BinSerializer(List<Continent> candy)
        //{
        //    BinaryFormatter formatter = new BinaryFormatter();
        //    using (FileStream fs = new FileStream("Continent.bin", FileMode.OpenOrCreate))
        //    {
        //        formatter.Serialize(fs, candy);
        //    }
        //    Console.WriteLine("Файл Continent.bin успешно создан.");
        //}

        public static void JSONSerializer(List<Continent> candy)
        {
            using (FileStream fs = new FileStream("Continent.json", FileMode.OpenOrCreate))
            {
                var json = Encoding.Default.GetBytes(JsonSerializer.Serialize(candy));
                fs.Write(json, 0, json.Length);
            }
            Console.WriteLine("Файл Continent.json успешно создан.");
        }
    }
}