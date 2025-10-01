from Crypto.PublicKey import RSA
from Crypto.Signature import pkcs1_15
from Crypto.Hash import SHA256

key = RSA.generate(2048) # Генерация ключей
private_key = key
public_key = key.publickey()

message = b'message for testing code lalalallala' # Сообщение

hash_message = SHA256.new(message)

signature = pkcs1_15.new(private_key).sign(hash_message) # Подпись сообщения

print("Signature:", signature)

try:
    pkcs1_15.new(public_key).verify(hash_message, signature)
    print("The signature is valid.")
except (ValueError, TypeError):
    print("The signature is not valid.")

# 1 генерируем ключи с помощью рса генерейт 
# 2 создаем соо для подписи
# 3 вычисляем его хеш
# 4 подписываем с помощью секретного ключа
# 5 печатаем соо в консоли
# 6 проверяем все ли у нас ок #