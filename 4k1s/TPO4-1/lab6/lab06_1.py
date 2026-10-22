"""Лабораторная работа 6, задание 1.

Программа открывает браузер, загружает сайт demoqa.com и находит элементы
пятью разными способами, сообщая о каждой находке и её содержимом.

Запуск:  python lab06_1.py
"""
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import WebDriverWait
from selenium.common.exceptions import NoSuchElementException, TimeoutException

BASE_URL = "https://demoqa.com"
IMPLICIT_WAIT = 5
EXPLICIT_WAIT = 15

LINE = "=" * 76


def header(text):
    print(f"\n{LINE}\n{text}\n{LINE}")


def report(name, element, extra=""):
    """Единый формат сообщения о найденном элементе."""
    text = (element.text or "").strip().replace("\n", " / ")
    if not text:
        text = f"<{element.tag_name}> без текста"
    print(f"  [НАЙДЕН]  {name}")
    print(f"            тег: <{element.tag_name}>  содержимое: {text[:66]}")
    if extra:
        print(f"            {extra}")


def not_found(name, reason):
    print(f"  [НЕ НАЙДЕН]  {name}  ({reason})")


def main():
    options = Options()
    options.add_argument("--window-size=1440,1000")
    driver = webdriver.Chrome(options=options)
    # НЕЯВНОЕ ожидание: действует на все вызовы find_element в течение сессии
    driver.implicitly_wait(IMPLICIT_WAIT)
    wait = WebDriverWait(driver, EXPLICIT_WAIT)

    try:
        driver.get(f"{BASE_URL}/automation-practice-form")

        # ── 1. By.ID и By.NAME ────────────────────────────────────────────
        header("Способ 1 — By.ID и By.NAME (самые стабильные локаторы)")

        # ЯВНОЕ ожидание: поле формы отрисовано
        first_name = wait.until(EC.presence_of_element_located((By.ID, "firstName")))
        report("поле имени, By.ID = 'firstName'", first_name,
               f"placeholder: {first_name.get_attribute('placeholder')}")

        genders = driver.find_elements(By.NAME, "gender")
        if genders:
            print(f"  [НАЙДЕНО]  переключателей пола, By.NAME = 'gender': {len(genders)}")
            for g in genders:
                label = driver.find_element(
                    By.CSS_SELECTOR, f"label[for='{g.get_attribute('id')}']")
                print(f"             id={g.get_attribute('id')} → «{label.text}»")
        else:
            not_found("группа By.NAME = 'gender'", "атрибут name отсутствует")

        # ── 2. CSS-селекторы, два составных примера ───────────────────────
        header("Способ 2 — составные CSS-селекторы")

        css_email = "form#userForm input#userEmail"
        try:
            el = driver.find_element(By.CSS_SELECTOR, css_email)
            report(f"поле почты внутри формы\n            селектор: {css_email}", el,
                   f"placeholder: {el.get_attribute('placeholder')}")
        except NoSuchElementException:
            not_found("поле почты", css_email)

        css_hobby = "#userForm div.form-check label.form-check-label[for='hobbies-checkbox-1']"
        try:
            el = driver.find_element(By.CSS_SELECTOR, css_hobby)
            report(f"подпись первого увлечения\n            селектор: {css_hobby}", el,
                   "путь: форма → блок чекбокса → подпись по атрибуту for")
        except NoSuchElementException:
            not_found("подпись увлечения", css_hobby)

        # ── 3. XPath, два составных примера ───────────────────────────────
        header("Способ 3 — составные XPath-выражения")

        xp_number = "//form[@id='userForm']//input[@id='userNumber']"
        try:
            el = driver.find_element(By.XPATH, xp_number)
            report(f"поле телефона внутри формы\n            XPath: {xp_number}", el,
                   f"placeholder: {el.get_attribute('placeholder')}")
        except NoSuchElementException:
            not_found("поле телефона", xp_number)

        # переход вверх по оси ancestor и снова вниз
        xp_label = ("//input[@id='hobbies-checkbox-2']"
                    "/ancestor::div[contains(@class,'form-check')][1]"
                    "//label")
        try:
            el = driver.find_element(By.XPATH, xp_label)
            report(f"подпись чекбокса через ось ancestor\n"
                   f"            XPath: {xp_label}", el,
                   "путь: чекбокс → вверх до своего блока → вниз до подписи")
        except NoSuchElementException:
            not_found("подпись чекбокса", xp_label)

        # ── 4. Частичный текст ссылки ─────────────────────────────────────
        header("Способ 4 — By.PARTIAL_LINK_TEXT (частичный текст ссылки)")
        driver.get(f"{BASE_URL}/elements")

        for fragment in ("Text Box", "Web Tables"):
            try:
                el = wait.until(EC.presence_of_element_located(
                    (By.PARTIAL_LINK_TEXT, fragment)))
                report(f"ссылка по фрагменту текста «{fragment}»", el,
                       f"href: {el.get_attribute('href')}")
            except TimeoutException:
                not_found(f"ссылка по фрагменту «{fragment}»", "истекло ожидание")

        # ── 5. Список элементов ───────────────────────────────────────────
        header("Способ 5 — поиск набора элементов (find_elements → список)")

        items = driver.find_elements(By.CSS_SELECTOR, "li.btn span")
        names = [i.text.strip() for i in items if i.text.strip()]
        print(f"  [НАЙДЕНО]  пунктов меню: {len(items)}, с текстом: {len(names)}")
        for i, name in enumerate(names[:8], 1):
            print(f"             {i}. {name}")

        links = driver.find_elements(By.TAG_NAME, "a")
        print(f"\n  [НАЙДЕНО]  ссылок на странице: {len(links)}")
        assert links, "на странице не найдено ни одной ссылки"

        header("Задание 1 выполнено")
    finally:
        driver.quit()


if __name__ == "__main__":
    main()
