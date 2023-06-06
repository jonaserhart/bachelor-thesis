import { createSlice } from '@reduxjs/toolkit';
import {
  AnalysisModel,
  AnalysisModelChange,
  KPI,
  Project,
  Query,
  QueryModelChange,
} from './types';
import axios from '../../backendClient';
import { RootState } from '../../app/store';
import { createAppAsyncThunk } from '../../app/hooks';
import { getLogger } from '../../util/logger';

const logger = getLogger('analysisSlice');

const PREFIX = 'analysis';

const prefix = (s: string) => `${PREFIX}/${s}`;

interface AnalysisState {
  models: AnalysisModel[];
}

const initialState: AnalysisState = {
  models: [],
};

export const getMyModels = createAppAsyncThunk(
  prefix('getMyModels'),
  async function () {
    const response = await axios.get<AnalysisModel[]>('/analysis/mymodels');
    return response.data;
  }
);

export const createModel = createAppAsyncThunk(
  prefix('createModel'),
  async function (model: { name: string; project: Project }) {
    const response = await axios.post<AnalysisModel>(
      '/analysis/createmodel',
      model
    );
    return response.data;
  }
);

export const updateModelDetails = createAppAsyncThunk(
  prefix('updateModelDetails'),
  async function (changedModel: AnalysisModelChange) {
    const response = await axios.put<AnalysisModel>(
      `/analysis/model/${changedModel.id}/update`,
      changedModel
    );
    return response.data;
  }
);

export const getModelDetails = createAppAsyncThunk(
  prefix('getModelDetails'),
  async function (modelId: string) {
    const response = await axios.get<AnalysisModel>(
      `/analysis/model/${modelId}/details`
    );
    return response.data;
  }
);

export const getQueryDetails = createAppAsyncThunk(
  prefix('getQueryDetails'),
  async function (args: { queryId: string; modelId: string }) {
    const { queryId } = args;
    const response = await axios.get<Query>(`/analysis/query/${queryId}`);
    return response.data;
  }
);

export const updateQueryDetails = createAppAsyncThunk(
  prefix('updateQueryDetails'),
  async function (args: { modelId: string; changeQuery: QueryModelChange }) {
    const response = await axios.put<QueryModelChange>(
      `/analysis/query`,
      args.changeQuery
    );
    return response.data;
  }
);

export const createQueryFrom = createAppAsyncThunk(
  prefix('createQueryFrom'),
  async function (args: { modelId: string; queryId: string }) {
    const response = await axios.post<Query>(
      `/analysis/createqueryfrom?modelId=${args.modelId}&queryId=${args.queryId}`
    );
    return response.data;
  }
);

export const createNewKPI = createAppAsyncThunk(
  prefix('createNewKPI'),
  async function (modelId: string) {
    const response = await axios.post<KPI>(`/analysis/kpi?modelId=${modelId}`);
    return response.data;
  }
);

export const deleteKPI = createAppAsyncThunk(
  prefix('deleteKPI'),
  async function (arg: { kpiId: string; modelId: string }) {
    await axios.delete<void>(`/analysis/kpi?id=${arg.kpiId}`);
  }
);

export const getKPIDetails = createAppAsyncThunk(
  prefix('getKPIDetails'),
  async function (arg: { kpiId: string; modelId: string }) {
    const response = await axios.get<KPI>(`/analysis/kpi?id=${arg.kpiId}`);
    return response.data;
  }
);

export const updateKPIDetails = createAppAsyncThunk(
  prefix('updateKPIDetails'),
  async function (arg: { id: string; name: string; modelId: string }) {
    const response = await axios.put<KPI>(`/analysis/kpi`, {
      id: arg.id,
      name: arg.name,
    });
    return response.data;
  }
);

const analysisSlice = createSlice({
  name: PREFIX,
  initialState,
  reducers: {},
  extraReducers(builder) {
    builder.addCase(getMyModels.fulfilled, (state, action) => {
      state.models = action.payload;
    });
    builder.addCase(createModel.fulfilled, (state, action) => {
      state.models.push(action.payload);
    });
    builder.addCase(updateModelDetails.fulfilled, (state, action) => {
      const index = state.models.findIndex((x) => x.id === action.payload.id);
      if (index >= 0) {
        state.models[index] = action.payload;
      }
    });
    builder.addCase(getModelDetails.fulfilled, (state, action) => {
      const index = state.models.findIndex((x) => x.id === action.payload.id);
      if (index >= 0) {
        state.models[index] = action.payload;
      } else {
        state.models.push(action.payload);
      }
    });
    builder.addCase(getQueryDetails.fulfilled, (state, action) => {
      const queryId = action.meta.arg.queryId;
      const modelId = action.meta.arg.modelId;
      const modelIndex = state.models.findIndex((x) => x.id === modelId);
      if (modelIndex < 0) {
        logger.logError(`Could not find model containing query ${queryId}`);
        return;
      }

      const queryIndex = state.models[modelIndex].queries.findIndex(
        (x) => x.id === queryId
      );
      if (queryIndex < 0) {
        logger.logError(
          `Could not find query ${queryId} within modelIndex ${modelIndex}`
        );
        return;
      }

      state.models[modelIndex].queries[queryIndex] = action.payload;
    });

    builder.addCase(createQueryFrom.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const modelIndex = state.models.findIndex((x) => x.id === modelId);
      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }
      state.models[modelIndex].queries.push(action.payload);
    });

    builder.addCase(updateQueryDetails.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const queryId = action.meta.arg.changeQuery.id;
      const modelIndex = state.models.findIndex((x) => x.id === modelId);
      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      const queryIndex = state.models[modelIndex].queries.findIndex(
        (x) => x.id === queryId
      );
      state.models[modelIndex].queries[queryIndex].name = action.payload.name;
    });

    builder.addCase(createNewKPI.fulfilled, (state, action) => {
      const modelId = action.meta.arg;
      const modelIndex = state.models.findIndex((x) => x.id === modelId);
      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      state.models[modelIndex].kpis.push(action.payload);
    });

    builder.addCase(deleteKPI.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const modelIndex = state.models.findIndex((x) => x.id === modelId);
      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      state.models[modelIndex].kpis = state.models[modelIndex].kpis.filter(
        (x) => x.id !== action.meta.arg.kpiId
      );
    });

    builder.addCase(getKPIDetails.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const kpiId = action.meta.arg.kpiId;
      const modelIndex = state.models.findIndex((x) => x.id === modelId);
      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      const kpiIndex = state.models[modelIndex].kpis.findIndex(
        (x) => x.id === kpiId
      );
      if (kpiIndex < 0) {
        logger.logError(
          `Could not find query ${kpiId} within model ${modelId}`
        );
        return;
      }

      state.models[modelIndex].kpis[kpiIndex] = action.payload;
    });

    builder.addCase(updateKPIDetails.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const kpiId = action.meta.arg.id;
      const modelIndex = state.models.findIndex((x) => x.id === modelId);
      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      const kpiIndex = state.models[modelIndex].kpis.findIndex(
        (x) => x.id === kpiId
      );
      if (kpiIndex < 0) {
        logger.logError(
          `Could not find query ${kpiId} within model ${modelId}`
        );
        return;
      }

      state.models[modelIndex].kpis[kpiIndex] = action.payload;
    });
  },
});

export const selectModels = (state: RootState) => state.analysis.models;
export const selectModel = (id: string) => (state: RootState) =>
  state.analysis.models.find((x) => x.id === id);
export const selectQuery =
  (modelId: string, queryId: string) => (state: RootState) =>
    state.analysis.models
      .find((x) => x.id === modelId)
      ?.queries.find((x) => x.id === queryId);

export const selectKPI =
  (modelId: string, kpiId: string) => (state: RootState) =>
    state.analysis.models
      .find((x) => x.id === modelId)
      ?.kpis.find((x) => x.id === kpiId);

export default analysisSlice.reducer;
