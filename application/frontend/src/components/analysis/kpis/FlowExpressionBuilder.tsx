import {
  Background,
  Controls,
  Edge,
  Handle,
  MiniMap,
  Node,
  Position,
  ReactFlow,
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

    const handles = useMemo(() => {
      switch (e.type) {
        case ExpressionType.Avg:
        case ExpressionType.Sum:
        case ExpressionType.Min:
        case ExpressionType.Max:
          return (
            <>
              <Handle type="source" position={Position.Bottom} />
            </>
          );
        case ExpressionType.Add:
        case ExpressionType.Div:
        case ExpressionType.Multiply:
        case ExpressionType.Subtract:
          return (
            <>
              <Handle type="source" position={Position.Bottom} />
              <Handle type="source" position={Position.Bottom} />
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
        {handles}
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
      const isMathExpression = type === ExpressionType.Add;

      setNodes((nds) => {
        const updated = nds.map((node) => {
          if (node.id === id) {
            return {
              ...node,
              data: {
                ...node.data,
                type,
              },
            };
          } else {
            return node;
          }
        });
        if (isMathExpression) {
          const newNodes: Node[] = [
            {
              id: 'left',
              type: 'expressionNode',
              data: {
                expression,
                onTypeChange: onTypeChange,
              },
              position: {
                x: 200,
                y: 300,
              },
            },
            {
              id: 'right',
              type: 'expressionNode',
              data: {
                expression,
                onTypeChange: onTypeChange,
              },
              position: {
                x: -200,
                y: 300,
              },
            },
          ];
          return [...updated, ...newNodes];
        }
        return updated;
      });
    },
    [setNodes, setEdges]
  );

  useEffect(() => {
    const initialNodes: Node[] = [
      {
        id: expression.id,
        type: 'expressionNode',
        data: {
          onTypeChange: onTypeChange,
          expression,
        },
        position: { x: 0, y: 50 },
      },
    ];

    if ((expression as any).left && (expression as any).right) {
      initialNodes.push({
        id: (expression as any).left.id,
        type: 'expressionNode',
        data: {
          onTypeChange,
          expression,
          childOf: expression.id,
        },
        position: { x: -200, y: 300 },
      });
      initialNodes.push({
        id: (expression as any).right.id,
        type: 'expressionNode',
        data: {
          onTypeChange,
          expression,
          childOf: expression.id,
        },
        position: { x: 200, y: 300 },
      });
    }

    setNodes(initialNodes);
  }, []);

  return (
    <div style={{ width: '100%', height: '50vh' }}>
      <ReactFlow
        nodes={nodes}
        edges={edges}
        nodeTypes={nodeTypes}
        onNodesChange={(changes) => {
          changes.forEach((change) => {
            console.log('Change: ', change);
            if (change.type === 'add') {
              const data = change.item.data;
              if (data.childOf) {
                // add new edge if a node has been added
                setEdges((e) => {
                  return [
                    ...e,
                    {
                      id: `${data.childOf}-${change.item.id}`,
                      source: `${data.childOf}`,
                      target: `${data.childOf}`,
                    },
                  ];
                });
              }
            }
          });
        }}>
        <MiniMap />
        <Controls />
        <Background color="#aaa" gap={16} />
      </ReactFlow>
    </div>
  );
};

export default FlowExpressionBuilder;
