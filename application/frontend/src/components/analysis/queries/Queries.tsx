import { Button, Popover, Space, Tag, message, theme } from 'antd';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch } from '../../../app/hooks';
import { createQueryFrom } from '../../../features/analysis/analysisSlice';
import CustomTable from '../../table/CustomTable';
import {
  FieldInfo,
  HierachyItem,
  Query,
} from '../../../features/analysis/types';
import { useCallback, useContext, useMemo, useState } from 'react';
import CustomTreeSelect from '../../CustomTreeSelect';
import { ModelContext } from '../../../context/ModelContext';
import handleError from '../../../util/handleError';

const Queries: React.FC = () => {
  const {
    token: { colorPrimary },
  } = theme.useToken();

  const dispatch = useAppDispatch();
  const nav = useNavigate();
  const [createPopoverOpen, setCreatePopoverOpen] = useState(false);

  const { model } = useContext(ModelContext);

  const modelId = useMemo(() => model?.id ?? '', [model]);

  const queries = useMemo(() => {
    return model?.queries ?? [];
  }, [model]);

  const handleCreatePopoverOpenChange = useCallback((newOpen: boolean) => {
    setCreatePopoverOpen(newOpen);
  }, []);

  const onCreateSubmit = useCallback(
    async (selected: HierachyItem<{ name: string }> | undefined) => {
      if (selected) {
        try {
          await dispatch(
            createQueryFrom({
              modelId,
              queryId: selected.id,
            })
          ).unwrap();
        } catch (err) {
          handleError(err);
        } finally {
          setCreatePopoverOpen(false);
        }
      } else {
        await message.error('No query selected!');
      }
    },
    [dispatch, modelId]
  );

  return (
    <>
      <div
        style={{
          width: '100%',
          display: 'flex',
          flexDirection: 'row-reverse',
        }}>
        <Popover
          placement="left"
          content={
            <CustomTreeSelect
              request={`/analysis/queries/${model?.project.id}`}
              title="Select a query"
              labelSelector={(p) => p.name}
              onSubmit={onCreateSubmit}
            />
          }
          title="Create query"
          trigger="click"
          open={createPopoverOpen}
          onOpenChange={handleCreatePopoverOpenChange}>
          <Button
            ghost
            onClick={() => setCreatePopoverOpen(true)}
            type="primary"
            style={{ marginBottom: 16 }}>
            Add a new query
          </Button>
        </Popover>
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
            key: 'select',
            dataIndex: 'select',
            title: 'Returns',
            render(value: FieldInfo[], _, index) {
              const spliceAt = 5;
              const values = [value.slice(0, spliceAt), value.slice(spliceAt)];
              return (
                <>
                  {values[0].map((fV) => (
                    <Tag
                      key={fV.id}
                      color={colorPrimary}>{`${fV.name} (${fV.type})`}</Tag>
                  ))}
                  {values[1].length > 0 && (
                    <Tag key={`show-more-${index}`} color={colorPrimary}>
                      {`+ ${values[1].length}`}
                    </Tag>
                  )}
                </>
              );
            },
          },
          {
            fixed: 'right',
            width: 100,
            title: 'Action',
            dataIndex: 'actions',
            key: 'action',
            render: (_, record) => (
              <Space size="middle">
                <Button
                  type="text"
                  onClick={() =>
                    nav(`/analyze/${modelId}/query/${(record as Query).id}`)
                  }>
                  Details
                </Button>
                <Button danger type="text">
                  Delete
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
