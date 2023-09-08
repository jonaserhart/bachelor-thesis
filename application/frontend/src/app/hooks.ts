import { TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';
import type { RootState, AppDispatch } from './store';
import { AsyncThunkPayloadCreator, createAsyncThunk } from '@reduxjs/toolkit';
import { useEffect, useRef } from 'react';
import { isEqual } from 'lodash';

// Use throughout your app instead of plain `useDispatch` and `useSelector`
export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;

interface AxiosErrorType {
  response: {
    data: {
      message: string;
    };
    status: number;
    statusText: string;
  };
}

export interface IBackendError {
  status: number;
  statusText: string;
  message: string;
}

export class BackendError implements IBackendError {
  public message: string;
  public status: number;
  public statusText: string;

  constructor(message: string, status: number, statusText: string) {
    this.status = status;
    this.statusText = statusText;
    this.message = message;
  }
}

function isNormalizedError<T>(obj: T): obj is T & AxiosErrorType {
  return (
    obj &&
    typeof obj === 'object' &&
    'response' in obj &&
    typeof obj.response === 'object' &&
    obj.response !== null &&
    'data' in obj.response &&
    typeof obj.response.data === 'object' &&
    obj.response.data !== null &&
    'message' in obj.response.data &&
    typeof obj.response.data.message === 'string' &&
    'status' in obj.response &&
    typeof obj.response.status === 'number' &&
    'statusText' in obj.response &&
    typeof obj.response.statusText === 'string'
  );
}

const defaultError: BackendError = {
  message: 'An unknown error occured',
  status: 500,
  statusText: 'Server Error',
};

function checkUnknownErrorProperty(
  str: string,
  unknownError: unknown
): BackendError | undefined {
  if (!unknownError) return undefined;
  const err = unknownError as any;
  if (str in err && typeof err[str] === 'string')
    return new BackendError(err[str] as string, 500, 'Server Error');
  return undefined;
}

interface AsyncAppThunkConfig {
  state: RootState;
  dispatch?: AppDispatch;
  serializedErrorType: BackendError;
}

export const createAppAsyncThunk = <ReturnType, ThunkArg = void>(
  typePrefix: string,
  func: AsyncThunkPayloadCreator<ReturnType, ThunkArg, AsyncAppThunkConfig>
) =>
  createAsyncThunk(typePrefix, func, {
    serializeError(x): BackendError {
      if (!x) return new BackendError('An error ocurred', 500, 'Server Error');
      if (isNormalizedError(x)) {
        return new BackendError(
          x.response.data.message,
          x.response.status,
          x.response.statusText
        );
      }
      if (typeof x === 'object') {
        let err =
          checkUnknownErrorProperty('error', x) ??
          checkUnknownErrorProperty('data', x) ??
          defaultError;
        return err;
      }
      return defaultError;
    },
  });

export default function useDeepEffect(effect: () => void, deps: any[]) {
  const isFirst = useRef(true);
  const prevDeps = useRef(deps);

  useEffect(() => {
    if (prevDeps.current) {
      const isSame = prevDeps.current.every((obj, index) =>
        isEqual(obj, deps[index])
      );
      if (isFirst.current || !isSame) {
        effect();
      }

      isFirst.current = false;
      prevDeps.current = deps;
    }
  }, deps);
}
