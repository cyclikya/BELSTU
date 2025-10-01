import React from 'react';
import ReactDOM from 'react-dom/client';
// import {First, Fourth, Second, Third } from './main';
import Catalog from './catalog';
import reportWebVitals from './reportWebVitals';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    {/* <First />
    <Second />
    <Third />
    <Fourth /> */}

    <Catalog/>
  </React.StrictMode>
);


reportWebVitals();
