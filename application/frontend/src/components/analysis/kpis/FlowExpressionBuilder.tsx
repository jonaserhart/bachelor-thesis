import {
  Background,
  Controls,
  Edge,
  Handle,
  MiniMap,
  Node,
  Position,
  ReactFlow,
  XYPosition,
  useEdgesState,
  useNodesState,
} from 'reactflow';
import {
  Expression,
  ExpressionType,
  MathOperationExpression,
} from '../../../features/analysis/types';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import { Card, Select } from 'antd';

interface CustomNodeData {
  expression: Expression;
  onTypeChange: (id: string, t: ExpressionType) => void;
}

const CustomNode = memo(
  ({ data, id }: { data: CustomNodeData; id: string }) => {
    const { onTypeChange, expression: e } = data;

    const srcHandles = useMemo(() => {
      switch (e.type) {
        case ExpressionType.Avg:
        case ExpressionType.Sum:
        case ExpressionType.Min:
        case ExpressionType.Max:
          return (
            <>
              <Handle id={id} type="source" position={Position.Bottom} />
            </>
          );
        case ExpressionType.Add:
        case ExpressionType.Div:
        case ExpressionType.Multiply:
        case ExpressionType.Subtract:
          return (
            <>
              <Handle
                id={`${id}-left`}
                type="source"
                position={Position.Bottom}
              />
              <Handle
                id={`${id}-right`}
                type="source"
                position={Position.Bottom}
              />
            </>
          );
        default:
          return null;
      }
    }, [e]);

    return (
      <>
        <Card title="Expression">
          <Select
            className="nodrag"
            style={{ minWidth: 200 }}
            onChange={(newVal) => onTypeChange(id, newVal)}
            defaultValue={e.type}
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
        </Card>
        {srcHandles}
        <Handle id={id} type="target" position={Position.Top} />
      </>
    );
  }
);

interface Props<T extends Expression> {
  expression?: T;
}

const nodeTypes = {
  expressionNode: CustomNode,
};

const mathExpressionTypes = [
  ExpressionType.Add,
  ExpressionType.Subtract,
  ExpressionType.Div,
  ExpressionType.Multiply,
];

const FlowExpressionBuilder = <T extends Expression>(props: Props<T>) => {
  const { expression: e } = props;

  const [expression, setExpression] = useState<T>(
    e ??
      ({
        id: 'parent',
        type: ExpressionType.Value,
      } as unknown as T)
  );

  const [nodes, setNodes] = useNodesState([]);
  const [edges, setEdges] = useEdgesState([]);

  const onTypeChange = useCallback(
    (id: string, type: ExpressionType) => {
      setExpression((e) => {
        e.type = type;
        if (mathExpressionTypes.includes(type)) {
          const mathExpr = e as unknown as MathOperationExpression<Expression>;
          if (!mathExpr.left) {
            mathExpr.left = {
              id: 'child-left',
              type: ExpressionType.Value,
            };
          }
          if (!mathExpr.right) {
            mathExpr.right = {
              id: 'child-right',
              type: ExpressionType.Value,
            };
          }
          e = mathExpr as unknown as T;
        }
        // maybe other modifications for other expr types?
        return { ...e };
      });
    },
    [setExpression]
  );

  const addExpression = useCallback(
    (n: Node[], e: Edge[], expression: Expression, position: XYPosition) => {
      n.push({
        id: expression.id,
        type: 'expressionNode',
        data: {
          onTypeChange: onTypeChange,
          expression,
        },
        position,
      });
      if (
        [
          ExpressionType.Add,
          ExpressionType.Subtract,
          ExpressionType.Div,
          ExpressionType.Multiply,
        ].includes(expression.type)
      ) {
        const mathExpr = expression as MathOperationExpression<Expression>;
        const left = addExpression(n, e, mathExpr.left, {
          x: position.x - 200,
          y: position.y + 200,
        });
        const right = addExpression(n, e, mathExpr.right, {
          x: position.x + 200,
          y: position.y + 200,
        });
        e.push({
          id: `${mathExpr.id}-${left.id}`,
          source: `${mathExpr.id}-left`,
          target: left.id,
        });
        e.push({
          id: `${mathExpr.id}-${right.id}`,
          source: `${mathExpr.id}-right`,
          target: right.id,
        });
      }
      return expression;
    },
    [onTypeChange]
  );

  const mapExpressionToNodesAndEdges = useCallback(
    (expression: Expression) => {
      const n: Node[] = [];
      const e: Edge[] = [];

      addExpression(n, e, expression, { x: 0, y: 50 });

      return { nodes: n, edges: e };
    },
    [addExpression]
  );

  useEffect(() => {
    const mapped = mapExpressionToNodesAndEdges(expression);

    setNodes(mapped.nodes);
    setEdges(mapped.edges);
  }, [expression, expression.type]);

  return (
    <div style={{ width: '100%', height: '50vh' }}>
      <ReactFlow nodes={nodes} edges={edges} nodeTypes={nodeTypes}>
        <MiniMap />
        <Controls />
        <Background color="#aaa" gap={16} />
      </ReactFlow>
    </div>
  );
};

export default FlowExpressionBuilder;
