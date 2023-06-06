export interface HasId {
  id: string;
}

export type HierachyItem<T> = HasId &
  T & {
    hasChildren: boolean;
    children?: HierachyItem<T>[];
  };
export interface Fields {
  [field: string]: string;
}

export interface FieldInfo extends HasId {
  name: string;
  type: string;
  referenceName: string;
}

export interface WorkItem extends HasId {
  fields: Fields;
}

export interface Sprint {
  name: string;
  workItems: WorkItem[];
}

export enum LogicalOperator {
  And = 'And',
  Or = 'Or',
  None = 'None',
}

export interface FieldOperation {
  name: string;
  referenceName: string;
}

export interface Clause extends HasId {
  clauses: Clause[];
  field: string;
  fieldValue?: string;
  isFieldValue: boolean;
  logicalOperator: LogicalOperator;
  operator: FieldOperation;
  value: string;
}

export interface Query extends HasId {
  name: string;
  select: FieldInfo[];
  where: Clause;
}

export interface Project extends HasId {
  name: string;
  description: string;
  imgeUrl: string;
}

export interface Team extends HasId {
  name: string;
  description: string;
}

export enum ExpressionType {
  Add = 'Add',
  Avg = 'Avg',
  CountIf = 'CountIf',
  Div = 'Div',
  Min = 'Min',
  Max = 'Max',
  Multiply = 'Multiply',
  Subtract = 'Subtract',
  Sum = 'Sum',
  Field = 'Field',
  Value = 'Value',
}

export interface Expression extends HasId {
  type: ExpressionType;
}

export interface FieldExpression extends Expression {
  field: string;
}

export interface ValueExpression extends Expression {
  value?: number;
}

export interface AggregateExpression extends Expression {
  fieldExpression: FieldExpression;
}

export interface CountIfExpression extends Expression {
  field: string;
  operator: string;
  value: string;
}

export interface MathOperationExpression<T extends Expression>
  extends Expression {
  left: T;
  right: T;
}

export interface KPI extends HasId {
  name: string;
  expression: Expression;
}

export interface AnalysisModel extends HasId {
  name: string;
  project: Project;
  team: string;
  queries: Query[];
  kpis: KPI[];
  // data: Sprint[];
}

// Request/Response
export interface AnalysisModelChange extends HasId {
  name: string;
}

export interface QueryModelChange extends HasId {
  name: string;
}

export enum QueryOperator {
  eq = '=',
  neq = '!=',
  ge = '>=',
  g = '>',
  le = '<=',
  l = '<',
}
