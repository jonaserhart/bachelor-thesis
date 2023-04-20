import { DashboardOutlined, DotChartOutlined, LogoutOutlined, UserOutlined } from "@ant-design/icons";
import { Breadcrumb, Button, Layout, Menu, Space, theme } from "antd";
import { Content, Footer, Header } from "antd/es/layout/layout";
import * as React from "react";
import { Outlet, useLocation, useNavigate } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../app/hooks";
import { getMe, selectAuthenticatedUser } from "../features/oauth/authSlice";

export default function AppLayout() {
  const {
    token: { colorBgContainer },
  } = theme.useToken();

  const nav = useNavigate();
  const location = useLocation();

  const dispatch = useAppDispatch();

  const me = useAppSelector(selectAuthenticatedUser);

  React.useEffect(() => {
    if (!me) {
      dispatch(getMe())
        .unwrap()
        .then((user) => console.log(user.id))
        .catch((err) => console.error(err.message));
    }
  }, [me, nav, dispatch]);

  const selectedKey = React.useMemo(() => {
    switch(location.pathname) {
      case '/': return 'root';
      case '/analyze': return 'analyze';
      default: return 'unknown';
    }
  }, [location]); 

  return (
    <Layout>
      <Header style={{ padding: 0 }}>
        <div style={{
          display: 'flex',
          background: 'white',
          alignItems: 'center',
          justifyContent: 'space-between',
          paddingInline: 20
        }}>
          <Menu
            theme="light"
            mode="horizontal"
            defaultSelectedKeys={[selectedKey]}
            selectedKeys={[
              selectedKey,
            ]}
            items={[
              {
                key: 'root',
                label: 'Dashboard',
                icon: <DashboardOutlined />,
                onClick() {
                  nav('/')
                },
              },
              {
                key: 'analyze',
                label: 'Analysis',
                icon: <DotChartOutlined />,
                onClick() {
                  nav('/analyze')
                }
              },
            ]}
          />
          <Space>
            <Button type="primary" ghost icon={<UserOutlined />} size="large">
              Hello, {me?.displayName}
            </Button>
            <Button danger icon={<LogoutOutlined />} size="large">
              Log out
            </Button>
          </Space>
        </div>
      </Header>
      <Content className="site-layout" style={{ padding: "0 50px" }}>
        <div
          style={{ margin: "16px 0", padding: 24, minHeight: 380, background: colorBgContainer }}
        >
          <Outlet />
        </div>
      </Content>
      <Footer style={{ textAlign: "center" }}>Scrum analysis tool</Footer>
    </Layout>
  );
}
