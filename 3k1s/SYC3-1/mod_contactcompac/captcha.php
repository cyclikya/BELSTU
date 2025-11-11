<?php
/**
 * Простая CAPTCHA для модуля Compact Contact
 */

session_start();

header('Content-Type: image/png');

$width = 120;
$height = 40;

// Создаём изображение
$image = imagecreatetruecolor($width, $height);

// Цвета
$bgColor = imagecolorallocate($image, 245, 249, 255);
$textColor = imagecolorallocate($image, 20, 40, 80);
$noiseColor = imagecolorallocate($image, 200, 210, 230);

// Заливаем фон
imagefilledrectangle($image, 0, 0, $width, $height, $bgColor);

// Генерируем текст
$chars = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789';
$text = '';
for ($i = 0; $i < 5; $i++) {
    $text .= $chars[rand(0, strlen($chars) - 1)];
}

// Сохраняем в сессию
$_SESSION['contactcompact_captcha'] = $text;

// Добавляем немного "шума"
for ($i = 0; $i < 50; $i++) {
    imagesetpixel($image, rand(0, $width), rand(0, $height), $noiseColor);
}

// Путь к шрифту (можно использовать системный)
$font = __DIR__ . '/arial.ttf'; // если нет, Joomla нарисует системным
if (!file_exists($font)) {
    $font = null;
}

// Рисуем текст
if ($font) {
    imagettftext($image, 18, rand(-10, 10), 10, 28, $textColor, $font, $text);
} else {
    imagestring($image, 5, 15, 10, $text, $textColor);
}

// Выводим картинку
imagepng($image);
imagedestroy($image);
