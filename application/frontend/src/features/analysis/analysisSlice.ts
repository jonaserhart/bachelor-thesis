import { createSlice } from "@reduxjs/toolkit";
import { AnalysisModel } from "./types";

type AnalysisState = {
    models : AnalysisModel[];
}

const initialState: AnalysisState = {
    models:[]
}

const analysisSlice = createSlice({
    name: 'analysis',
    initialState,
    reducers: {

    },
    extraReducers(builder) {
        
    },
});

export default analysisSlice.reducer;
