
server

    node server.js X 5001 1000
    node server.js Y 5002 2000
    node server.js Z 5003 3000

gateway

    dotnet run



lab05

    powershell -ExecutionPolicy Bypass -File .\test.ps1


в ocelot.json меняем 

    "LoadBalancerOptions": { "Type": "RoundRobin" }

    "LoadBalancerOptions": { "Type": "CookieStickySessions", "Key": "GatewayNode", "Expiry": 1200000 }

    "LoadBalancerOptions": { "Type": "WeightedLoadBalancer" }

