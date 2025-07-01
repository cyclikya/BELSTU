using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR2
{
    public partial class Airline
    {
        public string FPoint
        {
            get { return fPoint; }
            set { fPoint = value; }
        }
        public int Id
        {
            get { return id; }
        }
        public string TypeOfPlane
        {
            get { return typeOfPlane; }
            set { typeOfPlane = value; }
        }
        public string STime
        {
            get { return sTime; }
            set { sTime = value; }
        }
        public string DayOfWeek
        {
            get { return dayOfWeek; }
            set { dayOfWeek = value; }
        }
    }
}