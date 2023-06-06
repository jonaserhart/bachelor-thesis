import { Outlet, RouterProvider, createBrowserRouter } from 'react-router-dom';
import Dashboard from './components/Dashboard';
import Analysis from './features/analysis/Analysis';
import Callback from './features/oauth/Callback';
import NavBar from './components/Navbar';
import Authorize from './features/oauth/Authorize';
import ModelDetail from './components/analysis/ModelDetail';
import QueryDetail from './components/analysis/queries/QueryDetail';
import ModelContextProvider from './context/ModelContext';
import QueryContextProvider from './context/QueryContext';
import KPIContextProvider from './context/KPIContext';
import KPIDetail from './components/analysis/kpis/KPIDetail';

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
              path: 'query/:queryId',
              element: (
                <QueryContextProvider>
                  <Outlet />
                </QueryContextProvider>
              ),
              children: [
                {
                  index: true,
                  element: <QueryDetail />,
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
