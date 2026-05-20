@echo off
:: Запускаем 3 экземпляра сервера с разными IP
:: ВАЖНО: убедись, что эти IP добавлены в настройки сетевого адаптера, иначе bind упадет!
start "Server 1" ServerU.exe 192.168.56.101
start "Server 2" ServerU.exe 192.168.56.102
start "Server 3" ServerU.exe 192.168.56.103

:: Запуск посредника (агента)
start "Agent" ServerU_Agent.exe 192.168.56.1

start "Client" ClientU.exe
echo Cluster and Agent started.
pause