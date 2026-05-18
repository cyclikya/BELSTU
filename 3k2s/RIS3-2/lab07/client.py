import socket

server_ip = "192.168.228.78" 
port = 5555

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.sendto(b"get_time", (server_ip, port))

try:
    sock.settimeout(2)
    data, _ = sock.recvfrom(1024)
    print(f"[CLIENT] Получено: {data.decode()}")
except Exception as e:
    print(f"[CLIENT] Ошибка: {e}")
