import React from 'react';

function App() {

  React.useEffect(() => {
    fetch('/api/WeatherForecast')
    .then(x => x.json())
    .then(console.log)
    .catch(console.error)
  }, []);

  return (
    <div className="App">
      Sample application
    </div>
  );
}

export default App;
