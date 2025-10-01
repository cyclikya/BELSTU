using System;

namespace laba2
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //First task
            Console.WriteLine("ЭНТРОПИЯ");
            Console.WriteLine("Датский: " + Entropy.ComputeDatsky());
            Console.WriteLine("Казахский: " + Entropy.ComputeKazach());
            Console.WriteLine();

            //Second task
            Console.WriteLine("бинарный: " + Entropy.ComputeBinary());
            Console.WriteLine();

            //Third task
            Console.WriteLine("Объем информации, в имени (Datsky)(Uhorenko Violetta Romanovna): " + 
                               Entropy.ComputeDatsky() * "Uhorenko Violetta Romanovna".Length);
            Console.WriteLine("Объем информации, в имени (Kazach)(Угоренко Виолетта Романовна): " +
                               Entropy.ComputeKazach() * "Угоренко Виолетта Романовна".Length);

            string binaryFIO = File.ReadAllText("FIOBin.txt");
            int binaryLength = binaryFIO.Length;

            Console.WriteLine();
            Console.WriteLine("Объем информации в имени (ASCII, бинарное представление из файла FIOBin.txt): " +
                Entropy.ComputeBinary() * binaryLength);

            //Fourth task
            Console.WriteLine();

            Console.WriteLine("Объем информации в имени (ASCII, ошибка 0.1, бинарное представление): " +
                (Entropy.ComputeBinary(0.1f) * binaryLength));
            Console.WriteLine("Объем информации в имени (ASCII, ошибка 0.5, бинарное представление): " +
                (Entropy.ComputeBinary(0.5f) * binaryLength));
            Console.WriteLine("Объем информации в имени (ASCII, ошибка 1, бинарное представление): " +
                (Double.IsNaN(Entropy.ComputeBinary(1f)) ? 0 : Entropy.ComputeBinary(1f) * binaryLength));

        }
    }
}
