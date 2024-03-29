import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {
  AnalysisModel,
  AnalysisModelChange,
  Expression,
  GraphicalConfiguration,
  GraphicalItemProperties,
  GraphicalReportItem,
  GraphicalReportItemLayout,
  GraphicalReportItemSubmission,
  KPI,
  KPIConfig,
  KPIFolder,
  Report,
} from './types';
import axios from '../../backendClient';
import { RootState } from '../../app/store';
import { createAppAsyncThunk } from '../../app/hooks';
import { getLogger } from '../../util/logger';
import {
  CollectionKeys as CollectionKey,
  CollectionTypes,
} from '../../app/types';
import {
  findKPIInFolder,
  getAllKPIs,
  updateKPIFolderInModel,
  updateKPIInModel,
  updateKPIParentFolderInModel,
} from '../../util/kpiFolderUtils';
import { User } from '../auth/types';

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
    const response = await axios.get<AnalysisModel[]>('/analysis/models');
    return response.data;
  }
);

export const createModel = createAppAsyncThunk(
  prefix('createModel'),
  async function (model: { name: string }) {
    const response = await axios.post<AnalysisModel>('/analysis/models', model);
    return response.data;
  }
);

export const updateModelDetails = createAppAsyncThunk(
  prefix('updateModelDetails'),
  async function (changedModel: AnalysisModelChange) {
    const response = await axios.put<AnalysisModel>(
      `/analysis/models/${changedModel.id}`,
      changedModel
    );
    return response.data;
  }
);

export const deleteModel = createAppAsyncThunk(
  prefix('deleteModel'),
  async function (id: string) {
    await axios.delete(`/analysis/models/${id}`);
  }
);

export const getModelDetails = createAppAsyncThunk(
  prefix('getModelDetails'),
  async function (modelId: string) {
    const response = await axios.get<AnalysisModel>(
      `/analysis/models/${modelId}`
    );
    return response.data;
  }
);

export const createNewKPI = createAppAsyncThunk(
  prefix('createNewKPI'),
  async function (arg: { modelId: string; folderId?: string }) {
    const response = await axios.post<KPI>(
      `/analysis/models/${arg.modelId}/kpis?${
        arg.folderId ? '&folderId=' + arg.folderId : ''
      }`
    );
    return response.data;
  }
);

export const deleteKPI = createAppAsyncThunk(
  prefix('deleteKPI'),
  async function (arg: { kpiId: string; modelId: string }) {
    await axios.delete<void>(
      `/analysis/models/${arg.modelId}/kpis/${arg.kpiId}`
    );
  }
);

export const getKPIDetails = createAppAsyncThunk(
  prefix('getKPIDetails'),
  async function (arg: { kpiId: string; modelId: string }) {
    const response = await axios.get<KPI>(
      `/analysis/models/${arg.modelId}/kpis/${arg.kpiId}`
    );
    return response.data;
  }
);

export const updateKPIDetails = createAppAsyncThunk(
  prefix('updateKPIDetails'),
  async function (arg: { id: string; name: string; modelId: string }) {
    const response = await axios.put<KPI>(
      `/analysis/models/${arg.modelId}/kpis/${arg.id}`,
      {
        name: arg.name,
      }
    );
    return response.data;
  }
);

export const updateKPIConfig = createAppAsyncThunk(
  prefix('updateKPIConfig'),
  async function (arg: { id: string; config: KPIConfig; modelId: string }) {
    const response = await axios.put<KPIConfig>(
      `/analysis/models/${arg.modelId}/kpis/${arg.id}/config`,
      {
        ...arg.config,
      }
    );
    return response.data;
  }
);

export const createKPIFolder = createAppAsyncThunk(
  prefix('createKPIFolder'),
  async function (arg: { modelId: string; name: string; folderId?: string }) {
    const response = await axios.post<KPIFolder>(
      `/analysis/models/${arg.modelId}/kpifolders`,
      {
        name: arg.name,
        folderId: arg.folderId,
      }
    );
    return response.data;
  }
);

export const updateKPIFolder = createAppAsyncThunk(
  prefix('updateKPIFolder'),
  async function (arg: { modelId: string; folderId: string; name: string }) {
    const reponse = await axios.put<KPIFolder>(
      `/analysis/models/${arg.modelId}/kpifolders/${arg.folderId}`,
      {
        name: arg.name,
      }
    );

    return reponse.data;
  }
);

export const deleteKPIFolder = createAppAsyncThunk(
  prefix('deleteKPIFolder'),
  async function (arg: { modelId: string; folderId: string }) {
    await axios.delete(
      `/analysis/models/${arg.modelId}/kpifolders/${arg.folderId}`
    );
  }
);

export const addOrUpdateExpression = createAppAsyncThunk(
  prefix('addOrUpdateExpression'),
  async function (arg: {
    kpiId: string;
    modelId: string;
    expression: Expression;
  }) {
    const response = await axios.put<Expression>(
      `/analysis/models/${arg.modelId}/kpis/${arg.kpiId}/expression`,
      {
        expression: arg.expression,
      }
    );
    return response.data;
  }
);

export const removeConditionFromExpression = createAppAsyncThunk(
  prefix('removeConditionFromExpression'),
  async function (arg: {
    kpiId: string;
    modelId: string;
    conditionId: string;
  }) {
    await axios.delete(
      `/analysis/models/${arg.modelId}/kpis/${arg.kpiId}/expression/condition/${arg.conditionId}`
    );
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
    const response = await axios.post<Report>(
      `/analysis/models/${arg.modelId}/reports`,
      {
        ...arg,
      }
    );
    return response.data;
  }
);

export const getReportDetails = createAppAsyncThunk(
  prefix('getReportDetails'),
  async function (arg: { modelId: string; reportId: string }) {
    const response = await axios.get<Report>(
      `/analysis/models/${arg.modelId}/reports/${arg.reportId}`
    );
    return response.data;
  }
);

export const deleteReport = createAppAsyncThunk(
  prefix('deleteReport'),
  async function (arg: { reportId: string; modelId: string }) {
    await axios.delete(
      `/analysis/models/${arg.modelId}/reports/${arg.reportId}`
    );
  }
);

export const createGraphicalConfig = createAppAsyncThunk(
  prefix('createGraphicalConfig'),
  async function (arg: { modelId: string }) {
    const response = await axios.post<GraphicalConfiguration>(
      `/analysis/models/${arg.modelId}/graphicalconfig`
    );
    return response.data;
  }
);

export const getGraphicalConfigDetails = createAppAsyncThunk(
  prefix('getGraphicalConfigDetails'),
  async function (arg: { modelId: string; configId: string }) {
    const response = await axios.get<GraphicalConfiguration>(
      `/analysis/models/${arg.modelId}/graphicalconfig/${arg.configId}`
    );
    return response.data;
  }
);

export const updateGraphicalConfigDetails = createAppAsyncThunk(
  prefix('updateGraphicalConfigDetails'),
  async function (arg: { modelId: string; id: string; name: string }) {
    const response = await axios.put<GraphicalConfiguration>(
      `/analysis/models/${arg.modelId}/graphicalconfig/${arg.id}`,
      {
        name: arg.name,
      }
    );
    return response.data;
  }
);

export const deleteGraphicalConfig = createAppAsyncThunk(
  prefix('deleteGraphicalConfig'),
  async function (arg: { modelId: string; id: string }) {
    await axios.delete(
      `/analysis/models/${arg.modelId}/graphicalconfig/${arg.id}`
    );
  }
);

export const createGraphicalConfigItem = createAppAsyncThunk(
  prefix('createGraphicalConfigItem'),
  async function (arg: {
    modelId: string;
    graphicalConfigId: string;
    submission: GraphicalReportItemSubmission;
  }) {
    const response = await axios.post<GraphicalReportItem>(
      `/analysis/models/${arg.modelId}/graphicalconfig/${arg.graphicalConfigId}/item`,
      arg.submission
    );
    return response.data;
  }
);

export const updateGraphicalConfigItemDetails = createAppAsyncThunk(
  prefix('updateGraphicalConfigItemDetails'),
  async function (arg: {
    modelId: string;
    graphicalConfigId: string;
    id: string;
    name: string;
  }) {
    const response = await axios.put<GraphicalReportItem>(
      `/analysis/models/${arg.modelId}/graphicalconfig/${arg.graphicalConfigId}/item/${arg.id}`,
      {
        name: arg.name,
      }
    );
    return response.data;
  }
);

export const deleteGraphicalConfigItem = createAppAsyncThunk(
  prefix('deleteGraphicalConfigItem'),
  async function (arg: {
    modelId: string;
    graphicalConfigId: string;
    id: string;
  }) {
    await axios.delete(
      `/analysis/models/${arg.modelId}/graphicalconfig/${arg.graphicalConfigId}/item/${arg.id}`
    );
  }
);

export const updateLayout = createAppAsyncThunk(
  prefix('updateLayout'),
  async function (arg: {
    modelId: string;
    graphicalConfigId: string;
    id: string;
    layoutSubmission: GraphicalReportItemLayout;
  }) {
    const response = await axios.put<GraphicalReportItemLayout>(
      `/analysis/models/${arg.modelId}/graphicalconfig/${arg.graphicalConfigId}/item/${arg.id}/layout`,
      {
        layout: arg.layoutSubmission,
      }
    );

    return response.data;
  }
);

export const updateGraphicalItemKPIs = createAppAsyncThunk(
  prefix('updateGraphicalItemKPIs'),
  async function (arg: {
    modelId: string;
    graphicalConfigId: string;
    id: string;
    kpis: string[];
  }) {
    const response = await axios.put(
      `/analysis/models/${arg.modelId}/graphicalconfig/${arg.graphicalConfigId}/item/${arg.id}/kpis`,
      {
        kpis: arg.kpis,
      }
    );

    return response.data;
  }
);

export const updateGraphicalItemProperties = createAppAsyncThunk(
  prefix('updateGraphicalItemProperties'),
  async function (arg: {
    modelId: string;
    graphicalConfigId: string;
    id: string;
    itemProperties: GraphicalItemProperties;
  }) {
    const response = await axios.put(
      `/analysis/models/${arg.modelId}/graphicalconfig/${arg.graphicalConfigId}/item/${arg.id}/properties`,
      {
        properties: arg.itemProperties,
      }
    );

    return response.data;
  }
);

export const addUserToModel = createAppAsyncThunk(
  prefix('addUserToModel'),
  async function (arg: {
    email: string;
    modelId: string;
    permission: 'READER' | 'EDITOR' | 'ADMIN';
  }) {
    const response = await axios.post<User | undefined>(
      `/analysis/models/${arg.modelId}/users`,
      {
        email: arg.email,
        permission: arg.permission,
      }
    );

    return response.data;
  }
);

export const changeUserPermission = createAppAsyncThunk(
  prefix('changeUserPermission'),
  async function (arg: {
    userId: string;
    modelId: string;
    permission: 'READER' | 'EDITOR' | 'ADMIN';
  }) {
    await axios.post(
      `/analysis/models/${arg.modelId}/users/${arg.userId}?permission=${arg.permission}`
    );
  }
);

export const removeUserFromModel = createAppAsyncThunk(
  prefix('removeUserFromModel'),
  async function (arg: { userId: string; modelId: string }) {
    await axios.delete(`/analysis/models/${arg.modelId}/users/${arg.userId}`);
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
    builder.addCase(deleteModel.fulfilled, (state, action) => {
      state.models = state.models.filter((x) => x.id !== action.meta.arg);
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
      const { modelId, folderId } = action.meta.arg;

      if (folderId) {
        const modelIndex = state.models.findIndex(
          (x) => x.id === action.meta.arg.modelId
        );

        if (modelIndex < 0) {
          logger.logError(`Could not find model with id ${modelId}`);
          return;
        }

        updateKPIFolderInModel(state.models[modelIndex], folderId, (folder) =>
          folder.kpis.push(action.payload)
        );
      } else {
        addModelCollectionItem(state, action, modelId, 'kpis');
      }
    });

    builder.addCase(deleteKPI.fulfilled, (state, action) => {
      const { modelId, kpiId } = action.meta.arg;
      const modelIndex = state.models.findIndex(
        (x) => x.id === action.meta.arg.modelId
      );

      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      for (let kpiIndex in state.models[modelIndex].kpis) {
        const kpi = state.models[modelIndex].kpis[kpiIndex];
        if (kpi.id === kpiId) {
          state.models[modelIndex].kpis = state.models[modelIndex].kpis.filter(
            (x) => x.id !== kpiId
          );
          return;
        }
      }

      updateKPIParentFolderInModel(
        state.models[modelIndex],
        kpiId,
        (folder) =>
          (folder.kpis = [...folder.kpis.filter((x) => x.id !== kpiId)])
      );
    });

    builder.addCase(getKPIDetails.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const kpiId = action.meta.arg.kpiId;

      const modelIndex = state.models.findIndex(
        (x) => x.id === action.meta.arg.modelId
      );

      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      for (let kpiIndex in state.models[modelIndex].kpis) {
        const kpi = state.models[modelIndex].kpis[kpiIndex];
        if (kpi.id === kpiId) {
          state.models[modelIndex].kpis[kpiIndex] = {
            ...state.models[modelIndex].kpis[kpiIndex],
            ...action.payload,
          };
          return;
        }
      }
      updateKPIParentFolderInModel(
        state.models[modelIndex],
        kpiId,
        (folder) =>
          (folder.kpis = [
            ...folder.kpis.filter((x) => x.id !== kpiId),
            action.payload,
          ])
      );
    });

    builder.addCase(updateKPIDetails.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const kpiId = action.meta.arg.id;

      const modelIndex = state.models.findIndex(
        (x) => x.id === action.meta.arg.modelId
      );

      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }
      updateKPIInModel(state.models[modelIndex], kpiId, (kpi) => {
        kpi.name = action.payload.name;
      });
    });

    builder.addCase(updateKPIConfig.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const kpiId = action.meta.arg.id;

      const modelIndex = state.models.findIndex(
        (x) => x.id === action.meta.arg.modelId
      );

      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }
      updateKPIInModel(state.models[modelIndex], kpiId, (kpi) => {
        kpi.acceptableValues = action.payload.acceptableValues;
        kpi.unit = action.payload.unit;
        kpi.showInReport = action.payload.showInReport;
      });
    });

    builder.addCase(addOrUpdateExpression.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const kpiId = action.meta.arg.kpiId;

      const modelIndex = state.models.findIndex(
        (x) => x.id === action.meta.arg.modelId
      );

      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }
      updateKPIInModel(state.models[modelIndex], kpiId, (kpi) => {
        kpi.expression = { ...kpi.expression, ...action.payload };
      });
    });

    builder.addCase(
      removeConditionFromExpression.fulfilled,
      (state, action) => {
        const modelId = action.meta.arg.modelId;
        const kpiId = action.meta.arg.kpiId;
        const conditionId = action.meta.arg.conditionId;

        const modelIndex = state.models.findIndex(
          (x) => x.id === action.meta.arg.modelId
        );

        if (modelIndex < 0) {
          logger.logError(`Could not find model with id ${modelId}`);
          return;
        }
        updateKPIInModel(state.models[modelIndex], kpiId, (kpi) => {
          (kpi.expression as any).conditions = (
            kpi.expression as any
          ).conditions.filter((x: any) => x.id !== conditionId);
        });
      }
    );

    builder.addCase(createReport.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      addModelCollectionItem(state, action, modelId, 'reports');
    });

    builder.addCase(deleteReport.fulfilled, (state, action) => {
      removeModelCollectionItem(
        state,
        action.meta.arg.modelId,
        action.meta.arg.reportId,
        'reports'
      );
    });

    builder.addCase(getReportDetails.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const reportId = action.meta.arg.reportId;
      setModelCollectionItem(state, action, modelId, reportId, 'reports');
    });

    builder.addCase(getGraphicalConfigDetails.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const configId = action.meta.arg.configId;
      setModelCollectionItem(state, action, modelId, configId, 'graphical');
    });

    builder.addCase(createGraphicalConfig.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      addModelCollectionItem(state, action, modelId, 'graphical');
    });

    builder.addCase(updateGraphicalConfigDetails.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const configId = action.meta.arg.id;
      const indices = getIndices(state, modelId, configId, 'graphical');
      if (!indices) {
        return;
      }
      state.models[indices.modelIndex].graphical[indices.secondIndex].name =
        action.payload.name;
    });

    builder.addCase(deleteGraphicalConfig.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const configId = action.meta.arg.id;
      removeModelCollectionItem(state, modelId, configId, 'graphical');
    });

    builder.addCase(createGraphicalConfigItem.fulfilled, (state, action) => {
      const indices = getIndices(
        state,
        action.meta.arg.modelId,
        action.meta.arg.graphicalConfigId,
        'graphical'
      );
      if (!indices) {
        return;
      }
      state.models[indices.modelIndex].graphical[
        indices.secondIndex
      ].items.push(action.payload);
    });

    builder.addCase(
      updateGraphicalConfigItemDetails.fulfilled,
      (state, action) => {
        const indices = getIndices(
          state,
          action.meta.arg.modelId,
          action.meta.arg.graphicalConfigId,
          'graphical'
        );
        if (!indices) {
          return;
        }

        const index = state.models[indices.modelIndex].graphical[
          indices.secondIndex
        ].items.findIndex((x) => x.id === action.meta.arg.id);
        if (index < 0) {
          console.error(
            `Could not find graphical reportitem with id ${action.meta.arg.id}`
          );
          return;
        }
        state.models[indices.modelIndex].graphical[indices.secondIndex].items[
          index
        ].name = action.payload.name;
      }
    );

    builder.addCase(deleteGraphicalConfigItem.fulfilled, (state, action) => {
      const indices = getIndices(
        state,
        action.meta.arg.modelId,
        action.meta.arg.graphicalConfigId,
        'graphical'
      );
      if (!indices) {
        return;
      }

      state.models[indices.modelIndex].graphical[indices.secondIndex].items =
        state.models[indices.modelIndex].graphical[
          indices.secondIndex
        ].items.filter((x) => x.id !== action.meta.arg.id);
    });

    builder.addCase(updateLayout.fulfilled, (state, action) => {
      const indices = getIndices(
        state,
        action.meta.arg.modelId,
        action.meta.arg.graphicalConfigId,
        'graphical'
      );
      if (!indices) {
        return;
      }

      const index = state.models[indices.modelIndex].graphical[
        indices.secondIndex
      ].items.findIndex((x) => x.id === action.meta.arg.id);

      if (index < 0) {
        console.error(`Could not find item with id ${action.meta.arg.id}.`);
        return;
      }

      state.models[indices.modelIndex].graphical[indices.secondIndex].items[
        index
      ].layout = action.payload;
    });

    builder.addCase(updateGraphicalItemKPIs.fulfilled, (state, action) => {
      const indices = getIndices(
        state,
        action.meta.arg.modelId,
        action.meta.arg.graphicalConfigId,
        'graphical'
      );
      if (!indices) {
        return;
      }
      const index = state.models[indices.modelIndex].graphical[
        indices.secondIndex
      ].items.findIndex((x) => x.id === action.meta.arg.id);

      if (index < 0) {
        console.error(`Could not find item with id ${action.meta.arg.id}.`);
        return;
      }

      state.models[indices.modelIndex].graphical[indices.secondIndex].items[
        index
      ].dataSources.kpis = action.meta.arg.kpis;
    });

    builder.addCase(
      updateGraphicalItemProperties.fulfilled,
      (state, action) => {
        const indices = getIndices(
          state,
          action.meta.arg.modelId,
          action.meta.arg.graphicalConfigId,
          'graphical'
        );
        if (!indices) {
          return;
        }
        const index = state.models[indices.modelIndex].graphical[
          indices.secondIndex
        ].items.findIndex((x) => x.id === action.meta.arg.id);

        if (index < 0) {
          console.error(`Could not find item with id ${action.meta.arg.id}.`);
          return;
        }

        state.models[indices.modelIndex].graphical[indices.secondIndex].items[
          index
        ].properties = action.meta.arg.itemProperties;
      }
    );

    builder.addCase(createKPIFolder.fulfilled, (state, action) => {
      const { modelId, folderId } = action.meta.arg;
      const modelIndex = state.models.findIndex(
        (x) => x.id === action.meta.arg.modelId
      );

      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      if (folderId) {
        updateKPIFolderInModel(state.models[modelIndex], folderId, (folder) =>
          folder.subFolders.push(action.payload)
        );
      } else {
        state.models[modelIndex].kpiFolders.push(action.payload);
      }
    });

    builder.addCase(updateKPIFolder.fulfilled, (state, action) => {
      const { modelId, folderId } = action.meta.arg;
      const modelIndex = state.models.findIndex(
        (x) => x.id === action.meta.arg.modelId
      );

      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      updateKPIFolderInModel(
        state.models[modelIndex],
        folderId,
        (folder) => (folder.name = action.payload.name)
      );
    });

    builder.addCase(deleteKPIFolder.fulfilled, (state, action) => {
      const { modelId, folderId } = action.meta.arg;
      const modelIndex = state.models.findIndex(
        (x) => x.id === action.meta.arg.modelId
      );

      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      for (let kpiFolderIndex in state.models[modelIndex].kpiFolders) {
        const kpiFolder = state.models[modelIndex].kpiFolders[kpiFolderIndex];
        if (kpiFolder.id === folderId) {
          state.models[modelIndex].kpiFolders = state.models[
            modelIndex
          ].kpiFolders.filter((x) => x.id !== folderId);
          return;
        }
      }

      updateKPIParentFolderInModel(
        state.models[modelIndex],
        folderId,
        (folder) =>
          (folder.subFolders = [
            ...folder.subFolders.filter((x) => x.id !== folderId),
          ])
      );
    });

    builder.addCase(addUserToModel.fulfilled, (state, action) => {
      if (
        action.payload === undefined ||
        (action.payload as unknown as string).length === 0
      ) {
        return;
      }
      const { modelId, permission } = action.meta.arg;
      const modelIndex = state.models.findIndex(
        (x) => x.id === action.meta.arg.modelId
      );

      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      state.models[modelIndex].modelUsers.push({
        user: action.payload,
        permission: permission,
      });
    });

    builder.addCase(changeUserPermission.fulfilled, (state, action) => {
      const { modelId, userId, permission } = action.meta.arg;
      const modelIndex = state.models.findIndex(
        (x) => x.id === action.meta.arg.modelId
      );

      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      const userIndex = state.models[modelIndex].modelUsers.findIndex(
        (x) => x.user.id === userId
      );

      if (userIndex < 0) {
        logger.logError(`Could not find user in model with id ${modelId}`);
        return;
      }

      state.models[modelIndex].modelUsers[userIndex].permission = permission;
    });

    builder.addCase(removeUserFromModel.fulfilled, (state, action) => {
      const { modelId, userId } = action.meta.arg;
      const modelIndex = state.models.findIndex(
        (x) => x.id === action.meta.arg.modelId
      );

      if (modelIndex < 0) {
        logger.logError(`Could not find model with id ${modelId}`);
        return;
      }

      state.models[modelIndex].modelUsers = state.models[
        modelIndex
      ].modelUsers.filter((x) => x.user.id !== userId);
    });
  },
});

const getIndices = (
  state: AnalysisState,
  modelId: string,
  secondId: string,
  collection: CollectionKey<AnalysisModel>
) => {
  const modelIndex = state.models.findIndex((x) => x.id === modelId);
  if (modelIndex < 0) {
    logger.logError(`Could not find model with id ${modelId}`);
    return false;
  }
  const secondIndex = state.models[modelIndex][collection].findIndex(
    (x) => x.id === secondId
  );
  if (secondIndex < 0) {
    logger.logError(
      `Could not find item ${secondIndex} within model ${modelId} in collection ${collection}`
    );
    return false;
  }
  return { modelIndex, secondIndex };
};

const addModelCollectionItem = <
  P extends CollectionTypes<AnalysisModel>,
  T extends string = string,
  M = never,
  E = never
>(
  state: AnalysisState,
  action: PayloadAction<P, T, M, E>,
  modelId: string,
  collection: CollectionKey<AnalysisModel>
) => {
  const modelIndex = state.models.findIndex((x) => x.id === modelId);
  if (modelIndex < 0) {
    logger.logError(`Could not find model with id ${modelId}`);
    return false;
  }

  //@ts-ignore
  state.models[modelIndex][collection].push(action.payload);
};

const setModelCollectionItem = <
  P extends CollectionTypes<AnalysisModel>,
  T extends string = string,
  M = never,
  E = never
>(
  state: AnalysisState,
  action: PayloadAction<P, T, M, E>,
  modelId: string,
  secondId: string,
  collection: CollectionKey<AnalysisModel>
) => {
  const indices = getIndices(state, modelId, secondId, collection);
  if (!indices) return;

  state.models[indices.modelIndex][collection][indices.secondIndex] =
    action.payload;
};

const removeModelCollectionItem = <P extends CollectionTypes<AnalysisModel>>(
  state: AnalysisState,
  modelId: string,
  secondId: string,
  collection: CollectionKey<AnalysisModel>
) => {
  const indices = getIndices(state, modelId, secondId, collection);
  if (!indices) return;

  //@ts-ignore
  state.models[indices.modelIndex][collection] = state.models[
    indices.modelIndex
    //@ts-ignore
  ][collection].filter((x: P) => x.id !== secondId);
};

export const selectModels = (state: RootState) => state.analysis.models;
export const selectModel = (id: string) => (state: RootState) =>
  state.analysis.models.find((x) => x.id === id);

export const selectAllKPIs = (modelId: string) => (state: RootState) => {
  const model = state.analysis.models.find((x) => x.id === modelId);

  if (!model) return [];

  return getAllKPIs(model);
};

export const selectKPI =
  (modelId: string, kpiId: string) => (state: RootState) => {
    const model = state.analysis.models.find((x) => x.id === modelId);
    const directKPI = model?.kpis.find((x) => x.id === kpiId);

    if (directKPI) return directKPI;

    for (let folder of model?.kpiFolders ?? []) {
      const kpi = findKPIInFolder(folder, kpiId);
      if (kpi) return kpi;
    }
  };
export const selectReport =
  (modelId: string, reportId: string) => (state: RootState) =>
    state.analysis.models
      .find((x) => x.id === modelId)
      ?.reports.find((x) => x.id === reportId);

export const selectGraphicalConfig =
  (modelId: string, configId: string) => (state: RootState) =>
    state.analysis.models
      .find((x) => x.id === modelId)
      ?.graphical.find((x) => x.id === configId);

export default analysisSlice.reducer;
