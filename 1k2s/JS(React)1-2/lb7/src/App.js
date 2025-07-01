import React, { useState } from 'react';
import './App.css';

const avatarOptions = [
  'https://avatars.mds.yandex.net/i?id=b4d8eea8e5389983f36378bf698fbe5295d0a1d1-10465625-images-thumbs&n=13',
  'https://avatars.mds.yandex.net/i?id=75b661b9658fcdc4db5f7feae8762e38bea1d80d-5240525-images-thumbs&n=13',
  'https://i.pinimg.com/originals/7d/e5/8c/7de58cd88b4c623a2a2619fcfa15c281.png'
];

const Comments = () => {
  const [comments, setComments] = useState([]); // массив комментариев
  const [newComment, setNewComment] = useState({
    username: '',
    email: '',
    message: '',
    avatar: '',
    secretWord: ''
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setNewComment(prevState => ({ // получаем доступ к предыдущему состоянию
      ...prevState,
      [name]: value
    }));
  };

  const handleAvatarChange = (e) => {
    const avatarUrl = e.target.value;
    setNewComment(prevState => ({
      ...prevState,
      avatar: avatarUrl //заменяется значение аватарки на выбранную
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    const updatedComments = [...comments, { ...newComment, date: new Date().toLocaleString() }];
    setComments(updatedComments);
    setNewComment({
      username: '',
      email: '',
      message: '',
      avatar: '',
      secretWord: ''
    });
  };

  const handleEdit = (index, newMessage) => {
    const updatedComments = [...comments];
    updatedComments[index].message = newMessage;
    setComments(updatedComments);
  };

  const handleDelete = (index, secretWord) => {
    if (comments[index].secretWord === secretWord) {
      const updatedComments = comments.filter((_, i) => i !== index);
      setComments(updatedComments);
    } else {
      alert('Secret word does not match!');
    }
  };

  const handleInfo = (comment) => {
    alert(`Username: ${comment.username}\nEmail: ${comment.email}\nLast edited: ${comment.date}`);
};

  return (
    <div>
      <h2>Comments</h2>
      <form onSubmit={handleSubmit}>
       <div>
          <input type="text" name="username" placeholder="Имя пользователя" value={newComment.username} onChange={handleChange} required />
       </div>
        <div>
          <input type="email" name="email" placeholder="Email" value={newComment.email} onChange={handleChange} required
          />
        </div>
        <div>
          {avatarOptions.map((avatarUrl, index) => (
            <label key={index}>
              <input type="radio" name="avatar" value={avatarUrl} checked={newComment.avatar === avatarUrl} onChange={handleAvatarChange}
              />
              <img src={avatarUrl} alt="Avatar" style={{ width: '90px', cursor: 'pointer', marginRight: '10px' }}
              />
            </label>
          ))}
        </div>
        <div>
          <textarea
            name="message"
            placeholder="Сообщение"
            value={newComment.message}
            onChange={handleChange}
            required
          />
        </div>
        <div>
          <input type="password" name="secretWord" placeholder="Секретное слово" value={newComment.secretWord} onChange={handleChange} required
          />
        </div>
        <button type="submit">Submit</button>
      </form>
      <div>
        {comments.map((comment, index) => (
          <div key={index} style={{ border: '4px darkred solid', padding: '10px', margin: '10px', width: '500px' }}>
            <img src={comment.avatar} alt="Avatar" style={{ width: '90px' }} />
            <p>{comment.username}: {comment.message}</p>
            <button onClick={() => handleEdit(index, prompt('Введите новое сообщение'))}>Редактировать</button>
            <button onClick={() => handleDelete(index, prompt('Введите секретное слово'))}>Удалить</button>
            <button onClick={() => handleInfo(comment)}>Info</button>
            <p>Email: {comment.email}</p>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Comments;