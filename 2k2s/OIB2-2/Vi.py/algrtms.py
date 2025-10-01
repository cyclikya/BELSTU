#подстановачный перестановочный  
#



from Crypto.PublicKey import RSA
from Crypto.Cipher import PKCS1_OAEP
from Crypto.Random import get_random_bytes
from Crypto.Protocol.KDF import HKDF
import time
from Crypto.Hash import SHA256
import hashlib

# 1. RSA
def rsa_example():
    start_time = time.time()
    key = RSA.generate(2048)
    public_key = key.publickey()
    
    message = b'hello world, my name is Violetta'
    cipher = PKCS1_OAEP.new(public_key)
    encrypted_message = cipher.encrypt(message)
    
    cipher = PKCS1_OAEP.new(key)
    decrypted_message = cipher.decrypt(encrypted_message)

    end_time = time.time()

    print("Сообщение, расшифрованное через RSA:", decrypted_message)
    print("Время выполнения RSA: %s мс" % ((end_time - start_time) * 1000))


# 2. Диффи–Хеллман
def diffie_hellman_example():
    start_time = time.time()

    firstPublicKey = 23  #p
    secondPublicKey = 5  #g
    
    firstPrivateKey = 6 #a
    secondPrivateKey = 15 #b
    
    A = (secondPublicKey ** firstPrivateKey) % firstPublicKey # открытый ключ первой стороны
    B = (secondPublicKey ** secondPrivateKey) % firstPublicKey # открытый ключ второй стороны
    
    # общий ключ
    K1 = (B ** firstPrivateKey) % firstPublicKey
    K2 = (A ** secondPrivateKey) % firstPublicKey
    
    end_time = time.time()

    print("\nКлючи Диффи-Хеллман:", K1)
    print("Время выполнения Диффи-Хеллмана: %s мс" % ((end_time - start_time) * 1000))


# 3. Эль-Гамаль
def elgamal_example():
    start_time = time.time()

    simpDigit = 23 # простое число 
    digit = 5 # корень из 25, просто число
    privateKey = 15 # секретный ключ
    
    y = (digit ** privateKey) % simpDigit # открытый ключ
    
    message = 13
    randomDigit = 10 
    
    c1 = (digit ** randomDigit) % simpDigit 
    c2 = (message * (y ** randomDigit)) % simpDigit 
    
    # расшифровываем
    s = (c1 ** privateKey) % simpDigit 
    s_inv = pow(s, simpDigit-2, simpDigit) # обратное число к модулю simpDigit 

    decrypted_message = (c2 * s_inv) % simpDigit 
    
    end_time = time.time()

    print("\nРасшифрованное сообщение с помощью Эль-Гамаля:", decrypted_message)
    print("Время выполнения Эль-Гамаля: %s мс" % ((end_time - start_time) * 1000))




rsa_example()
diffie_hellman_example()
elgamal_example()

# 1 есть 3 функции для шифрования и дешифрования 
# 2 рса: генерируем ключи, с пом. сифер создаем паблик ключ,
#  шифруем соо, опять с пом. сифера создаем ключ, расшифровываем и выводим
# 3 д-х: 2 открытых и 2 закрытых ключа по два у каждого из 2 человек
# генерируем открытые ключи обеих сторон, а потом общий ключ
# выводим в консоль
# 4 э-г: 23 - модуль, 5 - примитивный корень по модулю 23
# потом секретный ключ
# генерируем открытый ключ ,шифруем соо
#  c1 — это первая часть шифротекста, которая зависит от случайного числа randomDigit.
# c2 — это вторая часть шифротекста, которая содержит зашифрованное сообщение. Она зависит как от сообщения, так и от открытого ключа.
# расшифровываем соо и проверяем