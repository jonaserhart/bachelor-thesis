import { Button, Empty, Tag, theme } from "antd";
import * as React from "react";
import { useParams } from "react-router-dom";
import { useAppSelector } from "../../../app/hooks";
import { selectModel } from "../../../features/analysis/analysisSlice";
import CustomTable from "../../table/EditableTable";
import { FieldInfo } from "../../../features/analysis/types";
import CreateQueryModal from "./CreateQueryModal";

export default function Queries() {

  const params = useParams();

  const [modalOpen , setModalOpen] = React.useState(false);

  const {
    token: { colorPrimary },
  } = theme.useToken();

  const modelId = React.useMemo(() => {
    return params.modelId ?? "";
  }, [params]);

  const model = useAppSelector(selectModel(modelId));

  const queries = React.useMemo(() => {
    return model?.queries ?? [];
  }, [model]);

  return (
    <>
      {queries.length <= 0 ? (
        <Empty
          image={Empty.PRESENTED_IMAGE_DEFAULT}
          imageStyle={{ height: 60 }}
          description={<span>No queries found for model</span>}
        >
          <Button
            onClick={() => setModalOpen(true)}
            ghost
            type="primary"
          >
            Create a query
          </Button>
        </Empty>
      ) : (
        <CustomTable
          dataSource={queries}
          defaultColumns={[
            {
              key: "name",
              dataIndex: "name",
              title: "Name",
            },
            {
              key: "fields",
              dataIndex: "fieldInfos",
              title: "Fields",
              render(value: FieldInfo[]) {
                return value.map((fV) => (
                  <Tag color={colorPrimary} title={`${fV.name} (${fV.type})`} />
                ));
              },
            },
          ]}
        />
      )}
        <CreateQueryModal handleClose={() => setModalOpen(false)} open={modalOpen}/>
    </>
  );
}
