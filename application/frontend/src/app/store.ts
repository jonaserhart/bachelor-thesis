import { configureStore, ThunkAction, Action } from '@reduxjs/toolkit';
import analysisReducer from '../analysis/analysisSlice';

export const store = configureStore({
  reducer: {
    analysis: analysisReducer
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
