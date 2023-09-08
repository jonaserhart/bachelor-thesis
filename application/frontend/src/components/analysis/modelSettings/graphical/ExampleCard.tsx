import {
  Bar,
  BarChart,
  Legend,
  Pie,
  PieChart,
  ResponsiveContainer,
  XAxis,
  YAxis,
} from 'recharts';
import {
  GraphicalReportItemType,
  KPI,
  KPIFolder,
} from '../../../../features/analysis/types';
import { useCallback, useContext, useEffect, useMemo, useState } from 'react';
import {
  Button,
  Card,
  Dropdown,
  Form,
  Menu,
  Popover,
  Select,
  Spin,
  Statistic,
  Tooltip,
  TreeSelect,
  Typography,
  message,
  theme,
} from 'antd';
import { BackendError, useAppSelector } from '../../../../app/hooks';
import {
  CloseOutlined,
  DragOutlined,
  EditOutlined,
  MoreOutlined,
} from '@ant-design/icons';
import {
  selectAllKPIs,
  selectModel,
} from '../../../../features/analysis/analysisSlice';
import { ModelContext } from '../../../../context/ModelContext';
import { mapToFolderStructure } from '../../../../util/kpiFolderUtils';

const { Paragraph } = Typography;

const isLeafNode = (
  value: string,
  data: (KPI | KPIFolder)[]
): boolean | undefined => {
  for (const node of data) {
    if (node.id === value) {
      return 'expression' in node;
    }
  }
  return undefined;
};

const multipleKpiData = [
  {
    name: 'KPI 1',
    value: 10,
  },
  {
    name: 'KPI 2',
    value: 30,
  },
  {
    name: 'KPI 3',
    value: 20,
  },
];

interface Props {
  type: GraphicalReportItemType;
  title: string;
  onTitleChange: (newTitle: string) => Promise<void>;
  enableEdit: boolean;
  onDelete?: () => Promise<void>;
  onKPIsChange: (kpis: string[]) => Promise<void>;
  selectedKPIsForItem: string[];
}

const ExampleCard: React.FC<Props> = (props) => {
  const {
    type,
    title,
    enableEdit,
    onDelete,
    onTitleChange,
    onKPIsChange,
    selectedKPIsForItem,
  } = props;

  const {
    token: { colorPrimary },
  } = theme.useToken();

  const [busy, setBusy] = useState(false);

  const multipleKPIs = useMemo(
    () => type !== GraphicalReportItemType.Plain,
    [type]
  );

  const { model } = useContext(ModelContext);

  const modelId = useMemo(() => model?.id ?? '', [model]);

  const kpis = useAppSelector(selectAllKPIs(modelId));

  const [selectedKPIs, setSelectedKPIs] = useState<string[]>([]);

  useEffect(() => {
    setSelectedKPIs(selectedKPIsForItem);
  }, [selectedKPIsForItem]);

  const onChange = useCallback(
    (values: string[] | string) => {
      const vals = Array.isArray(values) ? values : [values];
      const isLeafValue = vals.every((value) => isLeafNode(value, kpis));
      if (isLeafValue) {
        setSelectedKPIs(vals);
        onKPIsChange(vals);
      }
    },
    [setSelectedKPIs, kpis]
  );

  const [configOpen, setConfigOpen] = useState(false);

  const content = useMemo(() => {
    switch (type) {
      case GraphicalReportItemType.Plain:
        return <Statistic value={80} suffix={'%'} />;

      case GraphicalReportItemType.BarChart:
        return (
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={multipleKpiData}>
              <XAxis dataKey="name" />
              <YAxis />
              <Bar dataKey="value" fill="#8884d8" />
            </BarChart>
          </ResponsiveContainer>
        );
      case GraphicalReportItemType.PieChart:
        return (
          <ResponsiveContainer width="100%" height="100%">
            <PieChart>
              <Pie
                dataKey="value"
                data={multipleKpiData}
                innerRadius={40}
                outerRadius={80}
                fill="#82ca9d"
                label
              />
              <Tooltip />
              <Legend />
            </PieChart>
          </ResponsiveContainer>
        );
    }
  }, [type]);

  const kpiFolders = useMemo(() => {
    return [
      ...(model?.kpiFolders?.map(mapToFolderStructure) ?? []),
      ...(model?.kpis?.map(mapToFolderStructure) ?? []),
    ];
  }, [model]);

  return (
    <>
      <Card
        title={
          <Paragraph
            style={{
              margin: 0,
              marginInline: 12,
            }}
            editable={
              !enableEdit && {
                onChange: (newTitle: string) => {
                  onTitleChange(newTitle);
                },
                icon: busy ? (
                  <Spin />
                ) : (
                  <EditOutlined style={{ color: colorPrimary }} />
                ),
                tooltip: 'click to edit name',
              }
            }>
            {title}
          </Paragraph>
        }
        style={{ height: 'inherit' }}
        bodyStyle={{ height: '90%' }}
        extra={
          <>
            {enableEdit && (
              <div style={{ display: 'flex', alignItems: 'center' }}>
                <Dropdown
                  menu={{
                    items: [
                      {
                        key: 'configure',
                        label: 'Configure KPIs',
                        onClick() {
                          setConfigOpen(true);
                        },
                      },
                      {
                        key: 'delete',
                        label: 'Delete',
                        danger: true,
                        onClick() {
                          if (onDelete) {
                            setBusy(true);
                            onDelete()
                              .then(() =>
                                message.success(`Deleted item ${title}`)
                              )
                              .catch((err) => message.error(err.message))
                              .finally(() => setBusy(false));
                          }
                        },
                      },
                    ],
                  }}
                  trigger={['click']}>
                  <Tooltip title="Settings">
                    <Button icon={<MoreOutlined />} type="text" />
                  </Tooltip>
                </Dropdown>
                <div className="drag-handle" style={{ marginLeft: 10 }}>
                  <Tooltip title="Drag this element anywhere">
                    <Button icon={<DragOutlined />} type="text" />
                  </Tooltip>
                </div>
              </div>
            )}
          </>
        }>
        <div
          style={{
            position: 'relative',
            height: '100%',
            width: '100%',
          }}>
          {content}
          <div
            style={{
              display: configOpen ? 'unset' : 'none',
              position: 'absolute',
              top: 0,
              left: 0,
              width: '100%',
              height: '100%',
              backgroundColor: 'rgba(0,0,0,0.2)',
              zIndex: 2,
              overflow: 'visible',
            }}>
            <div
              style={{
                display: 'flex',
                width: '100%',
                height: '100%',
                alignItems: 'center',
                justifyContent: 'center',
                overflow: 'visible',
              }}>
              <Card
                title={`Displayed KPI${multipleKPIs ? 's' : ''}`}
                extra={
                  <Tooltip title="Close">
                    <Button
                      icon={<CloseOutlined />}
                      onClick={() => setConfigOpen(false)}
                      danger
                      type="text"
                    />
                  </Tooltip>
                }
                style={{ minWidth: 300, minHeight: 100 }}>
                <TreeSelect
                  onChange={onChange}
                  value={selectedKPIs}
                  style={{ width: '100%' }}
                  multiple={multipleKPIs}
                  treeData={kpiFolders}
                  placeholder="Please select some KPI"
                  fieldNames={{
                    label: 'name',
                    value: 'id',
                  }}
                />
              </Card>
            </div>
          </div>
        </div>
      </Card>
    </>
  );
};

export default ExampleCard;
