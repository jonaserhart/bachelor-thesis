import {
  DashboardOutlined,
  DotChartOutlined,
  UserOutlined,
} from '@ant-design/icons';
import { Breadcrumb, Button, Layout, Menu, Space, message, theme } from 'antd';
import { Content, Footer, Header } from 'antd/es/layout/layout';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../app/hooks';
import { getMe, selectAuthenticatedUser } from '../features/auth/authSlice';
import { useCallback, useEffect, useMemo } from 'react';
import { getLogger } from '../util/logger';
import handleError from '../util/handleError';

const logger = getLogger('NavBar');

const NavBar: React.FC = () => {
  const {
    token: { colorBgContainer },
  } = theme.useToken();

  const nav = useNavigate();
  const location = useLocation();

  const dispatch = useAppDispatch();

  const me = useAppSelector(selectAuthenticatedUser);

  useEffect(() => {
    if (!me) {
      dispatch(getMe())
        .unwrap()
        .then((user) => logger.logDebug(user.id))
        .catch((err) => handleError(err));
    }
  }, [me, nav, dispatch]);

  const selectedKey = useMemo(() => {
    switch (true) {
      case location.pathname === '/':
        return 'root';
      case location.pathname.includes('/analyze'):
        return 'analyze';
      default:
        return 'unknown';
    }
  }, [location]);

  const getTokenName = useCallback((tn: string) => {
    switch (tn) {
      case 'analyze':
        return 'Analysis';
      default:
        return tn;
    }
  }, []);

  const pathTokens = useMemo(() => {
    const tokens = location.pathname.split('/');
    if (tokens.length > 1) {
      return tokens.filter((x) => x.trim().length > 0).map(getTokenName);
    }
    return [];
  }, [getTokenName, location.pathname]);

  return (
    <Layout>
      <Header style={{ padding: 0 }}>
        <div
          style={{
            display: 'flex',
            background: '#141414',
            alignItems: 'center',
            justifyContent: 'space-between',
            paddingInline: 20,
          }}>
          <Menu
            theme="light"
            mode="horizontal"
            defaultSelectedKeys={[selectedKey]}
            selectedKeys={[selectedKey]}
            items={[
              {
                key: 'root',
                label: 'Dashboard',
                icon: <DashboardOutlined />,
                onClick() {
                  nav('/');
                },
              },
              {
                key: 'analyze',
                label: 'Analysis',
                icon: <DotChartOutlined />,
                onClick() {
                  nav('/analyze');
                },
              },
            ]}
          />
          <Space>
            <Button type="primary" ghost icon={<UserOutlined />}>
              Hello, {me?.displayName}
            </Button>
          </Space>
        </div>
      </Header>
      <Content className="site-layout" style={{ padding: '0 50px' }}>
        <Breadcrumb
          style={{
            marginTop: 10,
          }}
          items={[
            {
              title: 'Scrum tool',
            },
            ...pathTokens.map((x) => ({
              title: x,
            })),
          ]}></Breadcrumb>
        <div
          style={{
            margin: '16px 0',
            padding: 30,
            minHeight: 380,
            background: colorBgContainer,
          }}>
          <Outlet />
        </div>
      </Content>
      <Footer style={{ textAlign: 'center' }}>Scrum analysis tool</Footer>
    </Layout>
  );
};

export default NavBar;
