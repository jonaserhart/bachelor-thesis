import { PayloadAction, createSlice } from "@reduxjs/toolkit";
import { User } from "./types";
import { RootState } from "../../app/store";

type AuthState = {
    jwt: string | null;
    expires: number,
    user: User | null;
}

const initialState : AuthState = {
    jwt: null,
    expires: 0,
    user: null
}

const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        setUser: (state, action: PayloadAction<User>) => {
            state.user = action.payload;
        },
        setToken (state, action: PayloadAction<{jwt: string, expires: number}>) {
            state.jwt = action.payload.jwt;
            state.expires = action.payload.expires;
        }
    }
});

export const {
    setToken,
    setUser
} = authSlice.actions;

export const selectAuthenticatedUser = (state: RootState) => state.auth.user;

export default authSlice.reducer;