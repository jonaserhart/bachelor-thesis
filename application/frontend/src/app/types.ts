// global types

export interface HasId {
  id: string;
}

export type CollectionKeys<T> = {
  [K in keyof T]: T[K] extends Array<infer OBJ>
    ? OBJ extends HasId
      ? K
      : never
    : never;
}[keyof T];

export type CollectionTypes<T> = {
  [K in keyof T]: T[K] extends Array<infer Item>
    ? Item extends HasId
      ? Item
      : never
    : never;
}[keyof T];

export {};

declare global {
  interface Window {
    __RUNTIME_CONFIG__: {
      REACT_APP_BACKEND_URL: string;
    };
  }
}
