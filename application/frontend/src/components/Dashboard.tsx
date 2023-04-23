import axios from '../backendClient';
import * as React from 'react';

export default function Dashboard() {

    React.useEffect(() => {
        axios.get<boolean>('/devops/health').then((r) => console.log(`Backend healthy: ${r.data}`)).catch(console.log)
    }, [])

    return (
        <div>
            Hello
        </div>
    )
}
