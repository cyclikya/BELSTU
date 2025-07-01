import { useEffect, useState } from 'react';
import './App.css';

function App() {
  const [color, setColor] = useState('');
  const [selectedColor, setSelectedColor] = useState(false);
  
  const [savedColors, setSavedColors] = useState( () => {
    const x = JSON.parse(localStorage.getItem('saved'));
    return x || [];
  });

  const handleSelect = (selectedColor) => {
    setColor(selectedColor);
    setSelectedColor(true);
  };

  const resetSelect = () => {
    setColor('');
    setSelectedColor(false);
  };

  const saveSelect = () => {
    if (selectedColor) {
      setSavedColors(Colors => [...Colors, color]);
    }
  };
// -------------------------------------------------------- Функциональный компонент
  const Block = ({ color }) => {
    return (
      <div className='block' style={{ backgroundColor: color }} onClick={() => handleSelect(color)} />
    );
  };
// -------------------------------------------------------- Хук EseEffect
  useEffect(() => { // запись
    localStorage.setItem('saved', JSON.stringify(savedColors));
  }, [savedColors]);

  return (
    <div>
      <div className='palitra'>
        <h2>Палитра</h2>
        <Block color={'#FEDC7B'} />
        <Block color={'#F49069'} />
        <Block color={'#DF6149'} />
        <Block color={'#708240'} />
        <button onClick={resetSelect}>Сбросить</button>
        <button onClick={saveSelect}>Сохранить</button>
      </div>
      <div>
        <h2 style={{ color: selectedColor ? color : 'black' }}>Выбранный цвет</h2>
      </div>
      <div className='saved'>
        <h3>Сохраненные цвета:</h3>
        {savedColors.slice(-3).map((savedColor, index) => (
          <div key={index} style={{ backgroundColor: savedColor}}></div>
        ))}
      </div>
    </div>
  );
}

export default App;
