import { useCallback, useContext, useMemo, useState } from 'react';
import { BackendError, useAppDispatch } from '../../../app/hooks';
import { updateQueryDetails } from '../../../features/analysis/analysisSlice';
import {
  Divider,
  Empty,
  Space,
  Spin,
  Tag,
  Tooltip,
  Tree,
  Typography,
  message,
  theme,
} from 'antd';
import { DownOutlined, EditOutlined } from '@ant-design/icons';
import { QueryContext } from '../../../context/QueryContext';
import { ModelContext } from '../../../context/ModelContext';
import { Clause, LogicalOperator } from '../../../features/analysis/types';
import { DataNode } from 'antd/es/tree';

const { Title } = Typography;

const QueryDetail: React.FC = () => {
  const {
    token: { colorPrimary },
  } = theme.useToken();

  const { loading, query } = useContext(QueryContext);
  const { model } = useContext(ModelContext);

  const [nameLoading, setNameLoading] = useState(false);

  const dispatch = useAppDispatch();

  const onNameChange = useCallback(
    (strVal: string) => {
      const newVal = strVal.trim();
      if (!query || !model) return;
      if (query.name !== newVal) {
        setNameLoading(true);
        dispatch(
          updateQueryDetails({
            changeQuery: {
              id: query.id,
              name: newVal,
            },
            modelId: model.id,
          })
        )
          .unwrap()
          .catch((err: BackendError) => message.error(err.message))
          .finally(() => setNameLoading(false));
      }
    },
    [query, model, dispatch]
  );

  const getClauseString = useCallback((clause: Clause) => {
    if (clause.logicalOperator !== LogicalOperator.None) {
      return clause.logicalOperator;
    } else if (clause.isFieldValue) {
      return `${clause.field} ${clause.operator.name} ${
        clause.fieldValue ?? 'Any'
      }`;
    } else {
      return `${clause.field} ${clause.operator.name} ${
        clause.value === '' ? 'Any' : clause.value
      }`;
    }
  }, []);

  const mapClauseToDataNode: (clause: Clause) => DataNode = useCallback(
    (clause: Clause) => ({
      key: clause.id,
      title: <Typography>{getClauseString(clause)}</Typography>,
      children: clause.clauses.map(mapClauseToDataNode),
    }),
    [getClauseString]
  );

  const treeData = useMemo(() => {
    if (!query?.where) {
      return [];
    }
    const data: DataNode[] = [query.where].map(mapClauseToDataNode);
    return data;
  }, [query, mapClauseToDataNode]);

  return (
    <Spin spinning={loading}>
      <div>
        <Title
          editable={{
            onChange: onNameChange,
            icon: nameLoading ? (
              <Spin />
            ) : (
              <EditOutlined style={{ color: colorPrimary }} />
            ),
            tooltip: 'click to edit name',
          }}
          level={4}
          style={{ marginTop: 0 }}>
          {query?.name}
        </Title>
        <div>
          <Divider orientationMargin={0} orientation="left">
            Fields
          </Divider>
          <Space size={[0, 8]} wrap>
            {query?.select.map((field) => (
              <Tooltip key={field.id} title={field.referenceName}>
                <Tag color={colorPrimary}>
                  <Typography>{`${field.name} (${field.type})`}</Typography>
                </Tag>
              </Tooltip>
            ))}
          </Space>
          <Divider orientationMargin={0} orientation="left">
            Clauses
          </Divider>
          {query?.where ? (
            <Tree
              showLine
              treeData={treeData}
              defaultExpandAll
              switcherIcon={<DownOutlined style={{ color: colorPrimary }} />}
            />
          ) : (
            <Empty />
          )}
        </div>
      </div>
    </Spin>
  );
};

export default QueryDetail;
