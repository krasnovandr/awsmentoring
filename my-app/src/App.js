import React from 'react';
import logo from './logo.svg';
import './App.css';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Hello AWS!!! I'm React App.Running on EC2 Linux. 
        </p>
        <p>
          Node 3
        </p>
      </header>
    </div>
  );
}

export default App;
