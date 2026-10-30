    cd lab04\server
    node server.js X 5001

    cd lab04\server
    node server.js Y 5002

    cd lab04\server
    node server.js Z 5003

    cd lab04\gateway
    dotnet run


    curl.exe http://localhost:5001/A
    curl.exe -X POST http://localhost:5002/A
    curl.exe -X PUT http://localhost:5003/A
    curl.exe -X DELETE http://localhost:5003/A

    curl.exe http://localhost:5000/api
    curl.exe -X POST http://localhost:5000/api
    curl.exe -X PUT http://localhost:5000/api
    curl.exe -X DELETE http://localhost:5000/api

    curl.exe -i -X PATCH http://localhost:5000/api
    curl.exe -i http://localhost:5000/nope
