import { useContext, useMemo } from 'react';
import { ModelContext } from '../../../context/ModelContext';
import { Button, Space, message } from 'antd';
import CustomTable from '../../table/CustomTable';
import { formatDate } from '../../../util/formatDate';
import { Report } from '../../../features/analysis/types';
import { useNavigate } from 'react-router-dom';
import { BackendError, useAppDispatch } from '../../../app/hooks';
import { deleteReport } from '../../../features/analysis/analysisSlice';

const Reports: React.FC = () => {
  const { model } = useContext(ModelContext);

  const modelId = useMemo(() => model?.id ?? '', [model]);

  const reports = useMemo(() => model?.reports ?? [], [model]);

  const dispatch = useAppDispatch();

  const nav = useNavigate();

  return (
    <>
      <div
        style={{
          width: '100%',
          display: 'flex',
          flexDirection: 'row-reverse',
        }}>
        <Button
          ghost
          type="primary"
          style={{ marginBottom: 16 }}
          onClick={() => nav(`/analyze/${modelId}/report/create`)}>
          Create report
        </Button>
      </div>
      <CustomTable
        dataSource={reports}
        defaultColumns={[
          {
            key: 'title',
            dataIndex: 'title',
            title: 'Title',
            searchable: true,
          },
          {
            key: 'created',
            dataIndex: 'created',
            title: 'Created',
            render(value) {
              return formatDate(value);
            },
            sorter: {
              compare: (a, b) => (a as Report).created - (b as Report).created,
            },
          },
          {
            fixed: 'right',
            width: 100,
            title: 'Action',
            dataIndex: 'actions',
            key: 'action',
            render: (_, record) => {
              const report = record as Report;
              return (
                <Space size="middle">
                  <Button
                    type="text"
                    // onClick={() => nav(`/analyze/${modelId}/report/${report.id}`)}
                  >
                    Details
                  </Button>
                  <Button
                    danger
                    type="text"
                    onClick={() => {
                      dispatch(
                        deleteReport({
                          modelId,
                          reportId: report.id,
                        })
                      )
                        .unwrap()
                        .then(
                          () =>
                            void message.success(
                              `Successfully deleted report ${report.title}`
                            )
                        )
                        .catch(
                          (err: BackendError) => void message.error(err.message)
                        );
                    }}>
                    Delete
                  </Button>
                </Space>
              );
            },
          },
        ]}
      />
    </>
  );
};

export default Reports;
