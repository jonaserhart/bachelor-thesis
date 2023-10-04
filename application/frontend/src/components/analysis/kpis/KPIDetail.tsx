import { useCallback, useContext, useMemo, useState } from 'react';
import { KPIContext } from '../../../context/KPIContext';
import { CodeOutlined, EditOutlined, SettingOutlined } from '@ant-design/icons';
import { Spin, Tabs, Typography, message, theme } from 'antd';
import { BackendError, useAppDispatch } from '../../../app/hooks';
import { ModelContext } from '../../../context/ModelContext';
import { updateKPIDetails } from '../../../features/analysis/analysisSlice';
import { ExpressionBuilder } from './ExpressionBuilder';
import { useLocation, useNavigate } from 'react-router-dom';
import KPIConfigForm from './KPIConfigForm';

const { Title } = Typography;

const KPIDetail: React.FC = () => {
  const {
    token: { colorPrimary },
  } = theme.useToken();

  const { loading, kpi } = useContext(KPIContext);
  const { model } = useContext(ModelContext);

  const [nameLoading, setNameLoading] = useState(false);

  const dispatch = useAppDispatch();

  const navigate = useNavigate();
  const location = useLocation();

  const activeKey = useMemo(() => {
    if (['#expr', '#config'].includes(location.hash)) {
      return location.hash;
    } else return '#expr';
  }, [location]);

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
        <Tabs
          activeKey={activeKey}
          onChange={(activeKey) => {
            navigate(activeKey);
          }}
          items={[
            {
              key: '#expr',
              label: (
                <span>
                  <CodeOutlined />
                  Expression
                </span>
              ),
              children: <ExpressionBuilder />,
            },
            {
              key: '#config',
              label: (
                <span>
                  <SettingOutlined />
                  Configuration
                </span>
              ),
              children: <KPIConfigForm />,
            },
          ]}
        />
      </div>
    </Spin>
  );
};

export default KPIDetail;
