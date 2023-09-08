import { useCallback, useContext, useMemo, useState } from 'react';
import { ModelContext } from '../../../context/ModelContext';
import {
  Button,
  Space,
  message,
  Tag,
  Typography,
  Tree,
  theme,
  Popconfirm,
} from 'antd';
import { Expression, KPI, KPIFolder } from '../../../features/analysis/types';
import CustomTable from '../../table/CustomTable';
import {
  BackendError,
  useAppDispatch,
  useAppSelector,
} from '../../../app/hooks';
import {
  createKPIFolder,
  createNewKPI,
  deleteKPI,
  deleteKPIFolder,
  selectAllKPIs,
  updateKPIFolder,
} from '../../../features/analysis/analysisSlice';
import { useNavigate } from 'react-router-dom';
import { BulbOutlined, EditOutlined } from '@ant-design/icons';
import {
  getAllKPIsInFolder,
  mapToFolderStructure,
} from '../../../util/kpiFolderUtils';

const { Title } = Typography;

const KPIs: React.FC = () => {
  const { model } = useContext(ModelContext);

  const {
    token: { colorPrimary },
  } = theme.useToken();

  const modelId = useMemo(() => model?.id ?? '', [model]);

  const [selectedFolder, setSelectedFolder] = useState<KPIFolder | undefined>();

  const dispatch = useAppDispatch();

  const nav = useNavigate();

  const handleAddNewKPI = useCallback(() => {
    const folderId =
      selectedFolder?.id === 'root' ? undefined : selectedFolder?.id;
    dispatch(createNewKPI({ modelId, folderId }))
      .unwrap()
      .then((kpi) => {
        void message.success(
          `Create new KPI '${kpi.name}' for model '${model?.name}'`
        );
      })
      .catch((e: BackendError) => message.error(e.message));
  }, [modelId, dispatch, selectedFolder]);

  const handleAddNewKPIFolder = useCallback(() => {
    const folderId =
      selectedFolder?.id === 'root' ? undefined : selectedFolder?.id;
    dispatch(
      createKPIFolder({
        modelId,
        name: 'New folder',
        folderId: folderId,
      })
    )
      .unwrap()
      .then((folder) => {
        void message.success(`Create new folder ${folder.name}`);
      })
      .catch((e: BackendError) => message.error(e.message));
  }, [modelId, dispatch, nav, selectedFolder]);

  const kpis = useAppSelector(selectAllKPIs(modelId));

  const kpiFolders = useMemo(() => {
    return [
      ...(model?.kpiFolders?.map(mapToFolderStructure) ?? []),
      ...(model?.kpis?.map(mapToFolderStructure) ?? []),
    ];
  }, [model]);

  const treeData = useMemo(() => {
    return {
      //@ts-ignore
      name: 'KPIS',
      id: 'root',
      children: [...kpiFolders],
      kpis: model?.kpis ?? [],
    };
  }, [kpiFolders, kpis]);

  const onFolderRename = useCallback(
    (newName: string, id: string) => {
      dispatch(updateKPIFolder({ modelId, folderId: id, name: newName }))
        .unwrap()
        .then(() => {
          message.success(`Renamed folder to ${newName}!`);
        })
        .catch((err: BackendError) => message.error(err.message));
    },
    [modelId]
  );

  const handleFolderDelete = useCallback(() => {
    const folderId = selectedFolder?.id;
    if (!folderId) return;
    dispatch(
      deleteKPIFolder({
        modelId,
        folderId,
      })
    )
      .unwrap()
      .then(() => {
        message.success('Deleted folder, subfolders and kpis');
      })
      .catch((err: BackendError) => message.error(err.message));
  }, [modelId, selectedFolder]);

  const kpisToDisplay = useMemo(() => {
    return getAllKPIsInFolder(model, selectedFolder?.id);
  }, [model, selectedFolder]);

  return (
    <>
      <Title level={4} style={{ marginTop: 0 }}>
        Key performance indicators
      </Title>
      <Typography>
        Key performance indicators (KPIs) define how performance is measured
      </Typography>
      <Typography>
        Define useful KPIs here and use them later in your reports.
      </Typography>
      <div
        style={{
          width: '100%',
          display: 'flex',
          flexDirection: 'row-reverse',
        }}>
        <Button
          ghost
          onClick={handleAddNewKPI}
          type="primary"
          style={{ marginBottom: 16, marginLeft: 16 }}>
          Add a new kpi
        </Button>
        {selectedFolder?.id && selectedFolder.id !== 'root' && (
          <Popconfirm
            title="Delete"
            description="Are you sure to delete this folder? This will also delete all subfolders and KPIs it contains."
            onConfirm={handleFolderDelete}
            okText="Yes"
            cancelText="No">
            <Button
              ghost
              type="primary"
              danger
              style={{ marginBottom: 16, marginLeft: 16 }}>
              Delete current folder
            </Button>
          </Popconfirm>
        )}
        <Button
          ghost
          onClick={handleAddNewKPIFolder}
          type="primary"
          style={{ marginBottom: 16 }}>
          Add a new folder
        </Button>
      </div>
      <div
        style={{
          display: 'flex',
          flexDirection: 'row',
          justifyContent: 'start',
          alignItems: 'start',
        }}>
        <div
          style={{
            width: '20%',
            display: treeData.children.length > 0 ? 'block' : 'none',
          }}>
          <Tree
            onSelect={(keys, info) => {
              if (info.selectedNodes.length > 0) {
                if (
                  //@ts-ignore
                  info.selectedNodes[0].id === 'root'
                ) {
                  setSelectedFolder(undefined);
                  return;
                }
                if (
                  //@ts-ignore
                  !info.selectedNodes[0].isKPI
                ) {
                  //@ts-ignore
                  setSelectedFolder(info.selectedNodes[0]);
                } else {
                  //@ts-ignore
                  nav(`/analyze/${modelId}/kpi/${info.selectedNodes[0].id}`);
                }
              }
            }}
            defaultExpandedKeys={['root']}
            defaultSelectedKeys={['root']}
            //@ts-ignore
            treeData={[treeData]}
            titleRender={(node) => {
              return (
                <Title
                  level={5}
                  editable={
                    // @ts-ignore
                    node.id !== 'root' &&
                    // @ts-ignore
                    !node.isKPI && {
                      // @ts-ignore
                      onChange: (e) => onFolderRename(e, node.id),
                      icon: <EditOutlined style={{ color: colorPrimary }} />,
                      tooltip: 'click to edit name',
                    }
                  }
                  style={{
                    margin: 0,
                    fontSize: 'medium',
                    fontWeight: 'inherit',
                    color:
                      // @ts-ignore
                      node.id === selectedFolder?.id ||
                      // @ts-ignore
                      (node.id === 'root' && !selectedFolder)
                        ? colorPrimary
                        : 'unset',
                  }}>
                  {
                    // @ts-ignore
                    node.name
                  }
                </Title>
              );
            }}
            showLine={true}
            fieldNames={{
              title: 'name',
              key: 'id',
              children: 'children',
            }}
          />
        </div>
        <div style={{ width: treeData.children.length > 0 ? '80%' : '100%' }}>
          <CustomTable
            dataSource={kpisToDisplay}
            defaultColumns={[
              {
                key: 'name',
                dataIndex: 'name',
                title: 'Name',
                searchable: true,
              },
              {
                key: 'expression',
                dataIndex: 'expression',
                title: 'Expression',
                render(value?: Expression) {
                  return value?.type ?? 'No expression';
                },
              },
              {
                key: 'showInReport',
                dataIndex: 'showInReport',
                title: 'Shows up in report',
                filters: [
                  {
                    text: 'Yes',
                    value: true,
                  },
                  {
                    text: 'No',
                    value: false,
                  },
                ],
                onFilter: (value, record) => record.showInReport === value,
                render(value) {
                  const showsUp = value as boolean;
                  if (showsUp) return <Tag color="green">Yes</Tag>;
                  else return <Tag color="red">No</Tag>;
                },
              },
              {
                fixed: 'right',
                width: 100,
                title: 'Action',
                dataIndex: 'actions',
                key: 'action',
                render: (_, record) => {
                  const kpi = record as KPI;
                  return (
                    <Space size="middle">
                      <Button
                        type="text"
                        onClick={() =>
                          nav(`/analyze/${modelId}/kpi/${kpi.id}`)
                        }>
                        Details
                      </Button>
                      <Button
                        danger
                        type="text"
                        onClick={() => {
                          dispatch(
                            deleteKPI({
                              modelId,
                              kpiId: kpi.id,
                            })
                          )
                            .unwrap()
                            .then(
                              () =>
                                void message.success(
                                  `Successfully deleted ${kpi.name}`
                                )
                            )
                            .catch(
                              (err: BackendError) =>
                                void message.error(err.message)
                            );
                        }}>
                        Delete
                      </Button>
                    </Space>
                  );
                },
              },
            ]}
          />
        </div>
      </div>
    </>
  );
};

export default KPIs;
