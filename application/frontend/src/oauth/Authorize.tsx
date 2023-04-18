import axios from '../backendClient';
import * as React from 'react';

export default function Authorize() {

    const onAuthorize = React.useCallback(() => {
        axios.get<string>('/oauth/authorize')
        .then((s) => {
            window.location.href = s.data;
        });
    }, []);

    return (
        <button onClick={onAuthorize}>
            Authorize application
        </button>
    )
}
