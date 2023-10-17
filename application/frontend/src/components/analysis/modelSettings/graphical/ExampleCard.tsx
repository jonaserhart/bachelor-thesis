import {
  Bar,
  BarChart,
  CartesianGrid,
  Legend,
  Pie,
  PieChart,
  ResponsiveContainer,
  XAxis,
  YAxis,
  Tooltip as TT,
} from 'recharts';
import {
  ExpressionResultType,
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
import { useAppSelector } from '../../../../app/hooks';
import {
  CloseOutlined,
  DragOutlined,
  EditOutlined,
  MoreOutlined,
} from '@ant-design/icons';
import { selectAllKPIs } from '../../../../features/analysis/analysisSlice';
import { ModelContext } from '../../../../context/ModelContext';
import {
  isLeafNode,
  mapToTreeStructureWithCondition,
} from '../../../../util/kpiFolderUtils';
import CustomChartTooltip from '../../../CustomChartTooltip';
import { selectColors } from '../../../../util/graphicalUtils';
import { selectQueries } from '../../../../features/queries/querySclice';
import { QueryReturnType } from '../../../../features/queries/types';

const { Paragraph, Title } = Typography;

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

  const queries = useAppSelector(selectQueries);

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
    (
      values: string[],
      option:
        | { label: string; value: string }
        | { label: string; value: string }[]
    ) => {
      setSelectedListFields(values);
      onGraphicalItemPropertiesChange({
        ...graphicalItemProperties,
        listFields: values,
        listFieldsWithLabels: Array.isArray(option) ? option : [option],
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

  const allowedKPITypes = useMemo(() => {
    switch (type) {
      case GraphicalReportItemType.Plain:
        return Object.values(ExpressionResultType);
      case GraphicalReportItemType.BarChart:
      case GraphicalReportItemType.PieChart:
        return [ExpressionResultType.Number];
      case GraphicalReportItemType.List:
        return [ExpressionResultType.ObjectList];
      case GraphicalReportItemType.Text:
      default:
        return [];
    }
  }, [type]);

  const kpiFilterFn = useCallback(
    (k: KPI | KPIFolder) =>
      'subFolders' in k
        ? k.kpis.some((x) =>
            x.showInReport &&
            x.expression.returnType === ExpressionResultType.InheritFromQuery
              ? queries
                  .filter((q) => q.id === x.expression.queryId)
                  .map((q) => {
                    switch (q.type) {
                      case QueryReturnType.Number:
                        return ExpressionResultType.Number;
                      case QueryReturnType.String:
                        return ExpressionResultType.String;
                      case QueryReturnType.Object:
                        return ExpressionResultType.Object;
                      case QueryReturnType.NumberList:
                      case QueryReturnType.StringList:
                      case QueryReturnType.ObjectList:
                        return ExpressionResultType.ObjectList;
                    }
                  })
                  .every((qtype) => allowedKPITypes.includes(qtype))
              : allowedKPITypes.includes(x.expression.returnType)
          )
        : k.showInReport &&
          (k.expression.returnType === ExpressionResultType.InheritFromQuery
            ? queries
                .filter((q) => q.id === k.expression.queryId)
                .map((q) => {
                  switch (q.type) {
                    case QueryReturnType.Number:
                      return ExpressionResultType.Number;
                    case QueryReturnType.String:
                      return ExpressionResultType.String;
                    case QueryReturnType.Object:
                      return ExpressionResultType.Object;
                    case QueryReturnType.NumberList:
                    case QueryReturnType.StringList:
                    case QueryReturnType.ObjectList:
                      return ExpressionResultType.ObjectList;
                  }
                })
                .every((qtype) => allowedKPITypes.includes(qtype))
            : allowedKPITypes.includes(k.expression.returnType)),
    [allowedKPITypes, queries]
  );

  const kpiFolders = useMemo(() => {
    return [
      ...(model?.kpiFolders
        .filter(kpiFilterFn)
        ?.map(mapToTreeStructureWithCondition(kpiFilterFn)) ?? []),
      ...(model?.kpis
        .filter(kpiFilterFn)
        ?.map(mapToTreeStructureWithCondition(kpiFilterFn)) ?? []),
    ];
  }, [model, kpiFilterFn]);

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

  const treeSelectValue = useMemo(() => {
    if (multipleKPIs) {
      return selectedKPIs;
    }
    return selectedKPIs.length ? selectedKPIs[0] : undefined;
  }, [multipleKPIs, selectedKPIs]);

  const selectedKPI = useMemo(() => {
    if (typeof treeSelectValue === 'string') {
      return kpis.find((x) => x.id === treeSelectValue);
    }
    return undefined;
  }, [kpis, treeSelectValue]);

  const kpiFieldOptions = useMemo(() => {
    if (type === GraphicalReportItemType.List) {
      if (selectedKPI) {
        const query = queries.find(
          (x) => x.id === selectedKPI.expression.queryId
        );
        if (query?.type === QueryReturnType.ObjectList) {
          return query.additionalQueryData.possibleFields.map((x) => ({
            label: x.displayName,
            value: x.name,
          }));
        }
      }
    }
    return [];
  }, [type, queries, selectedKPI]);

  if (type === GraphicalReportItemType.Text) {
    return (
      <Card
        style={{
          width: '100%',
          height: '100%',
          position: 'relative',
        }}
        bodyStyle={{
          padding: 0,
          height: 'inherit',
          position: 'relative',
          display: 'flex',
          alignItems: 'center',
        }}>
        <div style={{ position: 'relative', height: '100%', width: '100%' }}>
          <div
            style={{
              height: '100%',
              width: '100%',
              position: 'absolute',
              display: 'flex',
              alignItems: 'center',
            }}>
            <Divider orientation="left" orientationMargin={0}>
              <Title
                level={4}
                style={{
                  margin: 0,
                  marginInline: 12,
                  zIndex: 100,
                  position: 'relative',
                }}
                editable={
                  enableEdit && {
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
          </div>
          <div
            style={{
              position: 'absolute',
              zIndex: 100,
              height: '100%',
              width: '100%',
              display: 'flex',
              justifyContent: 'center',
              alignItems: 'center',
            }}>
            {enableEdit && (
              <>
                <Dropdown
                  menu={{
                    items: [
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
                    <Button icon={<MoreOutlined />} type="dashed" />
                  </Tooltip>
                </Dropdown>
                <div className="drag-handle" style={{ marginLeft: 10 }}>
                  <Tooltip title="Drag this element anywhere">
                    <Button icon={<DragOutlined />} type="dashed" />
                  </Tooltip>
                </div>
              </>
            )}
          </div>
        </div>
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
              zIndex: 100,
              position: 'relative',
            }}
            editable={
              enableEdit && {
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
        headStyle={{
          height: 55,
        }}
        bodyStyle={{
          padding: 0,
          height: 'inherit',
          bottom: 55,
          position: 'relative',
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
        }}>
        <div style={{ position: 'relative', height: '100%', width: '100%' }}>
          <div
            style={{
              height: '100%',
              width: '100%',
              position: 'absolute',
              display: 'flex',
              justifyContent: 'center',
              alignItems: 'center',
              paddingTop: 60,
            }}>
            {content}
          </div>
          <div
            style={{
              position: 'absolute',
              height: '100%',
              width: '100%',
              paddingTop: 55,
              display: 'flex',
              justifyContent: 'center',
              alignItems: 'center',
            }}>
            {enableEdit && (
              <>
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
                    <Button icon={<MoreOutlined />} type="dashed" />
                  </Tooltip>
                </Dropdown>
                <div className="drag-handle" style={{ marginLeft: 10 }}>
                  <Tooltip title="Drag this element anywhere">
                    <Button icon={<DragOutlined />} type="dashed" />
                  </Tooltip>
                </div>
              </>
            )}
          </div>
          <div
            style={{
              display: configOpen ? 'unset' : 'none',
              position: 'absolute',
              top: 0,
              left: 0,
              width: '100%',
              height: '100%',
              backgroundColor: 'rgba(0,0,0,0.2)',
              zIndex: 200,
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
                  value={treeSelectValue}
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
                    allowClear
                    filterOption={(input, option) =>
                      (option?.label?.toLowerCase() ?? '').includes(
                        input.toLowerCase()
                      )
                    }
                    mode="multiple"
                    showSearch
                    value={selectedListFields}
                    options={kpiFieldOptions}
                    onChange={onConfigChange}
                    style={{ width: '100%' }}
                    disabled={!treeSelectValue}
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
