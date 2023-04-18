import axios from '../backendClient';
import * as React from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';

export default function Callback() {

    const [searchParams,] = useSearchParams();
    const [sent, setSent] = React.useState(false);
    const nav = useNavigate();

    React.useEffect(() => {      
        const code = searchParams.get('code');
        const state = searchParams.get('state');
        if (code && state && !sent) {
            setSent(true);
            axios.post('/oauth/callback', {
                code,
                state
            })
            .then((r) => {
                console.log('response:', r);
                nav('/');
            })
            .catch(console.error)
        }
    }, [searchParams, sent, setSent, nav]);

    return (
        <div>
            Oauth-Callback
        </div>
    );
}
