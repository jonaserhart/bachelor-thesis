import { configureStore, ThunkAction, Action } from '@reduxjs/toolkit';
import analysisReducer from '../features/analysis/analysisSlice';
import authReducer from '../features/auth/authSlice';
import queryReducer from '../features/queries/querySclice';
import healthReducer from '../features/health/healthSlice';

export const store = configureStore({
  reducer: {
    analysis: analysisReducer,
    auth: authReducer,
    queries: queryReducer,
    health: healthReducer,
  },
});

if (process.env.NODE_ENV === 'development') {
  // Add redux state to window to debug
  Object.defineProperty(window, 'reduxStore', {
    get() {
      return store.getState();
    },
  });
}

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk<ReturnType = void> = ThunkAction<
  ReturnType,
  RootState,
  unknown,
  Action<string>
>;
