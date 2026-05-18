import socket, json, logging

PORT = 5555
CONFIG = "C:\\Share\\cluster_config.json"   
logging.basicConfig(level=logging.INFO, format="[%(asctime)s] %(levelname)s: %(message)s")

def load_coordinator():
    with open(CONFIG) as f:
        return json.load(f)["coordinator"]

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.bind(("0.0.0.0", PORT))

logging.info("Посредник запущен на 0.0.0.0:5555")

while True:
    try:
        msg, addr = sock.recvfrom(1024)
        client_ip = addr[0]
        logging.info(f"Принят запрос от клиента {client_ip}")

        coordinator = load_coordinator()
        logging.info(f"Текущий координатор: {coordinator}")

        proxy = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        proxy.settimeout(2)
        proxy.sendto(msg, (coordinator, PORT))
        response, _ = proxy.recvfrom(1024)

        sock.sendto(response, addr)
        logging.info(f"Ответ отправлен клиенту {client_ip} от координатора {coordinator}")
    except Exception as e:
        logging.error(f"Ошибка при обработке запроса: {e}")
