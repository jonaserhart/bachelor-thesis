import {
  Button,
  Card,
  Statistic,
  Tooltip as DesignTooltip,
  Alert,
  Typography,
  Empty,
  Table,
  Divider,
  Spin,
} from 'antd';
import {
  GraphicalReportItem,
  GraphicalReportItemType,
} from '../../../features/analysis/types';
import { useContext, useMemo, useState } from 'react';
import {
  Bar,
  BarChart,
  CartesianGrid,
  Legend,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts';
import { useAppSelector } from '../../../app/hooks';
import { selectAllKPIs } from '../../../features/analysis/analysisSlice';
import { ModelContext } from '../../../context/ModelContext';
import { ReportContext } from '../../../context/ReportContext';
import { WarningOutlined } from '@ant-design/icons';
import { selectColors } from '../../../util/graphicalUtils';
import CustomChartTooltip from '../../CustomChartTooltip';
import {
  acceptableValuesToPrettyString,
  isAcceptable,
} from '../../../util/acceptableValueFunctions';

interface Props {
  item: GraphicalReportItem;
}

const { Title } = Typography;

const GraphicalKPICard: React.FC<Props> = (props) => {
  const { item } = props;

  const { report, loading } = useContext(ReportContext);

  const reportData = useMemo(
    () => report?.reportData?.kpisAndValues ?? undefined,
    [report]
  );

  const { model } = useContext(ModelContext);
  const modelId = useMemo(() => model?.id ?? '', [model]);

  const allModelKpis = useAppSelector(selectAllKPIs(modelId));

  const data = useMemo(() => {
    const preparedData = [];
    const preparedWarnings = [];

    if (!reportData) {
      return {
        data: [],
        warnings: [],
      };
    }

    const colors = selectColors(item.dataSources.kpis.length);

    let i = 0;

    for (let dataSourceId of item.dataSources.kpis) {
      const kpi = allModelKpis.find((kpi) => kpi.id === dataSourceId);
      const warnings = [];
      if (!kpi) {
        warnings.push(
          `KPI with id ${dataSourceId} was not found in configured model KPIs`
        );
      }
      if (!(dataSourceId in reportData)) {
        warnings.push(`No data found for KPI with id ${dataSourceId}`);
      }
      if (item.type === GraphicalReportItemType.List) {
        if (!Array.isArray(reportData[dataSourceId])) {
          warnings.push(`Data computed by kpi '${kpi?.name}' was not a list.`);
        } else if (typeof reportData[dataSourceId][0] !== 'object') {
          warnings.push(
            `Data computed by kpi '${kpi?.name}' was not a list or does not have any items.`
          );
        }
      }
      if (warnings.length > 0) {
        preparedWarnings.push(...warnings);
      } else {
        preparedData.push({
          ...kpi,
          value: reportData[dataSourceId],
          fill: colors[i],
        });
        if (
          kpi?.acceptableValues &&
          !isAcceptable(reportData[dataSourceId], kpi.acceptableValues)
        ) {
          preparedWarnings.push(
            `Value for '${
              kpi.name
            }' is not optimal (${acceptableValuesToPrettyString(
              kpi.acceptableValues
            )}).`
          );
        }
        i++;
      }
    }
    return {
      data: preparedData,
      warnings: preparedWarnings,
    };
  }, [reportData, item.dataSources.kpis, item.type, allModelKpis]);

  const content = useMemo(() => {
    switch (item.type) {
      case GraphicalReportItemType.Plain:
        return (
          <div
            style={{
              width: '100%',
              display: 'flex',
              justifyContent: 'center',
              alignItems: 'center',
            }}>
            <Statistic
              value={data.data.length ? data.data[0].value : 'No value'}
              suffix={data.data.length ? data.data[0].unit : undefined}
            />
          </div>
        );
      case GraphicalReportItemType.List:
        console.log('ITM:', item);
        let listFields = item?.properties?.listFieldsWithLabels;
        if (!listFields?.length) {
          listFields = item?.properties?.listFields?.map((x) => ({
            label: x,
            value: x,
          }));
        }
        if (!listFields || listFields.length <= 0 || data.data.length <= 0) {
          return <Empty />;
        }
        const tableData = data.data[0].value;

        const columns = listFields.map((field) => ({
          title: field.label,
          dataIndex: field.value,
          key: field.value,
          render: (text: any) => text || '',
        }));

        console.log('COLS: ', columns);
        console.log('DATA: ', tableData);

        return (
          <div style={{ width: '100%', height: '100%', overflow: 'scroll' }}>
            <Table
              style={{ minWidth: '300px', minHeight: '200px' }}
              dataSource={tableData}
              columns={columns}
            />
          </div>
        );
      case GraphicalReportItemType.BarChart:
        return (
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={data.data}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis />
              <Bar dataKey="value" />
              <Tooltip
                cursor={false}
                allowEscapeViewBox={{ y: true }}
                content={<CustomChartTooltip />}
              />
            </BarChart>
          </ResponsiveContainer>
        );
      case GraphicalReportItemType.PieChart:
        if (data.data.every((x) => x.value === 0)) {
          return (
            <Empty description="Every selected KPI for this chart has value '0'." />
          );
        }
        return (
          <ResponsiveContainer>
            <PieChart>
              <Pie
                nameKey="name"
                dataKey="value"
                data={data.data}
                cx="50%"
                cy="50%"
                innerRadius={40}
                outerRadius={80}
              />
              <Legend />
              <Tooltip
                cursor={false}
                allowEscapeViewBox={{ y: true }}
                content={<CustomChartTooltip />}
              />
            </PieChart>
          </ResponsiveContainer>
        );
      case GraphicalReportItemType.Text:
        return null;
    }
  }, [item.type, data]);

  if (item.type === GraphicalReportItemType.Text) {
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
              }}>
              {item.name}
            </Title>
          </Divider>
        </div>
      </Card>
    );
  }

  return (
    <Card
      key={item.id}
      title={item.name}
      style={{
        height: 'inherit',
      }}
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
      }}
      extra={
        data.warnings.length > 0 && (
          <DesignTooltip
            title={
              <Alert
                message="Warning"
                description={data.warnings.map((w, i) => (
                  <Typography key={`warning-${w}-${item.id}`}>{w}</Typography>
                ))}
                type="warning"
                showIcon
              />
            }>
            <Button
              style={{
                zIndex: 500,
              }}
              type="text"
              icon={<WarningOutlined style={{ color: 'orange' }} />}
            />
          </DesignTooltip>
        )
      }>
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
        {loading ? null : content}
      </div>
    </Card>
  );
};

export default GraphicalKPICard;
