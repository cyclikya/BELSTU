import React, { useState } from 'react';
import './App.css';

const products = [
  {
    id: 1,
    name: 'Букет Красных Роз',
    price: 50,
    quantity: 101,
    image: 'https://avatars.mds.yandex.net/i?id=2a0000018ff8f57c548631b0242dadddd282-1533388-fast-images&n=13',
    description: 'Элегантный букет красных роз, который добавит роскоши и красоты вашему дому или офису. Идеальный подарок для любого случая!',
    isNew: true,
    discount: 10,
    weight: 5.3,
  },
  {
    id: 2,
    name: 'Букет Синих Роз',
    price: 60,
    quantity: 51,
    image: 'https://avatars.mds.yandex.net/i?id=f17559b6c1c64a0d80d4dc25c2c2aa07f55942c1-4613979-images-thumbs&n=13',
    description: 'Чарующий букет синих роз, который принесет гармонию и спокойствие в ваше пространство. Прекрасный выбор для создания уютной атмосферы!',
    isNew: false,
    discount: 0,
    weight: 4.2,
  },
  {
    id: 3,
    name: 'Букет Желтых Роз',
    price: 45,
    quantity: 25,
    image: 'https://avatars.mds.yandex.net/i?id=eb15d39631f39a5a82efb6ada40f63f8a4fed1cf-12585680-images-thumbs&n=13',
    description: 'Яркий букет желтых роз, который принесет солнечное настроение в ваш дом или офис. Сочные оттенки желтого подарят вам радость и улучшат ваше настроение!',
    isNew: true,
    discount: 20,
    weight: 2.1,
  },
  {
    id: 4,
    name: 'Букет Розовых Роз',
    price: 40,
    quantity: 151,
    image: 'https://avatars.mds.yandex.net/i?id=dd76258548b8cccb8d3a49bff080e78a5e47407e-10928869-images-thumbs&n=13',
    description: 'Прекрасный букет розовых роз, который добавит романтики и нежности вашему дому или офису. Сочные оттенки розового подарят вам радость и улучшат ваше настроение!',
    isNew: false,
    discount: 15,
    weight: 5.1,
  },
];

const Catalog = () => {
  const [sortBy, setSortBy] = useState('name');
  const [basket, setBasket] = useState([]);
  const [orderFormVisible, setOrderFormVisible] = useState(false);

  const sortedProducts = [...products].sort((a, b) => {
    if (sortBy === 'name') {
      return a.name.localeCompare(b.name);
    } else if (sortBy === 'price') {
      return a.price - b.price;
    } else if (sortBy === 'quantity') {
      return a.quantity - b.quantity;
    } else if (sortBy === 'discount') {
      return b.discount - a.discount;
    }
    return 0;
  });

  const toggleBasket = (id) => {
    const index = basket.findIndex(item => item.id === id);
    if (index === -1) {
      const productToAdd = products.find(product => product.id === id);
      setBasket([...basket, { ...productToAdd, basket: true, quantity: 1 }]);
    } else {
      const newBasket = [...basket];
      newBasket.splice(index, 1);
      setBasket(newBasket);
    }
  };

  const calculateTotalCost = () => {
    return basket.reduce((total, item) => {
      return total + (item.price * item.quantity);
    }, 0);
  };

  const handleOrderClick = () => {
    setOrderFormVisible(true);
  };

  if (orderFormVisible) {
    return <OrderForm basket={basket} setOrderFormVisible={setOrderFormVisible} />;
  }

  return (
    <div>
      <div className='Block'>
        <label>
          Сортировать по:
          <select value={sortBy} onChange={(e) => setSortBy(e.target.value)}>
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
            <p className="price">Цена: {product.price} BYN</p>
            {product.discount > 0 && (
              <div>
                <p className="discount">Скидка: {product.discount}%</p>
              </div>
            )}
            <p>Количество: {product.quantity}</p>
            <p>Вес: {product.weight} кг</p>
            <button onClick={() => toggleBasket(product.id)}>
              {basket.some(item => item.id === product.id) ? 'Добавлено' : 'В корзину'}
            </button>
          </div>
        ))}
      </div>
      <Basket basket={basket} setBasket={setBasket} />
      <div className="total-cost">
        <p>Общая стоимость: {calculateTotalCost()} BYN</p>
      </div>
      {basket.length > 0 && <button onClick={handleOrderClick}>Заказать</button>}
    </div>
  );
};

const Basket = ({ basket, setBasket }) => {
  const updateQuantity = (id, quantity) => {
    const newBasket = basket.map(item => {
      if (item.id === id) {
        return { ...item, quantity: quantity };
      }
      return item;
    }).filter(item => item.quantity > 0);

    setBasket(newBasket);
  };

  return (
    <div className="basket">
      <h2>Корзина</h2>
      <ul>
        {basket.map((item) => (
          <li key={item.id} className="product">
            <h2>{item.name}</h2>
            <img src={item.image} alt={item.name} />
            <p className="price">Цена: {item.price} BYN</p>
            {item.discount > 0 && (
              <div>
                <p className="discount">Скидка: {item.discount}%</p>
              </div>
            )}
            <p>Вес: {item.weight} кг</p>
            <p>Количество: 
              <input
                type="number"
                min="0"
                value={item.quantity}
                onChange={(e) => updateQuantity(item.id, parseInt(e.target.value))}
              />
            </p>
          </li>
        ))}
      </ul>
    </div>
  );
};

const OrderForm = ({ basket, setOrderFormVisible }) => {
  const [deliveryMethod, setDeliveryMethod] = useState('courier');
  const [paymentMethod, setPaymentMethod] = useState('cash');
  const [address, setAddress] = useState('');

  const calculateDeliveryCost = () => {
    const totalWeight = basket.reduce((total, item) => total + (item.weight * item.quantity), 0);
    const totalPrice = basket.reduce((total, item) => total + (item.price * item.quantity), 0);

    if (deliveryMethod === 'courier') {
      return totalPrice > 200 ? 0 : 10;
    } else if (deliveryMethod === 'mail') {
      return 5 * totalWeight;
    } else {
      return 0;
    }
  };

  const handleDeliveryMethodChange = (e) => {
    setDeliveryMethod(e.target.value);
  };

  const handlePaymentMethodChange = (e) => {
    setPaymentMethod(e.target.value);
  };

  const handleAddressChange = (e) => {
    setAddress(e.target.value);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    alert('Order placed successfully!');
    setOrderFormVisible(false);
  };

  return (
    <form onSubmit={handleSubmit}>
      <h1>Оформление заказа</h1>
      <div>
        <label>
          <h3>Способ доставки:</h3>
          <div>
            <label>
              <input
                type="radio"
                value="courier"
                checked={deliveryMethod === 'courier'}
                onChange={handleDeliveryMethodChange}
              />
              Курьером (10 рублей, бесплатно при заказе от 200 рублей)
            </label><br/>
            <label>
              <input
                type="radio"
                value="mail"
                checked={deliveryMethod === 'mail'}
                onChange={handleDeliveryMethodChange}
              />
              Почтой (5 рублей за каждый килограмм массы)
            </label><br/>
            <label>
              <input
                type="radio"
                value="pickup"
                checked={deliveryMethod === 'pickup'}
                onChange={handleDeliveryMethodChange}
              />
              Самовывоз (бесплатно)
            </label>
          </div>
        </label>
      </div>
      <div>
        <label>
          <h3>Адрес доставки:</h3>
          <input
            type="text"
            value={address}
            onChange={handleAddressChange}
            disabled={deliveryMethod === 'pickup'}
            required={deliveryMethod !== 'pickup'}
          />
        </label>
      </div>
      <div>
        <label>
          <h3>Способ оплаты:</h3>
          <div>
            <label>
              <input
                type="radio"
                value="cash"
                checked={paymentMethod === 'cash'}
                onChange={handlePaymentMethodChange}
              />
              Наличными
            </label>
            <label>
              <input
                type="radio"
                value="card"
                checked={paymentMethod === 'card'}
                onChange={handlePaymentMethodChange}
              />
              Банковской картой
            </label>
            <label>
              <input
                type="radio"
                value="transfer"
                checked={paymentMethod === 'transfer'}
                onChange={handlePaymentMethodChange}
              />
              Банковским переводом
            </label>
          </div>
        </label>
      </div>
      <div className="total-cost">
        <p>Общая стоимость доставки: {calculateDeliveryCost()} BYN</p>
      </div>
      <button type="submit">Оформить заказ</button>
      <button type="button" onClick={() => setOrderFormVisible(false)}>Назад</button>
    </form>
  );
};

export default Catalog;

