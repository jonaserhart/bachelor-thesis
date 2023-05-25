import { Button, Popover, Space, Tag, message, theme } from "antd";
import { useNavigate, useParams } from "react-router-dom";
import {
  BackendError,
  useAppDispatch,
  useAppSelector,
} from "../../../app/hooks";
import {
  createQueryFrom,
  selectModel,
} from "../../../features/analysis/analysisSlice";
import CustomTable from "../../table/CustomTable";
import {
  FieldInfo,
  HierachyItem,
  Query,
} from "../../../features/analysis/types";
import { useCallback, useContext, useMemo, useState } from "react";
import CustomTreeSelect from "../../CustomTreeSelect";
import { ModelContext } from "../../../context/ModelContext";

const Queries: React.FC = () => {
  const params = useParams();

  const {
    token: { colorPrimary },
  } = theme.useToken();

  const dispatch = useAppDispatch();
  const nav = useNavigate();
  const [createPopoverOpen, setCreatePopoverOpen] = useState(false);
  const [createQueryLoading, setCreateQueryLoading] = useState(false);

  const { model } = useContext(ModelContext);

  const modelId = useMemo(() => model?.id ?? "", [model]);

  const queries = useMemo(() => {
    return model?.queries ?? [];
  }, [model]);

  const handleCreatePopoverOpenChange = useCallback((newOpen: boolean) => {
    setCreatePopoverOpen(newOpen);
  }, []);

  const onCreateSubmit = useCallback(
    (selected: HierachyItem<{ name: string }> | undefined) => {
      if (selected) {
        setCreateQueryLoading(true);
        dispatch(
          createQueryFrom({
            modelId,
            queryId: selected.id,
          })
        )
          .unwrap()
          .catch((err: BackendError) => message.error(err.message))
          .finally(() => {
            setCreatePopoverOpen(false);
            setCreateQueryLoading(false);
          });
      } else {
        message.error("No query selected!");
      }
    },
    [dispatch, modelId]
  );

  return (
    <>
      <div
        style={{
          width: "100%",
          display: "flex",
          flexDirection: "row-reverse",
        }}
      >
        <Popover
          placement="left"
          content={
            <CustomTreeSelect
              request={`/devops/queries/${model?.project.id}`}
              title="Select a query"
              labelSelector={(p) => p.name}
              onSubmit={onCreateSubmit}
              loading={createQueryLoading}
            />
          }
          title="Create query"
          trigger="click"
          open={createPopoverOpen}
          onOpenChange={handleCreatePopoverOpenChange}
        >
          <Button
            ghost
            onClick={() => setCreatePopoverOpen(true)}
            type="primary"
            style={{ marginBottom: 16 }}
          >
            Add a new query
          </Button>
        </Popover>
      </div>
      <CustomTable
        dataSource={queries}
        defaultColumns={[
          {
            key: "name",
            dataIndex: "name",
            title: "Name",
          },
          {
            key: "select",
            dataIndex: "select",
            title: "Fields",
            render(value: FieldInfo[], _, index) {
              const spliceAt = 5;
              const values = [value.slice(0, spliceAt), value.slice(spliceAt)];
              return (
                <>
                  {values[0].map((fV) => (
                    <Tag
                      key={fV.id}
                      color={colorPrimary}
                    >{`${fV.name} (${fV.type})`}</Tag>
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
            fixed: "right",
            width: 100,
            title: "Action",
            dataIndex: "actions",
            key: "action",
            render: (_, record) => (
              <Space size="middle">
                <Button
                  type="text"
                  onClick={() =>
                    nav(`/analyze/${modelId}/query/${(record as Query).id}`)
                  }
                >
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
