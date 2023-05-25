import React from 'react';
import { createRoot } from 'react-dom/client';
import { Provider } from 'react-redux';
import { store } from './app/store';
import App from './App';
import reportWebVitals from './reportWebVitals';
import { ConfigProvider, theme } from 'antd';
import './styles/globals.css';
import { getLogger } from './util/logger';

const container = document.getElementById('root')!;
const root = createRoot(container);

const logger = getLogger('app');

logger.logDebug(process.env);

root.render(
  <React.StrictMode>
    <ConfigProvider
      theme={{
        algorithm: theme.darkAlgorithm,
        token: {
          colorPrimary: '#348843',
        },
      }}>
      <Provider store={store}>
        <App />
      </Provider>
    </ConfigProvider>
  </React.StrictMode>
);

reportWebVitals();
