import { createSlice } from '@reduxjs/toolkit';
import { createAppAsyncThunk } from '../../app/hooks';
import axios from '../../backendClient';
import { RootState } from '../../app/store';
import { Query } from './types';

const PREFIX = 'query';

const prefix = (s: string) => [PREFIX, s].join('/');

interface QueryState {
  queries: Query[];
}

const initialState: QueryState = {
  queries: [],
};

export const getQueries = createAppAsyncThunk(
  prefix('getCustomQueries'),
  async function () {
    const response = await axios.get<Query[]>('/analysis/customqueries');
    return response.data;
  }
);

export const getQuery = createAppAsyncThunk(
  prefix('getCustomQuery'),
  async function (queryId: string) {
    const response = await axios.get<Query>(
      `/analysis/customqueries/${queryId}`
    );
    return response.data;
  }
);

const querySlice = createSlice({
  name: PREFIX,
  initialState,
  reducers: {},
  extraReducers(builder) {
    builder.addCase(getQueries.fulfilled, (state, action) => {
      state.queries = action.payload;
    });

    builder.addCase(getQuery.fulfilled, (state, action) => {
      const idx = state.queries.findIndex((x) => x.id === action.payload.id);

      if (idx >= 0) {
        state.queries[idx] = action.payload;
      } else {
        state.queries.push(action.payload);
      }
    });
  },
});

export const selectQuery = (id?: string) => (rootState: RootState) =>
  rootState.queries.queries.find((x) => x.id === id);
export const selectQueries = (rootState: RootState) =>
  rootState.queries.queries;

export default querySlice.reducer;
