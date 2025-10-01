import React, { useState, useEffect } from 'react'; 
import './catalog.css';
import logo from './img/logo.png';
import pineapple from './img/pineapple.png'

import vitamix_cherry from './img/vitamix_cherry.png';
import vitamix_limon_myata from './img/vitamix_limon-myata.png';
import vitamix_multifrukt from './img/vitamix_multifrukt.png';
import vitamix_persik from './img/vitamix_persik.png';
import vitamix_yabloko_vinograd from './img/vitamix_yabloko-vinograd.png';

import yourtonic_berrymore from './img/yourtonic_berrymore.png';
import yourtonic_bitter_lemon from './img/yourtonic_bitter_lemon.png';
import yourtonic_indian from './img/yourtonic_indian.png';

import zeronad_grape from './img/zeronad-grape.png';
import zeronad_green_kiwi_lime from './img/zeronad-green-kiwi-lime.png';
import zeronad_banana from './img/zeronad_banana_.png';
import zeronad_grapefruit from './img/zeronad_grapefruit.png';

import yourtea_lesnie_yagodi from './img/yourtea_lesnie_yagodi.png';
import yourtea_limon_myata from './img/yourtea_limon_myata.png';
import yourtea_persik_ from './img/yourtea_persik_.png';

import bottle1 from './img/bottle1.png';
import bottle2 from './img/bottle2.png';
import bottle3 from './img/bottle3.png';

import bagIcon from './img/basket.png';
import close from './img/close.png';

const products = [
    { id: 1, name: 'VITAMIX Cherry', price: 2.99, image: vitamix_cherry, description: 'со вкусом вишни', isNew: true },
    { id: 2, name: 'VITAMIX Lemon-Mint', price: 2.89, image: vitamix_limon_myata, description: 'со вкусом лимон-мята', isNew: true },
    { id: 3, name: 'VITAMIX Multifruit', price: 3.49, image: vitamix_multifrukt, description: 'со вкусом мультифрукт', isNew: false },
    { id: 4, name: 'VITAMIX Peach', price: 3.49, image: vitamix_persik, description: 'со вкусом персика', isNew: false },
    { id: 5, name: 'VITAMIX Apple-Grapes', price: 1.99, image: vitamix_yabloko_vinograd, description: 'со вкусом яблоко-виноград', isNew: false },
    { id: 6, name: 'YOURTONIC BerryMore', price: 3.99, image: yourtonic_berrymore, description: 'тоник с ягодами', isNew: true },
    { id: 7, name: 'YOURTONIC Lemon', price: 2.99, image: yourtonic_bitter_lemon, description: 'тоник с лимоном', isNew: true },
    { id: 8, name: 'YOURTONIC Indian', price: 2.99, image: yourtonic_indian, description: 'индийский тоник', isNew: false },
    { id: 9, name: 'ZERONAD Grapes', price: 1.99, image: zeronad_grape, description: 'со вкусом винограда', isNew: true },
    { id: 10, name: 'ZERONAD Kiwi-Lime', price: 1.89, image: zeronad_green_kiwi_lime, description: 'со вкусом киви-лайм', isNew: true },
    { id: 11, name: 'ZERONAD Banana', price: 2.49, image: zeronad_banana, description: 'со вкусом банана', isNew: false },
    { id: 12, name: 'ZERONAD Grapefruit', price: 2.99, image: zeronad_grapefruit, description: 'со вкусом грейпфрута', isNew: true },
    { id: 13, name: 'YOURTEA Wild berries', price: 3.99, image: yourtea_lesnie_yagodi, description: 'чай лесные ягоды', isNew: true },
    { id: 14, name: 'YOURTEA Lemon-Mint', price: 3.49, image: yourtea_limon_myata, description: 'чай лимон и мята', isNew: false },
    { id: 15, name: 'YOURTEA Peach', price: 3.99, image: yourtea_persik_, description: 'чай персик', isNew: false },
    { id: 16, name: 'YOURWATER Still water', price: 0.99, image: bottle1, description: 'негазированная вода', isNew: false },
    { id: 17, name: 'YOURWATER CARBON', price: 0.89, image: bottle2, description: 'газированная вода', isNew: false },
    { id: 18, name: 'YOURWATER Sport', price: 1.09, image: bottle3, description: 'негазированная вода', isNew: false },
  ];
  
  const Catalog = () => {
    const [searchTerm, setSearchTerm] = useState('');
    const [cart, setCart] = useState([]);
    const [isCartVisible, setIsCartVisible] = useState(false);
    const [sortBy, setSortBy] = useState('name');

    useEffect(() => {
        const storedCart = JSON.parse(localStorage.getItem('cart')) || [];
        setCart(storedCart);
    }, []);

    useEffect(() => {
        localStorage.setItem('cart', JSON.stringify(cart));
    }, [cart]);

    const handleAddProduct = (productId) => {
        const existingProduct = cart.find(item => item.id === productId);
        if (existingProduct) {
            setCart(cart.map(item => 
                item.id === productId ? { ...item, count: item.count + 1 } : item
            ));
        } else {
            const product = products.find(p => p.id === productId);
            setCart([...cart, { ...product, count: 1 }]);
        }
    };

    const handleIncrement = (productId) => {
        setCart(cart.map(item => 
            item.id === productId ? { ...item, count: item.count + 1 } : item
        ));
    };

    const handleDecrement = (productId) => {
        setCart(cart.map(item => 
            item.id === productId && item.count > 1 ? { ...item, count: item.count - 1 } : item
        ).filter(item => item.count > 0));
    };

    const handleRemove = (productId) => {
        setCart(cart.filter(item => item.id !== productId));
    };

    const filteredProducts = products.filter((product) =>
        product.name.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const sortedProducts = filteredProducts.sort((a, b) => {
        if (sortBy === 'name') {
            return a.name.localeCompare(b.name);
        } else if (sortBy === 'price') {
            return a.price - b.price;
        }
        return 0;
    });

    const handleOrder = () => {
      setCart([]);             
      setIsCartVisible(false);  
      alert(`Ваш заказ на сумму: ${totalPrice.toFixed(2)} BYN принят!`);
    }  

    const totalPrice = cart.reduce((acc, item) => acc + item.price * item.count, 0);

    return (
        <div>
            <div className="first">
                <img className="logo" src={logo} alt="logo" />
                <nav>
                    <ul>
                        <li><a href="#products">НОВИНКА</a></li>
                        <li><a href="#catalog">КАТАЛОГ</a></li>
                        <li><a href="#about">О НАС</a></li>
                        <li><a href="#contacts">КОНТАКТЫ</a></li>
                    </ul>
                </nav>
                <img className="cartIcon" src={bagIcon} alt="Корзина" onClick={() => setIsCartVisible(!isCartVisible)} />
            </div>
            <h1>КАТАЛОГ</h1>

            <input
                type="text"
                placeholder="Поиск товаров..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                style={{ padding: '10px', width: '300px' }}
            />

            <div className="sort">
                <label>
                    Сортировать по:
                    <select value={sortBy} onChange={(e) => setSortBy(e.target.value)} style={{ marginLeft: '10px' }}>
                        <option value="name">Название</option>
                        <option value="price">Цена</option>
                    </select>
                </label>
            </div>

            <div className="catalog">
                {sortedProducts.map((product) => (
                    <div key={product.id} className="product">
                        {product.isNew && <h3 className="new">NEW!</h3>}
                        <img src={product.image} alt={product.name} />
                        <h2>{product.name}</h2>
                        <p>{product.description}</p>
                        <p className="price">{product.price} BYN</p>
                        <div className="product-controls">
                            <button onClick={() => handleAddProduct(product.id)} className="add-button">
                                ДОБАВИТЬ
                            </button>
                        </div>
                    </div>
                ))}
            </div>

            {isCartVisible && (
                <div className='back'>
                  <div className="basket">
                      <h2>Корзина</h2>
                      <img className="close" src={close} onClick={() => setIsCartVisible(!isCartVisible)} />
                      {cart.length === 0 ? (
                          <p>Корзина пуста</p>
                      ) : (
                          <div className='cards'>
                            {cart.map(item => (
                              <div key={item.id} className="cart-item">
                                  <img className='basketImg' src={item.image} alt={item.name} />
                                  <div>
                                      <h4>{item.name}</h4>
                                      <p>{item.price} BYN</p>
                                      <div className="counter">
                                          <button className='minus' onClick={() => handleDecrement(item.id)}>-</button>
                                          <span>{item.count}</span>
                                          <button onClick={() => handleIncrement(item.id)}>+</button>
                                      </div>
                                  </div>
                                  <button onClick={() => handleRemove(item.id)} className="remove-button">Удалить</button>
                              </div>
                            ))}
                            <h3>Итого: {totalPrice.toFixed(2)} BYN</h3>
                            <button className="Order" onClick={handleOrder}>Заказать</button>
                          </div>
                      )}
                  </div>
                </div>
            )}

            <div className="fourth" id="contacts">
                <div className="logo">
                    <img src={pineapple} alt="pineapple" />
                </div>
                <div className="contact-info">
                    <h2>КОНТАКТЫ</h2>
                    <p>Darida Your Water VITAMIX
                        <br />Адрес: Линейная ул. 1А, агрогородок Ждановичи
                        <br />Телефон: +375 17 500-16-16
                        <br />Email: office@darida.by</p>
                </div>
            </div>
        </div>
    );
};

export default Catalog;