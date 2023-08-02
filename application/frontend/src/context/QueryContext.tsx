import { createContext, useEffect, useMemo, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getQuery, selectQuery } from '../features/queries/querySclice';
import { BackendError, useAppDispatch, useAppSelector } from '../app/hooks';
import { message } from 'antd';
import { Query } from '../features/queries/types';

interface QueryContextType {
  query: Query | undefined;
  loading: boolean;
}

const initial: QueryContextType = {
  query: undefined,
  loading: false,
};

export const QueryContext = createContext<QueryContextType>(initial);

const QueryContextProvider = (props: React.PropsWithChildren) => {
  const { children } = props;

  const params = useParams();

  const ids = useMemo(() => {
    return {
      query: params.queryId ?? '',
    };
  }, [params]);

  const [loading, setLoading] = useState(false);

  const query = useAppSelector(selectQuery(ids.query));

  const dispatch = useAppDispatch();

  useEffect(() => {
    if (ids.query.length) {
      setLoading(true);
      dispatch(getQuery(ids.query))
        .unwrap()
        .catch((err: BackendError) => message.error(err.message))
        .finally(() => setLoading(false));
    }
  }, [ids.query]);

  const value = useMemo(
    () => ({
      loading,
      query,
    }),
    [loading, query]
  );

  return (
    <QueryContext.Provider value={value}>{children}</QueryContext.Provider>
  );
};

export default QueryContextProvider;
