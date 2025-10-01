def print_matrix(matrix):
    for row in matrix:
        print(' | '.join(row))


def encrypt_message(message: str, rows: int, cols: int) -> str:
    message = message.strip()
    length = rows * cols

    if len(message) < length:
        message += ' ' * (length - len(message))
    else:
        message = message[:length]

    matrix = []
    index = 0
    for r in range(rows):
        row = []
        for c in range(cols):
            row.append(message[index])
            index += 1
        matrix.append(row)

    print("Матрица:")
    print_matrix(matrix)

    encrypted = []
    for c in range(cols):
        for r in range(rows):
            encrypted.append(matrix[r][c])

    return ''.join(encrypted)


def decrypt_message(encrypted: str, rows: int, cols: int) -> str:
    length = rows * cols

    if len(encrypted) < length:
        encrypted += ' ' * (length - len(encrypted))
    else:
        encrypted = encrypted[:length]

    matrix = [[''] * cols for _ in range(rows)]

    index = 0
    for c in range(cols):
        for r in range(rows):
            matrix[r][c] = encrypted[index]
            index += 1

    decrypted = []
    for r in range(rows):
        for c in range(cols):
            decrypted.append(matrix[r][c])

    return ''.join(decrypted).rstrip()


def brute_force_decrypt(encrypted: str):
    length = len(encrypted)
    results = []

    for rows in range(1, length + 1):
        if length % rows == 0:
            cols = length // rows

            decrypted = decrypt_message(encrypted, rows, cols)
            results.append((rows, cols, decrypted))

    for rows, cols, decrypted in results:
        print(f"rows={rows}, cols={cols} -> '{decrypted}'")

    return results


msg = input("Введите сообщение для шифрования: ")
cols = int(input("Введите количество столбцов: "))
rows = (len(msg) + cols - 1) // cols

encrypted_msg = encrypt_message(msg, rows, cols)
print(f"\nЗашифрованное сообщение: '{encrypted_msg}'")

decrypted_msg = decrypt_message(encrypted_msg, rows, cols)
print(f"Расшифрованное сообщение: '{decrypted_msg}'\n")

hack_msg = brute_force_decrypt(encrypted_msg)

