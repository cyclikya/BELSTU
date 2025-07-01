import React, { useState } from 'react';
import './App.css';

function ContactForm() {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    message: '',
    gender: ''
  });

  const [formSubmitted, setFormSubmitted] = useState(false);

  const [list, setList] = useState({
    name: '',
    email: '',
    message: '',
    gender: ''
  });

  return (
    <div>
      <Form className='form' formData={formData} setFormData={setFormData} setFormSubmitted={setFormSubmitted} setList={setList} />
      {formSubmitted && <Table formData={formData} list={list} />}
    </div>
  );
}

const Form = ({ formData, setFormData, setFormSubmitted, setList }) => {

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    if (!formData.name || !formData.email || !formData.message || !formData.gender || formData.name === ' '|| formData.email === ' '|| formData.message === ' ' || formData.gender === ' ') {
      alert('Заполните все поля для отправки формы');
      return;
    }
    if (!formData.email.includes('@gmail.com')) {
      alert('Введите корректный адрес электронной почты');
      return;
    }

    const newList = {
      name: formData.name,
      email: formData.email,
      message: formData.message,
      gender: formData.gender
    };

    setList(newList);

    setFormSubmitted(true);
      
    setFormData({
      name: '',
      email: '',
      message: '',
      gender: ''
    });
  };

  return (
    <form onSubmit={handleSubmit}>
      <table>
        <tbody>
          <tr>
            <td>
              <label>Введите имя:</label>
            </td>
            <td>
              <input type="text" name="name" placeholder="Введите имя" value={formData.name} onChange={handleChange} />
            </td>
          </tr>
          <tr>
            <td>
              <label>Введите электронную почту:</label>
            </td>
            <td>
              <input type="text" name="email" placeholder="Введите электронную почту" value={formData.email} onChange={handleChange} />
            </td>
          </tr>
          <tr>
            <td>
              <label>Введите сообщение:</label>
            </td>
            <td>
              <input type="text" name="message" placeholder="Введите сообщение" value={formData.message} onChange={handleChange} />
            </td>
          </tr>
          <tr>
            <td>
              <label>Укажите пол:</label>
            </td>
            <td>
              <input type="radio" name="gender" value="Мужской" checked={formData.gender === 'Мужской'} onChange={handleChange} />
              <label>Муж</label>
              <input type="radio" name="gender" value="Женский" checked={formData.gender === 'Женский'} onChange={handleChange} />
              <label>Жен</label>
            </td>
          </tr>
        </tbody>
      </table>
      <button type="submit">Отправить</button>
    </form>
  );
};

const Table = ({ list }) =>{
  return(
    <table className='result'>
      <tr>
        <td><p>Имя контакта</p></td>
        <td>{list.name}</td>
      </tr>
      <tr>
        <td><p>Электронная почта</p></td>
        <td>{list.email}</td>
      </tr>
      <tr>
        <td><p>Сообщение</p></td>
        <td>{list.message}</td>
      </tr>
      <tr>
        <td><p>Пол</p></td>
        <td>{list.gender}</td>
      </tr>
    </table>
  )
}

export default ContactForm;

