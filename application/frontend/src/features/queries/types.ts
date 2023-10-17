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

interface SimpleQuery extends HasId {
  name: string;
  type:
    | QueryReturnType.Number
    | QueryReturnType.String
    | QueryReturnType.NumberList
    | QueryReturnType.StringList;
  additionalQueryData: {};
}

interface ObjectQuery extends HasId {
  name: string;
  type: QueryReturnType.ObjectList | QueryReturnType.Object;
  additionalQueryData: {
    possibleFields: {
      name: string;
      displayName: string;
    }[];
  };
}

export type Query = ObjectQuery | SimpleQuery;
