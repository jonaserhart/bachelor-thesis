import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { AnalysisModel, AnalysisModelChange, Project } from "./types";
import axios from "../../backendClient";
import { RootState } from "../../app/store";

const PREFIX = "analysis";

const prefix = (s: string) => `${PREFIX}/${s}`;

type AnalysisState = {
  models: AnalysisModel[];
};

const initialState: AnalysisState = {
  models: [],
};

export const getMyModels = createAsyncThunk(
  prefix("getMyModels"),
  async function () {
    const response = await axios.get<AnalysisModel[]>("/devops/mymodels");
    return response.data;
  }
);

export const createModel = createAsyncThunk(
  prefix("createModel"),
  async function (model: { name: string; project: Project }) {
    const response = await axios.post<AnalysisModel>(
      "/devops/createmodel",
      model
    );
    return response.data;
  }
);

export const updateModelDetails = createAsyncThunk(
  prefix("updateModelDetails"),
  async function (changedModel: AnalysisModelChange) {
    const response = await axios.put<AnalysisModel>(
      `/devops/updatemodel/${changedModel.id}`,
      changedModel
    );
    return response.data;
  }
);

export const getModelDetails = createAsyncThunk(
  prefix("getModelDetails"),
  async function (modelId: string) {
    const response = await axios.get<AnalysisModel>(
      `/devops/getmodel/${modelId}`
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
  },
});

export const selectModels = (state: RootState) => state.analysis.models;
export const selectModel = (id: string) => (state: RootState) =>
  state.analysis.models.find((x) => x.id === id);

export default analysisSlice.reducer;
