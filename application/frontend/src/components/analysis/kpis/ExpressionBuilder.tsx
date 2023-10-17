import {
  ConditionConnection,
  Expression,
  ExpressionType,
  SomeExpression,
  countIfOperatorsWithLabels,
} from '../../../features/analysis/types';
import {
  Button,
  Card,
  Form,
  Input,
  Select,
  Space,
  Spin,
  Tooltip,
  Typography,
  message,
} from 'antd';
import { useContext, useEffect, useMemo, useState } from 'react';
import {
  BackendError,
  useAppDispatch,
  useAppSelector,
} from '../../../app/hooks';
import {
  selectAllKPIs,
  addOrUpdateExpression,
  removeConditionFromExpression,
} from '../../../features/analysis/analysisSlice';
import { ModelContext } from '../../../context/ModelContext';
import { KPIContext } from '../../../context/KPIContext';
import { useForm, useWatch } from 'antd/es/form/Form';
import { selectQueries } from '../../../features/queries/querySclice';
import { QueryReturnType } from '../../../features/queries/types';
import { CloseOutlined } from '@ant-design/icons';
import { useSelector } from 'react-redux';

const formInputStyles = {
  maxWidth: 200,
};

const descriptions = {
  [ExpressionType.Add]: {
    title: 'Add',
    kind: 'Mathematical',
    description: 'Adds the result of two other KPIs together (Left + Right).',
    arguments: [
      {
        name: 'Left',
        description: 'Left kpi',
      },
      {
        name: 'Right',
        description: 'Right kpi',
      },
    ],
  },
  [ExpressionType.Avg]: {
    title: 'Average',
    kind: 'Aggregation',
    description: `
    Iterates over all list items extracting the given field.
    When all the values are extracted, the mathematical average is calculated 
    `,
    arguments: [
      {
        name: 'Query',
        description: 'Query to get data from',
      },
      {
        name: 'Field',
        description:
          'Field to get data from (or none if its just a number list)',
      },
    ],
  },
  [ExpressionType.Div]: {
    title: 'Divide',
    kind: 'Mathematical',
    description: `
    Divides the result of two other KPIs (Left / Right). 
    Useful for calculating ratios.
    `,
    arguments: [
      {
        name: 'Left',
        description: 'Left kpi',
      },
      {
        name: 'Right',
        description: 'Right kpi',
      },
    ],
  },
  [ExpressionType.Multiply]: {
    title: 'Multiply',
    kind: 'Mathematical',
    description: `
    Multiplies the result of two other KPIs (Left * Right). 
    Useful to scale a value by a certain factor.
    `,
    arguments: [
      {
        name: 'Left',
        description: 'Left kpi',
      },
      {
        name: 'Right',
        description: 'Right kpi',
      },
    ],
  },
  [ExpressionType.Subtract]: {
    title: 'Subtract',
    kind: 'Mathematical',
    description: `
    Subtracts the result of two other KPIs (Left - Right). 
    `,
    arguments: [
      {
        name: 'Left',
        description: 'Left kpi',
      },
      {
        name: 'Right',
        description: 'Right kpi',
      },
    ],
  },
  [ExpressionType.Min]: {
    title: 'Minimal',
    kind: 'Aggregation',
    description: `
    Iterates over all list items extracting the given field.
    When all the values are extracted, the mathematical minimum is calculated 
    `,
    arguments: [
      {
        name: 'Query',
        description: 'Query to get data from',
      },
      {
        name: 'Field',
        description:
          'Field to get data from (or none if its just a number list)',
      },
    ],
  },
  [ExpressionType.Max]: {
    title: 'Maximum',
    kind: 'Aggregation',
    description: `
    Iterates over all work items extracting the given field.
    When all the values are extracted, the mathematical maximum is calculated 
    `,
    arguments: [
      {
        name: 'Query',
        description: 'Query to get data from',
      },
      {
        name: 'Field',
        description:
          'Field to get data from (or none if its just a number list)',
      },
    ],
  },
  [ExpressionType.Sum]: {
    title: 'Sum',
    kind: 'Aggregation',
    description: `
    Iterates over all work items extracting the given field.
    When all the values are extracted, the mathematical sum is calculated 
    `,
    arguments: [
      {
        name: 'Query',
        description: 'Query to get data from',
      },
      {
        name: 'Field',
        description:
          'Field to get data from (or none if its just a number list)',
      },
    ],
  },
  [ExpressionType.Value]: {
    title: 'Simple value',
    kind: 'Atomic',
    description: `
      Expression to define a simple numeric value. Useful for mathematical operations.
    `,
    arguments: [
      {
        name: 'Value',
        description: 'The value',
      },
    ],
  },
  [ExpressionType.CountIfMultiple]: {
    title: 'Count if (multiple)',
    kind: 'Conditional',
    description: `
      Count values in a list if they meet multiple conditions.
    `,
    arguments: [
      {
        name: 'Query',
        description: 'Query to get data from',
      },
      {
        name: 'Extract field',
        description:
          'Extract a field from an object list to count (if the values are the same it only counts as one). Leave empty to just count every entry.',
      },
      {
        name: 'Connection',
        description: 'How the conditions are chained',
      },
      {
        name: 'Conditions',
        description: 'Conditions that are tested',
      },
    ],
  },
  [ExpressionType.SumIfMultiple]: {
    title: 'Sum if (multiple)',
    kind: 'Conditional',
    description: `
      Sum values in a list that they meet multiple conditions.
    `,
    arguments: [
      {
        name: 'Query',
        description: 'Query to get data from',
      },
      {
        name: 'Extract field',
        description: 'Extract a field from an object list to sum.',
      },
      {
        name: 'Connection',
        description: 'How the conditions are chained',
      },
      {
        name: 'Conditions',
        description: 'Conditions that are tested',
      },
    ],
  },
  [ExpressionType.ListIfMultiple]: {
    title: 'List if (multiple)',
    kind: 'Conditional',
    description: `
      List values of a query that they meet multiple conditions.
    `,
    arguments: [
      {
        name: 'Query',
        description: 'Query to get data from',
      },
      {
        name: 'Connection',
        description: 'How the conditions are chained',
      },
      {
        name: 'Conditions',
        description: 'Conditions that are tested',
      },
    ],
  },
  [ExpressionType.CountIf]: {
    title: 'Count if',
    kind: 'Conditional',
    description: `
      Count values in a list if they meet a certain condition.
    `,
    arguments: [
      {
        name: 'Query',
        description: 'Query to get data from',
      },
      {
        name: 'Field',
        description:
          'If the query yields an object list you can specify a field which to extract first',
      },
      {
        name: 'Operator',
        description: 'Operator which is used to compare the values',
      },
      {
        name: 'Compare value',
        description: 'Value to compare each item in the list to',
      },
    ],
  },
  [ExpressionType.Count]: {
    title: 'Count',
    kind: 'Aggregation',
    description: `
      Count values in a list.
    `,
    arguments: [
      {
        name: 'Query',
        description: 'Query to get data from',
      },
    ],
  },
  [ExpressionType.Plain]: {
    title: 'Plain query value',
    kind: 'Atomic',
    description: `
      Just return the plain result of a query.
    `,
    arguments: [
      {
        name: 'Query',
        description: 'Query to get data from',
      },
      {
        name: 'Value',
        description: 'The value',
      },
    ],
  },
};

export const ExpressionBuilder: React.FC = () => {
  const [expression, setExpression] = useState({
    type: ExpressionType.Value,
  });

  const { kpi, loading } = useContext(KPIContext);

  useEffect(() => {
    if (kpi) {
      setExpression(kpi.expression);
    }
  }, [kpi]);

  if (loading) {
    return (
      <div
        style={{
          display: 'flex',
          width: '100%',
          height: '100%',
          justifyContent: 'center',
          alignItems: 'center',
        }}>
        <Spin />
      </div>
    );
  }

  return <ExpressionForm expression={expression as Expression} />;
};

interface ExpressionFormProps {
  expression: Expression;
}

export const ExpressionForm: React.FC<ExpressionFormProps> = ({
  expression,
}) => {
  const [form] = useForm<SomeExpression>();

  const queries = useAppSelector(selectQueries);

  const dispatch = useAppDispatch();

  const { model } = useContext(ModelContext);
  const { kpi } = useContext(KPIContext);

  const modelId = useMemo(() => model?.id ?? '', [model]);
  const kpiId = useMemo(() => kpi?.id ?? '', [kpi]);

  useEffect(() => {
    form.setFieldsValue(expression);
  }, [expression]);

  const kpis = useSelector(selectAllKPIs(modelId));

  const type = useWatch('type', form);
  const selectedQueryId = useWatch('queryId', form);

  const toolTipContent = useMemo(() => {
    if (!type) {
      return {
        title: 'Expression types',
        kind: '',
        description: `
        Get detailed information about an expression type by selecting it
        `,
        arguments: [],
      };
    }
    return descriptions[type];
  }, [type]);

  const allowedQueryTypes = useMemo(() => {
    switch (type) {
      case ExpressionType.Value:
      case ExpressionType.Plain:
        return Object.values(QueryReturnType);
      case ExpressionType.Add:
      case ExpressionType.Subtract:
      case ExpressionType.Multiply:
      case ExpressionType.Div:
        return [QueryReturnType.Number];
      case ExpressionType.Sum:
      case ExpressionType.Avg:
      case ExpressionType.Min:
      case ExpressionType.Max:
      case ExpressionType.CountIf:
      case ExpressionType.Count:
        return [
          QueryReturnType.NumberList,
          QueryReturnType.ObjectList,
          QueryReturnType.StringList,
        ];
      case ExpressionType.CountIfMultiple:
      case ExpressionType.ListIfMultiple:
      case ExpressionType.SumIfMultiple:
        return [QueryReturnType.ObjectList];
      default:
        return [];
    }
  }, [type]);

  const selectedQuery = useMemo(
    () => queries.find((x) => x.id === selectedQueryId),
    [queries, selectedQueryId]
  );

  useEffect(() => console.log(selectedQuery), [selectedQuery]);

  const formFields = useMemo(() => {
    switch (type) {
      case ExpressionType.Value:
        return (
          <Form.Item
            name={['value']}
            label="Value"
            rules={[{ required: true }]}
            getValueFromEvent={(event) => parseFloat(event.target.value)}>
            <Input type="number" style={formInputStyles} />
          </Form.Item>
        );
      case ExpressionType.Add:
      case ExpressionType.Subtract:
      case ExpressionType.Multiply:
      case ExpressionType.Div:
        return (
          <>
            <Form.Item
              name={['leftId']}
              label="Left (KPI)"
              rules={[{ required: true }]}>
              <Select
                showSearch
                filterOption={(input, option) =>
                  option?.label !== undefined && option.label.includes(input)
                }
                options={kpis
                  .filter((x) => x.id !== kpiId)
                  .map((q) => ({
                    label: q.name,
                    value: q.id,
                  }))}
              />
            </Form.Item>
            <Form.Item
              name={['rightId']}
              label="Right (KPI)"
              rules={[{ required: true }]}>
              <Select
                showSearch
                filterOption={(input, option) =>
                  option?.label !== undefined && option.label.includes(input)
                }
                options={kpis
                  .filter((x) => x.id !== kpiId)
                  .map((q) => ({
                    label: q.name,
                    value: q.id,
                  }))}
              />
            </Form.Item>
          </>
        );

      case ExpressionType.Sum:
      case ExpressionType.Avg:
      case ExpressionType.Min:
      case ExpressionType.Max:
        return (
          <>
            <Form.Item
              name={['queryId']}
              label="Query"
              rules={[{ required: true }]}>
              <Select
                options={queries
                  .filter((x) => allowedQueryTypes.includes(x.type))
                  .map((q) => ({
                    label: q.name,
                    value: q.id,
                  }))}
              />
            </Form.Item>
            <Form.Item
              name={['field']}
              label="Field"
              rules={[
                {
                  required: selectedQuery?.type === QueryReturnType.ObjectList,
                },
              ]}>
              <Select
                allowClear
                showSearch
                filterOption={(input, option) =>
                  (option?.label?.toLowerCase() ?? '').includes(
                    input.toLowerCase()
                  )
                }
                options={
                  selectedQuery?.type === QueryReturnType.ObjectList
                    ? selectedQuery.additionalQueryData.possibleFields.map(
                        (x) => ({ label: x.displayName, value: x.name })
                      )
                    : []
                }
              />
            </Form.Item>
          </>
        );
      case ExpressionType.Count:
        return (
          <Form.Item
            name={['queryId']}
            label="Query"
            rules={[{ required: true }]}>
            <Select
              options={queries
                .filter((x) => allowedQueryTypes.includes(x.type))
                .map((q) => ({
                  label: q.name,
                  value: q.id,
                }))}
            />
          </Form.Item>
        );
      case ExpressionType.Plain:
        return (
          <Form.Item
            name={['queryId']}
            label="Query"
            rules={[{ required: true }]}>
            <Select
              options={queries.map((q) => ({
                label: q.name,
                value: q.id,
              }))}
            />
          </Form.Item>
        );
      case ExpressionType.CountIf:
        return (
          <>
            <Form.Item
              name={['queryId']}
              label="Query"
              rules={[{ required: true }]}>
              <Select
                options={queries
                  .filter((x) => allowedQueryTypes.includes(x.type))
                  .map((q) => ({
                    label: q.name,
                    value: q.id,
                  }))}
              />
            </Form.Item>
            <Form.Item
              name={['field']}
              label="Field"
              rules={[
                {
                  required: selectedQuery?.type === QueryReturnType.ObjectList,
                },
              ]}>
              <Select
                allowClear
                showSearch
                filterOption={(input, option) =>
                  (option?.label?.toLowerCase() ?? '').includes(
                    input.toLowerCase()
                  )
                }
                disabled={selectedQuery?.type !== QueryReturnType.ObjectList}
                options={
                  selectedQuery?.type === QueryReturnType.ObjectList
                    ? selectedQuery.additionalQueryData.possibleFields.map(
                        (x) => ({ label: x.displayName, value: x.name })
                      )
                    : []
                }
              />
            </Form.Item>
            <Form.Item name={['operator']} label="Operator">
              <Select
                options={countIfOperatorsWithLabels
                  .filter((o) =>
                    selectedQuery
                      ? o.allowed.includes(selectedQuery.type)
                      : false
                  )
                  .map((o) => ({
                    label: o.label,
                    value: o.value,
                  }))}
              />
            </Form.Item>
            <Form.Item name={['compareValue']} label="Comparevalue">
              <Input style={formInputStyles} />
            </Form.Item>
          </>
        );

      case ExpressionType.CountIfMultiple:
      case ExpressionType.SumIfMultiple:
        return (
          <>
            <Form.Item
              name={['queryId']}
              label="Query"
              rules={[{ required: true }]}>
              <Select
                options={queries
                  .filter((x) => allowedQueryTypes.includes(x.type))
                  .map((q) => ({
                    label: q.name,
                    value: q.id,
                  }))}
              />
            </Form.Item>
            <Form.Item name={['extractField']} label="Extract field">
              <Select
                allowClear
                showSearch
                filterOption={(input, option) =>
                  (option?.label?.toLowerCase() ?? '').includes(
                    input.toLowerCase()
                  )
                }
                disabled={selectedQuery?.type !== QueryReturnType.ObjectList}
                options={
                  selectedQuery?.type === QueryReturnType.ObjectList
                    ? selectedQuery.additionalQueryData.possibleFields.map(
                        (x) => ({ label: x.displayName, value: x.name })
                      )
                    : []
                }
              />
            </Form.Item>
            <Form.Item
              name={['connection']}
              label="Connection"
              rules={[{ required: true }]}>
              <Select
                options={[
                  {
                    label: 'All',
                    value: ConditionConnection.All,
                  },
                  {
                    label: 'Any',
                    value: ConditionConnection.Any,
                  },
                ]}
              />
            </Form.Item>
            <Form.Item label="Conditions">
              <Form.List name={['conditions']}>
                {(fields, { add, remove }) => (
                  <div
                    style={{
                      display: 'flex',
                      rowGap: 16,
                      flexDirection: 'column',
                    }}>
                    {fields.map((field) => (
                      <Card
                        size="small"
                        title={`Condition ${field.name + 1}`}
                        key={field.key}
                        extra={
                          <CloseOutlined
                            onClick={() => {
                              const val = form.getFieldValue([
                                'conditions',
                                field.name,
                              ]);
                              if (!val) {
                                remove(field.key);
                              } else {
                                dispatch(
                                  removeConditionFromExpression({
                                    modelId,
                                    kpiId,
                                    conditionId: val.id,
                                  })
                                )
                                  .unwrap()
                                  .then(() => remove(field.key))
                                  .catch((err: BackendError) =>
                                    message.error(err.message)
                                  );
                              }
                            }}
                          />
                        }>
                        <Form.Item
                          name={[field.name, 'field']}
                          label="Field"
                          rules={[
                            {
                              required:
                                selectedQuery?.type ===
                                QueryReturnType.ObjectList,
                            },
                          ]}>
                          <Select
                            allowClear
                            showSearch
                            filterOption={(input, option) =>
                              (option?.label?.toLowerCase() ?? '').includes(
                                input.toLowerCase()
                              )
                            }
                            disabled={
                              selectedQuery?.type !== QueryReturnType.ObjectList
                            }
                            options={
                              selectedQuery?.type === QueryReturnType.ObjectList
                                ? selectedQuery.additionalQueryData.possibleFields.map(
                                    (x) => ({
                                      label: x.displayName,
                                      value: x.name,
                                    })
                                  )
                                : []
                            }
                          />
                        </Form.Item>
                        <Form.Item
                          name={[field.name, 'operator']}
                          label="Operator">
                          <Select
                            options={countIfOperatorsWithLabels
                              .filter((o) =>
                                selectedQuery
                                  ? o.allowed.includes(selectedQuery.type)
                                  : false
                              )
                              .map((o) => ({
                                label: o.label,
                                value: o.value,
                              }))}
                          />
                        </Form.Item>
                        <Form.Item
                          name={[field.name, 'compareValue']}
                          label="Comparevalue">
                          <Input style={formInputStyles} />
                        </Form.Item>
                      </Card>
                    ))}

                    <Button type="dashed" onClick={() => add()} block>
                      + Add condition
                    </Button>
                  </div>
                )}
              </Form.List>
            </Form.Item>
          </>
        );
      case ExpressionType.ListIfMultiple:
        return (
          <>
            <Form.Item
              name={['queryId']}
              label="Query"
              rules={[{ required: true }]}>
              <Select
                options={queries
                  .filter((x) => allowedQueryTypes.includes(x.type))
                  .map((q) => ({
                    label: q.name,
                    value: q.id,
                  }))}
              />
            </Form.Item>
            <Form.Item
              name={['connection']}
              label="Connection"
              rules={[{ required: true }]}>
              <Select
                options={[
                  {
                    label: 'All',
                    value: ConditionConnection.All,
                  },
                  {
                    label: 'Any',
                    value: ConditionConnection.Any,
                  },
                ]}
              />
            </Form.Item>
            <Form.Item label="Conditions">
              <Form.List name={['conditions']}>
                {(fields, { add, remove }) => (
                  <div
                    style={{
                      display: 'flex',
                      rowGap: 16,
                      flexDirection: 'column',
                    }}>
                    {fields.map((field) => (
                      <Card
                        size="small"
                        title={`Condition ${field.name + 1}`}
                        key={field.key}
                        extra={
                          <CloseOutlined
                            onClick={() => {
                              const val = form.getFieldValue([
                                'conditions',
                                field.name,
                              ]);
                              if (!val) {
                                remove(field.key);
                              } else {
                                dispatch(
                                  removeConditionFromExpression({
                                    modelId,
                                    kpiId,
                                    conditionId: val.id,
                                  })
                                )
                                  .unwrap()
                                  .then(() => remove(field.key))
                                  .catch((err: BackendError) =>
                                    message.error(err.message)
                                  );
                              }
                            }}
                          />
                        }>
                        <Form.Item
                          name={[field.name, 'field']}
                          label="Field"
                          rules={[
                            {
                              required:
                                selectedQuery?.type ===
                                QueryReturnType.ObjectList,
                            },
                          ]}>
                          <Select
                            allowClear
                            showSearch
                            filterOption={(input, option) =>
                              (option?.label?.toLowerCase() ?? '').includes(
                                input.toLowerCase()
                              )
                            }
                            disabled={
                              selectedQuery?.type !== QueryReturnType.ObjectList
                            }
                            options={
                              selectedQuery?.type === QueryReturnType.ObjectList
                                ? selectedQuery.additionalQueryData.possibleFields.map(
                                    (x) => ({
                                      label: x.displayName,
                                      value: x.name,
                                    })
                                  )
                                : []
                            }
                          />
                        </Form.Item>
                        <Form.Item
                          name={[field.name, 'operator']}
                          label="Operator">
                          <Select
                            options={countIfOperatorsWithLabels
                              .filter((o) =>
                                selectedQuery
                                  ? o.allowed.includes(selectedQuery.type)
                                  : false
                              )
                              .map((o) => ({
                                label: o.label,
                                value: o.value,
                              }))}
                          />
                        </Form.Item>
                        <Form.Item
                          name={[field.name, 'compareValue']}
                          label="Comparevalue">
                          <Input style={formInputStyles} />
                        </Form.Item>
                      </Card>
                    ))}

                    <Button type="dashed" onClick={() => add()} block>
                      + Add condition
                    </Button>
                  </div>
                )}
              </Form.List>
            </Form.Item>
          </>
        );
      default:
        return null;
    }
  }, [type, allowedQueryTypes, selectedQuery]);

  return (
    <>
      <Form
        initialValues={expression}
        labelCol={{ span: 3 }}
        wrapperCol={{ span: 12 }}
        style={{
          textAlign: 'left',
        }}
        onFinish={(values) => {
          dispatch(
            addOrUpdateExpression({
              kpiId,
              modelId,
              expression: { ...values, id: expression.id } as Expression,
            })
          )
            .unwrap()
            .then(() => message.success(`Updated expression`))
            .catch((err: BackendError) => message.error(err.message));
        }}
        form={form}>
        <Form.Item label="Expression Type">
          <Space>
            <Form.Item noStyle name={['type']} rules={[{ required: true }]}>
              <Select
                style={{ minWidth: 200 }}
                options={[
                  {
                    label: 'Atomic',
                    options: [
                      {
                        label: 'Simple value',
                        value: ExpressionType.Value,
                      },
                      {
                        label: 'Plain query',
                        value: ExpressionType.Plain,
                      },
                    ],
                  },
                  {
                    label: 'Mathematical',
                    options: [
                      {
                        label: 'Add',
                        value: ExpressionType.Add,
                      },
                      {
                        label: 'Subtract',
                        value: ExpressionType.Subtract,
                      },
                      {
                        label: 'Divide',
                        value: ExpressionType.Div,
                      },
                      {
                        label: 'Multiply',
                        value: ExpressionType.Multiply,
                      },
                    ],
                  },
                  {
                    label: 'Aggregation',
                    options: [
                      {
                        label: 'Sum',
                        value: ExpressionType.Sum,
                      },
                      {
                        label: 'Average',
                        value: ExpressionType.Avg,
                      },
                      {
                        label: 'Minimum',
                        value: ExpressionType.Min,
                      },
                      {
                        label: 'Maximum',
                        value: ExpressionType.Max,
                      },
                      {
                        label: 'Count',
                        value: ExpressionType.Count,
                      },
                    ],
                  },
                  {
                    label: 'Conditional',
                    options: [
                      {
                        label: 'Count if',
                        value: ExpressionType.CountIf,
                      },
                      {
                        label: 'Count if (multiple)',
                        value: ExpressionType.CountIfMultiple,
                      },
                      {
                        label: 'Sum if (multiple)',
                        value: ExpressionType.SumIfMultiple,
                      },
                      {
                        label: 'List if (multiple)',
                        value: ExpressionType.ListIfMultiple,
                      },
                    ],
                  },
                ]}
              />
            </Form.Item>
            <Tooltip
              placement="right"
              title={
                <>
                  <Typography.Title
                    style={{
                      fontSize: 'x-large',
                    }}
                    level={1}>
                    {toolTipContent.title} ({toolTipContent.kind})
                  </Typography.Title>
                  <Typography.Text>
                    {toolTipContent.description}
                  </Typography.Text>
                  <Typography.Title
                    style={{
                      fontSize: 'large',
                    }}
                    level={2}>
                    Arguments
                  </Typography.Title>
                  {toolTipContent.arguments.map((arg) => (
                    <>
                      <Typography.Title
                        level={3}
                        style={{ fontSize: 'medium' }}>
                        {arg.name}
                      </Typography.Title>
                      <Typography.Text>{arg.description}</Typography.Text>
                    </>
                  ))}
                </>
              }>
              <Typography.Link>Need help?</Typography.Link>
            </Tooltip>
          </Space>
        </Form.Item>
        {formFields}
        <Button onClick={() => form.submit()}>Save</Button>
      </Form>
    </>
  );
};
