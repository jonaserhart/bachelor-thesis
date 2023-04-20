import React from 'react';
import { RouterProvider, createBrowserRouter } from 'react-router-dom';
import Dashboard from './components/Dashboard';
import Analysis from './features/analysis/Analysis';
import Callback from './features/oauth/Callback';
import NavBar from './components/Navbar';
import Authorize from './features/oauth/Authorize';

function App() {


  const router = createBrowserRouter(
    [
      {
        element: <NavBar />,
        children: [
          {
            path: '/',
            element: <Dashboard />
          },
          {
            path: '/analyze',
            element: <Analysis />,
            // for l8er
            // children: [
            //   {
            //     path: ':modelId',
            //     element: <ModelDetails/>,
            //     children: [
            //       {
            //         path: ':sprintid',
            //         element: <SprintReview />
            //       }
            //     ]
            //   }
            // ]
          },
        ]
      },
      {
        path: '/oauth-authorize',
        element: <Authorize />
      },
      {
        path: '/oauth-callback',
        element: <Callback />,    
      },
    ]
  );

  return (
    <RouterProvider router={router} />
  );
}

export default App;
