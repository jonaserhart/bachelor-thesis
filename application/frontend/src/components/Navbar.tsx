import {
  DashboardOutlined,
  DotChartOutlined,
  UserOutlined,
} from '@ant-design/icons';
import { Breadcrumb, Button, Layout, Menu, Space, message, theme } from 'antd';
import { Content, Footer, Header } from 'antd/es/layout/layout';
import { Outlet, useLocation, useNavigate, useParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../app/hooks';
import { getMe, selectAuthenticatedUser } from '../features/auth/authSlice';
import { useCallback, useEffect, useMemo } from 'react';
import { getLogger } from '../util/logger';
import handleError from '../util/handleError';
import axios from '../backendClient';
import {
  State as BackendStatusInfo,
  checkHealthEndpoint,
} from '../features/health/healthSlice';
import { useSelector } from 'react-redux';
import { selectModels } from '../features/analysis/analysisSlice';
import {
  AnalysisModel,
  GraphicalConfiguration,
  KPI,
  Report,
} from '../features/analysis/types';
import { getAllKPIs } from '../util/kpiFolderUtils';

const logger = getLogger('NavBar');

const NavBar: React.FC = () => {
  const {
    token: { colorBgContainer },
  } = theme.useToken();

  const nav = useNavigate();
  const location = useLocation();

  const { modelId, reportId, kpiId, configId } = useParams();

  const models = useSelector(selectModels);

  const currentModel = useMemo(
    () => models.find((model) => model.id === modelId),
    [models, modelId]
  );

  const breadCrumbs = useMemo(() => {
    const entries = [
      {
        title: 'SCRUM tool',
        path: `/`,
      },
    ];

    if (!currentModel) {
      return entries;
    }

    if (location.pathname.includes('/analyze')) {
      entries.push({
        title: 'Analysis',
        path: `/analyze`,
      });
    }

    entries.push({
      title: currentModel.name,
      path: `/analyze/${currentModel.id}`,
    });

    if (location.pathname.includes('/report/')) {
      entries.push({
        title: 'Reports',
        path: `/analyze/${currentModel.id}?tab=l8estreports`,
      });
      const report = currentModel?.reports?.find(
        (report) => report.id === reportId
      );
      if (report) {
        entries.push({
          title: report.title,
          path: `/analyze/${currentModel.id}/report/${report.id}`,
        });
      }
    } else if (location.pathname.includes('/kpi/')) {
      entries.push({
        title: 'KPIs',
        path: `/analyze/${currentModel.id}?tab=kpis`,
      });
      const kpi = getAllKPIs(currentModel).find((kpi) => kpi.id === kpiId);
      if (kpi) {
        entries.push({
          title: kpi.name,
          path: `/analyze/${currentModel.id}/kpi/${kpi.id}`,
        });
      }
    } else if (location.pathname.includes('/graphicalConfig/')) {
      entries.push({
        title: 'Configuration',
        path: `/analyze/${currentModel.id}?tab=settings`,
      });

      const config = currentModel?.graphical?.find(
        (config) => config.id === configId
      );
      if (config) {
        entries.push({
          title: config.name,
          path: `/analyze/${currentModel.id}/graphicalConfig/${config.id}`,
        });
      }
    }

    return entries;
  }, [location, currentModel]);

  const dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(checkHealthEndpoint())
      .unwrap()
      .then((result) => {
        if (result.status.code !== 'Healthy') {
          nav('/server-info');
        }
      })
      .catch(() => nav('/server-info'));
  }, [dispatch]);

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
            <Button type="text" icon={<UserOutlined />}>
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
          separator={
            <div
              style={{
                height: '100%',
                display: 'flex',
                alignItems: 'center',
              }}>
              /
            </div>
          }
          items={[
            ...breadCrumbs.map((x) => ({
              title: (
                <Button
                  style={{
                    margin: 0,
                  }}
                  type="text">
                  {x.title}
                </Button>
              ),
              onClick() {
                nav(x.path);
              },
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
