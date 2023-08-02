import { HasId } from '../../app/types';

export enum QueryReturnType {
  Number = 'Number',
  String = 'String',
  Object = 'Object',
  NumberList = 'NumberList',
  StringList = 'StringList',
  ObjectList = 'ObjectList',
}

export interface QueryResult {
  type: QueryReturnType;
  value: any;
}

export enum QueryParameterValueType {
  Number = 'Number',
  String = 'String',
  Select = 'Select',
  Date = 'Date',
}

export interface QueryParameter {
  name: string;
  displayName: string;
  description: string;
  type: QueryParameterValueType;
  data: any;
}

export interface Query extends HasId {
  name: string;
  type: QueryReturnType;
}
