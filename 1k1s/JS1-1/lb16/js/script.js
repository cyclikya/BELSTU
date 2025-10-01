function showSubMenu(item) {
    closeMenu();
    item.querySelector('.submenu').style.display = 'block';
}

document.onmouseover = function(event) {
    var target = event.target;
    if (!target.matches('.menu-item, .submenu, .submenu a')) {
        closeMenu();
    }
};

function closeMenu() {
    var subm = document.querySelectorAll('.submenu');
    subm.forEach(function(item) {
        item.style.display = 'none';
    });
}




//------------------------------------------------

function startSwingAnimation() {
    const icon = document.querySelector('.icon');
    icon.classList.add('swing-animation');

    // Удаление класса через 0.5 секунды, чтобы анимация могла завершиться
    setTimeout(() => {
        icon.classList.remove('swing-animation');
    }, 200);
}

//------------------------------------------------
function showMessage(action) {
    const messageElement = document.getElementById('message');

    if (action === 'leaveReview') {
        messageElement.textContent = '"Оставьте пожалуйста отзыв:)"';
    } else if (action === 'tryAgain') {
        messageElement.textContent = '"Зря, обязательно попробуйте!!!"';
    }
}