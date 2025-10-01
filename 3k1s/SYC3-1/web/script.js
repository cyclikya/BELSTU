const tours = [
    {
        name: "Италия",
        desc: "Погрузитесь в атмосферу искусства, архитектуры и вкусной кухни Италии.",
        price: "от 1200$",
        img: "https://avatars.mds.yandex.net/i?id=b429a5efdb5ad673877f455c907bf43aaf45c746-15439472-images-thumbs&n=13"
    },
    {
        name: "Япония",
        desc: "Откройте для себя сочетание древних традиций и современных технологий в Японии.",
        price: "от 1500$",
        img: "https://avatars.mds.yandex.net/i?id=812dc440ae25beae3df70a1f3c3ba4c2f6bd9533-16334266-images-thumbs&n=13"
    },
    {
        name: "Исландия",
        desc: "Исследуйте вулканы, водопады и ледники Исландии в дикой природе.",
        price: "от 1700$",
        img: "https://avatars.mds.yandex.net/i?id=7e38db281ed3cf718ed9de552b6b5c0904519791-9098836-images-thumbs&n=13"
    },
    {
        name: "Франция",
        desc: "Париж, Лувр, Эйфелева башня и романтика.",
        price: "от 1100$",
        img: "https://images.unsplash.com/photo-1502602898657-3e91760cbb34"
    },
    {
        name: "Египет",
        desc: "Познакомьтесь с древними пирамидами и насладитесь Красным морем в Египте.",
        price: "от 900$",
        img: "https://avatars.mds.yandex.net/i?id=7e27aa59924977fa66431796f97631fdc485429a-10139706-images-thumbs&n=13"
    },
    {
        name: "Испания",
        desc: "Барселона, море и захватывающие культурные достопримечательности Испании.",
        price: "от 1300$",
        img: "https://images.unsplash.com/photo-1528909514045-2fa4ac7a08ba"
    },
    {
        name: "Мальдивы",
        desc: "Отдых на райских островах с белым песком и бирюзовой водой Мальдив.",
        price: "от 2500$",
        img: "https://images.unsplash.com/photo-1507525428034-b723cf961d3e"
    },
    {
        name: "Норвегия",
        desc: "Увидьте фьорды, северное сияние и уникальную природу Норвегии.",
        price: "от 1800$",
        img: "https://images.unsplash.com/photo-1501785888041-af3ef285b470"
    }
];

// Вывод туров на главной
document.addEventListener("DOMContentLoaded", () => {
    const toursList = document.getElementById("tours-list");
    if (toursList) {
        tours.forEach(tour => {
            const card = document.createElement("div");
            card.className = "card";
            card.innerHTML = `
                <img src="${tour.img}" alt="${tour.name}">
                <h3>${tour.name}</h3>
                <h4>${tour.price}</h4>
                <p>${tour.desc}</p>
                <a href="form.html?tour=${encodeURIComponent(tour.name)}" class="btn">Заказать</a>
            `;
            toursList.appendChild(card);
        });
    }

    // Подставляем список туров в форму
    const tourSelect = document.getElementById("tour-select");
    if (tourSelect) {
        tours.forEach(tour => {
            const option = document.createElement("option");
            option.value = tour.name;
            option.textContent = tour.name;
            tourSelect.appendChild(option);
        });

        // Подставляем выбранный тур из ссылки
        const params = new URLSearchParams(window.location.search);
        const selectedTour = params.get("tour");
        if (selectedTour) {
            tourSelect.value = selectedTour;
        }
    }

    // Обработка формы
    const orderForm = document.getElementById("order-form");
    if (orderForm) {
        orderForm.addEventListener("submit", (e) => {
            e.preventDefault();
            alert("Заявка отправлена, мы свяжемся с вами в течение 2 рабочих дней!");
            orderForm.reset();
        });
    }
});

