import tkinter as tk
import cv2 
from tkinter import filedialog
from PIL import Image, ImageTk
import numpy as np


class ImageEditor:
    def __init__(self, root):
        self.root = root
        self.root.title("Image Editor")
        self.root.configure(bg="#2C3E50")

        self.image = None
        self.processed_image = None

        self.canvas = tk.Canvas(root, width=500, height=300, bg="#34495E", highlightthickness=2, relief="ridge")
        self.canvas.grid(row=0, column=0, columnspan=3, padx=10, pady=10)

        self.open_button = tk.Button(root, text="Open", command=self.open_image, bg="#1ABC9C", fg="white", font=("Arial", 10, "bold"))
        self.open_button.grid(row=1, column=0, padx=5, pady=5)
        self.reset_button = tk.Button(root, text="Reset", command=self.reset_image, bg="#E74C3C", fg="white", font=("Arial", 10, "bold"))
        self.reset_button.grid(row=1, column=1, padx=5, pady=5)
        self.save_button = tk.Button(root, text="Save", command=self.save_image, bg="#3498DB", fg="white", font=("Arial", 10, "bold"))
        self.save_button.grid(row=1, column=2, padx=5, pady=5)

        self.edit_frame = tk.Frame(root, bg="#2C3E50", relief="ridge", bd=2)
        self.edit_frame.grid(row=0, column=3, rowspan=3, padx=10, pady=10)
        
        self.rotate_button = tk.Button(self.edit_frame, text="Rotate", command=self.rotate_image, bg="#9B59B6", fg="white", font=("Arial", 10, "bold"))
        self.rotate_button.pack(pady=5, padx=5)
        self.gray_button = tk.Button(self.edit_frame, text="Gray", command=self.convert_to_gray, bg="#F1C40F", fg="black", font=("Arial", 10, "bold"))
        self.gray_button.pack(pady=5, padx=5)
        self.light_button = tk.Button(self.edit_frame, text="Light", command=self.adjust_brightness, bg="#E67E22", fg="white", font=("Arial", 10, "bold"))
        self.light_button.pack(pady=5, padx=5)
        self.blur_button = tk.Button(self.edit_frame, text="Blur", command=self.apply_blur, bg="#16A085", fg="white", font=("Arial", 10, "bold"))
        self.blur_button.pack(pady=5, padx=5)

        self.contrast_slider = tk.Scale(self.edit_frame, from_=0.5, to=3.0, resolution=0.1, orient=tk.HORIZONTAL, label="Contrast", bg="#ECF0F1", fg="black", font=("Arial", 10))
        self.contrast_slider.pack(pady=5, padx=5)
        self.contrast_slider.bind("<Motion>", self.adjust_contrast)

    def open_image(self):
        file_path = filedialog.askopenfilename()
        if file_path:
            try:
                with open(file_path, "rb") as f:
                    file_bytes = np.asarray(bytearray(f.read()), dtype=np.uint8)
                    self.image = cv2.imdecode(file_bytes, cv2.IMREAD_COLOR)
                
                if self.image is None:
                    print(f"Ошибка: не удалось загрузить изображение {file_path}")
                    return

                self.processed_image = self.image.copy()
                self.display_image()
            except Exception as e:
                print(f"Ошибка открытия файла: {e}")

    def display_image(self):
        img = cv2.cvtColor(self.processed_image, cv2.COLOR_BGR2RGB)
        img = Image.fromarray(img)
        img.thumbnail((500, 300))
        self.tk_image = ImageTk.PhotoImage(img)
        self.canvas.create_image(250, 150, image=self.tk_image, anchor=tk.CENTER)

    def reset_image(self):
        if self.image is not None:
            self.processed_image = self.image.copy()
            self.display_image()

    def save_image(self):
        if self.processed_image is not None:
            file_path = filedialog.asksaveasfilename(defaultextension=".png", filetypes=[("PNG files", "*.png"), ("JPEG files", "*.jpg")])
            if file_path:
                cv2.imwrite(file_path, self.processed_image)

    def rotate_image(self):
        if self.processed_image is not None:
            self.processed_image = cv2.rotate(self.processed_image, cv2.ROTATE_90_CLOCKWISE)
            self.display_image()

    def convert_to_gray(self):
        if self.processed_image is not None:
            self.processed_image = cv2.cvtColor(self.processed_image, cv2.COLOR_BGR2GRAY)
            self.processed_image = cv2.cvtColor(self.processed_image, cv2.COLOR_GRAY2BGR)
            self.display_image()

    def adjust_brightness(self):
        if self.processed_image is not None:
            self.processed_image = cv2.convertScaleAbs(self.processed_image, alpha=1.2, beta=30)
            self.display_image()

    def apply_blur(self):
        if self.processed_image is not None:
            self.processed_image = cv2.GaussianBlur(self.processed_image, (5, 5), 0)
            self.display_image()

    def adjust_contrast(self, event):
        if self.processed_image is not None:
            alpha = self.contrast_slider.get()
            self.processed_image = cv2.convertScaleAbs(self.image, alpha=alpha, beta=0)
            self.display_image()

if __name__ == "__main__":
    root = tk.Tk()
    app = ImageEditor(root)
    root.mainloop()
