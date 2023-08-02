import { createSlice } from '@reduxjs/toolkit';
import {
  AnalysisModel,
  AnalysisModelChange,
  Expression,
  KPI,
  KPIConfig,
  Report,
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
  async function (model: { name: string }) {
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
    const response = await axios.put<KPIConfig>(`/analysis/kpi`, {
      id: arg.id,
      name: arg.name,
    });
    return response.data;
  }
);

export const updateKPIConfig = createAppAsyncThunk(
  prefix('updateKPIConfig'),
  async function (arg: { id: string; config: KPIConfig; modelId: string }) {
    const response = await axios.put<KPI>(`/analysis/kpi/config?id=${arg.id}`, {
      ...arg.config,
    });
    return response.data;
  }
);

export const updateExpression = createAppAsyncThunk(
  prefix('updateExpression'),
  async function (arg: {
    kpiId: string;
    modelId: string;
    expression: Expression;
  }) {
    const response = await axios.put<Expression>(
      `/analysis/kpi/expression?kpiId=${arg.kpiId}`,
      {
        expression: arg.expression,
      }
    );
    return response.data;
  }
);

export const createReport = createAppAsyncThunk(
  prefix('createReport'),
  async function (arg: {
    modelId: string;
    queryParameterValues: { [key: string]: any };
    title: string;
    notes: string;
  }) {
    const response = await axios.post<Report>(`/analysis/model/createreport`, {
      ...arg,
    });
    return response.data;
  }
);

export const deleteReport = createAppAsyncThunk(
  prefix('deleteReport'),
  async function (arg: { reportId: string; modelId: string }) {
    await axios.delete(`/analysis/model/deleteReport?reportId=${arg.reportId}`);
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
        logger.logError(`Could not find kpi ${kpiId} within model ${modelId}`);
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
        logger.logError(`Could not find kpi ${kpiId} within model ${modelId}`);
        return;
      }

      state.models[modelIndex].kpis[kpiIndex] = {
        ...state.models[modelIndex].kpis[kpiIndex],
        ...action.payload,
      };
    });

    builder.addCase(updateKPIConfig.fulfilled, (state, action) => {
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
        logger.logError(`Could not find kpi ${kpiId} within model ${modelId}`);
        return;
      }

      state.models[modelIndex].kpis[kpiIndex] = {
        ...state.models[modelIndex].kpis[kpiIndex],
        ...action.payload,
      };
    });

    builder.addCase(updateExpression.fulfilled, (state, action) => {
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
        logger.logError(`Could not find kpi ${kpiId} within model ${modelId}`);
        return;
      }

      state.models[modelIndex].kpis[kpiIndex].expression = {
        ...state.models[modelIndex].kpis[kpiIndex].expression,
        ...action.payload,
      };
    });

    builder.addCase(createReport.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const modelIndex = state.models.findIndex((x) => x.id === modelId);
      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      state.models[modelIndex].reports.push(action.payload);
    });

    builder.addCase(deleteReport.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const modelIndex = state.models.findIndex((x) => x.id === modelId);
      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      const reportId = action.meta.arg.reportId;
      state.models[modelIndex].reports = state.models[
        modelIndex
      ].reports.filter((x) => x.id !== reportId);
    });
  },
});

export const selectModels = (state: RootState) => state.analysis.models;
export const selectModel = (id: string) => (state: RootState) =>
  state.analysis.models.find((x) => x.id === id);
export const selectKPI =
  (modelId: string, kpiId: string) => (state: RootState) =>
    state.analysis.models
      .find((x) => x.id === modelId)
      ?.kpis.find((x) => x.id === kpiId);

export default analysisSlice.reducer;
