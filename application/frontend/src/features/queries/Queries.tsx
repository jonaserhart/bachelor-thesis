import { Button, Space, Typography, message } from 'antd';
import { BackendError, useAppDispatch, useAppSelector } from '../../app/hooks';
import CustomTable from '../../components/table/CustomTable';
import { useEffect } from 'react';
import { getQueries, selectQueries } from './querySclice';

const { Title } = Typography;

const Queries: React.FC = () => {
  const dispatch = useAppDispatch();

  const queries = useAppSelector(selectQueries);

  useEffect(() => {
    dispatch(getQueries())
      .unwrap()
      .then((q) => {
        if (q.length > 0) {
          message.success(`${q.length} queries found`);
        } else {
          message.info('No queries found');
        }
      })
      .catch((err: BackendError) => message.error(err.message));
  }, []);

  return (
    <>
      <div
        style={{
          marginBottom: 50,
        }}>
        <Title level={4} style={{ marginTop: 0 }}>
          Queries
        </Title>
        <Typography>
          Queries define how data is fetched for your reports.
        </Typography>
        <Typography>
          Queries are defined by the application backend and depend on the data
          source you are using. They are automatically associated with any model
          and can be used to retrieve data for KPIs to evaluate.
        </Typography>
      </div>
      <CustomTable
        dataSource={queries}
        defaultColumns={[
          {
            key: 'name',
            dataIndex: 'name',
            title: 'Name',
          },
          {
            fixed: 'right',
            width: 100,
            title: 'Action',
            dataIndex: 'actions',
            key: 'action',
            render: (_, record) => (
              <Space size="middle">
                <Button type="text" onClick={() => console.log('nav to query')}>
                  Details
                </Button>
              </Space>
            ),
          },
        ]}
      />
    </>
  );
};

export default Queries;
