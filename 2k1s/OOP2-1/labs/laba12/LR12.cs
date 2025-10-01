using System.IO.Compression;

namespace HAProject
{
    public class UVRLog
    {
        private readonly string logFilePath;

        public UVRLog(string logFilePath)
        {
            this.logFilePath = logFilePath;
            if (!File.Exists(logFilePath))
                File.Create(logFilePath).Close();
        }

        public void WriteLog(string action, string details)
        {
            string logEntry = $"{DateTime.Now}: {action} - {details}";
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }

        public string ReadLog()
        {
            return File.ReadAllText(logFilePath);
        }

        public string SearchLog(string keyword)
        {
            var lines = File.ReadAllLines(logFilePath);
            return string.Join(Environment.NewLine, lines.Where(line => line.Contains(keyword)));
        }

        public void DeleteLogEntries(Func<string, bool> predicate)
        {
            var lines = File.ReadAllLines(logFilePath).Where(line => !predicate(line));
            File.WriteAllLines(logFilePath, lines);
        }

        public IEnumerable<string> SearchByDate(DateTime date)
        {
            var lines = File.ReadAllLines(logFilePath);
            return lines.Where(line => line.Contains(date.ToShortDateString()));
        }

        public int CountEntries()
        {
            return File.ReadAllLines(logFilePath).Length;
        }
    }

    public class HADiskInfo
    {
        public void DisplayDiskInfo()
        {
            var drives = DriveInfo.GetDrives();

            foreach (var drive in drives.Where(d => d.IsReady))
            {
                Console.WriteLine($"Имя диска: {drive.Name}");
                Console.WriteLine($"Метка тома: {drive.VolumeLabel}");
                Console.WriteLine($"Формат диска: {drive.DriveFormat}");
                Console.WriteLine($"Общий размер: {drive.TotalSize / 1_000_000} MB");
                Console.WriteLine($"Свободное место: {drive.AvailableFreeSpace / 1_000_000} MB");
                Console.WriteLine();
            }
        }
    }

    public class HAFileInfo
    {
        public void DisplayFileInfo(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                Console.WriteLine("Файл не найден.");
                return;
            }

            Console.WriteLine($"Полный путь: {fileInfo.FullName}");
            Console.WriteLine($"Размер: {fileInfo.Length} байтов");
            Console.WriteLine($"Расширение: {fileInfo.Extension}");
            Console.WriteLine($"Имя: {fileInfo.Name}");
            Console.WriteLine($"Время создания: {fileInfo.CreationTime}");
            Console.WriteLine($"Время последнего изменения: {fileInfo.LastWriteTime}");
        }
    }

    public class HADirInfo
    {
        public void DisplayDirInfo(string dirPath)
        {
            Console.WriteLine();
            var dirInfo = new DirectoryInfo(dirPath);
            if (!dirInfo.Exists)
            {
                Console.WriteLine("Директория не найдена.");
                return;
            }

            Console.WriteLine($"Имя директории: {dirInfo.Name}");
            Console.WriteLine($"Время создания: {dirInfo.CreationTime}");
            Console.WriteLine($"Количество файлов: {dirInfo.GetFiles().Length}");
            Console.WriteLine($"Количество поддиректорий: {dirInfo.GetDirectories().Length}");
            Console.WriteLine("Родительские директории:");
            var parent = dirInfo.Parent;
            while (parent != null)
            {
                Console.WriteLine(parent.FullName);
                parent = parent.Parent;
            }
        }
    }
    public class HAFileManager
    {
        public void ManageFiles(string drive)
        {
            string inspectDir = Path.Combine(drive, "HAInspect");
            Directory.CreateDirectory(inspectDir);

            string logFile = Path.Combine(inspectDir, "hadirinfo.txt");
            File.WriteAllText(logFile, "Првоверка работоспособности");

            string copiedFile = Path.Combine(inspectDir, "copy.txt");

            if (File.Exists(copiedFile))
            {
                File.Delete(copiedFile);
            }

            File.Copy(logFile, copiedFile, overwrite: true);
            File.Delete(logFile);

            string filesDir = Path.Combine(drive, "HAFiles");
            Directory.CreateDirectory(filesDir);

            foreach (var file in Directory.GetFiles(drive, "*.txt"))
            {
                string destinationFile = Path.Combine(filesDir, Path.GetFileName(file));

                if (File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }

                File.Copy(file, destinationFile, overwrite: true);
            }

            string archive = Path.Combine(inspectDir, "HAFiles.zip");

            if (File.Exists(archive))
            {
                File.Delete(archive);
            }

            ZipFile.CreateFromDirectory(filesDir, archive);
            Directory.Delete(filesDir, true);

            string extractedDir = Path.Combine(drive, "Извлеченные файлы");
            Directory.CreateDirectory(extractedDir);
            ZipFile.ExtractToDirectory(archive, extractedDir);
        }
    }

    class LR12
    {
        static void Main()
        {
            string logPath = "halogfile.txt";
            var logger = new UVRLog(logPath);

            logger.WriteLog("Инициализация", "Программа запущена");

            var diskInfo = new HADiskInfo();
            diskInfo.DisplayDiskInfo();

            var fileInfo = new HAFileInfo();
            fileInfo.DisplayFileInfo("halogfile.txt");

            var dirInfo = new HADirInfo();
            dirInfo.DisplayDirInfo("D:\\БГТУ\\III сем\\ООП\\lab12");

            var fileManager = new HAFileManager();
            fileManager.ManageFiles("D:\\БГТУ\\III сем\\ООП\\lab12");
        }
    }
}