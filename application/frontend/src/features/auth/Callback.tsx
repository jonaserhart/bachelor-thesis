import axios from '../../backendClient';
import * as React from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { AuthResponse } from './types';
import { useAppDispatch } from '../../app/hooks';
import { setToken, setTokenExpired, setUser } from './authSlice';
import { getLogger } from '../../util/logger';

const logger = getLogger('Callback');

const Callback: React.FC = () => {
  const [searchParams] = useSearchParams();
  const nav = useNavigate();

  const dispatch = useAppDispatch();

  React.useEffect(() => {
    const code = searchParams.get('code');
    const state = searchParams.get('state');
    if (code && state) {
      axios
        .post<AuthResponse>('/auth/callback', {
          code,
          state,
        })
        .then((r) => {
          dispatch(setUser(r.data.user));
          dispatch(setToken(r.data.token));
          setTimeout(() => {
            dispatch(setTokenExpired(true));
          }, r.data.token.expires);
          nav('/');
        })
        .catch(logger.logError);
    }
  }, [searchParams, dispatch, nav]);

  return (
    <div
      style={{
        width: '100%',
        height: '100%',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
      }}>
      Hold on a moment while we log you in
    </div>
  );
};

export default Callback;
