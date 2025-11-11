<?php
defined('_JEXEC') or die;
?>

<link rel="stylesheet" href="<?php echo JUri::base() . 'modules/mod_contactcompact/css/style.css'; ?>">

<div class="contact-compact">
    <button class="toggle-button" onclick="toggleContactForm()">📩 Связаться</button>

    <div id="contact-form" class="contact-form hidden">
        <form method="post">
            <label>Имя:</label>
            <input type="text" name="name" placeholder="Ваше имя">

            <label>Email:</label>
            <input type="email" name="email" placeholder="Ваш email">

            <label>Сообщение:</label>
            <textarea name="message" placeholder="Ваше сообщение"></textarea>

            <label>Решите пример: 
                <strong><?php echo $_SESSION['captcha_question'] ?? ''; ?></strong>
            </label>
            <input type="text" name="captcha" placeholder="Ответ">

            <div class="buttons">
                <button type="submit" name="contact_submit">Отправить</button>
                <button type="button" onclick="toggleContactForm()">Закрыть</button>
            </div>
        </form>
    </div>
</div>

<script>
function toggleContactForm() {
    document.getElementById('contact-form').classList.toggle('hidden');
}
</script>
