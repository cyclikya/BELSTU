import pygame
import sys
import math
import random

# Настройки окна
WIDTH, HEIGHT = 400, 400
FPS = 60

# Цвета
WHITE = (255, 255, 255)
BLUE = (50, 100, 255)
RED = (255, 50, 50)
BLACK = (0, 0, 0)
YELLOW = (255, 255, 0)
CYAN = (0, 255, 255)
GOLD_GLOW = (255, 215, 0)
SHIELD_COLOR = (200, 200, 255)

# Игрок
PLAYER_SIZE = 50
SPEED = 1
INERTIA = 0.8
BOOST_DURATION = 5 * FPS
SHIELD_DURATION = 3 * FPS

# Инициализация
pygame.init()
screen = pygame.display.set_mode((WIDTH, HEIGHT))
pygame.display.set_caption("Not My Flash - Final")
clock = pygame.time.Clock()
font = pygame.font.SysFont(None, 60)

# Игроки
player1 = pygame.Rect(100, HEIGHT // 2, PLAYER_SIZE, PLAYER_SIZE)
player2 = pygame.Rect(WIDTH - 150, HEIGHT // 2, PLAYER_SIZE, PLAYER_SIZE)

# Векторы движения
vel1 = [0, 0]
vel2 = [0, 0]

# Баффы
boosts = []
shields = []
boost_timer = [0, 0]
shield_timer = [0, 0]
last_boost_time = 0
boost_spawn_delay = 5000  # 5 секунд между спавнами

# Звуки
try:
    bump_sound = pygame.mixer.Sound(pygame.mixer.get_init() and pygame.mixer.Sound(file=None))
    boost_sound = pygame.mixer.Sound(pygame.mixer.get_init() and pygame.mixer.Sound(file=None))
    shield_sound = pygame.mixer.Sound(pygame.mixer.get_init() and pygame.mixer.Sound(file=None))
except:
    bump_sound = boost_sound = shield_sound = None

# Победитель
winner = None

def reset_game():
    global player1, player2, vel1, vel2, winner, boosts, shields, last_boost_time
    player1 = pygame.Rect(100, HEIGHT // 2, PLAYER_SIZE, PLAYER_SIZE)
    player2 = pygame.Rect(WIDTH - 150, HEIGHT // 2, PLAYER_SIZE, PLAYER_SIZE)
    vel1 = [0, 0]
    vel2 = [0, 0]
    winner = None
    boosts = []
    shields = []
    boost_timer[0] = boost_timer[1] = 0
    shield_timer[0] = shield_timer[1] = 0
    last_boost_time = pygame.time.get_ticks()
    spawn_boost()  # Начальный спавн при старте

def spawn_boost():
    # Спавн бустов с проверкой коллизий
    for _ in range(10):  # Максимум 10 попыток
        boost_type = random.choice(["power", "shield"])
        x = random.randint(20, WIDTH - 50)
        y = random.randint(20, HEIGHT - 50)
        new_boost = pygame.Rect(x, y, 30, 30)
        
        if not any(new_boost.colliderect(obj) for obj in [player1, player2] + boosts + shields):
            if boost_type == "power":
                boosts.append(new_boost)
            else:
                shields.append(new_boost)
            return

def draw():
    screen.fill(WHITE)
    
    # Рисуем бусты
    for boost in boosts:
        pygame.draw.rect(screen, YELLOW, boost)
    for shield in shields:
        pygame.draw.rect(screen, CYAN, shield)
    
    # Игроки с эффектами
    pygame.draw.rect(screen, BLUE, player1)
    pygame.draw.rect(screen, RED, player2)
    
    # Эффекты баффов
    if boost_timer[0] > 0:
        pygame.draw.rect(screen, GOLD_GLOW, player1.inflate(10, 10), 3)
    if boost_timer[1] > 0:
        pygame.draw.rect(screen, GOLD_GLOW, player2.inflate(10, 10), 3)
    
    if shield_timer[0] > 0:
        pygame.draw.rect(screen, SHIELD_COLOR, player1.inflate(15, 15), 3)
    if shield_timer[1] > 0:
        pygame.draw.rect(screen, SHIELD_COLOR, player2.inflate(15, 15), 3)
    
    if winner:
        text = font.render(f"{winner} WINS!", True, BLACK)
        screen.blit(text, (WIDTH // 2 - text.get_width() // 2, HEIGHT // 2 - text.get_height() // 2))
    
    pygame.display.flip()

def move_players(keys):
    global vel1, vel2
    
    # Управление игроком 1
    vel1 = [0, 0]
    if keys[pygame.K_w]: vel1[1] -= SPEED * (1.5 if boost_timer[0] > 0 else 1)
    if keys[pygame.K_s]: vel1[1] += SPEED * (1.5 if boost_timer[0] > 0 else 1)
    if keys[pygame.K_a]: vel1[0] -= SPEED * (1.5 if boost_timer[0] > 0 else 1)
    if keys[pygame.K_d]: vel1[0] += SPEED * (1.5 if boost_timer[0] > 0 else 1)

    # Управление игроком 2
    vel2 = [0, 0]
    if keys[pygame.K_UP]: vel2[1] -= SPEED * (1.5 if boost_timer[1] > 0 else 1)
    if keys[pygame.K_DOWN]: vel2[1] += SPEED * (1.5 if boost_timer[1] > 0 else 1)
    if keys[pygame.K_LEFT]: vel2[0] -= SPEED * (1.5 if boost_timer[1] > 0 else 1)
    if keys[pygame.K_RIGHT]: vel2[0] += SPEED * (1.5 if boost_timer[1] > 0 else 1)

    # Применяем движение
    player1.x += vel1[0]
    player1.y += vel1[1]
    player2.x += vel2[0]
    player2.y += vel2[1]

def check_bounds():
    global winner
    if not screen.get_rect().contains(player1) and shield_timer[0] <= 0:
        winner = "RED"
    if not screen.get_rect().contains(player2) and shield_timer[1] <= 0:
        winner = "BLUE"

def check_collision():
    global vel1, vel2
    
    if player1.colliderect(player2):
        dx = player2.centerx - player1.centerx
        dy = player2.centery - player1.centery
        magnitude = max(1, math.sqrt(dx*dx + dy*dy))
        
        power1 = 1.5 if boost_timer[0] > 0 else 1
        power2 = 1.5 if boost_timer[1] > 0 else 1
        
        push_x = int(dx / magnitude * 15)
        push_y = int(dy / magnitude * 15)
        
        if shield_timer[0] <= 0:
            player1.x -= int(push_x * power2 * 0.7)
            player1.y -= int(push_y * power2 * 0.7)
            vel1 = [-push_x * power2 * 0.5, -push_y * power2 * 0.5]
        
        if shield_timer[1] <= 0:
            player2.x += int(push_x * power1 * 0.7)
            player2.y += int(push_y * power1 * 0.7)
            vel2 = [push_x * power1 * 0.5, push_y * power1 * 0.5]
        
        if bump_sound:
            bump_sound.play()

def check_powerups():
    global boosts, shields, boost_timer, shield_timer, last_boost_time
    
    # Бусты силы
    for boost in boosts[:]:
        if player1.colliderect(boost):
            boosts.remove(boost)
            boost_timer[0] = BOOST_DURATION
            if boost_sound: boost_sound.play()
            last_boost_time = pygame.time.get_ticks()
            spawn_boost()  # Добавляем немедленный спавн нового буста
        elif player2.colliderect(boost):
            boosts.remove(boost)
            boost_timer[1] = BOOST_DURATION
            if boost_sound: boost_sound.play()
            last_boost_time = pygame.time.get_ticks()
            spawn_boost()
    
    # Щиты
    for shield in shields[:]:
        if player1.colliderect(shield):
            shields.remove(shield)
            shield_timer[0] = SHIELD_DURATION
            if shield_sound: shield_sound.play()
            last_boost_time = pygame.time.get_ticks()
            spawn_boost()
        elif player2.colliderect(shield):
            shields.remove(shield)
            shield_timer[1] = SHIELD_DURATION
            if shield_sound: shield_sound.play()
            last_boost_time = pygame.time.get_ticks()
            spawn_boost()

def update_timers():
    global boost_timer, shield_timer, last_boost_time
    
    # Убираем автоспавн по времени, теперь спавн только после подбора
    for i in range(2):
        if boost_timer[i] > 0: boost_timer[i] -= 1
        if shield_timer[i] > 0: shield_timer[i] -= 1

# В игровом цикле убираем вызов spawn_boost из условия таймера
while True:
    clock.tick(FPS)
    keys = pygame.key.get_pressed()

    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            pygame.quit()
            sys.exit()
        if event.type == pygame.KEYDOWN and event.key == pygame.K_SPACE and winner:
            reset_game()

    if not winner:
        move_players(keys)
        check_collision()
        check_bounds()
        check_powerups()
        update_timers()
    
    # Постепенное замедление
    player1.x += vel1[0] * INERTIA
    player1.y += vel1[1] * INERTIA
    player2.x += vel2[0] * INERTIA
    player2.y += vel2[1] * INERTIA
    
    vel1 = [v * INERTIA for v in vel1]
    vel2 = [v * INERTIA for v in vel2]
    
    draw()
