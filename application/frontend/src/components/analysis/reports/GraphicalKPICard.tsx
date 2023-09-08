import {
  Button,
  Card,
  Statistic,
  Tooltip as DesignTooltip,
  Alert,
  Typography,
} from 'antd';
import {
  GraphicalReportItem,
  GraphicalReportItemType,
} from '../../../features/analysis/types';
import { useContext, useMemo, useState } from 'react';
import {
  Bar,
  BarChart,
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

interface Props {
  item: GraphicalReportItem;
}

const GraphicalKPICard: React.FC<Props> = (props) => {
  const { item } = props;

  const { report } = useContext(ReportContext);

  const reportData = useMemo(
    () => report?.kpisAndValues ?? undefined,
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
      if (warnings.length > 0) {
        preparedWarnings.push(...warnings);
      } else {
        preparedData.push({
          ...kpi,
          value: reportData[dataSourceId],
        });
      }
    }
    return {
      data: preparedData,
      warnings: preparedWarnings,
    };
  }, [reportData, item.dataSources.kpis, allModelKpis]);

  const content = useMemo(() => {
    switch (item.type) {
      case GraphicalReportItemType.Plain:
        return (
          <Statistic
            value={data.data.length ? data.data[0].value : 'No value'}
            suffix={data.data.length ? data.data[0].unit : undefined}
          />
        );

      case GraphicalReportItemType.BarChart:
        return (
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={data.data}>
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
                data={data.data}
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
  }, [item.type, data]);

  return (
    <Card
      key={item.id}
      title={item.name}
      style={{
        height: 'inherit',
      }}
      bodyStyle={{
        height: '90%',
      }}
      extra={
        data.warnings.length > 0 && (
          <DesignTooltip
            title={
              <Alert
                message="Warning"
                description={data.warnings.map((w) => (
                  <Typography>{w}</Typography>
                ))}
                type="warning"
                showIcon
              />
            }>
            <Button
              type="text"
              icon={<WarningOutlined style={{ color: 'orange' }} />}
            />
          </DesignTooltip>
        )
      }>
      <div
        style={{
          position: 'relative',
          height: '100%',
          width: '100%',
        }}>
        {content}
      </div>
    </Card>
  );
};

export default GraphicalKPICard;
