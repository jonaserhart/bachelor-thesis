import { useContext, useMemo } from 'react';
import { Report } from '../../../features/analysis/types';
import { ModelContext } from '../../../context/ModelContext';
import { Avatar, Card, List, Statistic } from 'antd';
import { KeyOutlined } from '@ant-design/icons';
import { isNumber } from 'util';
import CustomTable from '../../table/CustomTable';
import { useAppSelector } from '../../../app/hooks';
import { selectAllKPIs } from '../../../features/analysis/analysisSlice';

interface Props {
  report: Report;
}

const KpiReportListDisplay: React.FC<Props> = (props) => {
  const { report } = props;

  const { model } = useContext(ModelContext);

  const modelId = useMemo(() => model?.id ?? '', [model]);

  const modelKPIs = useAppSelector(selectAllKPIs(modelId));

  const kpis = useMemo(() => {
    const ids = Object.keys(report.kpisAndValues);

    return modelKPIs
      .filter((x) => ids.includes(x.id) && x.showInReport)
      .map((x) => ({ ...x, value: report.kpisAndValues[x.id] }));
  }, [modelKPIs, report.kpisAndValues]);

  return (
    <div>
      <CustomTable
        dataSource={kpis}
        defaultColumns={[
          {
            key: 'name',
            title: 'KPI',
            dataIndex: 'name',
            searchable: true,
          },
          {
            key: 'value',
            dataIndex: 'value',
            title: 'Value',
            render(value, record, index) {
              if (Array.isArray(value)) {
                return `List of length ${value.length}`;
              }
              return value;
            },
          },
          {
            key: 'unit',
            dataIndex: 'unit',
            title: 'Unit',
          },
        ]}
      />
    </div>
  );
};

export default KpiReportListDisplay;
