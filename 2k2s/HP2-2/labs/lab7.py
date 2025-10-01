import pygame
import random
import math

# Инициализация Pygame
pygame.init()

# Размеры экрана
screen_width = 800
screen_height = 600
screen = pygame.display.set_mode((screen_width, screen_height))
pygame.display.set_caption("Homing Bullets")

# Цвета
white = (255, 255, 255)
black = (0, 0, 0)
red = (255, 0, 0)
green = (0, 255, 0)
cyan = (0, 255, 255)
yellow = (255, 255, 0)

# Игрок
player_size = 50
player_x = screen_width // 2 - player_size // 2
player_y = screen_height // 2 - player_size // 2
player_speed = 5
player_health = 100
num_bullets = 1  # начальное количество пуль

# Управление игроком
player_moving_left = False
player_moving_right = False
player_moving_up = False
player_moving_down = False

# Враги
enemy_size = 30
enemies = []
enemy_speed = 2
enemy_spawn_rate = 200  # ms
last_enemy_spawn = 0

# Пули
bullet_size = 10
bullets = []
bullet_speed = 10
bullet_cooldown = 200  # ms
last_bullet_time = 0
bullet_lifetime = 3000  # 3 секунды

# Аптечки
health_pack_size = 20
health_packs = []
health_pack_spawn_rate = 7000
last_health_pack_spawn = 0

# Улучшения
upgrade_size = 20
upgrades = []
upgrade_spawn_rate = 10000  # 10 сек
last_upgrade_spawn = 0

# Очки
score = 0
font = pygame.font.Font(None, 36)
game_over_font = pygame.font.Font(None, 72)

# Звуки
pygame.mixer.init()
shoot_sound = pygame.mixer.Sound("sounds/shoot.wav")
hit_sound = pygame.mixer.Sound("sounds/hit.wav")
upgrade_sound = pygame.mixer.Sound("sounds/upgrade.wav")

# Функция для создания врага
def spawn_enemy():
    side = random.choice(["top", "bottom", "left", "right"])
    if side == "top":
        enemy_x = random.randint(0, screen_width - enemy_size)
        enemy_y = 0
    elif side == "bottom":
        enemy_x = random.randint(0, screen_width - enemy_size)
        enemy_y = screen_height - enemy_size
    elif side == "left":
        enemy_x = 0
        enemy_y = random.randint(0, screen_height - enemy_size)
    else:  # right
        enemy_x = screen_width - enemy_size
        enemy_y = random.randint(0, screen_height - enemy_size)
    enemies.append([enemy_x, enemy_y])

# Функция для создания аптечки
def spawn_health_pack():
    health_pack_x = random.randint(0, screen_width - health_pack_size)
    health_pack_y = random.randint(0, screen_height - health_pack_size)
    health_packs.append([health_pack_x, health_pack_y])

# Функция для создания улучшения
def spawn_upgrade():
    upgrade_x = random.randint(0, screen_width - upgrade_size)
    upgrade_y = random.randint(0, screen_height - upgrade_size)
    upgrades.append([upgrade_x, upgrade_y])

# Функция для поиска ближайшего врага
def find_closest_enemy(x, y, enemies_list):
    closest_enemy = None
    closest_distance = float('inf')
    for enemy_x, enemy_y in enemies_list:
        distance = math.hypot(x - enemy_x, y - enemy_y)
        if distance < closest_distance:
            closest_distance = distance
            closest_enemy = (enemy_x, enemy_y)
    return closest_enemy

# Функция для стрельбы самонаводящимися пулями
def shoot():
    global last_bullet_time
    current_time = pygame.time.get_ticks()
    if current_time - last_bullet_time > bullet_cooldown:
        available_enemies = list(enemies)  # Копия списка врагов
        fired_bullets = 0

        bullet_x = player_x + player_size // 2 - bullet_size // 2
        bullet_y = player_y + player_size // 2 - bullet_size // 2

        while fired_bullets < num_bullets and available_enemies:
            closest_enemy = find_closest_enemy(bullet_x, bullet_y, available_enemies)

            if closest_enemy:
                bullets.append([bullet_x, bullet_y, closest_enemy, current_time])
                pygame.mixer.Sound.play(shoot_sound)
                last_bullet_time = current_time
                fired_bullets += 1
                available_enemies.remove([closest_enemy[0], closest_enemy[1]])
            else:
                break  # Нет больше врагов

# Основной цикл игры
running = True
game_over = False  # Переменная для состояния игры
clock = pygame.time.Clock()

while running:
    # Обработка событий
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False
        elif event.type == pygame.KEYDOWN:
            if game_over:  # Если игра окончена, ждем нажатия пробела для перезапуска
                if event.key == pygame.K_SPACE:
                    # Сброс всех переменных для начала новой игры
                    game_over = False
                    player_health = 100
                    player_x = screen_width // 2 - player_size // 2
                    player_y = screen_height // 2 - player_size // 2
                    num_bullets = 1
                    enemies = []
                    bullets = []
                    health_packs = []
                    upgrades = []
                    score = 0
                    last_enemy_spawn = 0
                    last_health_pack_spawn = 0
                    last_upgrade_spawn = 0
            else:
                if event.key == pygame.K_LEFT:
                    player_moving_left = True
                elif event.key == pygame.K_RIGHT:
                    player_moving_right = True
                elif event.key == pygame.K_UP:
                    player_moving_up = True
                elif event.key == pygame.K_DOWN:
                    player_moving_down = True
                elif event.key == pygame.K_SPACE:
                    shoot()
        elif event.type == pygame.KEYUP:
            if event.key == pygame.K_LEFT:
                player_moving_left = False
            elif event.key == pygame.K_RIGHT:
                player_moving_right = False
            elif event.key == pygame.K_UP:
                player_moving_up = False
            elif event.key == pygame.K_DOWN:
                player_moving_down = False

    # Логика игры (если не game over)
    if not game_over:
        # Движение игрока
        if player_moving_left:
            player_x -= player_speed
        if player_moving_right:
            player_x += player_speed
        if player_moving_up:
            player_y -= player_speed
        if player_moving_down:
            player_y += player_speed

        # Ограничение игрока границами экрана
        player_x = max(0, min(player_x, screen_width - player_size))
        player_y = max(0, min(player_y, screen_height - player_size))

        # Спавн врагов
        current_time = pygame.time.get_ticks()
        if current_time - last_enemy_spawn > enemy_spawn_rate:
            spawn_enemy()
            last_enemy_spawn = current_time

        # Спавн аптечек
        if current_time - last_health_pack_spawn > health_pack_spawn_rate:
            spawn_health_pack()
            last_health_pack_spawn = current_time

        # Спавн улучшений
        if current_time - last_upgrade_spawn > upgrade_spawn_rate:
            spawn_upgrade()
            last_upgrade_spawn = current_time

        # Движение врагов
        for enemy in enemies:
            dx = player_x - enemy[0]
            dy = player_y - enemy[1]
            distance = math.hypot(dx, dy)
            if distance > 0:
                enemy[0] += dx / distance * enemy_speed
                enemy[1] += dy / distance * enemy_speed

        # Движение пуль (самонаводящиеся)
        for bullet in bullets:
            bullet_x, bullet_y, target, spawn_time = bullet
            enemy_x, enemy_y = target

            dx = enemy_x - bullet_x
            dy = enemy_y - bullet_y
            distance = math.hypot(dx, dy)
            if distance > 0:
                bullet[0] += dx / distance * bullet_speed
                bullet[1] += dy / distance * bullet_speed

        # Проверка времени жизни пуль
        bullets_to_remove = []
        for bullet in bullets:
            if current_time - bullet[3] > bullet_lifetime:
                bullets_to_remove.append(bullet)

        # Исключение ошибки "ValueError: list.remove(x): x not in list"
        for bullet in bullets_to_remove:
            if bullet in bullets:  # Проверяем, есть ли пуля в списке перед удалением
                bullets.remove(bullet)

        # Столкновения пуль и врагов
        for bullet in bullets[:]:
            for enemy in enemies[:]:
                if bullet[0] < enemy[0] + enemy_size and bullet[0] + bullet_size > enemy[0] and \
                        bullet[1] < enemy[1] + enemy_size and bullet[1] + bullet_size > enemy[1]:
                    if bullet in bullets:
                        bullets.remove(bullet)
                    enemies.remove(enemy)
                    score += 10
                    pygame.mixer.Sound.play(hit_sound)
                    break  # Важно: чтобы пуля не попала в нескольких врагов за один кадр

        # Столкновения врагов и игрока
        for enemy in enemies[:]:
            if player_x < enemy[0] + enemy_size and player_x + player_size > enemy[0] and \
                    player_y < enemy[1] + enemy_size and player_y + player_size > enemy[1]:
                enemies.remove(enemy)
                player_health -= 10
                pygame.mixer.Sound.play(hit_sound)
                if player_health <= 0:
                    game_over = True

        # Столкновения с аптечками
        for health_pack in health_packs[:]:
            if player_x < health_pack[0] + health_pack_size and player_x + player_size > health_pack[0] and \
                    player_y < health_pack[1] + health_pack_size and player_y + player_size > health_pack[1]:
                health_packs.remove(health_pack)
                player_health = min(100, player_health + 50)
                pygame.mixer.Sound.play(upgrade_sound)

        # Столкновения с улучшениями
        for upgrade in upgrades[:]:
            if player_x < upgrade[0] + upgrade_size and player_x + player_size > upgrade[0] and \
                    player_y < upgrade[1] + upgrade_size and player_y + player_size > upgrade[1]:
                upgrades.remove(upgrade)
                pygame.mixer.Sound.play(upgrade_sound)
                num_bullets += 1  # увеличение количества пуль

    # Отрисовка
    screen.fill(black)  # Заливаем экран черным цветом

    if not game_over:
        # Отрисовка игровых объектов (если не game over)
        pygame.draw.rect(screen, white, (player_x, player_y, player_size, player_size))
        for enemy in enemies:
            pygame.draw.rect(screen, red, (enemy[0], enemy[1], enemy_size, enemy_size))
        for bullet in bullets:
            pygame.draw.rect(screen, white, (bullet[0], bullet[1], bullet_size, bullet_size))
        for health_pack in health_packs:
            pygame.draw.rect(screen, green, (health_pack[0], health_pack[1], health_pack_size, health_pack_size))
        for upgrade in upgrades:
            pygame.draw.rect(screen, yellow, (upgrade[0], upgrade[1], upgrade_size, upgrade_size))

        # Отображение здоровья и очков (если не game over)
        health_text = font.render(f"Health: {player_health}", True, white)
        score_text = font.render(f"Score: {score}", True, white)
        bullets_text = font.render(f"Bullets: {num_bullets}", True, white)
        screen.blit(health_text, (10, 10))
        screen.blit(score_text, (10, 40))
        screen.blit(bullets_text, (10, 70))
    else:
        # Вывод GAME OVER
        game_over_text = game_over_font.render("GAME OVER", True, red)
        text_rect = game_over_text.get_rect(center=(screen_width // 2, screen_height // 2))
        screen.blit(game_over_text, text_rect)

        # Вывод сообщения о перезапуске
        restart_text = font.render("Press SPACE to restart", True, white)
        restart_rect = restart_text.get_rect(center=(screen_width // 2, screen_height // 2 + 50))  # Немного ниже
        screen.blit(restart_text, restart_rect)

    pygame.display.flip()
    clock.tick(60)

pygame.quit()
