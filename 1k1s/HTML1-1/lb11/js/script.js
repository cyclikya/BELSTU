function showMessage(action) {
    const messageElement = document.getElementById('message');

    if (action === 'leaveReview') {
        messageElement.textContent = '"Оставьте пожалуйста отзыв:)"';
    } else if (action === 'tryAgain') {
        messageElement.textContent = '"Зря, обязательно попробуйте!!!"';
    }
}