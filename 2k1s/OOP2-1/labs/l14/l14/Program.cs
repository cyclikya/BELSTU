using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;

class Program
{
    // 3
    private static bool isPaused = false;
    private static readonly object pauseLock = new object();
    private static bool isRunning = true;

    // 4
    private static readonly object lockObject = new object();
    private static bool isEvenTurn = false;
    static void Main(string[] args)
    {
        First();
        Second();
        Third();
        Fourth();
        Fifth();

    }

    private static void First()
    {
        var allProcesses = Process.GetProcesses();

        Console.WriteLine("{0,-10} {1,-50} {2,-10} {3,-25} {4,-15} {5,-20}",
                          "ID", "Process Name", "Priority", "Start Time", "State", "CPU Time");

        foreach (var process in allProcesses)
        {
            try
            {

                var id = process.Id;
                var name = process.ProcessName;
                var priority = process.BasePriority;
                var startTime = process.StartTime;
                var state = process.Responding ? "Running" : "Not Responding";
                var cpuTime = process.TotalProcessorTime;


                Console.WriteLine("{0,-10} {1,-50} {2,-10} {3,-25} {4,-15} {5,-20}",
                                  id, name, priority, startTime, state, cpuTime);
            }
            catch (Exception)
            {
                Console.WriteLine("{0,-10} {1,-50} {2,-10} {3,-25} {4,-15} {5,-20}",
                                  process.Id, "Access Denied", "-", "-", "-", "-");
            }
        }

    }

    private static void Second()
    {
        Console.WriteLine($"Domain Name: {AppDomain.CurrentDomain.FriendlyName}");

        Console.WriteLine("Configuration Details:");
        Console.WriteLine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
        Console.WriteLine();

        Console.WriteLine("Loaded Assemblies:");
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Console.WriteLine(assembly.FullName);
        }
        Console.WriteLine();

        var loadContext = new MyLoadContext();
        string assemblyPath = Path.Combine(Directory.GetCurrentDirectory(), "l14.dll");

        try
        {
            Assembly loadedAssembly = loadContext.LoadFromAssemblyPath(assemblyPath);
            Console.WriteLine($"Loaded Assembly: {loadedAssembly.FullName}");

            Type type = loadedAssembly.GetType("MyLibrary.HelloWorld");
            var instance = Activator.CreateInstance(type);
            MethodInfo methodInfo = type.GetMethod("SayHello");
            methodInfo.Invoke(instance, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading assembly: {ex.Message}");
        }
        finally
        {
            loadContext.Unload();
            Console.WriteLine("Unload context.");
        }
    }

    static void Third()
    {
        Console.Write("Введите число n: ");
        if (!int.TryParse(Console.ReadLine(), out int n) || n < 1)
        {
            Console.WriteLine("Некорректный ввод. Пожалуйста, введите положительное целое число.");
            return;
        }

        Thread workerThread = new Thread(() => CalculateAndWritePrimes(n));
        workerThread.Start();


        while (true)
        {
            Console.WriteLine("\nВведите команду (pause, resume, stop, status):");
            string command = Console.ReadLine();

            switch (command.ToLower())
            {
                case "pause":
                    PauseThread();
                    break;
                case "resume":
                    ResumeThread();
                    break;
                case "stop":
                    StopThread(ref workerThread);
                    return;
                case "status":
                    PrintThreadStatus(workerThread);
                    break;
                default:
                    Console.WriteLine("Неизвестная команда.");
                    break;
            }
        }
    }

    static void CalculateAndWritePrimes(int n)
    {
        for (int i = 1; i <= n; i++)
        {

            if (!isRunning)
                break;


            lock (pauseLock)
            {
                while (isPaused)
                {
                    Monitor.Wait(pauseLock);
                }
            }

            if (IsPrime(i))
            {
                Console.WriteLine(i);
                File.AppendAllText("primes.txt", i + Environment.NewLine);
            }

            Thread.Sleep(200);
        }
    }

    static bool IsPrime(int number)
    {
        if (number < 2) return false;
        for (int i = 2; i <= Math.Sqrt(number); i++)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

    static void PauseThread()
    {
        isPaused = true;
        Console.WriteLine("Поток приостановлен.");
    }

    static void ResumeThread()
    {
        lock (pauseLock)
        {
            isPaused = false;
            Monitor.PulseAll(pauseLock);
        }
        Console.WriteLine("Поток возобновлен.");
    }

    static void StopThread(ref Thread workerThread)
    {
        isRunning = false;
        if (workerThread.IsAlive)
        {
            workerThread.Join();
            Console.WriteLine("Поток остановлен.");
        }
    }

    static void PrintThreadStatus(Thread thread)
    {
        Console.WriteLine($"Имя потока: {thread.Name}");
        Console.WriteLine($"Статус потока: {thread.ThreadState}");
        Console.WriteLine($"Приоритет потока: {thread.Priority}");
        Console.WriteLine($"ID потока: {thread.ManagedThreadId}");
    }

    static void Fourth()
    {
        Console.Write("Введите число n: ");
        if (!int.TryParse(Console.ReadLine(), out int n) || n < 1)
        {
            Console.WriteLine("Некорректный ввод. Пожалуйста, введите положительное целое число.");
            return;
        }

        Thread evenThread = new Thread(() => PrintEvenNumbers(n));
        Thread oddThread = new Thread(() => PrintOddNumbers(n));


        oddThread.Start();
        evenThread.Start();



        evenThread.Join();
        oddThread.Join();


        Thread even = new Thread(() => PrintEven(n));
        Thread odd = new Thread(() => PrintOdd(n));
        odd.Priority = ThreadPriority.AboveNormal;
        even.Priority = ThreadPriority.Normal;
        odd.Start();
        odd.Join();
        even.Start();

        even.Join();




    }

    static void PrintEvenNumbers(int n)
    {
        for (int i = 2; i <= n; i += 2)
        {
            lock (lockObject)
            {
                while (!isEvenTurn) Monitor.Wait(lockObject);
                Console.WriteLine($"Четное: {i}");
                File.AppendAllText("numbers.txt", $"Четное: {i}\n");
                isEvenTurn = false;
                Monitor.Pulse(lockObject);
            }

            Thread.Sleep(100);
        }
    }

    static void PrintEven(int n)
    {
        for (int i = 2; i <= n; i += 2)
        {
            Console.WriteLine($"Четное: {i}");
        }
    }

    static void PrintOddNumbers(int n)
    {
        for (int i = 1; i <= n; i += 2)
        {
            lock (lockObject)
            {
                while (isEvenTurn) Monitor.Wait(lockObject);
                Console.WriteLine($"Нечетное: {i}");
                File.AppendAllText("numbers.txt", $"Нечетное: {i}\n");
                isEvenTurn = true;
                Monitor.Pulse(lockObject);
            }

            Thread.Sleep(100);
        }
    }

    static void PrintOdd(int n)
    {
        for (int i = 1; i <= n; i += 2)
        {
            Console.WriteLine($"Нечетное: {i}");
        }
    }

    static void Fifth()
    {
        Timer timer = new Timer(TimerCallback, null, 0, 1000);
        Console.WriteLine("Нажмите клавишу");
        Console.ReadKey();
        timer.Dispose();
    }

    private static void TimerCallback(object state)
    {

        Console.WriteLine(DateTime.Now);
    }
}
public class MyLoadContext : AssemblyLoadContext
{
    public MyLoadContext() : base(isCollectible: true) { }
}