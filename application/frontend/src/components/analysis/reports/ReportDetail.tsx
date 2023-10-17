import { Tabs, Typography } from 'antd';
import { useContext, useMemo } from 'react';
import { ReportContext } from '../../../context/ReportContext';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  AreaChartOutlined,
  BulbOutlined,
  FileSearchOutlined,
  InsertRowAboveOutlined,
} from '@ant-design/icons';
import KpiReportListDisplay from './KpiReportListDisplay';
import GraphicalReportDisplay from './GraphicalReportDisplay';
import CompareReports from './compare/CompareReports';
const { Title } = Typography;

const ReportDetail: React.FC = () => {
  const { report } = useContext(ReportContext);

  const navigate = useNavigate();
  const location = useLocation();

  const activeKey = useMemo(() => {
    if (
      ['#kpitable', '#graphical', '#rawdata', '#compare'].includes(
        location.hash
      )
    ) {
      return location.hash;
    } else return '#kpitable';
  }, [location]);

  return (
    <div>
      <Title level={4} style={{ marginTop: 0 }}>
        {report?.title}
      </Title>
      <Typography>{report?.notes}</Typography>
      <Tabs
        activeKey={activeKey}
        onChange={(activeKey) => {
          navigate(activeKey);
        }}
        items={[
          {
            key: '#kpitable',
            label: (
              <span>
                <BulbOutlined />
                KPI table
              </span>
            ),
            children: <KpiReportListDisplay />,
          },
          {
            key: '#graphical',
            label: (
              <span>
                <AreaChartOutlined />
                Graphical
              </span>
            ),
            children: <GraphicalReportDisplay />,
          },
          {
            key: '#rawdata',
            disabled: true,
            label: (
              <span>
                <InsertRowAboveOutlined />
                Raw data
              </span>
            ),
            children: <div>Raw</div>,
          },
          {
            key: '#compare',
            label: (
              <span>
                <FileSearchOutlined />
                Compare
              </span>
            ),
            children: <CompareReports />,
          },
        ]}
      />
    </div>
  );
};

export default ReportDetail;
