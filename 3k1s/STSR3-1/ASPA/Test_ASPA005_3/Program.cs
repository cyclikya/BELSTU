using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

class Test
{
    class Answer<T>
    {
        public T? x { get; set; } = default;
        public T? y { get; set; } = default;
        public string? message { get; set; } = null;
    }


    public static string OK = "OK";
    public static string NOK = "NOK";
    private HttpClient client = new HttpClient();

    public async Task ExecuteGET<T>(string path, Func<T?, T?, int, string> result)
    {
        await resultPRINT<T>("GET", path, await this.client.GetAsync(path), result);
    }

    public async Task ExecutePOST<T>(string path, Func<T?, T?, int, string> result)
    {
        await resultPRINT<T>("POST", path, await this.client.PostAsync(path, null), result);
    }

    public async Task ExecutePUT<T>(string path, Func<T?, T?, int, string> result)
    {
        await resultPRINT<T>("PUT", path, await this.client.PutAsync(path, null), result);
    }

    public async Task ExecuteDELETE<T>(string path, Func<T?, T?, int, string> result)
    {
        await resultPRINT<T>("DELETE", path, await this.client.DeleteAsync(path), result);
    }

    private async Task resultPRINT<T>(string method, string path, HttpResponseMessage rm, Func<T?, T?, int, string> result)
    {
        int status = (int)rm.StatusCode;
        try
        {
            Answer<T>? answer = await rm.Content.ReadFromJsonAsync<Answer<T>>() ?? default(Answer<T>);

            string r = result(default(T), default(T), status);
            T? x = default(T), y = default(T);

            if (answer != null)
            {
                x = answer.x;
                y = answer.y;
                Console.WriteLine($"{r}: {method} {path} статус = {status}, x = {x}, y = {y}, m = {answer?.message}");
            }
        }
        catch (System.Text.Json.JsonException ex)
        {
            string r = result(default(T), default(T), status);
            Console.WriteLine($"{r}: {method} {path} статус = {status}, x = {{ null }}, y = {{ null }}, m = {ex.Message}");
        }
    }

    public static async Task Main(string[] args) // Асинхронный метод Main
    {

        Test test = new Test();

        Console.WriteLine("-- /A -------------------------------------");

        await test.ExecuteGET<int?>("https://localhost:7034/A/3", (int? x, int? y, int status) => (x == 3 && y == null && status == 200) ? Test.NOK : Test.OK);

        await test.ExecuteGET<int?>("https://localhost:7034/A/-3", (int? x, int? y, int status) => (x == -3 && y == null && status == 200) ? Test.NOK : Test.OK);

        await test.ExecuteGET<int?>("https://localhost:7034/A/118", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

        await test.ExecutePOST<int?>("https://localhost:7034/A/5", (int? x, int? y, int status) => (x == 5 && y == null && status == 200) ? Test.NOK : Test.OK);

        await test.ExecutePOST<int?>("https://localhost:7034/A/-5", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

        await test.ExecutePOST<int?>("https://localhost:7034/A/118", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

        await test.ExecutePUT<int?>("https://localhost:7034/A/2/3", (int? x, int? y, int status) => (x == 2 && y == 3 && status == 200) ? Test.NOK : Test.OK);

        await test.ExecutePUT<int?>("https://localhost:7034/A/0/3", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

        await test.ExecutePUT<int?>("https://localhost:7034/A/25/-3", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

        await test.ExecutePUT<int?>("https://localhost:7034/A/0/-3", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

        await test.ExecuteDELETE<int?>("https://localhost:7034/A/1-99", (int? x, int? y, int status) => (x == 1 && y == 99 && status == 200) ? Test.NOK : Test.OK);

        await test.ExecuteDELETE<int?>("https://localhost:7034/A/99-1", (int? x, int? y, int status) => (x == 99 && y == 1 && status == 200) ? Test.NOK : Test.OK);

        await test.ExecuteDELETE<int?>("https://localhost:7034/A/-1-25", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

        await test.ExecuteDELETE<int?>("https://localhost:7034/A/-1--25", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

        await test.ExecuteDELETE<int?>("https://localhost:7034/A/25-101", (int? x, int? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

        Console.WriteLine("-- /B -------------------------------------");

        await test.ExecuteGET<float?>("https://localhost:7034/B/2.5", (float? x, float? y, int status) => (x == 2.5 && y == null && status == 200) ? Test.NOK : Test.OK);
        await test.ExecuteGET<float?>("https://localhost:7034/B/2", (float? x, float? y, int status) => (x == 2.0 && y == null && status == 200) ? Test.NOK : Test.OK);
        await test.ExecuteGET<float?>("https://localhost:7034/B/2X", (float? x, float? y, int status) => (x == null && y == null && status == 404) ? Test.OK : Test.NOK);
        
        await test.ExecutePOST<float?>("https://localhost:7034/B/2.5/3.2",
            (float? x, float? y, int status) =>
                (x == 2.5 &&
                y == 3.2 &&
                status == 200)
                ? Test.OK : Test.NOK);
        
        await test.ExecuteDELETE<float?>("https://localhost:7034/B/2.5-3.2",
            (float? x, float? y, int status) => 
                (x == 2.5 &&
                y == 3.2 &&
                status == 200)
                ? Test.OK : Test.NOK);

        Console.WriteLine("-- /C -------------------------------------");


        await test.ExecuteGET<bool?>("https://localhost:7034/C/true", (bool? x, bool? y, int status) => (x == true && y == null && status == 200) ? Test.NOK : Test.OK);
        await test.ExecutePOST<bool?>("https://localhost:7034/C/true,false", (bool? x, bool? y, int status) => (x == true && y == false && status == 200) ? Test.NOK : Test.OK);


        Console.WriteLine("-- /D -------------------------------------");
        await test.ExecuteGET<DateTime?>("https://localhost:7034/D/2025-02-25", (DateTime? x, DateTime? y, int status) =>
            (x == new DateTime(2025, 02, 25) && y == null && status == 200) ? Test.NOK : Test.OK);
        await test.ExecuteGET<DateTime?>("https://localhost:7034/D/2025-02-29", (DateTime? x, DateTime? y, int status) =>
            (x == null && y == null && status == 404) ? Test.OK : Test.NOK);
        await test.ExecutePOST<DateTime?>("https://localhost:7034/D/2024-02-29", (DateTime? x, DateTime? y, int status) =>
            (x == new DateTime(2024, 02, 29) && y == null && status == 200) ? Test.NOK : Test.OK);
        await test.ExecutePUT<DateTime?>("https://localhost:7034/D/2025-02-25T19:25", (DateTime? x, DateTime? y, int status) =>
            (x == new DateTime(2025, 02, 25, 19, 25, 0) && y == null && status == 200) ? Test.NOK : Test.OK);
        await test.ExecutePOST<DateTime?>("https://localhost:7034/D/2025-02-25|2025-03-25", (DateTime? x, DateTime? y, int status) =>
            (x == new DateTime(2025, 02, 25) && y == new DateTime(2025, 03, 25) && status == 200) ? Test.NOK : Test.OK);
        await test.ExecutePUT<DateTime?>("https://localhost:7034/D/2025-02-25T19:25", (DateTime? x, DateTime? y, int status) =>
            (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

        Console.WriteLine("-- /E -------------------------------------");

        await test.ExecuteGET<string?>("https://localhost:7034/E/12-bis", (string? x, string? y, int status) =>
            (x == "bis" && y == null && status == 200) ? Test.NOK : Test.OK);

        await test.ExecuteGET<string?>("https://localhost:7034/E/11-bis", (string? x, string? y, int status) =>
            (x == null && y == null && status == 404) ? Test.NOK : Test.OK);

        await test.ExecuteGET<string?>("https://localhost:7034/E/12-777", (string? x, string? y, int status) =>
            (x == "777" && y == null && status == 200) ? Test.NOK : Test.OK);

        await test.ExecuteGET<string?>("https://localhost:7034/E/12-", (string? x, string? y, int status) =>
            (x == null && y == null && status == 404) ? Test.NOK : Test.OK);

        await test.ExecutePUT<string?>("https://localhost:7034/E/abcd", (string? x, string? y, int status) =>
            (x == "abcd" && y == null && status == 200) ? Test.NOK : Test.OK);

        await test.ExecutePUT<string?>("https://localhost:7034/E/abcd123", (string? x, string? y, int status) =>
            (x == null && y == null && status == 404) ? Test.NOK : Test.OK);

        await test.ExecutePUT<string?>("https://localhost:7034/E/a", (string? x, string? y, int status) =>
            (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

        await test.ExecutePUT<string?>("https://localhost:7034/E/123456", (string? x, string? y, int status) =>
            (x == null && y == null && status == 404) ? Test.NOK : Test.OK);

        await test.ExecutePUT<string?>("https://localhost:7034/E/aabccddeeffgghh", (string? x, string? y, int status) =>
            (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

        Console.WriteLine("-- /F -------------------------------------");


        await test.ExecuteGET<string?>("https://localhost:7034/F/xxx@yyyy", (string? x, string? y, int status) =>
            (x == null && y == null && status == 404) ? Test.OK : Test.NOK);

    }
}