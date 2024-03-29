import { useAppDispatch, useAppSelector } from '../../app/hooks';
import {
  createModel,
  deleteModel,
  getMyModels,
  selectModels,
  updateModelDetails,
} from './analysisSlice';
import { Button, Space, Spin, Typography, message } from 'antd';
import CustomTable from '../../components/table/CustomTable';
import { AnalysisModel } from './types';
import { useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import handleError from '../../util/handleError';

const { Title } = Typography;

const Analysis: React.FC = () => {
  const dispatch = useAppDispatch();

  const [loading, setLoading] = useState(false);

  const nav = useNavigate();

  const models = useAppSelector(selectModels);

  useEffect(() => {
    setLoading(true);
    dispatch(getMyModels())
      .unwrap()
      .catch(handleError)
      .finally(() => setLoading(false));
  }, [dispatch]);

  if (loading) {
    return (
      <div
        style={{
          width: '100%',
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
        }}>
        <Spin tip="Loading models" size="default" />
      </div>
    );
  }

  return (
    <div>
      <Title level={4} style={{ marginTop: 0 }}>
        Analysis
      </Title>
      <div
        style={{
          width: '100%',
          display: 'flex',
          flexDirection: 'row-reverse',
        }}>
        <Button
          ghost
          onClick={() =>
            dispatch(
              createModel({
                name: 'new model',
              })
            )
              .unwrap()
              .then((v) =>
                message.success(`Successfully created model ${v.name}`)
              )
              .catch(handleError)
          }
          type="primary"
          style={{ marginBottom: 16 }}>
          Add a new model
        </Button>
      </div>
      <CustomTable
        dataSource={models}
        defaultColumns={[
          {
            key: 'name',
            dataIndex: 'name',
            title: 'Name',
          },
          {
            key: 'noReports',
            title: 'Reports',
            dataIndex: 'reports',
            render(value) {
              const kpi = value as Report[];
              if (kpi.length <= 0) {
                return <div style={{ color: 'gray' }}>No reports yet</div>;
              }
              return kpi.length;
            },
            sorter: {
              compare: (a, b) =>
                (a as AnalysisModel).kpis.length -
                (b as AnalysisModel).kpis.length,
            },
          },
          {
            fixed: 'right',
            width: 100,
            title: 'Action',
            dataIndex: 'actions',
            key: 'action',
            render: (_, record) => (
              <Space size="middle">
                <Button
                  type="text"
                  onClick={() =>
                    nav(`/analyze/${(record as AnalysisModel).id}`)
                  }>
                  Details
                </Button>
                <Button
                  onClick={() => {
                    dispatch(deleteModel((record as AnalysisModel).id));
                  }}
                  danger
                  type="text">
                  Delete{' '}
                </Button>
              </Space>
            ),
          },
        ]}
        handleSave={async (newRecord) => {
          await dispatch(
            updateModelDetails({
              id: newRecord.id,
              name: newRecord.name,
            })
          );
        }}
      />
    </div>
  );
};

export default Analysis;
