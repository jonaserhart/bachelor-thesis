import { useMemo } from 'react';
import { Table } from 'antd';

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

const CompareTable: React.FC<Props> = (props) => {
  const { reportsWithData, fields, loading } = props;

  const columns = useMemo(() => {
    return fields.map((field) => ({
      render: (text: any) => text || '',
      ...field,
    }));
  }, [fields]);

  return (
    <div style={{ width: '100%', height: '100%' }}>
      <Table
        loading={loading}
        style={{ minWidth: '300px', minHeight: '200px' }}
        dataSource={reportsWithData}
        columns={columns}
      />
    </div>
  );
};

export default CompareTable;
