export type Condition = {
  field: string;
  operator: string;
  value: string | number | boolean;
};

export interface HasId {
  id: string;
}

export type HierachyItem<T> = HasId &
  T & {
    hasChildren: boolean;
    children?: HierachyItem<T>[];
  };

type BaseOperator = "sum" | "class" | "avg" | "max" | "min";

export type Operator = BaseOperator | "div" | "countIf";

export interface ConditionExpression {
  operator: "countIf";
  condition: Condition;
}

export interface DivExpression {
  operator: "div";
  operand: [Expression, Expression];
}

export interface BaseExpression {
  operator: BaseOperator;
  operand: string;
}

export type Expression = BaseExpression | DivExpression | ConditionExpression;

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
  And = "And",
  Or = "Or",
  None = "None",
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
  where: Clause[];
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

export interface AnalysisModel extends HasId {
  name: string;
  project: Project;
  team: string;
  queries: Query[];
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
  eq = "=",
  neq = "!=",
  ge = ">=",
  g = ">",
  le = "<=",
  l = "<",
}
