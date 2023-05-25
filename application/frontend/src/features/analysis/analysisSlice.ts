import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import {
  AnalysisModel,
  AnalysisModelChange,
  Condition,
  FieldInfo,
  Project,
  Query,
  Clause,
  QueryModelChange,
} from "./types";
import axios from "../../backendClient";
import { RootState } from "../../app/store";
import { createAppAsyncThunk } from "../../app/hooks";

const PREFIX = "analysis";

const prefix = (s: string) => `${PREFIX}/${s}`;

interface AnalysisState {
  models: AnalysisModel[];
}

const initialState: AnalysisState = {
  models: [],
};

export const getMyModels = createAppAsyncThunk(
  prefix("getMyModels"),
  async function () {
    const response = await axios.get<AnalysisModel[]>("/devops/mymodels");
    return response.data;
  }
);

export const createModel = createAppAsyncThunk(
  prefix("createModel"),
  async function (model: { name: string; project: Project }) {
    const response = await axios.post<AnalysisModel>(
      "/devops/createmodel",
      model
    );
    return response.data;
  }
);

export const updateModelDetails = createAppAsyncThunk(
  prefix("updateModelDetails"),
  async function (changedModel: AnalysisModelChange) {
    const response = await axios.put<AnalysisModel>(
      `/devops/model/${changedModel.id}/update`,
      changedModel
    );
    return response.data;
  }
);

export const getModelDetails = createAppAsyncThunk(
  prefix("getModelDetails"),
  async function (modelId: string) {
    const response = await axios.get<AnalysisModel>(
      `/devops/model/${modelId}/details`
    );
    return response.data;
  }
);

export const getQueryDetails = createAppAsyncThunk(
  prefix("getQueryDetails"),
  async function (args: { queryId: string; modelId: string }) {
    const { queryId } = args;
    const response = await axios.get<Query>(`/devops/query/${queryId}`);
    return response.data;
  }
);

export const updateQueryDetails = createAppAsyncThunk(
  prefix("updateQueryDetails"),
  async function (args: { modelId: string; changeQuery: QueryModelChange }) {
    const response = await axios.put<QueryModelChange>(
      `/devops/query`,
      args.changeQuery
    );
    return response.data;
  }
);

export const createQueryFrom = createAppAsyncThunk(
  prefix("createQueryFrom"),
  async function (args: { modelId: string; queryId: string }) {
    const response = await axios.post<Query>(
      `/devops/createqueryfrom?modelId=${args.modelId}&queryId=${args.queryId}`
    );
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
        console.error(`Could not find model containing query ${queryId}`);
        return;
      }

      const queryIndex = state.models[modelIndex].queries.findIndex(
        (x) => x.id === queryId
      );
      if (queryIndex < 0) {
        console.error(
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
        console.error(`Could not find model with id ${modelId}`);
        return;
      }
      state.models[modelIndex].queries.push(action.payload);
    });

    builder.addCase(updateQueryDetails.fulfilled, (state, action) => {
      const modelId = action.meta.arg.modelId;
      const queryId = action.meta.arg.changeQuery.id;
      const modelIndex = state.models.findIndex((x) => x.id === modelId);
      if (modelIndex < 0) {
        console.error(`Could not find model with id ${modelId}`);
        return;
      }

      const queryIndex = state.models[modelIndex].queries.findIndex(
        (x) => x.id === queryId
      );
      state.models[modelIndex].queries[queryIndex].name = action.payload.name;
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

export default analysisSlice.reducer;
