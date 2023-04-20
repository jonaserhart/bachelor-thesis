import axios from '../backendClient';
import * as React from 'react';
import { useAppSelector } from '../app/hooks';
import { selectAuthenticatedUser } from '../features/oauth/authSlice';

export default function Root() {

    const user = useAppSelector(selectAuthenticatedUser);

    return (
        <div>
            <h1>
                {`Hello ${user?.displayName ?? ''}`}
            </h1>
            <button onClick={() => axios.get('/workitems/health').then(console.log).catch(console.log)}>
                health
            </button>
        </div>
    )
}
