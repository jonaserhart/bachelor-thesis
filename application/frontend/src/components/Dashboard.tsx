import { Button, Empty, Spin, Typography } from 'antd';
import { useSelector } from 'react-redux';
import { selectAuthenticatedUser } from '../features/auth/authSlice';
import { useNavigate } from 'react-router-dom';

const { Title } = Typography;

const Dashboard: React.FC = () => {
  const user = useSelector(selectAuthenticatedUser);

  const nav = useNavigate();

  if (!user) {
    return (
      <div
        style={{
          position: 'fixed',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          backgroundColor: 'rgba(0, 0, 0, 0.3)',
          zIndex: 1000,
        }}>
        <Spin size="large" tip="Logging you in..." />
      </div>
    );
  }

  return (
    <div>
      <Empty
        description={
          <div>
            <p>The dashboard has no content yet</p>
            <Button onClick={() => nav(`/analyze`)}>
              Go to your analysis models
            </Button>
          </div>
        }
      />
    </div>
  );
};

export default Dashboard;
