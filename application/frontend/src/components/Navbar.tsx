import { DashboardOutlined, DotChartOutlined } from "@ant-design/icons";
import { Breadcrumb, Layout, Menu, theme } from "antd";
import { Content, Footer, Header } from "antd/es/layout/layout";
import * as React from "react";
import { Outlet, useLocation, useNavigate } from "react-router-dom";

export default function AppLayout() {
  const {
    token: { colorBgContainer },
  } = theme.useToken();

  const nav = useNavigate();
  const location = useLocation();

  const selectedKey = React.useMemo(() => {
    switch(location.pathname) {
      case '/': return 'root';
      case '/analyze': return 'analyse';
      default: return 'unknown';
    }
  }, [location]); 

  return (
    <Layout>
      <Header style={{ padding: 0 }}>
        <Menu
          theme="light"
          mode="horizontal"
          defaultSelectedKeys={[selectedKey]}
          items={[
            {
              key: 'root',
              label: 'Dashboard',
              icon: <DashboardOutlined />,
              onClick() {
                nav('/')
              }
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
      </Header>
      <Content className="site-layout" style={{ padding: "0 50px" }}>
        <Breadcrumb style={{ margin: "16px 0" }}>
          <Breadcrumb.Item>Home</Breadcrumb.Item>
          <Breadcrumb.Item>List</Breadcrumb.Item>
          <Breadcrumb.Item>App</Breadcrumb.Item>
        </Breadcrumb>
        <div
          style={{ padding: 24, minHeight: 380, background: colorBgContainer }}
        >
          <Outlet />
        </div>
      </Content>
      <Footer style={{ textAlign: "center" }}>Scrum analysis tool</Footer>
    </Layout>
  );
}
