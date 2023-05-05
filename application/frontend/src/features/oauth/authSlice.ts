import { PayloadAction, createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { User } from "./types";
import { RootState } from "../../app/store";
import axios from "../../backendClient";

const PREFIX = "auth";

const prefix = (s: string) => `${PREFIX}/${s}`;

type AuthState = {
  jwt: string | null;
  expired: boolean;
  user: User | null;
};

const initialState: AuthState = {
  jwt: null,
  expired: true,
  user: null,
};

export const getMe = createAsyncThunk(prefix("getMe"), async function () {
  const response = await axios.get<User>("/user/me");
  return response.data;
});

const authSlice = createSlice({
  name: PREFIX,
  initialState,
  reducers: {
    setUser(state, action: PayloadAction<User>) {
      state.user = action.payload;
    },
    setToken(state, action: PayloadAction<{ jwt: string; expires: number }>) {
      state.jwt = action.payload.jwt;
      if (action.payload.expires > 0) {
        state.expired = false;
      }
    },
    setTokenExpired(state, action: PayloadAction<boolean>) {
      state.expired = action.payload;
    },
  },
  extraReducers(builder) {
    builder.addCase(getMe.fulfilled, (state, action) => {
      state.user = action.payload;
    });
  },
});

export const { setToken, setUser, setTokenExpired } = authSlice.actions;

export const selectAuthenticatedUser = (state: RootState) => state.auth.user;
export const selectToken = (state: RootState) => ({
  token: state.auth.jwt,
  expired: state.auth.expired,
});

export default authSlice.reducer;
