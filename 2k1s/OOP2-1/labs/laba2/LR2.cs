using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace LR2
{
    public partial class Airline
    {
        private string fPoint;
        private readonly int id;
        private string typeOfPlane;
        private string sTime;
        private string dayOfWeek;

        public static string AirlineCompanyName;
        private static int PlaneCount;
        public const string AirportName = "International Airport";

        public Airline(string fPoint, string typeOfPlane, string sTime, string dayOfWeek)
        {
            FPoint = fPoint;
            TypeOfPlane = typeOfPlane;
            STime = sTime;
            DayOfWeek = dayOfWeek;

            PlaneCount++;

            id = GetHashCode();
        }
        public Airline()
        {
            FPoint = "Unknown";
            TypeOfPlane = "Unknown";
            STime = "Unknown";
            DayOfWeek = "Unknown";

            PlaneCount++;

            id = GetHashCode();
        }
        public Airline(string fPoint) 
        {
            FPoint = fPoint;
            TypeOfPlane = "Unknown";
            STime = "Unknown";
            DayOfWeek = "Unknown";

            PlaneCount++;

            id = GetHashCode();
        }

        static Airline()
        {
            AirlineCompanyName = "Global Airlines";
            PlaneCount = 0;
        }

        private Airline(int id)
        {
            this.id = id;

            PlaneCount++;
        }
        public static Airline CreateNewPlaneByID(int id)
        {
            return new Airline(id);
        }

        public void SetSTime(ref string newSTime, out string messege)
        {
            this.STime = newSTime;
            messege = $"Время вылета обновлено на {STime}";
        }

        public static int GetCount()
        {
            return PlaneCount;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj.GetType() != this.GetType()) return false;
            Airline plane = (Airline)obj;

            return (this.Id == plane.Id);
        }
        public override int GetHashCode()
        {
            int hash = 269;
            hash = (hash * 47) + (string.IsNullOrEmpty(fPoint) ? 0 : fPoint.GetHashCode());
            hash = (hash * 47) + (string.IsNullOrEmpty(typeOfPlane) ? 0 : typeOfPlane.GetHashCode());
            hash = (hash * 47) + (string.IsNullOrEmpty(sTime) ? 0 : sTime.GetHashCode());
            return Math.Abs(hash);
        }
        public override string ToString()
        {
            return $" ID: {id}, Пункт назначения: {FPoint}, Самолет: {TypeOfPlane}, Время: {STime}, День: {DayOfWeek}, Аэропорт: {AirportName}, Авиакомпания: {AirlineCompanyName}.";
        }
    }
    class LR2
    {
        static void Main()
        {
            Console.WriteLine($"Количество рейсов: {Airline.GetCount()}");

            Console.WriteLine(Airline.AirlineCompanyName);

            Airline plane1 = new Airline();
            Airline plane2 = new Airline("Grodno");
            Airline plane3 = new Airline("Minsk", "Boeing", "12:30", "Mon");
            Airline plane4 = new Airline("Minsk", "Boeing", "12:30", "Mon");
            Airline plane5 = new Airline("Bitebsk", "Boeing", "9:25", "Fri");
            Airline plane99 = Airline.CreateNewPlaneByID(99);

            Console.WriteLine($"\n{plane1.ToString()}");
            Console.WriteLine($"{plane2.ToString()}");
            Console.WriteLine($"{plane3.ToString()}");
            Console.WriteLine($"{plane4.ToString()}");
            Console.WriteLine($"{plane5.ToString()}");
            Console.WriteLine($"{plane99.ToString()}");

            string newTime = "14:00";
            plane1.SetSTime(ref newTime, out string updateMessage);
            Console.WriteLine(updateMessage);
            Console.WriteLine($"\n{plane1.ToString()}");

            Console.WriteLine($"Количество рейсов: {Airline.GetCount()}");

            Console.WriteLine($"pl3 and pl5: {plane3.Equals(plane5)}");
            Console.WriteLine($"pl3 and pl4: {plane3.Equals(plane4)}");

            Console.WriteLine(plane1.GetType());

            Airline[] array = new Airline[4];
            array[0] = plane1;
            array[1] = plane2;
            array[2] = plane3;
            array[3] = plane4;
            Console.WriteLine("\nРейсы с пунктом назначения Minsk:");
            foreach (Airline plane in array)
            {
                if (plane.FPoint == "Minsk")
                {
                    Console.WriteLine(plane.ToString());
                }
            }
            Console.WriteLine("\nРейсы с днем недели Mon:");
            foreach (Airline plane in array)
            {
                if (plane.DayOfWeek == "Mon")
                {
                    Console.WriteLine(plane.ToString());
                }
            }

            var planes = new
            {
                Id = 1,
                FPoint = "Minsk",
                TypeOfPlane = "Boeing",
                STime = "10:30",
                DayOfWeek = "Mon",
                AirportName = "International Airport",
                AirlineCompanyName = "Global Airlines"
            };
            Console.WriteLine("\nАнонимный тип:");
            Console.WriteLine($"ID: {planes.Id}, Пункт назначения: {planes.FPoint}, Самолет: {planes.TypeOfPlane}, Время: {planes.STime}, День: {planes.DayOfWeek}, Аэропорт: {planes.AirportName}, Авиакомпания: {planes.AirlineCompanyName}.");


            C c = new C();

            c.X = 5;
            int a = c.X;



            int x = c.SQ();

        }


        class C {
            private int x;

            public int X
            {
                get { return x; }
                set {
                    x = value;
                }
            }
            public int SQ()
            {
                return (X ^ 2);
            }



         }

        
        
        
        
        
        
        
        
        }
    }
