using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lec04LibN
{
    public class Logger : ILogger
    {
        private static Logger logger;
        private string LogFileName;
        private int logID;
        private List<string> logNamespaces;

        private Logger()
        {
            LogFileName = $"../../../LOG_{DateTime.Now.ToString("yyyyMMdd-hhmmss")}.txt";
            logID = 0;
            logNamespaces = new List<string>();
        }

        public void log(string message) 
        { 
            logID++;

            WriteToLogFile("INFO", message);
        }

        public static ILogger create() 
        {
            if (logger != null)
            {
                return logger;
            }

            logger = new Logger();

            return logger;
        }

        public void start(string title)
        {
            logID++;

            logNamespaces.Add(title);

            WriteToLogFile("STRT", $"");
        }

        public void stop()
        {
            logID++;

            string removedNamespace = string.Empty;

            if (logNamespaces.Count > 0)
            {
                removedNamespace = logNamespaces.Last();

                logNamespaces.RemoveAt(logNamespaces.Count - 1);

                WriteToLogFile("STOP", $"");

                return;
            }

            WriteToLogFile("STOP", $"");
        }

        private void WriteToLogFile(string logTypeName, string message)
        {
            string logID = this.logID.ToString("D6");
            string logNamespace = string.Empty;
            foreach (var namespc in logNamespaces)
            {
                logNamespace += $"{namespc}:";
            }

            string log = $"{logID} - {DateTime.Now} - {logTypeName} {logNamespace}   {message}";

            if (File.Exists(LogFileName))
            {
                using (StreamWriter sw = File.AppendText(LogFileName))
                {
                    sw.WriteLine(log);
                }

                return;
            }

            using (StreamWriter sw = File.CreateText(LogFileName))
            {
                sw.WriteLine($"{logID} - {DateTime.Now} - INIT");
            }

            this.logID++;

            WriteToLogFile(logTypeName, message);
        }
    }
}
