import { Spin, Typography, message, theme, Tabs } from 'antd';
import {
  BulbOutlined,
  CloudServerOutlined,
  EditOutlined,
  FileDoneOutlined,
} from '@ant-design/icons';
import { BackendError, useAppDispatch } from '../../app/hooks';
import Queries from '../../components/analysis/queries/Queries';
import { useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { updateModelDetails } from '../../features/analysis/analysisSlice';
import { ModelContext } from '../../context/ModelContext';
import { useLocation, useNavigate } from 'react-router-dom';
import KPIs from './kpis/KPIs';

const { Title } = Typography;

const ModelDetail: React.FC = () => {
  const { loading, model } = useContext(ModelContext);

  const dispatch = useAppDispatch();

  const [nameLoading, setNameLoading] = useState(false);

  const {
    token: { colorPrimary },
  } = theme.useToken();

  const navigate = useNavigate();
  const location = useLocation();

  const activeKey = useMemo(() => {
    if (['#kpis', '#queries', '#l8estreports'].includes(location.hash)) {
      return location.hash;
    } else return '#l8estreports';
  }, [location]);

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
          activeKey={activeKey}
          onChange={(activeKey) => {
            navigate(activeKey);
          }}
          items={[
            {
              key: '#l8estreports',
              label: (
                <span>
                  <FileDoneOutlined />
                  Latest Reports
                </span>
              ),
            },
            {
              key: '#kpis',
              label: (
                <span>
                  <BulbOutlined />
                  KPIs
                </span>
              ),
              children: <KPIs />,
            },
            {
              key: '#queries',
              label: (
                <span>
                  <CloudServerOutlined />
                  Queries
                </span>
              ),
              children: <Queries />,
            },
          ]}
        />
      </div>
    </Spin>
  );
};

export default ModelDetail;
