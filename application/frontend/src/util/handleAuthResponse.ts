import { AxiosResponse } from 'axios';
import { AuthResponse } from '../features/auth/types';
import { store } from '../app/store';
import { setToken, setTokenExpired, setUser } from '../features/auth/authSlice';

export default function handleAuthResponse(
  response: AxiosResponse<AuthResponse, any>
) {
  const { data } = response;
  store.dispatch(setToken(data.token));
  const timer = setTimeout(() => {
    store.dispatch(setTokenExpired(true));
  }, data.token.expires);
  store.dispatch(setUser(data.user));
  return timer;
}
