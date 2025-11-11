<?php
defined('_JEXEC') or die;

session_start();

// Генерация капчи (если ещё не создана)
if (!isset($_SESSION['captcha_question'])) {
    $a = rand(1, 9);
    $b = rand(1, 9);
    $_SESSION['captcha_answer'] = $a + $b;
    $_SESSION['captcha_question'] = "$a + $b = ?";
}

// Обработка формы
if ($_SERVER['REQUEST_METHOD'] === 'POST' && isset($_POST['contact_submit'])) {
    $name    = trim($_POST['name'] ?? '');
    $email   = trim($_POST['email'] ?? '');
    $message = trim($_POST['message'] ?? '');
    $captcha = trim($_POST['captcha'] ?? '');

    $errors = [];

    // Проверка капчи
    if ($captcha != ($_SESSION['captcha_answer'] ?? -1)) {
        $errors[] = 'Неверно решён пример!';
    }

    if (empty($name))  $errors[] = 'Введите имя';
    if (empty($email)) $errors[] = 'Введите email';
    if (empty($message)) $errors[] = 'Введите сообщение';

    if (empty($errors)) {
        echo '<div class="contact-success">✅ Ваше сообщение успешно отправлено!</div>';
        // Очистить капчу после успешной отправки
        unset($_SESSION['captcha_question'], $_SESSION['captcha_answer']);
        // Здесь можно добавить логику отправки письма
    } else {
        echo '<div class="contact-error">❌ ' . implode('<br>', $errors) . '</div>';
    }
}

require JModuleHelper::getLayoutPath('mod_contactcompact', $params->get('layout', 'default'));
