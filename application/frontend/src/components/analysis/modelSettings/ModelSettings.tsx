import { Button, Divider, Space, Typography, message } from 'antd';
import CustomTable from '../../table/CustomTable';
import { useCallback, useContext, useMemo } from 'react';
import { ModelContext } from '../../../context/ModelContext';
import { BackendError, useAppDispatch } from '../../../app/hooks';
import {
  createGraphicalConfig,
  deleteGraphicalConfig,
} from '../../../features/analysis/analysisSlice';
import { useNavigate } from 'react-router-dom';
import { GraphicalConfiguration } from '../../../features/analysis/types';

const { Title } = Typography;

const ModelSettings: React.FC = () => {
  const { model } = useContext(ModelContext);

  const dispatch = useAppDispatch();

  const nav = useNavigate();

  const graphicals = useMemo(() => {
    return model?.graphical ?? [];
  }, [model]);

  const modelId = useMemo(() => {
    return model?.id ?? '';
  }, [model]);

  const handleConfigAdd = useCallback(() => {
    if (model?.id) {
      dispatch(createGraphicalConfig({ modelId: model.id }))
        .unwrap()
        .then((gc) => {
          void message.success('Created graphical config!');
          nav(`/analyze/${model.id}/graphicalConfig/${gc.id}`);
        })
        .catch((err: BackendError) => message.error(err.message));
    }
  }, [model]);

  return (
    <div>
      <Title level={4} style={{ marginTop: 0 }}>
        Model settings
      </Title>
      <Divider orientationMargin={0} orientation="left">
        Graphical layouts
      </Divider>
      <Typography>
        Graphical layouts are ways to display your data in a report.
      </Typography>
      <Typography>
        You can choose from a variation of charts and widgets to display your
        report the best.
      </Typography>
      <div
        style={{
          width: '100%',
          display: 'flex',
          flexDirection: 'row-reverse',
        }}>
        <Button
          ghost
          onClick={handleConfigAdd}
          type="primary"
          style={{ marginBottom: 16 }}>
          Add a new graphical layout
        </Button>
      </div>
      <CustomTable
        dataSource={graphicals}
        defaultColumns={[
          {
            key: 'name',
            title: 'Name',
            dataIndex: 'name',
            searchable: true,
          },
          {
            key: 'items',
            title: '# of Items',
            dataIndex: 'items',
            render(value) {
              return value.length;
            },
          },
          {
            fixed: 'right',
            width: 100,
            title: 'Action',
            dataIndex: 'actions',
            key: 'action',
            render: (_, record) => {
              const graphicalConfig = record as GraphicalConfiguration;
              return (
                <Space size="middle">
                  <Button
                    type="text"
                    onClick={() =>
                      nav(
                        `/analyze/${modelId}/graphicalConfig/${graphicalConfig.id}`
                      )
                    }>
                    Details
                  </Button>
                  <Button
                    danger
                    type="text"
                    onClick={() => {
                      dispatch(
                        deleteGraphicalConfig({
                          modelId,
                          id: graphicalConfig.id,
                        })
                      )
                        .unwrap()
                        .then(
                          () =>
                            void message.success(
                              `Successfully deleted ${graphicalConfig.name}`
                            )
                        )
                        .catch(
                          (err: BackendError) => void message.error(err.message)
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
  );
};

export default ModelSettings;
