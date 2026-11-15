$url = "http://localhost:5000/lb"
$count = 50

[System.Net.ServicePointManager]::DefaultConnectionLimit = 500
Add-Type -AssemblyName System.Net.Http

$handler = New-Object System.Net.Http.HttpClientHandler
$handler.CookieContainer = New-Object System.Net.CookieContainer
$client = New-Object System.Net.Http.HttpClient($handler)
$client.Timeout = [TimeSpan]::FromSeconds(120)

function Send-Request($method) {
    $request = New-Object System.Net.Http.HttpRequestMessage(
        (New-Object System.Net.Http.HttpMethod($method)), $url)
    return $client.SendAsync($request)
}

$timer = [System.Diagnostics.Stopwatch]::StartNew()

# Первый запрос отдельно: от него клиент получает cookie, нужную для Sticky Sessions
$first = Send-Request "GET"
$first.Wait()

$tasks = New-Object System.Collections.ArrayList
foreach ($method in @("GET", "POST", "PUT", "DELETE")) {
    $n = $count
    if ($method -eq "GET") { $n = $count - 1 }
    for ($i = 1; $i -le $n; $i++) {
        [void]$tasks.Add((Send-Request $method))
    }
}

[System.Threading.Tasks.Task]::WaitAll($tasks.ToArray())
$timer.Stop()

$stats = [ordered]@{ X = 0; Y = 0; Z = 0 }

$answers = New-Object System.Collections.ArrayList
[void]$answers.Add($first.Result.Content.ReadAsStringAsync().Result)
foreach ($task in $tasks) {
    [void]$answers.Add($task.Result.Content.ReadAsStringAsync().Result)
}

foreach ($answer in $answers) {
    $nick = ($answer | ConvertFrom-Json).Nick
    $stats[$nick]++
}

$total = $answers.Count

Write-Host ""
Write-Host "Всего запросов: $total  (по $count каждого метода)"
Write-Host "Время: $([math]::Round($timer.Elapsed.TotalSeconds, 1)) c"
Write-Host ""
foreach ($key in $stats.Keys) {
    $percent = [math]::Round(100 * $stats[$key] / $total, 1)
    Write-Host ("{0}: {1,3}  ({2}%)" -f $key, $stats[$key], $percent)
}
