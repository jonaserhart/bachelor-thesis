import axios from '../backendClient';
import * as React from 'react';

export default function Root() {

    const [t, setT] = React.useState("");

    return (
        <div>
            <h1>
                root
            </h1>
            <input value={t} onChange={(ev) => setT(ev.target.value)} type='text'/>
            <button onClick={() => axios.get('/workitems/health', {
                headers: {
                    "Authorization": `Bearer ${t}` 
                }
            }).then(console.log).catch(console.log)}>
                health
            </button>
        </div>
    )
}
