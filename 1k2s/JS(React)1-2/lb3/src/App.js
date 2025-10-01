import {useState} from 'react';
import Catalog from './Catalog';
export default function App() {
    const [email, setEmail] = useState('');
    const [message, setMessage] = useState(null);

    const products = [
        { id: 1, name: 'Футболка', price: 25, quantity: 10 },
        { id: 2, name: 'Джинсы', price: 50, quantity: 5 },
        { id: 3, name: 'Кроссовки', price: 80, quantity: 3 },
        { id: 4, name: 'Ремни', price: 35, quantity: 15 },
        { id: 5, name: 'Байки', price: 65, quantity: 2 },
    ];
    function isValidEmail(email) {
        return /\S+@\S+\.\S+/.test(email);
    }

    const handleChange = event => {
        setEmail(event.target.value);
    };

    const handleSubmit = event => {
        event.preventDefault();

        setMessage(null);

        if (isValidEmail(email)) {
            setMessage('Почта валидна письмо отправлено');
        } else {
            setMessage('почта не валидна');
        }
    };

    return (
        <div>
            <form onSubmit={handleSubmit}>
                <input
                    id="message"
                    name="message"
                    value={email}
                    onChange={handleChange}
                />
                <hr/>
                <button type="submit">Submit</button>
                {message && <h2>{message}</h2>}
                <hr/>
            </form>
            <Catalog products={products}/>
        </div>
    );
}