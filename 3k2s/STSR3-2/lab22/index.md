скачать библиотеку OpenSSL

**CA**: создает свою печать и сертификат
*Resource*: создает свою печать и запрос CSR и отправляет этот запрос в CA
**CA**: подписывает запрос и формирует сертификат для ресурса и отправляет его
*Resource*: импортирует этот сертификат в доверенные центры Windows и добавляет в hosts
*Resource*: запуск ресурса без вопросов от браузера


## CA:
//создаёт приватный ключ CA
openssl genrsa -out ca-key.pem 2048
//создаёт сертификат CA
openssl req -new -x509 -days 365 -key ca-key.pem -out ca-cert.pem -subj "/CN=CA-LAB22-UVR"

## Resource:
//создаёт приватный ключ сервера
openssl genrsa -out resource-key.pem 2048
//создаёт CSR. CSR = запрос на сертификат
openssl req -new -key resource-key.pem -out resource-csr.pem -subj "/CN=RS-LAB22-UVR"

## CA:
// создаёт файл extensions.cnf, в него записываются разрешённые домены
Set-Content -Path extensions.cnf -Value "subjectAltName=DNS:LAB22-DUS,DNS:DUS" -Encoding ascii

// CA подписывает CSR-запрос сервера, здесь рождается настоящий HTTPS сертификат
openssl x509 -req -days 365 -in resource-csr.pem -CA ca-cert.pem -CAkey ca-key.pem -CAcreateserial -out resource-cert.pem -extfile extensions.cnf

## Resourse:
// импортирует сертификат CA в доверенные центры Windows, после этого браузер начинает доверять вашему CA
certutil -addstore "Root" ca-cert.pem

notepad C:\Windows\System32\drivers\etc\hosts - сюда добавила адреса 
127.0.0.1 LAB22-UVR
127.0.0.1 UVR


















