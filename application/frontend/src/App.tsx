import { Outlet, RouterProvider, createBrowserRouter } from 'react-router-dom';
import Dashboard from './components/Dashboard';
import Analysis from './features/analysis/Analysis';
import Callback from './features/oauth/Callback';
import NavBar from './components/Navbar';
import Authorize from './features/oauth/Authorize';
import ModelDetail from './components/analysis/ModelDetail';
import ModelContextProvider from './context/ModelContext';
import QueryContextProvider from './context/QueryContext';
import KPIContextProvider from './context/KPIContext';
import KPIDetail from './components/analysis/kpis/KPIDetail';
import CreateReport from './components/analysis/reports/CreateReport';
import { ReportContextProvider } from './context/ReportContext';
import ReportDetail from './components/analysis/reports/ReportDetail';
import {
  GraphicalConfigContext,
  GraphicalConfigContextProvider,
} from './context/GraphicalConfigContext';
import GraphicalConfigDetail from './components/analysis/modelSettings/graphical/GraphicalConfigDetail';

const App = () => {
  const router = createBrowserRouter([
    {
      element: <NavBar />,
      children: [
        {
          path: '/',
          element: <Dashboard />,
        },
        {
          path: 'analyze',
          element: <Analysis />,
        },
        {
          element: (
            <ModelContextProvider>
              <Outlet />
            </ModelContextProvider>
          ),
          path: 'analyze/:modelId',
          children: [
            {
              index: true,
              element: <ModelDetail />,
            },
            {
              path: 'report/create',
              element: <CreateReport />,
            },
            {
              path: 'report/:reportId',
              element: (
                <ReportContextProvider>
                  <Outlet />
                </ReportContextProvider>
              ),
              children: [
                {
                  index: true,
                  element: <ReportDetail />,
                },
              ],
            },
            {
              path: 'kpi/:kpiId',
              element: (
                <KPIContextProvider>
                  <Outlet />
                </KPIContextProvider>
              ),
              children: [
                {
                  index: true,
                  element: <KPIDetail />,
                },
              ],
            },
            {
              path: 'graphicalConfig/:configId',
              element: (
                <GraphicalConfigContextProvider>
                  <Outlet />
                </GraphicalConfigContextProvider>
              ),
              children: [
                {
                  index: true,
                  element: <GraphicalConfigDetail />,
                },
              ],
            },
          ],
        },
      ],
    },
    {
      path: '/oauth-authorize',
      element: <Authorize />,
    },
    {
      path: '/oauth-callback',
      element: <Callback />,
    },
  ]);

  return <RouterProvider router={router} />;
};

export default App;
