import { useForm, useWatch } from 'antd/es/form/Form';
import {
  Expression,
  ExpressionType,
  MathOperationExpression,
} from '../../../features/analysis/types';
import { Button, Form, Input, Select, Space, Tooltip, Typography } from 'antd';
import { useCallback, useEffect, useMemo, useState } from 'react';

interface Props {
  onExpressionSubmit: (e: Expression) => Promise<void>;
}

interface FormExpressionType {
  type: ExpressionType;
  // aggregate expression
  fieldExpression?: {
    field: string;
    query: string;
  };
  // conditional
  condition?: {
    field: string;
    operation: string;
    value: string;
  };
  // value
  value?: number;
}

interface FormType extends FormExpressionType {
  // Math expression
  left?: FormExpressionType;
  right?: FormExpressionType;
}

const descriptions = {
  [ExpressionType.Add]: {
    title: 'Add',
    kind: 'Mathematical',
    description: 'Adds the result of two expressions together (Left + Right).',
    arguments: [
      {
        name: 'Left',
        description: 'Left expression',
      },
      {
        name: 'Right',
        description: 'Right expression',
      },
    ],
  },
  [ExpressionType.Avg]: {
    title: 'Average',
    kind: 'Aggregation',
    description: `
    Iterates over all work items extracting the given field.
    When all the values are extracted, the mathematical average is calculated 
    `,
    arguments: [
      {
        name: 'FieldExpression',
        description: 'Field to get data from',
      },
    ],
  },
  [ExpressionType.CountIf]: {
    title: 'Count if',
    kind: 'Conditional',
    description: `
    Counts every work item that fulfills a certain condition
    `,
    arguments: [
      {
        name: 'Field',
        description: 'Field to compare to value',
      },
      {
        name: 'Operand',
        description: 'How the field and the value are compared',
      },
      {
        name: 'Value',
        description: 'Value to compare the field value to',
      },
    ],
  },
  [ExpressionType.Div]: {
    title: 'Divide',
    kind: 'Mathematical',
    description: `
    Divides the result of two child expressions (Left / Right). 
    Useful for calculating ratios.
    `,
    arguments: [
      {
        name: 'Left',
        description: 'Left expression',
      },
      {
        name: 'Right',
        description: 'Right expression',
      },
    ],
  },
  [ExpressionType.Multiply]: {
    title: 'Multiply',
    kind: 'Mathematical',
    description: `
    Multiplies the result of two child expressions (Left / Right). 
    Useful to scale a value by a certain factor.
    `,
    arguments: [
      {
        name: 'Left',
        description: 'Left expression',
      },
      {
        name: 'Right',
        description: 'Right expression',
      },
    ],
  },
  [ExpressionType.Subtract]: {
    title: 'Subtract',
    kind: 'Mathematical',
    description: `
    Subtracts the result of two child expressions (Left - Right). 
    `,
    arguments: [
      {
        name: 'Left',
        description: 'Left expression',
      },
      {
        name: 'Right',
        description: 'Right expression',
      },
    ],
  },
  [ExpressionType.Min]: {
    title: 'Minimal',
    kind: 'Aggregation',
    description: `
    Iterates over all work items extracting the given field.
    When all the values are extracted, the mathematical minimum is calculated 
    `,
    arguments: [
      {
        name: 'FieldExpression',
        description: 'Field to get data from',
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
        name: 'FieldExpression',
        description: 'Field to get data from',
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
        name: 'FieldExpression',
        description: 'Field to get data from',
      },
    ],
  },
  [ExpressionType.Field]: {
    title: 'Field Value',
    kind: 'Atomic',
    description: `
      Expression to tell the evaluation logic which field to extract from a work item.
    `,
    arguments: [
      {
        name: 'Field',
        description: 'Field to get data from',
      },
      {
        name: 'Query',
        description:
          'Which query the field is from (there might be the same field names across queries.',
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
};

export const ExpressionBuilder: React.FC = () => {
  const [expression, setExpression] = useState<Expression>({
    id: 'abc',
    type: ExpressionType.Value,
  });

  const handleExpressionChange = (updatedExpression: Expression) => {
    setExpression(updatedExpression);
    console.log(updatedExpression);
  };

  const handleSubmit = () => {
    // Handle form submission with the final expression
    console.log(expression);
  };

  return (
    <div>
      <ExpressionForm
        expression={expression}
        onChange={handleExpressionChange}
      />
      <Button onClick={handleSubmit}>Submit</Button>
    </div>
  );
};

interface ExpressionFormProps {
  expression: Expression;
  onChange: (expression: Expression) => void;
}

export const ExpressionForm: React.FC<ExpressionFormProps> = ({
  expression,
  onChange,
}) => {
  const handleExpressionChange = (value: ExpressionType) => {
    const updatedExpression = { type: value };
    onChange(updatedExpression as Expression);
  };

  const handleNestedExpressionChange = (changedValues: any) => {
    const updatedExpression: Expression = { ...expression, ...changedValues };
    onChange(updatedExpression);
  };

  const toolTipContent = useMemo(() => {
    if (!expression?.type) {
      return {
        title: 'Expression types',
        kind: '',
        description: `
      Get detailed information about an expression type by selecting it
    `,
        arguments: [],
      };
    }
    return descriptions[expression.type];
  }, [expression]);

  const renderFormFields = () => {
    switch (expression?.type) {
      case ExpressionType.Value:
        return (
          <Form.Item
            name={['value']}
            label="Value"
            rules={[{ required: true }]}
            getValueFromEvent={(event) => parseFloat(event.target.value)}>
            <Input type="number" />
          </Form.Item>
        );
      case ExpressionType.Add:
      case ExpressionType.Subtract:
      case ExpressionType.Multiply:
      case ExpressionType.Div:
        return (
          <>
            <Form.Item
              name={['left']}
              label="Left Expression"
              rules={[{ required: true }]}>
              <ExpressionForm
                expression={(expression as MathOperationExpression<any>)?.left}
                onChange={(value) =>
                  handleNestedExpressionChange({ left: value })
                }
              />
            </Form.Item>
            <Form.Item
              name={['right']}
              label="Right Expression"
              rules={[{ required: true }]}>
              <ExpressionForm
                expression={(expression as MathOperationExpression<any>)?.right}
                onChange={(value) =>
                  handleNestedExpressionChange({ right: value })
                }
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
              name={['fieldExpression', 'queryId']}
              label="Query ID"
              rules={[{ required: true }]}>
              <Input />
            </Form.Item>
            <Form.Item
              name={['fieldExpression', 'field']}
              label="Field"
              rules={[{ required: true }]}>
              <Input />
            </Form.Item>
          </>
        );
      case ExpressionType.CountIf:
        return (
          <>
            <Form.Item
              name={['field']}
              label="Field"
              rules={[{ required: true }]}>
              <Input />
            </Form.Item>
            <Form.Item
              name={['operator']}
              label="Operator"
              rules={[{ required: true }]}>
              <Input />
            </Form.Item>
            <Form.Item
              name={['value']}
              label="Value"
              rules={[{ required: true }]}
              getValueFromEvent={(event) => event.target.value}>
              <Input />
            </Form.Item>
          </>
        );
      default:
        return null;
    }
  };

  return (
    <Form
      initialValues={expression}
      wrapperCol={{ span: 24 }}
      onValuesChange={handleNestedExpressionChange}>
      <Form.Item label="Expression Type">
        <Space>
          <Form.Item noStyle name={['type']} rules={[{ required: true }]}>
            <Select
              style={{ minWidth: 200 }}
              onChange={handleExpressionChange}
              options={[
                {
                  label: 'Atomic',
                  options: [
                    {
                      label: 'Simple value',
                      value: ExpressionType.Value,
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
                  ],
                },
                {
                  label: 'Conditional',
                  options: [
                    {
                      label: 'Count if',
                      value: ExpressionType.CountIf,
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
                <Typography.Text>{toolTipContent.description}</Typography.Text>
                <Typography.Title
                  style={{
                    fontSize: 'large',
                  }}
                  level={2}>
                  Arguments
                </Typography.Title>
                {toolTipContent.arguments.map((arg) => (
                  <>
                    <Typography.Title level={3} style={{ fontSize: 'medium' }}>
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
      {renderFormFields()}
    </Form>
  );
};

// const ExpressionBuilder: React.FC<Props> = (props) => {
//   const { onExpressionSubmit } = props;

//   const [form] = useForm<FormType>();

//   const expressionType = useWatch('type', form);

//   const fields = form.getFieldsValue();
//   console.log(fields);

//   const expressionForm = (prefix: string[]) => (
//     <>
//       <Form.Item label="Expression type">
//         <Space>
//           <Form.Item
//             name={[...prefix, 'type']}
//             noStyle
//             rules={[{ required: true, message: 'Please select a type' }]}>
//             <Select
//               style={{
//                 minWidth: 200,
//               }}
//               options={[
//                 {
//                   label: 'Atomic',
//                   options: [
//                     {
//                       label: 'Simple value',
//                       value: ExpressionType.Value,
//                     },
//                   ],
//                 },
//                 {
//                   label: 'Mathematical',
//                   options: [
//                     {
//                       label: 'Add',
//                       value: ExpressionType.Add,
//                     },
//                     {
//                       label: 'Subtract',
//                       value: ExpressionType.Subtract,
//                     },
//                     {
//                       label: 'Divide',
//                       value: ExpressionType.Div,
//                     },
//                     {
//                       label: 'Multiply',
//                       value: ExpressionType.Multiply,
//                     },
//                   ],
//                 },
//                 {
//                   label: 'Aggregation',
//                   options: [
//                     {
//                       label: 'Sum',
//                       value: ExpressionType.Sum,
//                     },
//                     {
//                       label: 'Average',
//                       value: ExpressionType.Avg,
//                     },
//                     {
//                       label: 'Minimum',
//                       value: ExpressionType.Min,
//                     },
//                     {
//                       label: 'Maximum',
//                       value: ExpressionType.Max,
//                     },
//                   ],
//                 },
//                 {
//                   label: 'Conditional',
//                   options: [
//                     {
//                       label: 'Count if',
//                       value: ExpressionType.CountIf,
//                     },
//                   ],
//                 },
//               ]}
//             />
//           </Form.Item>
//           <Tooltip
//             placement="right"
//             title={
//               <>
//                 <Typography.Title
//                   style={{
//                     fontSize: 'x-large',
//                   }}
//                   level={1}>
//                   {toolTipContent.title} ({toolTipContent.kind})
//                 </Typography.Title>
//                 <Typography.Text>{toolTipContent.description}</Typography.Text>
//                 <Typography.Title
//                   style={{
//                     fontSize: 'large',
//                   }}
//                   level={2}>
//                   Arguments
//                 </Typography.Title>
//                 {toolTipContent.arguments.map((arg) => (
//                   <>
//                     <Typography.Title level={3} style={{ fontSize: 'medium' }}>
//                       {arg.name}
//                     </Typography.Title>
//                     <Typography.Text>{arg.description}</Typography.Text>
//                   </>
//                 ))}
//               </>
//             }>
//             <Typography.Link>Need help?</Typography.Link>
//           </Tooltip>
//         </Space>
//       </Form.Item>
//       {restForm(form.getFieldValue([...prefix, 'type']), [...prefix])}
//     </>
//   );

//   const restForm = useCallback(
//     (tpe: ExpressionType, prefix?: string[]) => {
//       const namePath = [];
//       if (prefix && prefix.length > 0) {
//         namePath.push(...prefix);
//       }
//       switch (tpe) {
//         case ExpressionType.Avg:
//         case ExpressionType.Sum:
//         case ExpressionType.Min:
//         case ExpressionType.Max:
//           return (
//             <>
//               <Form.Item
//                 label="Field"
//                 name={[...namePath, 'fieldExpression', 'field']}>
//                 <Input />
//               </Form.Item>
//               <Form.Item
//                 label="Query"
//                 name={[...namePath, 'fieldExpression', 'query']}>
//                 <Input />
//               </Form.Item>
//             </>
//           );

//         case ExpressionType.Add:
//         case ExpressionType.Div:
//         case ExpressionType.Multiply:
//         case ExpressionType.Subtract:
//           return (
//             <>
//               <Form.Item label="Left" name={[...namePath, 'left']}>
//                 {expressionForm([...namePath, 'left'])}
//               </Form.Item>
//               <Form.Item label="Right" name={[...namePath, 'right']}>
//                 {expressionForm([...namePath, 'right'])}
//               </Form.Item>
//             </>
//           );
//         case ExpressionType.Value:
//           return (
//             <Form.Item label="Value" name="value">
//               <Input type="number" />
//             </Form.Item>
//           );
//       }
//     },
//     [expressionForm]
//   );

//   return (
//     <Form form={form} wrapperCol={{ span: 12 }}>
//       <Form.Item label="Expression type">
//         <Space>
//           <Form.Item
//             name="type"
//             noStyle
//             rules={[{ required: true, message: 'Please select a type' }]}>
//             <Select
//               style={{
//                 minWidth: 200,
//               }}
//               options={[
//                 {
//                   label: 'Atomic',
//                   options: [
//                     {
//                       label: 'Simple value',
//                       value: ExpressionType.Value,
//                     },
//                   ],
//                 },
//                 {
//                   label: 'Mathematical',
//                   options: [
//                     {
//                       label: 'Add',
//                       value: ExpressionType.Add,
//                     },
//                     {
//                       label: 'Subtract',
//                       value: ExpressionType.Subtract,
//                     },
//                     {
//                       label: 'Divide',
//                       value: ExpressionType.Div,
//                     },
//                     {
//                       label: 'Multiply',
//                       value: ExpressionType.Multiply,
//                     },
//                   ],
//                 },
//                 {
//                   label: 'Aggregation',
//                   options: [
//                     {
//                       label: 'Sum',
//                       value: ExpressionType.Sum,
//                     },
//                     {
//                       label: 'Average',
//                       value: ExpressionType.Avg,
//                     },
//                     {
//                       label: 'Minimum',
//                       value: ExpressionType.Min,
//                     },
//                     {
//                       label: 'Maximum',
//                       value: ExpressionType.Max,
//                     },
//                   ],
//                 },
//                 {
//                   label: 'Conditional',
//                   options: [
//                     {
//                       label: 'Count if',
//                       value: ExpressionType.CountIf,
//                     },
//                   ],
//                 },
//               ]}
//             />
//           </Form.Item>
//           <Tooltip
//             placement="right"
//             title={
//               <>
//                 <Typography.Title
//                   style={{
//                     fontSize: 'x-large',
//                   }}
//                   level={1}>
//                   {toolTipContent.title} ({toolTipContent.kind})
//                 </Typography.Title>
//                 <Typography.Text>{toolTipContent.description}</Typography.Text>
//                 <Typography.Title
//                   style={{
//                     fontSize: 'large',
//                   }}
//                   level={2}>
//                   Arguments
//                 </Typography.Title>
//                 {toolTipContent.arguments.map((arg) => (
//                   <>
//                     <Typography.Title level={3} style={{ fontSize: 'medium' }}>
//                       {arg.name}
//                     </Typography.Title>
//                     <Typography.Text>{arg.description}</Typography.Text>
//                   </>
//                 ))}
//               </>
//             }>
//             <Typography.Link>Need help?</Typography.Link>
//           </Tooltip>
//         </Space>
//       </Form.Item>
//       {restForm(expressionType)}
//     </Form>
//   );
// };

// export default ExpressionBuilder;
