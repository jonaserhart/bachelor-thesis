import {
  Bar,
  BarChart,
  CartesianGrid,
  Legend,
  Pie,
  PieChart,
  ResponsiveContainer,
  Sector,
  XAxis,
  YAxis,
} from 'recharts';
import {
  GraphicalItemProperties,
  GraphicalReportItemType,
  KPI,
  KPIFolder,
} from '../../../../features/analysis/types';
import { useCallback, useContext, useEffect, useMemo, useState } from 'react';
import {
  Button,
  Card,
  Divider,
  Dropdown,
  Select,
  Spin,
  Statistic,
  Table,
  Tooltip,
  TreeSelect,
  Typography,
  message,
  theme,
} from 'antd';
import { Tooltip as TT } from 'recharts';
import { useAppSelector } from '../../../../app/hooks';
import {
  CloseOutlined,
  DragOutlined,
  EditOutlined,
  MoreOutlined,
} from '@ant-design/icons';
import { selectAllKPIs } from '../../../../features/analysis/analysisSlice';
import { ModelContext } from '../../../../context/ModelContext';
import { mapToFolderStructure } from '../../../../util/kpiFolderUtils';
import CustomChartTooltip from '../../../CustomChartTooltip';
import { selectColors } from '../../../../util/graphicalUtils';

const { Paragraph, Title } = Typography;

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

const colors = selectColors(3);

const multipleKpiData = [
  {
    name: 'KPI 1',
    value: 10,
    fill: colors[0],
  },
  {
    name: 'KPI 2',
    value: 30,
    fill: colors[1],
  },
  {
    name: 'KPI 3',
    value: 20,
    fill: colors[2],
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
  graphicalItemProperties: GraphicalItemProperties;
  onGraphicalItemPropertiesChange: (
    props: GraphicalItemProperties
  ) => Promise<void>;
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
    graphicalItemProperties,
    onGraphicalItemPropertiesChange,
  } = props;

  const {
    token: { colorPrimary },
  } = theme.useToken();

  const [busy, setBusy] = useState(false);

  const multipleKPIs = useMemo(
    () =>
      [
        GraphicalReportItemType.BarChart,
        GraphicalReportItemType.PieChart,
      ].includes(type),
    [type]
  );

  const { model } = useContext(ModelContext);

  const modelId = useMemo(() => model?.id ?? '', [model]);

  const kpis = useAppSelector(selectAllKPIs(modelId));

  const [selectedKPIs, setSelectedKPIs] = useState<string[] | string>([]);
  const [selectedListFields, setSelectedListFields] = useState<string[]>([]);

  useEffect(() => {
    if (selectedKPIsForItem.length) {
      setSelectedKPIs(selectedKPIsForItem);
    }
  }, [selectedKPIsForItem]);

  useEffect(() => {
    setSelectedListFields(graphicalItemProperties?.listFields ?? []);
  }, [graphicalItemProperties]);

  const onKPISelect = useCallback(
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

  const onConfigChange = useCallback(
    (values: string[]) => {
      setSelectedListFields(values);
      onGraphicalItemPropertiesChange({
        ...graphicalItemProperties,
        listFields: values,
      });
    },
    [setSelectedListFields, graphicalItemProperties]
  );

  const [configOpen, setConfigOpen] = useState(false);

  const content = useMemo(() => {
    switch (type) {
      case GraphicalReportItemType.Plain:
        return (
          <div
            style={{
              width: '100%',
              display: 'flex',
              justifyContent: 'center',
              alignItems: 'center',
            }}>
            <Statistic value={80} suffix={'%'} />
          </div>
        );

      case GraphicalReportItemType.BarChart:
        return (
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={multipleKpiData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis />
              <Bar dataKey="value" />
              <TT
                cursor={false}
                allowEscapeViewBox={{ y: true }}
                content={<CustomChartTooltip />}
              />
            </BarChart>
          </ResponsiveContainer>
        );
      case GraphicalReportItemType.PieChart:
        return (
          <ResponsiveContainer width="100%" height="100%">
            <PieChart>
              <Pie
                nameKey="name"
                dataKey="value"
                data={multipleKpiData}
                cx="50%"
                cy="50%"
                innerRadius={40}
                outerRadius={80}
              />
              <Legend />
              <TT
                cursor={false}
                allowEscapeViewBox={{ y: true }}
                content={<CustomChartTooltip />}
              />
            </PieChart>
          </ResponsiveContainer>
        );
      case GraphicalReportItemType.List:
        return <Table />;
      case GraphicalReportItemType.Text:
        return null;
    }
  }, [type]);

  const kpiFolders = useMemo(() => {
    return [
      ...(model?.kpiFolders?.map(mapToFolderStructure) ?? []),
      ...(model?.kpis?.map(mapToFolderStructure) ?? []),
    ];
  }, [model]);

  const canDefineKPIs = useMemo(
    () =>
      [
        GraphicalReportItemType.BarChart,
        GraphicalReportItemType.List,
        GraphicalReportItemType.PieChart,
        GraphicalReportItemType.Plain,
      ].includes(type),
    [type]
  );

  if (type === GraphicalReportItemType.Text) {
    return (
      <Card
        style={{
          width: '100%',
          height: '100%',
          position: 'relative',
        }}>
        {enableEdit && (
          <div
            className="drag-handle"
            style={{
              position: 'absolute',
              top: 0,
              right: 0,
              zIndex: 1,
            }}>
            <Tooltip title="Drag this element anywhere">
              <Button icon={<DragOutlined />} type="text" />
            </Tooltip>
          </div>
        )}
        <Divider orientation="left" orientationMargin={0}>
          <Title
            level={4}
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
                tooltip: 'click to edit text',
              }
            }>
            {title}
          </Title>
        </Divider>
      </Card>
    );
  }

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
        bodyStyle={{
          height: '90%',
          paddingLeft: 0,
        }}
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
                        disabled: !canDefineKPIs,
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
                  onChange={onKPISelect}
                  value={
                    multipleKPIs
                      ? selectedKPIs
                      : selectedKPIs.length
                      ? selectedKPIs[0]
                      : undefined
                  }
                  style={{ width: '100%' }}
                  multiple={multipleKPIs}
                  treeData={kpiFolders}
                  placeholder="Please select some KPI"
                  fieldNames={{
                    label: 'name',
                    value: 'id',
                  }}
                />
                {type === GraphicalReportItemType.List && (
                  <Select
                    value={selectedListFields}
                    mode="tags"
                    onChange={onConfigChange}
                    style={{ width: '100%' }}
                    placeholder="Add fields to display in your list"
                  />
                )}
              </Card>
            </div>
          </div>
        </div>
      </Card>
    </>
  );
};

export default ExampleCard;
