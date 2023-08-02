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
