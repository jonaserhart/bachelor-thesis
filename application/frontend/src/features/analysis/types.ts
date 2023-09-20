import { HasId } from '../../app/types';
import { User } from '../auth/types';
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
  CountIfMultiple = 'CountIfMultiple',
  SumIfMultiple = 'SumIfMultiple',
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

export enum ConditionConnection {
  All = 'All',
  Any = 'Any',
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
  folderId?: string;
}

export interface KPIFolder extends HasId {
  name: string;
  parentFolderId?: string;
  subFolders: KPIFolder[];
  kpis: KPI[];
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

export interface GraphicalReportItemLayout {
  id: string;
  i: string;
  x: number;
  y: number;
  w: number;
  h: number;
  maxH?: number;
  maxW?: number;
  minH?: number;
  minW?: number;
}

export enum GraphicalReportItemType {
  Plain = 'Plain',
  BarChart = 'BarChart',
  PieChart = 'PieChart',
  List = 'List',
  Text = 'Text',
}

export type GraphicalReportItemData = {
  kpis: string[];
};

export interface GraphicalItemProperties extends HasId {
  listFields?: string[];
}

export interface GraphicalReportItem extends HasId {
  name: string;
  type: GraphicalReportItemType;
  dataSources: GraphicalReportItemData;
  layout: GraphicalReportItemLayout;
  properties: GraphicalItemProperties;
}

export interface GraphicalConfiguration extends HasId {
  name: string;
  items: GraphicalReportItem[];
}

export type ModelPermission = 'READER' | 'EDITOR' | 'ADMIN';

export interface ModelUser {
  user: User;
  permission: ModelPermission;
}

export interface AnalysisModel extends HasId {
  name: string;
  kpis: KPI[];
  kpiFolders: KPIFolder[];
  reports: Report[];
  graphical: GraphicalConfiguration[];
  modelUsers: ModelUser[];
}

// Request/Response
export interface AnalysisModelChange extends HasId {
  name: string;
}

export interface GraphicalReportItemLayoutSubmission {
  h: number;
  w: number;
  x: number;
  y: number;
  maxH?: number;
  maxW?: number;
  minH?: number;
  minW?: number;
}

export interface GraphicalReportItemSubmission {
  type: GraphicalReportItemType;
  layout: GraphicalReportItemLayoutSubmission;
}
