import { useCallback, useContext, useMemo, useState } from 'react';
import { ModelContext } from '../../../context/ModelContext';
import { Popover, Button, Space, message } from 'antd';
import { Expression, KPI } from '../../../features/analysis/types';
import CustomTable from '../../table/CustomTable';
import { BackendError, useAppDispatch } from '../../../app/hooks';
import {
  createNewKPI,
  deleteKPI,
} from '../../../features/analysis/analysisSlice';
import { useNavigate } from 'react-router-dom';

const KPIs: React.FC = () => {
  const { model } = useContext(ModelContext);

  const modelId = useMemo(() => model?.id ?? '', [model]);

  const dispatch = useAppDispatch();

  const handleAddNewKPI = useCallback(() => {
    dispatch(createNewKPI(modelId))
      .unwrap()
      .then(() => {
        void message.success(`Create new KPI for model ${model?.name}`);
      })
      .catch((e: BackendError) => message.error(e.message));
  }, [modelId, dispatch]);

  const kpis = useMemo(() => {
    return model?.kpis ?? [];
  }, [model]);

  const nav = useNavigate();

  return (
    <>
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
          style={{ marginBottom: 16 }}>
          Add a new kpi
        </Button>
      </div>
      <CustomTable
        dataSource={kpis}
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
                    onClick={() => nav(`/analyze/${modelId}/kpi/${kpi.id}`)}>
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
    </>
  );
};

export default KPIs;
