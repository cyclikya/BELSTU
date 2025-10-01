# Животные в зоопарке
# Классы:
# Animal (name, age).
#   метод move() выводит сообщение «Я двигаюсь».
# Mammal, Bird, Fish (вид, скорость)
#   методы реализуют запись и считывание полей (проверка корректности) выведите информацию по каждому животному функцией print()
#  метод move() выводит сообщение «Я бегаю» или «Я летаю»….
# Действия:
#  Создайте список животных разных видов.
#  Выведите кто умеет летать, плавать.
#  Проверьте, кто самый старый.
#  Определите самого быстрого среди птиц зоопарка

class Animal:
    @property
    def name(self): return self._name
    @name.setter
    def name(self, value):
        if not value:
            raise ValueError("Имя не может быть пустым")
        self._name = value

    @property
    def age(self): return self._age
    @age.setter
    def age(self, value):
        if not isinstance(value, int) or value < 0:
            raise ValueError("Возраст не может быть отрицательным")
        self._age = value

    def __init__(self, name, age):
        self.name = name
        self.age = age
    
    def move(self):
        print("Я двигаюсь")
    
    def __str__(self):
        return f"Животное: {self.name}, Возраст: {self.age} лет"

class Mammal(Animal):
    def __init__(self, name, age, species, speed):
        super().__init__(name, age)
        self.species = species
        self.speed = max(0, speed)
    
    def move(self):
        print("Я бегаю")
    
    def __str__(self):
        return super().__str__() + f", Вид: {self.species}, Скорость: {self.speed} км/ч"

class Bird(Animal):
    def __init__(self, name, age, species, speed):
        super().__init__(name, age)
        self.species = species
        self.speed = max(0, speed)
    
    def move(self):
        print("Я летаю")
    
    def __str__(self):
        return super().__str__() + f", Вид: {self.species}, Скорость: {self.speed} км/ч"

class Fish(Animal):
    def __init__(self, name, age, species, speed):
        super().__init__(name, age)
        self.species = species
        self.speed = max(0, speed)
    
    def move(self):
        print("Я плаваю")
    
    def __str__(self):
        return super().__str__() + f", Вид: {self.species}, Скорость: {self.speed} км/ч"

zoo = [
    Mammal("Лев", -5, "Хищник", 50),
    Bird("Орел", 3, "Хищная птица", 80),
    Fish("Акула", 7, "Морская", 40),
    Bird("Попугай", 2, "Тропическая птица", 20),
    Mammal("Слон", 10, "Травоядное", 25)
]

for animal in zoo:
    print(animal)
    animal.move()

print("\nЖивотные, которые умеют летать:")
for animal in zoo:
    if isinstance(animal, Bird):
        print(animal.name)

print("\nЖивотные, которые умеют плавать:")
for animal in zoo:
    if isinstance(animal, Fish):
        print(animal.name)

oldest_animal = max(zoo, key=lambda x: x.age)
print(f"\nСамое старое животное: {oldest_animal.name}, Возраст: {oldest_animal.age} лет")

fastest_bird = max((animal for animal in zoo if isinstance(animal, Bird)), key=lambda x: x.speed, default=None)
if fastest_bird:
    print(f"\nСамая быстрая птица: {fastest_bird.name}, Скорость: {fastest_bird.speed} км/ч")
