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
    id: string;
    name: string;
    type: string;
    referenceName: string;
}

export type WorkItem = {
    id: number;
    fields: Fields;
}

export type Sprint = {
    name: string;
    workItems: WorkItem[];
}

export type Query = {
    id: string;
    name: string;
    select: string[];
    where: string[];
    fieldInfos: FieldInfo[];
}

export type Project = {
    id: string;
    name: string;
    description: string;
    imgeUrl: string;
}

export type Team = {
    id: string;
    name: string;
    description: string;
}

export type AnalysisModel = {
    id: string;
    name: string;
    project: Project;
    team: string;
    queries: Query[];
    // data: Sprint[];
}

// Request/Response
export type AnalysisModelChange = { 
    id: string, 
    name: string 
}
