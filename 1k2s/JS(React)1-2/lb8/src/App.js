import React, { useState } from 'react';
import './App.css';

const products = [
  {
    id: 1,
    name: 'Красные цветы',
    price: 50,
    quantity: 20,
    image: 'https://i.pinimg.com/originals/6a/18/79/6a187995e3d75f4263a184f0f311cf40.jpg',
    description: 'Прекрасный красный цветок для украшения вашего дома или офиса. Этот цветок привнесет красоту и свежесть в ваше пространство!',
    isNew: true,
    discount: 10,
  },
  {
    id: 2,
    name: 'Синие цветы',
    price: 60,
    quantity: 15,
    image: 'https://avatars.mds.yandex.net/i?id=9379656fdbc837e3f0b44a4bf8261e2323d572fd-9739761-images-thumbs&n=13',
    description: 'Очаровательный синий цветок, который подарит вашему дому или офису нотки гармонии и спокойствия. Идеальный выбор для создания уютного атмосферного!',
    isNew: false,
    discount: 0,
  },
  {
    id: 3,
    name: 'Желтые цветы',
    price: 45,
    quantity: 10,
    image: 'https://avatars.mds.yandex.net/i?id=2a00000179f4b83f310cf9f0daa37fd8bfe3-3751806-images-thumbs&n=13',
    description: 'Яркий желтый цветок, который принесет солнечное настроение в ваш дом или офис. Сочные оттенки желтого подарят вам радость и улучшат ваше настроение!',
    isNew: true,
    discount: 20,
  },
  {
    id: 3,
    name: 'Розовые цветы',
    price: 40,
    quantity: 10,
    image: 'https://avatars.mds.yandex.net/i?id=c0a1f88aedb895eb82a2287845a28623677d950c-5236957-images-thumbs&n=13',
    description: 'Яркий желтый цветок, который принесет солнечное настроение в ваш дом или офис. Сочные оттенки желтого подарят вам радость и улучшат ваше настроение!',
    isNew: false,
    discount: 15,
  },
];

const Catalog = () => {
  const [sortBy, setSortBy] = useState('name');

  const sortedProducts = [...products].sort((a, b) => {
    if (sortBy === 'name') {
      return a.name.localeCompare(b.name); //для сравнения строк по алфавиту
    } else if (sortBy === 'price') {
      return a.price - b.price;
    } else if (sortBy === 'quantity') {
      return a.quantity - b.quantity;
    } else if (sortBy === 'discount') {
      return b.discount - a.discount;
    }
    return 0;
  });

  return (
    <div>
      <div style={{ textAlign: 'center', marginBottom: '20px', marginTop: '20px' }}>
        <label>
          Сортировать по:
          <select value={sortBy} onChange={(e) => setSortBy(e.target.value)} style={{ marginLeft: '10px'}}>
            <option value="name">Название</option>
            <option value="price">Цена</option>
            <option value="quantity">Количество</option>
            <option value="discount">Скидка</option>
          </select>
        </label>
      </div>
      <div className="catalog">
        {sortedProducts.map((product) => (
          <div key={product.id} className="product">
            {product.isNew && <span className="new">New!</span>}
            <h2>{product.name}</h2>
            <img src={product.image} alt={product.name} />
            <p>{product.description}</p>
            {product.discount > 0 && (
              <div>
                <p className="price">Цена: {product.price}BYN</p>
                <p className="discount">Скидка: {product.discount}%</p>
              </div>
            )}
            {!product.discount && <p className="price">Цена: {product.price}BYN</p>}
            <p>Количество: {product.quantity}</p>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Catalog;