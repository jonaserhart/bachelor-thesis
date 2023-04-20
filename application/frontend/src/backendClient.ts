import axios, { AxiosRequestConfig } from "axios";
import { store } from "./app/store";
import { AuthResponse } from "./features/oauth/types";
import { setToken, setUser } from "./features/oauth/authSlice";

type TokenPromiseFunction = {
    resolve: (token: unknown) => void;
    reject: (reason?: any) => void;
}

const instance = axios.create({
    baseURL: process.env.REACT_APP_API_URL
});

/**
 * Before every request:
 * - Add authorization header to request if present
 */
instance.interceptors.request.use(
    (request) => {
        const token = store.getState().auth.jwt;
        
        if (token) {
            request.headers.Authorization = `Bearer ${token}`;
        }
        return request;
    },
    Promise.reject,
)

let isRefreshing = false;
let failedQueue: TokenPromiseFunction[] = [];

const urlsWithoutAuthenticationRequirement = [
    'oauth/refresh-token',
    'oauth/authorize',
    'oauth/callback'
];

const processQueue = (error: any, token: string | null = null) => {
    failedQueue.forEach((promise) => {
        if (error){
            promise.reject(error);
        }
        else {
            promise.resolve(token);
        }
        failedQueue = [];
    });
}

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
        const originalRequest = error.config as AxiosRequestConfig & { isRetry: boolean };

        if (
            error?.response?.status
            && error.response.status === 401
            && !originalRequest.isRetry
            && !urlsWithoutAuthenticationRequirement.some((val) => originalRequest.url?.includes(val))
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
                    instance.get<AuthResponse>('/oauth/refresh-token', {
                        headers: {
                            'X-IS-FST': isFirstRequest
                        }
                    })
                    .then(({ data }) => {
                        const {
                            token,
                            user
                        } = data;
                        if (!token || !user?.id) {
                            reject({ error: 'New token could not be retrieved. Please log in again.' });
                        }

                        store.dispatch(setUser(user));
                        store.dispatch(setToken(token));

                        processQueue(null, token.jwt);
                        
                        resolve(
                            instance({
                                ...originalRequest,
                                headers: {
                                    ...originalRequest.headers,
                                    Authorization: `Bearer ${token.jwt}`
                                }
                            })
                        );
                    })
                    .catch((err) => {
                        // eslint-disable-next-line no-restricted-globals
                        location.href = '/oauth-authorize';
                        processQueue(err, null);
                        reject(error);
                    })
                    .then(() => {
                        isRefreshing = false;
                    });
                });
            }
            return Promise.reject(error);
    },
);

export default instance;