import React, { useState } from "react";
import './App.css';

const ToDoList = () => {
  const [tasks, setTasks] = useState([]);
  const [showCompletedTasks, setShowCompletedTasks] = useState(false); // Добавлено состояние для управления отображением выполненных задач
  const [showUnCompletedTasks, setShowUnCompletedTasks] = useState(false); // Добавлено состояние для управления отображением выполненных задач

  const handleAdd = event => {
    event.preventDefault(); // Предотвращаем стандартное поведение отправки формы
    const newTask = document.getElementById('newTask').value;
    setTasks([...tasks, { text: newTask, isChecked: false }]); // Задача теперь объект с текстом и состоянием isChecked
    document.getElementById('newTask').value = '';
  };

  const showChecked = event => {
    event.preventDefault();
    setShowCompletedTasks(true);
  };

  const showUnChecked = event => {
    event.preventDefault();
    setShowUnCompletedTasks(true); 
  };

  const handleCheck = (index) => {
    const newTasks = [...tasks];
    newTasks[index].isChecked = !newTasks[index].isChecked;
    setTasks(newTasks);
  }

  return (
    <div>
      <ToDoForm handleAdd={handleAdd} showUnChecked={showUnChecked} showChecked={showChecked}  />
      {tasks.map((item, index) => ( 
        <div key={index}>
          {/* Передаем текст задачи, ее состояние isChecked и обработчик handleCheck */}
          <ToDoItems task={item.text} isChecked={item.isChecked} handleCheck={() => handleCheck(index)} />
        </div>
      ))}
      {showCompletedTasks && (
        <div>
          <h1>Выполненные задачи:</h1>
          <ul>
            {tasks.map((item, index) => (
              item.isChecked && <li key={index}>{item.text}</li>
            ))}
          </ul>
        </div>
      )}
      {showUnCompletedTasks && (
        <div>
          <h1>Невыполненные задачи:</h1>
          <ul>
            {tasks.map((item, index) => (
              !item.isChecked && <li key={index}>{item.text}</li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
};

const ToDoForm = ({showUnChecked, showChecked, handleAdd}) => {
  return (
    <form>
      <h1>Добавить задачу: </h1>
      <input id="newTask" type="text" />
      <button id="add" onClick={handleAdd}>Добавить</button>
      <button onClick={showChecked}>Отправить</button>
      <button onClick={showUnChecked}>Предстоит выполнить</button>
    </form>
  );
};

const ToDoItems = ({ task, isChecked, handleCheck }) => {
  return (
    <div>
      <input type="checkbox" checked={isChecked} onChange={handleCheck}/>
      <label style={{ textDecoration: isChecked ? 'line-through' : 'none' }}>
        {task}
      </label>
    </div>
  );
};

export default ToDoList;
