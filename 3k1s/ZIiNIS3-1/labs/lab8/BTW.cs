using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LB88
{
    public class BWT
    {
        public static string[] GetShiftsMatrixW1(string message)
        {
            string[] messageMatrix = new string[message.Length];

            for (int i = 0; i < message.Length; i++)
            {
                messageMatrix[i] = message;
                message = message.Substring(1) + message[0];
            }

            return messageMatrix;
        }

        public static string[] GetSortMatrixW(string[] matrix)
        {
            return (matrix.OrderBy(x => x).ToArray());
        }


        public static string GetLastColumnMk(string[] matrix)
        {
            string lastColumn = "";

            foreach (var row in matrix)
            {
                lastColumn += row[row.Length - 1];
            }
            return lastColumn;
        }

        public static string GetZRowM(string[] matrix, int rowNumber)
        {
            string zRow = "";

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (i == rowNumber - 1)
                {
                    zRow += matrix[i];
                }
            }

            return zRow;

        }

        public static int GetZRowPosition(string sourceStringM, string[] matrix)
        {
            int zRowPosition = -1;
            for (int i = 0; i < matrix.Count(); i++)
            {
                if (matrix[i] == sourceStringM)
                {
                    return i;
                }
            }
            return zRowPosition;
        }

        public static string[] GetDecodingMatrix(string message)
        {
            string[] messageMatrix = new string[message.Length];

            for (int i = 0; i < message.Length; i++)
            {
                messageMatrix = AddMkToMatrixFromLeft(message, messageMatrix);
                MatrixOperations.PrintMatrix(messageMatrix);
                messageMatrix = GetSortMatrixW(messageMatrix);
            }
            return messageMatrix;
        }

        public static string[] AddMkToMatrixFromLeft(string Mk, string[] matrix)
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i] = Mk[i] + matrix[i];
            }
            return matrix;
        }
    }
}
