import React from 'react';
import { Outlet, RouterProvider, createBrowserRouter } from 'react-router-dom';
import Root from './components/Root';
import Analysis from './analysis/Analysis';
import Callback from './oauth/Callback';
import NavBar from './components/Navbar';
import Authorize from './oauth/Authorize';

const HeaderLayout = () => (
  <>
    <header>
      <NavBar />
    </header>
    <Outlet />
  </>
);

function App() {


  const router = createBrowserRouter(
    [
      {
        element: <HeaderLayout />,
        children: [
          {
            path: '/',
            element: <Root />
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
          {
            path: 'oauth-authorize',
            element: <Authorize />
          },
          {
            path: 'oauth-callback',
            element: <Callback />,    
          },
        ]
      }
    ]
  );

  return (
    <RouterProvider router={router} />
  );
}

export default App;
