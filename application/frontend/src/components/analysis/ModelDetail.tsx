import { useParams } from 'react-router-dom';
import { Spin, Typography, message, theme, Tabs, Skeleton } from 'antd';
import {
  CloudServerOutlined,
  EditOutlined,
  FileDoneOutlined,
} from '@ant-design/icons';
import { BackendError, useAppDispatch, useAppSelector } from '../../app/hooks';
import Queries from '../../components/analysis/queries/Queries';
import { useCallback, useContext, useEffect, useMemo, useState } from 'react';
import {
  getModelDetails,
  selectModel,
  updateModelDetails,
} from '../../features/analysis/analysisSlice';
import { ModelContext } from '../../context/ModelContext';

const { Title } = Typography;

const ModelDetail: React.FC = () => {
  const { loading, model } = useContext(ModelContext);

  const dispatch = useAppDispatch();

  const [nameLoading, setNameLoading] = useState(false);

  const {
    token: { colorPrimary },
  } = theme.useToken();

  const onNameChange = useCallback(
    (strVal: string) => {
      const newVal = strVal.trim();
      if (!model) return;
      if (model.name !== newVal) {
        setNameLoading(true);
        dispatch(
          updateModelDetails({
            id: model.id,
            name: newVal,
          })
        )
          .unwrap()
          .catch((err: BackendError) => message.error(err.message))
          .finally(() => setNameLoading(false));
      }
    },
    [model, dispatch]
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
          {model?.name}
        </Title>
        <Tabs
          items={[
            {
              key: '1',
              label: (
                <span>
                  <CloudServerOutlined />
                  Queries
                </span>
              ),
              children: <Queries />,
            },
            {
              key: '2',
              label: (
                <span>
                  <FileDoneOutlined />
                  Latest Reports
                </span>
              ),
            },
          ]}
        />
      </div>
    </Spin>
  );
};

export default ModelDetail;
