import { useCallback, useContext, useEffect, useMemo, useState } from "react";
import { useParams } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../../app/hooks";
import {
  getQueryDetails,
  selectQuery,
  updateQueryDetails,
} from "../../../features/analysis/analysisSlice";
import { Spin, Typography, message, theme } from "antd";
import { EditOutlined } from "@ant-design/icons";
import { QueryContext } from "../../../context/QueryContext";
import { ModelContext } from "../../../context/ModelContext";

const { Title } = Typography;

const QueryDetail: React.FC = () => {
  const {
    token: { colorPrimary },
  } = theme.useToken();

  const { loading, query } = useContext(QueryContext);
  const { model } = useContext(ModelContext);

  const dispatch = useAppDispatch();

  const onNameChange = useCallback(
    (strVal: string) => {
      const newVal = strVal.trim();
      if (!query || !model) return;
      if (query.name !== newVal) {
        dispatch(
          updateQueryDetails({
            changeQuery: {
              id: query.id,
              name: newVal,
            },
            modelId: model.id,
          })
        );
      }
    },
    [query, model, dispatch]
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
          {query?.name}
        </Title>
      </div>
    </Spin>
  );
};

export default QueryDetail;
