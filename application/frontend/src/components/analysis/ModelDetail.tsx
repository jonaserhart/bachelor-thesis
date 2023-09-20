import { Spin, Typography, message, theme, Tabs } from 'antd';
import {
  BulbOutlined,
  CloudServerOutlined,
  EditOutlined,
  FileDoneOutlined,
  LockOutlined,
  SettingOutlined,
} from '@ant-design/icons';
import { BackendError, useAppDispatch } from '../../app/hooks';
import Queries from '../../features/queries/Queries';
import { useCallback, useContext, useMemo, useState } from 'react';
import { updateModelDetails } from '../../features/analysis/analysisSlice';
import { ModelContext } from '../../context/ModelContext';
import { useSearchParams } from 'react-router-dom';
import KPIs from './kpis/KPIs';
import Reports from './reports/Reports';
import ModelSettings from './modelSettings/ModelSettings';
import ModelAccess from './access/ModelAccess';

const { Title } = Typography;

const TAB_PARAM = 'tab';

const ModelDetail: React.FC = () => {
  const { loading, model } = useContext(ModelContext);

  const dispatch = useAppDispatch();

  const [nameLoading, setNameLoading] = useState(false);

  const {
    token: { colorPrimary },
  } = theme.useToken();

  const [searchParams, setSearchParams] = useSearchParams();

  const tabId = useMemo(() => {
    const param = searchParams.get(TAB_PARAM) ?? 'l8estreports';

    if (
      ['kpis', 'queries', 'l8estreports', 'settings', 'access'].includes(param)
    ) {
      return param;
    }

    return 'l8estreports';
  }, [searchParams]);

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
          activeKey={tabId}
          onChange={(activeKey) => {
            setSearchParams((prev) => {
              prev.set(TAB_PARAM, activeKey);
              return prev;
            });
          }}
          items={[
            {
              key: 'l8estreports',
              label: (
                <span>
                  <FileDoneOutlined />
                  Latest Reports
                </span>
              ),
              children: <Reports />,
            },
            {
              key: 'kpis',
              label: (
                <span>
                  <BulbOutlined />
                  KPIs
                </span>
              ),
              children: <KPIs />,
            },
            {
              key: 'queries',
              label: (
                <span>
                  <CloudServerOutlined />
                  Queries
                </span>
              ),
              children: <Queries />,
            },
            {
              key: 'settings',
              label: (
                <span>
                  <SettingOutlined />
                  Settings
                </span>
              ),
              children: <ModelSettings />,
            },
            {
              key: 'access',
              label: (
                <span>
                  <LockOutlined />
                  Access
                </span>
              ),
              children: <ModelAccess />,
            },
          ]}
        />
      </div>
    </Spin>
  );
};

export default ModelDetail;
