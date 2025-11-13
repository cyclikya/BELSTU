using System;
using System.Collections.Generic;
using System.Text;

namespace LB88
{
    public class MatrixOperations
    {
        public static void PrintMatrix(string[] matrix)
        {
            foreach (var row in matrix)
            {
                Console.WriteLine(row);
            }
            Console.WriteLine();
        }
    }
}
