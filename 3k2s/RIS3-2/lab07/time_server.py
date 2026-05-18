import socket, threading, time, json, sys, logging
from datetime import datetime

CONFIG = "C:\\Share\\cluster_config.json"   
PORT = 5555
PING_INTERVAL = 5
FAIL_THRESHOLD = 3

logging.basicConfig(level=logging.INFO, format="[%(asctime)s] %(levelname)s: %(message)s")

class TimeServer:
    def __init__(self, ip):
        self.ip = ip
        self.load_config()
        self.fail_count = 0
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.sock.bind((self.ip, PORT))
        logging.info(f"Сервер {self.ip}:{PORT} запущен. Координатор: {self.coordinator}")

    def load_config(self):  # грузим конфиг
        with open(CONFIG) as f:
            config = json.load(f)
        self.peers = [s for s in config["servers"] if s != self.ip]
        self.coordinator = config["coordinator"]

    def listen(self):
        while True:
            try:
                msg, addr = self.sock.recvfrom(1024)
                msg = msg.decode()
                if msg == "get_time":
                    logging.info(f"Получен запрос времени от {addr}")
                    now = datetime.now().strftime("%d%m%Y:%H:%M:%S")
                    self.sock.sendto(now.encode(), addr)
                elif msg == "ping":
                    logging.info(f"Получен ping от {addr}")
                    self.sock.sendto(b"pong", addr)
                elif msg == "election":
                    logging.info(f"Получен запрос на выборы от {addr}")
                    self.sock.sendto(b"ok", addr)
                    threading.Thread(target=self.start_election).start()
                elif msg.startswith("coordinator:"):
                    new_coordinator = msg.split(":")[1]
                    self.coordinator = new_coordinator
                    logging.info(f"Обновлён координатор: {self.coordinator}")
                    self.update_config()
            except Exception as e:
                logging.error(f"Ошибка в listen(): {e}")

    def health_check(self):
        while True:
            time.sleep(PING_INTERVAL)
            try:
                logging.info(f"[CHECK] Проверка координатора {self.coordinator}...")
                check_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
                check_sock.settimeout(1.0)
                check_sock.sendto(b"ping", (self.coordinator, PORT))
                check_sock.recvfrom(1024)
                self.fail_count = 0
                logging.info(f"[CHECK] Координатор {self.coordinator} доступен")
            except:
                self.fail_count += 1
                logging.warning(f"{self.coordinator} не отвечает ({self.fail_count})")
                if self.fail_count >= FAIL_THRESHOLD:
                    self.start_election()

    def start_election(self):
        logging.info(f"[ELECTION] Инициирую выборы. Мой IP: {self.ip}, соседи: {self.peers}")
        higher = [ip for ip in self.peers if ip > self.ip]
        got_ok = False

        for ip in higher:
            try:
                self.sock.sendto(b"election", (ip, PORT))
                self.sock.settimeout(1.0)
                data, _ = self.sock.recvfrom(1024)
                if data == b"ok":
                    logging.info(f"[ELECTION] Получен OK от {ip}, жду нового координатора")
                    got_ok = True
            except Exception as e:
                logging.warning(f"[ELECTION] Не удалось связаться с {ip}: {e}")

        self.sock.settimeout(None)

        if not got_ok:
            self.coordinator = self.ip
            logging.info(f"[ELECTION] Я стал координатором: {self.coordinator}")
            for peer in self.peers:
                msg = f"coordinator:{self.coordinator}"
                try:
                    self.sock.sendto(msg.encode(), (peer, PORT))
                    logging.info(f"[ELECTION] Уведомил {peer} о новом координаторе")
                except Exception as e:
                    logging.warning(f"[ELECTION] Не удалось уведомить {peer}: {e}")
            self.update_config()

    def update_config(self):
        try:
            with open(CONFIG, "r+") as f:
                data = json.load(f)
                data["coordinator"] = self.coordinator
                f.seek(0)
                json.dump(data, f)
                f.truncate()
            logging.info(f"[CONFIG] Обновлён файл конфигурации. Новый координатор: {self.coordinator}")
        except Exception as e:
            logging.error(f"[CONFIG] Ошибка при обновлении: {e}")

    def run(self):
        # Проверка при старте: если IP выше текущего координатора — попробовать взять лидерство
        if self.ip > self.coordinator:
            logging.info(f"[BOOT] Я выше координатора {self.coordinator}, инициирую выборы")
            self.start_election()

        threading.Thread(target=self.listen, daemon=True).start()
        threading.Thread(target=self.health_check, daemon=True).start()

        while True:
            time.sleep(1)

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python server.py <IP>")
        sys.exit(1)
    server = TimeServer(sys.argv[1])
    server.run()
