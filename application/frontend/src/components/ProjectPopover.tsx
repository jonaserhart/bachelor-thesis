import { Button, Select, Space, message } from 'antd';
import * as React from 'react';
import { Project } from '../features/analysis/types';
import axios from '../backendClient';

type Props = {
    onSubmit: (val: Project | undefined) => void
}

export default function ProjectPopover(props: Props) {

    const { onSubmit } = props;

    const [projects, setProjects] = React.useState<Project[]>([]);
    const [selected, setSelected] = React.useState<string>('');
    const [loading, setLoading] = React.useState(false);
    
    React.useEffect(() => {
        setLoading(true);
        axios.get<Project[]>('/devops/projects')
            .then((response) => {
                setProjects(response.data);
            })
            .catch((err) => message.error(err.error))
            .finally(() => setLoading(false));
    }, []);

    const selectedProject = React.useMemo(() => {
        return projects.find(x => x.id === selected);
    }, [projects, selected]);

    return (
        <Space direction="vertical">
            <Select
                defaultValue="Select a project"
                style={{ width: 200 }}
                loading={loading}
                onSelect={(id) => setSelected(id)}
                showSearch
                filterOption={(input, option) =>
                    (option?.label ?? '').toLowerCase().includes(input.toLowerCase())
                }
                options={
                    projects.map(p => ({
                        label: p.name,
                        value: p.id,
                    }))
                }
                
                />
            
            <div style={{
                width: '100%',
                display: 'flex',
                flexDirection: 'row-reverse'
            }}>
                <Button disabled={selected === ''} type='primary' ghost onClick={() => onSubmit(selectedProject)}>
                    Create
                </Button>
            </div>
        </Space>
    )
}