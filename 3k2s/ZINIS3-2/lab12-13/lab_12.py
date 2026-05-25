import tkinter as tk
from tkinter import filedialog, messagebox, ttk
from PIL import Image, ImageTk
import numpy as np
import time


class StegoApp:
    def __init__(self, root):
        self.root = root
        self.root.title("Стеганография LSB")
        self.root.geometry("1040x640")
        self.root.resizable(False, False)

        self.source_image = None
        self.result_image = None

        self.build_interface()

    def build_interface(self):
        self.root.configure(bg="#eef1f5")

        style = ttk.Style()
        style.theme_use("clam")
        style.configure("TButton", font=("Segoe UI", 10), padding=8)
        style.configure("TLabel", background="#eef1f5", font=("Segoe UI", 10))
        style.configure("Header.TLabel", font=("Segoe UI", 18, "bold"))
        style.configure("Info.TLabel", font=("Segoe UI", 9), foreground="#4b5563")

        main = tk.Frame(self.root, bg="#eef1f5")
        main.pack(fill="both", expand=True, padx=22, pady=18)

        left = tk.Frame(main, bg="#ffffff", bd=0, relief="flat")
        left.place(x=0, y=0, width=470, height=570)

        right = tk.Frame(main, bg="#ffffff", bd=0, relief="flat")
        right.place(x=495, y=0, width=500, height=570)

        ttk.Label(left, text="Параметры").pack(anchor="w", padx=18, pady=(18, 5))

        button_row = tk.Frame(left, bg="#ffffff")
        button_row.pack(anchor="w", padx=18, pady=8)

        ttk.Button(
            button_row,
            text="Открыть изображение",
            command=self.open_image
        ).grid(row=0, column=0, padx=(0, 10))

        ttk.Button(
            button_row,
            text="Очистить",
            command=self.clear_all
        ).grid(row=0, column=1)

        ttk.Label(left, text="Способ записи").pack(anchor="w", padx=18, pady=(10, 3))

        self.method = tk.StringVar(value="linear")
        method_box = ttk.Combobox(
            left,
            textvariable=self.method,
            values=["linear", "even_first"],
            state="readonly",
            width=25
        )
        method_box.pack(anchor="w", padx=18)

        ttk.Label(
            left,
            text="linear - запись подряд, even_first - сначала четные позиции",
            style="Info.TLabel"
        ).pack(anchor="w", padx=18, pady=(3, 12))

        ttk.Label(left, text="Сообщение").pack(anchor="w", padx=18)

        self.text = tk.Text(
            left,
            height=9,
            width=50,
            font=("Consolas", 10),
            bg="#f8fafc",
            fg="#111827",
            relief="solid",
            bd=1,
            wrap="word"
        )
        self.text.pack(padx=18, pady=(5, 12))

        action_row = tk.Frame(left, bg="#ffffff")
        action_row.pack(anchor="w", padx=18, pady=5)

        ttk.Button(
            action_row,
            text="Встроить",
            command=self.embed_message
        ).grid(row=0, column=0, padx=(0, 8))

        ttk.Button(
            action_row,
            text="Извлечь",
            command=self.extract_message
        ).grid(row=0, column=1, padx=(0, 8))

        ttk.Button(
            action_row,
            text="LSB слои",
            command=self.show_lsb_layers
        ).grid(row=0, column=2)

        self.status = tk.Label(
            left,
            text="Изображение не выбрано",
            bg="#ffffff",
            fg="#374151",
            font=("Segoe UI", 9),
            justify="left",
            anchor="w"
        )
        self.status.pack(fill="x", padx=18, pady=(18, 0))

        ttk.Label(right, text="Предпросмотр").pack(anchor="w", padx=18, pady=(18, 8))

        preview_area = tk.Frame(right, bg="#f8fafc", relief="solid", bd=1)
        preview_area.pack(padx=18, pady=5, fill="both", expand=True)

        self.preview_label = tk.Label(
            preview_area,
            bg="#f8fafc",
            text="Здесь будет изображение",
            fg="#6b7280",
            font=("Segoe UI", 11)
        )
        self.preview_label.pack(expand=True)

    def open_image(self):
        path = filedialog.askopenfilename(
            filetypes=[
                ("PNG и BMP", "*.png *.bmp"),
                ("PNG", "*.png"),
                ("BMP", "*.bmp")
            ]
        )

        if not path:
            return

        self.source_image = Image.open(path).convert("RGB")
        self.result_image = None
        self.show_preview(self.source_image)

        width, height = self.source_image.size
        capacity_bytes = (width * height * 3 - 32) // 8

        self.status.config(
            text=(
                f"Файл открыт: {path}\n"
                f"Размер: {width} x {height}\n"
                f"Доступная емкость: примерно {capacity_bytes} байт"
            )
        )

    def show_preview(self, image):
        copy = image.copy()
        copy.thumbnail((430, 430))

        photo = ImageTk.PhotoImage(copy)
        self.preview_label.config(image=photo, text="")
        self.preview_label.image = photo

    def message_to_bits(self, message):
        data = message.encode("utf-8")
        length = len(data)

        length_bits = format(length, "032b")
        data_bits = "".join(format(byte, "08b") for byte in data)

        return length_bits + data_bits

    def bits_to_message(self, bits):
        if len(bits) < 32:
            return ""

        length = int(bits[:32], 2)
        data_bits = bits[32:32 + length * 8]

        bytes_list = []

        for i in range(0, len(data_bits), 8):
            byte = data_bits[i:i + 8]

            if len(byte) == 8:
                bytes_list.append(int(byte, 2))

        return bytes(bytes_list).decode("utf-8", errors="replace")

    def get_indexes(self, total_count):
        if self.method.get() == "linear":
            return list(range(total_count))

        even = list(range(0, total_count, 2))
        odd = list(range(1, total_count, 2))
        return even + odd

    def embed_message(self):
        if self.source_image is None:
            messagebox.showerror("Ошибка", "Сначала выберите PNG или BMP изображение")
            return

        message = self.text.get("1.0", tk.END).strip()

        if not message:
            messagebox.showerror("Ошибка", "Введите сообщение")
            return

        start = time.time()

        bits = self.message_to_bits(message)

        image_array = np.array(self.source_image)
        flat = image_array.flatten()

        if len(bits) > len(flat):
            messagebox.showerror(
                "Ошибка",
                "Сообщение слишком большое для выбранного изображения"
            )
            return

        indexes = self.get_indexes(len(flat))

        for i, bit in enumerate(bits):
            index = indexes[i]
            flat[index] = (flat[index] & 254) | int(bit)

        result_array = flat.reshape(image_array.shape)
        self.result_image = Image.fromarray(result_array.astype(np.uint8))

        end = time.time()

        save_path = filedialog.asksaveasfilename(
            defaultextension=".png",
            filetypes=[
                ("PNG", "*.png"),
                ("BMP", "*.bmp")
            ]
        )

        if save_path:
            self.result_image.save(save_path)
            self.show_preview(self.result_image)

            used_percent = len(bits) / len(flat) * 100

            self.status.config(
                text=(
                    f"Сообщение встроено и сохранено\n"
                    f"Использовано бит: {len(bits)} из {len(flat)}\n"
                    f"Заполнение контейнера: {used_percent:.4f}%\n"
                    f"Время встраивания: {end - start:.6f} сек"
                )
            )

            messagebox.showinfo("Готово", "Стеганоконтейнер сохранен")

    def extract_message(self):
        path = filedialog.askopenfilename(
            filetypes=[
                ("PNG и BMP", "*.png *.bmp"),
                ("PNG", "*.png"),
                ("BMP", "*.bmp")
            ]
        )

        if not path:
            return

        start = time.time()

        image = Image.open(path).convert("RGB")
        image_array = np.array(image)
        flat = image_array.flatten()

        indexes = self.get_indexes(len(flat))

        first_32_bits = ""
        for i in range(32):
            first_32_bits += str(flat[indexes[i]] & 1)

        message_length = int(first_32_bits, 2)
        total_bits = 32 + message_length * 8

        if total_bits > len(flat):
            messagebox.showerror(
                "Ошибка",
                "В этом файле не найдено корректное скрытое сообщение"
            )
            return

        bits = ""
        for i in range(total_bits):
            bits += str(flat[indexes[i]] & 1)

        try:
            message = self.bits_to_message(bits)
        except Exception:
            messagebox.showerror("Ошибка", "Не удалось извлечь сообщение")
            return

        end = time.time()

        self.text.delete("1.0", tk.END)
        self.text.insert(tk.END, message)

        self.source_image = image
        self.result_image = image
        self.show_preview(image)

        self.status.config(
            text=(
                f"Сообщение извлечено из файла:\n"
                f"{path}\n"
                f"Размер сообщения: {message_length} байт\n"
                f"Время извлечения: {end - start:.6f} сек"
            )
        )

        messagebox.showinfo("Готово", "Сообщение извлечено")

    def show_lsb_layers(self):
        if self.result_image is None:
            messagebox.showerror(
                "Ошибка",
                "Сначала встроите сообщение или откройте стеганоконтейнер через извлечение"
            )
            return

        array = np.array(self.result_image)

        red = (array[:, :, 0] & 1) * 255
        green = (array[:, :, 1] & 1) * 255
        blue = (array[:, :, 2] & 1) * 255

        Image.fromarray(red.astype(np.uint8)).show(title="LSB red")
        Image.fromarray(green.astype(np.uint8)).show(title="LSB green")
        Image.fromarray(blue.astype(np.uint8)).show(title="LSB blue")

    def clear_all(self):
        self.source_image = None
        self.result_image = None
        self.text.delete("1.0", tk.END)
        self.preview_label.config(image="", text="Здесь будет изображение")
        self.preview_label.image = None
        self.status.config(text="Изображение не выбрано")


if __name__ == "__main__":
    root = tk.Tk()
    app = StegoApp(root)
    root.mainloop()