import * as React from "react";
import { useParams } from "react-router-dom";
import { Spin, Typography, message, theme, Tabs } from "antd";
import {
  CloudServerOutlined,
  EditOutlined,
  FileDoneOutlined,
} from "@ant-design/icons";
import { useAppDispatch, useAppSelector } from "../../app/hooks";
import {
  getModelDetails,
  selectModel,
  updateModelDetails,
} from "../../features/analysis/analysisSlice";
import Queries from "./queries/Queries";

const { Title } = Typography;

export default function ModelDetail() {
  const params = useParams();

  const modelId = React.useMemo(() => {
    return params.modelId ?? "";
  }, [params]);

  const [loading, setLoading] = React.useState(false);

  const model = useAppSelector(selectModel(modelId));

  const dispatch = useAppDispatch();

  const {
    token: { colorPrimary },
  } = theme.useToken();

  React.useEffect(() => {
    if (!model && modelId) {
      setLoading(true);
      dispatch(getModelDetails(modelId))
        .unwrap()
        .catch((err) => message.error(err.error))
        .finally(() => setLoading(false));
    }
  }, [model, modelId, dispatch]);

  const onNameChange = React.useCallback(
    (strVal: string) => {
      const newVal = strVal.trim();
      if (!model) return;
      if (model.name !== newVal) {
        dispatch(
          updateModelDetails({
            id: model.id,
            name: newVal,
          })
        );
      }
    },
    [model, dispatch]
  );

  return (
    <Spin spinning={loading}>
      <div>
        <Title
          editable={{
            onChange: onNameChange,
            icon: <EditOutlined style={{ color: colorPrimary }} />,
            tooltip: "click to edit name",
          }}
          level={4}
          style={{ marginTop: 0 }}
        >
          {model?.name}
        </Title>
        <Tabs
          items={[
            {
              key: "1",
              label: (
                <span>
                  <CloudServerOutlined />
                  Queries
                </span>
              ),
              children: <Queries />,
            },
            {
              key: "2",
              label: (
                <span>
                  <FileDoneOutlined />
                  Latest Reports
                </span>
              ),
            },
          ]}
        />
      </div>
    </Spin>
  );
}
