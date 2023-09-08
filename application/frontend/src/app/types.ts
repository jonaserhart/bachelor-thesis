// global types

export interface HasId {
  id: string;
}

export type CollectionKeys<T> = {
  [K in keyof T]: T[K] extends Array<any> | Map<any, any> | Set<any>
    ? K
    : never;
}[keyof T];

export type CollectionTypes<T> = {
  [K in keyof T]: T[K] extends Array<infer Item>
    ? Item
    : T[K] extends Map<any, infer Value>
    ? Value
    : T[K] extends Set<infer Value>
    ? Value
    : never;
}[keyof T];
