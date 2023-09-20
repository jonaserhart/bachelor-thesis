import { Alert, Button, Space } from 'antd';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import {
  checkHealthEndpoint,
  selectHealth,
} from '../../features/health/healthSlice';
import { useCallback, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';

const ServerInfoPage: React.FC = () => {
  const health = useAppSelector(selectHealth);

  const [loading, setLoading] = useState(false);

  const dispatch = useAppDispatch();

  const nav = useNavigate();

  const tryAgain = useCallback(() => {
    setLoading(true);
    dispatch(checkHealthEndpoint()).finally(() => setLoading(false));
  }, [dispatch, setLoading]);

  const type = useMemo(() => {
    switch (health.code) {
      case 'Healthy':
        return 'success';
      case 'Degraded':
        return 'warning';
      case 'Unhealthy':
        return 'error';
    }
  }, [health.code]);

  return (
    <div
      style={{
        width: '100vw',
        height: '100vh',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
      }}>
      <Alert
        message={`Backend status: ${health.code}`}
        showIcon
        description={
          <div>
            {health.details.map((d) => (
              <p key={d.key}>{`${d.key}: ${d.value}`}</p>
            ))}
          </div>
        }
        action={
          <Space direction="vertical">
            <Button onClick={tryAgain} loading={loading} type="text">
              Request status again
            </Button>
            {health.code === 'Healthy' && (
              <Button
                onClick={() => {
                  nav('/');
                }}
                type="text">
                Go home
              </Button>
            )}
          </Space>
        }
        type={type}
      />
    </div>
  );
};

export default ServerInfoPage;
