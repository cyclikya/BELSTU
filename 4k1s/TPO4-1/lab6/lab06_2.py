"""Лабораторная работа 6, задание 2.

Пять автоматизированных тестов на сайте demoqa.com.
В каждом тесте используются оба вида ожиданий — неявное и явное,
и каждый тест завершается сравнением фактического результата с ожидаемым.

Запуск:  pytest -v lab06_2.py
"""
import pytest
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import Select, WebDriverWait
from selenium.common.exceptions import ElementClickInterceptedException

BASE_URL = "https://demoqa.com"
# таблица на странице Web Tables — обычная bootstrap-таблица
TABLE_ROWS = "div.web-tables-wrapper table tbody tr"
IMPLICIT_WAIT = 5
EXPLICIT_WAIT = 15


@pytest.fixture
def driver():
    """Новый браузер на каждый тест: тесты не влияют друг на друга."""
    options = Options()
    options.add_argument("--window-size=1440,1000")
    drv = webdriver.Chrome(options=options)
    # НЕЯВНОЕ ожидание: действует на все find_element в течение сессии
    drv.implicitly_wait(IMPLICIT_WAIT)
    yield drv
    drv.quit()


@pytest.fixture
def wait(driver):
    """ЯВНОЕ ожидание: ждём конкретное условие там, где это нужно."""
    return WebDriverWait(driver, EXPLICIT_WAIT)


def open_page(driver, wait, path):
    driver.get(f"{BASE_URL}{path}")
    wait.until(lambda d: d.execute_script("return document.readyState") == "complete")


def table_text(driver):
    """Весь текст тела таблицы."""
    return "\n".join(r.text for r in driver.find_elements(By.CSS_SELECTOR, TABLE_ROWS))


def visible_rows(driver):
    """Непустые строки таблицы."""
    return [r for r in driver.find_elements(By.CSS_SELECTOR, TABLE_ROWS)
            if r.text.strip()]


def click(driver, element):
    """Клик с запасным путём: снизу страницы demoqa висит рекламный блок."""
    driver.execute_script("arguments[0].scrollIntoView({block:'center'});", element)
    try:
        element.click()
    except ElementClickInterceptedException:
        driver.execute_script("arguments[0].click();", element)


# ─────────────────────── 1. Авторизация ───────────────────────
def test_login_with_invalid_credentials(driver, wait):
    """Вход с неверными данными не выполняется и даёт сообщение об ошибке."""
    open_page(driver, wait, "/login")

    # ЯВНОЕ ожидание: форма входа отрисована
    user_field = wait.until(EC.visibility_of_element_located((By.ID, "userName")))
    user_field.send_keys("nesushchestvuyushchiy_polzovatel")
    driver.find_element(By.ID, "password").send_keys("ZavedomoNevernyi123!")

    click(driver, wait.until(EC.element_to_be_clickable((By.ID, "login"))))

    # ЯВНОЕ ожидание: появилось сообщение об ошибке
    error = wait.until(EC.visibility_of_element_located((By.ID, "name")))

    # СРАВНЕНИЕ ФАКТИЧЕСКОГО И ОЖИДАЕМОГО РЕЗУЛЬТАТА
    actual = error.text.strip()
    assert "invalid username or password" in actual.lower(), (
        f"ожидали сообщение о неверных данных, получили «{actual}»")
    assert "/login" in driver.current_url, (
        "при неверных данных не должно происходить перехода в профиль")
    print(f"\nфактический результат: «{actual}», остались на странице входа")


# ─────────────────────── 2. Форма Text Box ───────────────────────
def test_text_box_form_submission(driver, wait):
    """Отправленная форма выводит ровно те данные, что были введены."""
    open_page(driver, wait, "/text-box")

    data = {
        "userName": "Угоренко Виолетта",
        "userEmail": "violetta.test@example.com",
        "currentAddress": "г. Минск, ул. Свердлова, 13а",
        "permanentAddress": "г. Гомель, ул. Советская, 10",
    }

    wait.until(EC.visibility_of_element_located((By.ID, "userName")))
    for field_id, value in data.items():
        field = driver.find_element(By.ID, field_id)
        field.clear()
        field.send_keys(value)

    click(driver, driver.find_element(By.ID, "submit"))

    # ЯВНОЕ ожидание: блок с результатом появился
    output = wait.until(EC.visibility_of_element_located((By.ID, "output")))

    # СРАВНЕНИЕ ФАКТИЧЕСКОГО И ОЖИДАЕМОГО РЕЗУЛЬТАТА
    actual = output.text
    for value in data.values():
        assert value in actual, (
            f"в результате нет введённого значения «{value}»")
    print(f"\nфактический результат: форма вывела все {len(data)} значения")


# ─────────────────────── 3. Добавление записи в таблицу ───────────────────────
NEW_RECORD = {
    "firstName": "Violetta",
    "lastName": "Ugorenko",
    "userEmail": "violetta.qa@example.com",
    "age": "21",
    "salary": "1500",
    "department": "QA",
}


def fill_record_form(driver, wait, values):
    wait.until(EC.visibility_of_element_located((By.ID, "firstName")))
    for field_id, value in values.items():
        field = driver.find_element(By.ID, field_id)
        field.clear()
        field.send_keys(value)
    click(driver, driver.find_element(By.ID, "submit"))


def test_web_tables_add_record(driver, wait):
    """Добавленная запись появляется в таблице со всеми значениями."""
    open_page(driver, wait, "/webtables")

    click(driver, wait.until(EC.element_to_be_clickable((By.ID, "addNewRecordButton"))))
    fill_record_form(driver, wait, NEW_RECORD)

    # ЯВНОЕ ожидание: таблица перестроилась и содержит новую почту
    wait.until(lambda d: NEW_RECORD["userEmail"] in table_text(d))

    # СРАВНЕНИЕ ФАКТИЧЕСКОГО И ОЖИДАЕМОГО РЕЗУЛЬТАТА
    table_text_value = table_text(driver)
    for value in NEW_RECORD.values():
        assert value in table_text_value, (
            f"в таблице нет значения «{value}» из добавленной записи")
    print(f"\nфактический результат: запись «{NEW_RECORD['userEmail']}» "
          f"найдена в таблице со всеми полями")


# ─────────────────────── 4. Сквозной сценарий ───────────────────────
def find_row(driver, email):
    """Строка таблицы, содержащая указанную почту."""
    for row in driver.find_elements(By.CSS_SELECTOR, TABLE_ROWS):
        if email in (row.text or ""):
            return row
    return None


def test_end_to_end_web_tables(driver, wait):
    """Полный цикл работы с записью: создание, поиск, правка, удаление."""
    open_page(driver, wait, "/webtables")
    email = "e2e.scenario@example.com"
    record = dict(NEW_RECORD, userEmail=email, firstName="Elena", lastName="Petrova")

    # Шаг 1: создать запись
    click(driver, wait.until(EC.element_to_be_clickable((By.ID, "addNewRecordButton"))))
    fill_record_form(driver, wait, record)
    wait.until(lambda d: email in table_text(d))
    print(f"\nшаг 1: запись «{email}» создана")

    # Шаг 2: найти её поиском
    search = wait.until(EC.visibility_of_element_located((By.ID, "searchBox")))
    search.clear()
    search.send_keys(email)
    wait.until(lambda d: len(visible_rows(d)) == 1)
    rows = visible_rows(driver)
    assert len(rows) == 1, f"поиск вернул {len(rows)} строк вместо одной"
    assert record["firstName"] in rows[0].text
    print("шаг 2: поиск вернул ровно одну строку")

    # Шаг 3: изменить зарплату
    row = find_row(driver, email)
    click(driver, row.find_element(By.CSS_SELECTOR, "[id^='edit-record']"))
    salary = wait.until(EC.visibility_of_element_located((By.ID, "salary")))
    salary.clear()
    salary.send_keys("9999")
    click(driver, driver.find_element(By.ID, "submit"))

    wait.until(lambda d: "9999" in (find_row(d, email) or d.find_element(
        By.TAG_NAME, "body")).text)
    assert "9999" in find_row(driver, email).text, "зарплата не изменилась"
    print("шаг 3: зарплата изменена на 9999")

    # Шаг 4: удалить запись
    row = find_row(driver, email)
    click(driver, row.find_element(By.CSS_SELECTOR, "[id^='delete-record']"))

    # ЯВНОЕ ожидание: запись исчезла из таблицы
    wait.until(lambda d: find_row(d, email) is None)

    # СРАВНЕНИЕ ФАКТИЧЕСКОГО И ОЖИДАЕМОГО РЕЗУЛЬТАТА
    assert find_row(driver, email) is None, (
        "после удаления запись всё ещё присутствует в таблице")
    assert not visible_rows(driver), (
        "после удаления единственной найденной записи таблица должна быть пуста")
    print("шаг 4: запись удалена, сквозной сценарий пройден")


# ─────────────────────── 5. Списки, чекбоксы, радиокнопки ───────────────────────
def test_select_dropdown_and_checkbox(driver, wait):
    """Выбор в выпадающих списках, установка чекбокса и радиокнопки."""
    open_page(driver, wait, "/select-menu")

    # обычный выпадающий список
    wait.until(EC.presence_of_element_located((By.ID, "oldSelectMenu")))
    colors = Select(driver.find_element(By.ID, "oldSelectMenu"))
    colors.select_by_visible_text("Green")
    assert colors.first_selected_option.text == "Green", (
        f"ожидали «Green», выбрано «{colors.first_selected_option.text}»")
    print(f"\nвыпадающий список: выбрано «{colors.first_selected_option.text}»")

    # список с множественным выбором
    cars = Select(driver.find_element(By.ID, "cars"))
    assert cars.is_multiple, "список машин должен допускать множественный выбор"
    cars.select_by_visible_text("Volvo")
    cars.select_by_visible_text("Audi")
    chosen = sorted(o.text for o in cars.all_selected_options)
    assert chosen == ["Audi", "Volvo"], f"выбрано {chosen} вместо Audi и Volvo"
    print(f"множественный выбор: {chosen}")

    # чекбокс и радиокнопка на форме практики
    open_page(driver, wait, "/automation-practice-form")
    hobby = wait.until(EC.presence_of_element_located((By.ID, "hobbies-checkbox-1")))
    assert not hobby.is_selected(), "чекбокс изначально должен быть снят"

    click(driver, driver.find_element(
        By.CSS_SELECTOR, "label[for='hobbies-checkbox-1']"))
    click(driver, driver.find_element(
        By.CSS_SELECTOR, "label[for='gender-radio-2']"))

    # ЯВНОЕ ожидание: элементы переключились
    wait.until(lambda d: d.find_element(By.ID, "hobbies-checkbox-1").is_selected())

    # СРАВНЕНИЕ ФАКТИЧЕСКОГО И ОЖИДАЕМОГО РЕЗУЛЬТАТА
    assert driver.find_element(By.ID, "hobbies-checkbox-1").is_selected(), (
        "чекбокс «Sports» не отметился")
    assert driver.find_element(By.ID, "gender-radio-2").is_selected(), (
        "радиокнопка «Female» не выбралась")
    print("чекбокс «Sports» и радиокнопка «Female» отмечены")
