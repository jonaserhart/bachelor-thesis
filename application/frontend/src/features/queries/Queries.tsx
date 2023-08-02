import { Button, Space, message } from 'antd';
import { BackendError, useAppDispatch, useAppSelector } from '../../app/hooks';
import CustomTable from '../../components/table/CustomTable';
import { useEffect } from 'react';
import { getQueries, selectQueries } from './querySclice';

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
