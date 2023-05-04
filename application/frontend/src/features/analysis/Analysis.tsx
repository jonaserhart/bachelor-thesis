import * as React from 'react';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import { createModel, getMyModels, selectModels, updateModelDetails } from './analysisSlice';
import { Button, Popover, Space, Spin, Typography, message } from 'antd';
import CustomTable from '../../components/table/EditableTable';
import { AnalysisModel, Project, Query } from './types';
import ProjectPopover from '../../components/ProjectPopover';
import { useNavigate } from 'react-router-dom';

const { Title } = Typography;

export default function Analysis() {

    const dispatch = useAppDispatch();

    const [loading, setLoading] = React.useState(false);

    const nav = useNavigate();

    const models = useAppSelector(selectModels);

    const [createPopoverOpen, setCreatePopoverOpen] = React.useState(false);
  
    const handleCreatePopoverOpenChange = React.useCallback((newOpen: boolean) => {
      setCreatePopoverOpen(newOpen);
    }, []);

    React.useEffect(() => {
        setLoading(true);
        dispatch(getMyModels())
            .unwrap()
            .catch(console.error)
            .finally(() => setLoading(false));
    }, [dispatch]);

    if (loading) {
        return (
            <div style={{
                width: '100%',
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center'
            }}>
                <Spin tip="Loading models" size="default" />
            </div>
        )
    }

    return (
        <div>
            <Title level={4} style={{ marginTop: 0 }}>Analysis</Title>
            <div style={{
                width: '100%',
                display: 'flex',
                flexDirection: 'row-reverse'
            }}>
                <Popover
                    content={
                        <ProjectPopover onSubmit={(project) => {
                            if (project) {
                                dispatch(createModel({
                                    name: 'new model',
                                    project
                                }))
                                .unwrap()
                                .then((v) => message.success(`Successfully created model ${v.name}`))
                                .catch((err) => message.error(err.error))
                                .finally(() => setCreatePopoverOpen(false));
                            } else {
                                message.error("No project selected!");
                            }
                        }}/>
                    }
                    title="Create analysis model"
                    trigger="click"
                    open={createPopoverOpen}
                    onOpenChange={handleCreatePopoverOpenChange}
                >
                    <Button ghost onClick={() => setCreatePopoverOpen(true)} type="primary" style={{ marginBottom: 16 }}>
                        Add a new model
                    </Button>
                </Popover>
            </div>
            <CustomTable
                dataSource={models}
                defaultColumns={[
                    {
                        key: 'name',
                        dataIndex: 'name',
                        editable: true,
                        title: 'Name'
                    },
                    {
                        key: 'proj',
                        dataIndex: 'project',
                        title: 'Project',
                        render(value) {
                            var proj = value as Project;
                            return proj?.name ?? 'No project' 
                        }
                    },
                    {
                        key: 'noQueries',
                        title: '# Queries',
                        dataIndex: 'queries',
                        render(value) {
                            const queries = value as Query[];
                            if (queries.length <= 0) {
                                return (
                                    <div style={{ color: 'gray' }}>
                                        No queries yet
                                        <Button type='link'>Create one 🚀</Button>
                                    </div>
                                );
                            }
                            return queries.length;
                        },
                        sorter: {
                            compare: (a, b) => (a as AnalysisModel).queries.length - (b as AnalysisModel).queries.length
                        }
                    },
                    {
                        fixed: 'right',
                        width: 100,
                        title: 'Action',
                        dataIndex: 'actions',
                        key: 'action',
                        render: (_, record) => (
                          <Space size="middle">
                            <Button type='text' onClick={() => nav(`/analyze/${(record as AnalysisModel).id}`)}>Details</Button>
                            <Button danger type='text'>Delete </Button>
                          </Space>
                        ),
                    },
                ]}
                handleSave={async(newRecord) => {
                    await dispatch(updateModelDetails({
                        id: newRecord.id,
                        name: newRecord.name
                    }));
                }}
            />
        </div>
    )
}
