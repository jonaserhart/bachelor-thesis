export type Condition = {
    field: string;
    operator: string;
    value: string | number | boolean;
};

type BaseOperator = 
    'sum' 
    | 'class' 
    | 'avg' 
    | 'max' 
    | 'min';

export type Operator = 
    BaseOperator
    | 'div'
    | 'countIf';

export type ConditionExpression = {
    operator: 'countIf';
    condition: Condition;
}

export type DivExpression = {
    operator: 'div';
    operand : [Expression, Expression];
};

export type BaseExpression = {
    operator: BaseOperator;
    operand : string;
};

export type Expression = BaseExpression | DivExpression | ConditionExpression;
    
export type Fields = {
    [field: string]: string;
};

export type FieldInfo = {
    name: string;
    displayName: string;
}

export type WorkItem = {
    id: number;
    fields: Fields;
}

export type Sprint = {
    name: string;
    workItems: WorkItem[];
}

export type Wiql = {
    select: string[];
    where: string[];
    orderBy: string[];
}

export type AnalysisModel = {
    id: string;
    project: string;
    team: string;
    wiql: Wiql;
    fields: FieldInfo[];
    data: Sprint[];
}
