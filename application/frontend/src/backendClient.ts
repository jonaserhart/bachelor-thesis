import axios, { AxiosRequestConfig } from 'axios';
import { store } from './app/store';
import { AuthResponse } from './features/auth/types';
import handleAuthResponse from './util/handleAuthResponse';

type TokenPromiseFunction = {
  resolve: (token: unknown) => void;
  reject: (reason?: any) => void;
};

const instance = axios.create({
  withCredentials: true,
  baseURL:
    process.env.NODE_ENV === 'development'
      ? process.env.REACT_APP_BACKEND_URL
      : window.__RUNTIME_CONFIG__.REACT_APP_BACKEND_URL,
});

/**
 * Before every request:
 * - Add authorization header to request if present
 */
instance.interceptors.request.use((request) => {
  const token = store.getState().auth.jwt;

  if (token) {
    request.headers.Authorization = `Bearer ${token}`;
  }
  return request;
}, Promise.reject);

let isRefreshing = false;
let timer: NodeJS.Timeout | undefined = undefined;
let failedQueue: TokenPromiseFunction[] = [];

const urlsWithoutAuthenticationRequirement = [
  'auth/refresh-token',
  'auth/authorize',
  'auth/callback',
];

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach((promise) => {
    if (error) {
      promise.reject(error);
    } else {
      promise.resolve(token);
    }
    failedQueue = [];
  });
};

/**
 * Axios interceptor for responses:
 * on successful response:
 *  - just pass the response back to the initiator
 * on failed response:
 *  - if the error does not indicate authentication failure:
 *      - just pass the error onto the initiator
 *  - if the error is in authentication context:
 *      - send a request for a new token and hold off all requests whire this request is pending
 *      - send all requests that arrive while refreshing to a queue
 *      - all requests in this queue will be executed once a new token has been issued
 */
instance.interceptors.response.use(
  (response) => response,
  (error) => {
    const originalRequest = error.config as AxiosRequestConfig & {
      isRetry: boolean;
    };

    if (
      error?.response?.status &&
      error.response.status === 401 &&
      !originalRequest.isRetry &&
      !urlsWithoutAuthenticationRequirement.some((val) =>
        originalRequest.url?.includes(val)
      )
    ) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then((token) => {
            originalRequest.headers!.Authorization = `Bearer ${token}`;
            return instance(originalRequest);
          })
          .catch(Promise.reject);
      }

      originalRequest.isRetry = true;
      isRefreshing = true;

      const isFirstRequest = store.getState().auth.jwt === null;

      return new Promise((resolve, reject) => {
        instance
          .get<AuthResponse>('/auth/refresh-token', {
            headers: {
              'X-IS-FST': isFirstRequest,
            },
          })
          .then((response) => {
            const { token, user } = response.data;
            if (!token || !user?.id) {
              reject({
                error: 'New token could not be retrieved. Please log in again.',
              });
            }

            if (timer) {
              clearTimeout(timer);
            }
            timer = handleAuthResponse(response);

            processQueue(null, token.jwt);

            resolve(
              instance({
                ...originalRequest,
                headers: {
                  ...originalRequest.headers,
                  Authorization: `Bearer ${token.jwt}`,
                },
              })
            );
          })
          .catch((err) => {
            // eslint-disable-next-line no-restricted-globals
            location.href = '/auth-authorize';
            processQueue(err, null);
            reject(error);
          })
          .then(() => {
            isRefreshing = false;
          });
      });
    }
    return Promise.reject(error);
  }
);

export default instance;
