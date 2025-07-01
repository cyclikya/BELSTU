
function handleProductLinkClick(event) {
	event.preventDefault();
	var price = event.target.getAttribute('data-price');
	localStorage.setItem('productPrice', price);
	window.location.href = 'order.html';
}


var productLinks = document.querySelectorAll('.product-link');
productLinks.forEach(function(link) {
	link.addEventListener('click', handleProductLinkClick);
});
