import { useCallback, useContext, useState } from 'react';
import { KPIContext } from '../../../context/KPIContext';
import { EditOutlined } from '@ant-design/icons';
import { Spin, Typography, message, theme } from 'antd';
import { BackendError, useAppDispatch } from '../../../app/hooks';
import { ModelContext } from '../../../context/ModelContext';
import { updateKPIDetails } from '../../../features/analysis/analysisSlice';

const { Title } = Typography;

const KPIDetail: React.FC = () => {
  const {
    token: { colorPrimary },
  } = theme.useToken();

  const { loading, kpi } = useContext(KPIContext);
  const { model } = useContext(ModelContext);

  const [nameLoading, setNameLoading] = useState(false);

  const dispatch = useAppDispatch();

  const onNameChange = useCallback(
    (strVal: string) => {
      const newVal = strVal.trim();
      if (!kpi || !model) return;
      if (kpi.name !== newVal) {
        setNameLoading(true);
        dispatch(
          updateKPIDetails({
            id: kpi.id,
            name: newVal,
            modelId: model.id,
          })
        )
          .unwrap()
          .catch((err: BackendError) => message.error(err.message))
          .finally(() => setNameLoading(false));
      }
    },
    [kpi, model, dispatch]
  );

  return (
    <Spin spinning={loading}>
      <div>
        <Title
          editable={{
            onChange: onNameChange,
            icon: nameLoading ? (
              <Spin />
            ) : (
              <EditOutlined style={{ color: colorPrimary }} />
            ),
            tooltip: 'click to edit name',
          }}
          level={4}
          style={{ marginTop: 0 }}>
          {kpi?.name}
        </Title>
        <div>Main</div>
      </div>
    </Spin>
  );
};

export default KPIDetail;
