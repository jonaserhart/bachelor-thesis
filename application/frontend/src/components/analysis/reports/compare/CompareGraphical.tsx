import { Spin } from 'antd';
import { useMemo } from 'react';
import {
  Legend,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts';
import CustomChartTooltip from '../../../CustomChartTooltip';
import { formatDate } from '../../../../util/formatDate';
import { selectColors } from '../../../../util/graphicalUtils';

interface Props {
  loading: boolean;
  fields: {
    title: string;
    dataIndex: string;
    key: string;
  }[];
  reportsWithData: {
    [key: string]: any;
  }[];
}

const CompareGraphical: React.FC<Props> = (props) => {
  const { fields, reportsWithData, loading } = props;

  const colors = useMemo(() => selectColors(fields.length), [reportsWithData]);

  const data = useMemo(() => {
    const transformed: Array<{ report: string } & { [key: string]: any }> = [];

    for (const reportId in reportsWithData) {
      if (reportsWithData.hasOwnProperty(reportId)) {
        const report = reportsWithData[reportId];

        const extractedKPIs = fields.reduce((acc, kpiField) => {
          if (report.hasOwnProperty(kpiField.dataIndex)) {
            acc[kpiField.key] = report[kpiField.dataIndex].value;
          }
          return acc;
        }, {} as any);

        transformed.push({
          label: report.reportTitle,
          xAxisName: `${report.reportTitle} (${formatDate(report.created)})`,
          report: reportId,
          created: report.created,
          ...extractedKPIs,
        });
      }
    }

    return transformed.reverse();
  }, [reportsWithData, fields]);
  return (
    <Spin spinning={loading}>
      <div
        style={{
          minHeight: 400,
          width: '100%',
          height: 100,
        }}>
        <ResponsiveContainer>
          <LineChart data={data}>
            <XAxis dataKey="xAxisName" padding={{ left: 30, right: 30 }} />
            <YAxis />
            <Tooltip
              cursor={false}
              allowEscapeViewBox={{ y: true }}
              content={<CustomChartTooltip />}
            />
            <Legend />
            {fields
              .filter((x) => !['created', 'reportTitle'].includes(x.key))
              .map((x, i) => {
                return (
                  <Line
                    connectNulls
                    key={x.key}
                    label={x.title}
                    name={x.title}
                    type="monotone"
                    dataKey={x.key}
                    stroke={colors[i]}
                  />
                );
              })}
          </LineChart>
        </ResponsiveContainer>
      </div>
    </Spin>
  );
};

export default CompareGraphical;
