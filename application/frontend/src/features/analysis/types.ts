import { HasId } from '../../app/types';
import { QueryResult, QueryReturnType } from '../queries/types';

export enum ExpressionType {
  Add = 'Add',
  Avg = 'Avg',
  Div = 'Div',
  Min = 'Min',
  Max = 'Max',
  Multiply = 'Multiply',
  Subtract = 'Subtract',
  Sum = 'Sum',
  Value = 'Value',
  CountIf = 'CountIf',
  Count = 'Count',
  Plain = 'Plain',
}

export enum CountIfOperator {
  IsEqual = 'IsEqual',
  IsNotEqual = 'IsNotEqual',
  IsLess = 'IsLess',
  IsMore = 'IsMore',
  IsLessOrEqual = 'IsLessOrEqual',
  IsMoreOrEqual = 'IsMoreOrEqual',
  Matches = 'Matches',
}

export const countIfOperatorsWithLabels = [
  {
    label: 'Is equal to',
    value: CountIfOperator.IsEqual,
    allowed: [
      QueryReturnType.ObjectList,
      QueryReturnType.NumberList,
      QueryReturnType.StringList,
    ],
  },
  {
    label: 'Is not equal to',
    value: CountIfOperator.IsNotEqual,
    allowed: [
      QueryReturnType.ObjectList,
      QueryReturnType.NumberList,
      QueryReturnType.StringList,
    ],
  },
  {
    label: 'Is less than',
    value: CountIfOperator.IsLess,
    allowed: [QueryReturnType.NumberList],
  },
  {
    label: 'Is less than or equal to',
    value: CountIfOperator.IsLessOrEqual,
    allowed: [QueryReturnType.NumberList],
  },
  {
    label: 'Is more than',
    value: CountIfOperator.IsMore,
    allowed: [QueryReturnType.NumberList],
  },
  {
    label: 'Is more than or equal to',
    value: CountIfOperator.IsMoreOrEqual,
    allowed: [QueryReturnType.NumberList],
  },
  {
    label: 'Matches (regex)',
    value: CountIfOperator.Matches,
    allowed: [QueryReturnType.StringList, QueryReturnType.ObjectList],
  },
];

export interface Expression extends HasId {
  type: ExpressionType;
  queryId: string;
  allowedQueryTypes: QueryReturnType[];
}

export interface ValueExpression extends Expression {
  value?: number;
}

export interface AggregateExpression extends Expression {
  field?: string;
}

export interface MathOperationExpression extends Expression {
  left?: KPI;
  right?: KPI;
}

export type SomeExpression = ValueExpression &
  AggregateExpression &
  MathOperationExpression;

export interface KPIConfig {
  showInReport: boolean;
  acceptableValues: string;
  unit: string;
}

export interface KPI extends HasId, KPIConfig {
  name: string;
  expression: Expression;
}

export interface Report extends HasId {
  title: string;
  notes: string;
  created: number;
  queryResults: {
    [key: string]: QueryResult;
  };
  kpisAndValues: {
    [key: string]: any;
  };
}

export interface AnalysisModel extends HasId {
  name: string;
  kpis: KPI[];
  reports: Report[];
}

// Request/Response
export interface AnalysisModelChange extends HasId {
  name: string;
}
