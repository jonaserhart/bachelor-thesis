import { configureStore, ThunkAction, Action } from '@reduxjs/toolkit';
import analysisReducer from '../features/analysis/analysisSlice';
import authReducer from '../features/oauth/authSlice';
import queryReducer from '../features/queries/querySclice';

export const store = configureStore({
  reducer: {
    analysis: analysisReducer,
    auth: authReducer,
    queries: queryReducer,
  },
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk<ReturnType = void> = ThunkAction<
  ReturnType,
  RootState,
  unknown,
  Action<string>
>;
