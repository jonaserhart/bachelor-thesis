import { createSlice } from '@reduxjs/toolkit';
import { RootState } from '../../app/store';
import { createAppAsyncThunk } from '../../app/hooks';
import axios from '../../backendClient';

type HealthStatus = 'Healthy' | 'Degraded' | 'Unhealthy';

export interface State {
  code: HealthStatus;
  details: {
    key: string;
    value: {
      status: HealthStatus;
      details: string | undefined;
    };
  }[];
}

const initialState: State = {
  code: 'Healthy',
  details: [],
};

export const checkHealthEndpoint = createAppAsyncThunk(
  'connectivity/check',
  async function () {
    const reponse = await axios.get<{ status: State }>('/healthz');
    return reponse.data;
  }
);

const healthSlice = createSlice({
  name: 'connectivity',
  initialState,
  reducers: {},
  extraReducers(builder) {
    builder.addCase(checkHealthEndpoint.fulfilled, (state, action) => {
      state.details = action.payload.status.details;
      state.code = action.payload.status.code;
    });

    builder.addCase(checkHealthEndpoint.rejected, (state, action) => {
      state.details = [
        {
          key: 'Backend error',
          value: {
            status: 'Unhealthy',
            details:
              'There was an error requesting the health endpoint of the backend',
          },
        },
      ];
      state.code = 'Unhealthy';
    });
  },
});

export const selectHealth = (rootState: RootState) => rootState.health;

export default healthSlice.reducer;
