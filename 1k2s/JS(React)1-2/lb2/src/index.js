import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import Clock from'./ex1.js';
import JobMenu from "./ex2.js";


const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>

    <h1>Ex1:</h1>
    <Clock />
    <h1>Ex2:</h1>
    <JobMenu/>
  </React.StrictMode>
);

