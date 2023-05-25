import { createContext, useEffect, useMemo, useState } from 'react';
import { Query } from '../features/analysis/types';
import { useParams } from 'react-router-dom';
import {
  getQueryDetails,
  selectQuery,
} from '../features/analysis/analysisSlice';
import { useAppDispatch, useAppSelector } from '../app/hooks';
import { message } from 'antd';

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
      model: params.modelId ?? '',
    };
  }, [params]);

  const [loading, setLoading] = useState(false);

  const query = useAppSelector(selectQuery(ids.model, ids.query));

  const dispatch = useAppDispatch();

  useEffect(() => {
    if (ids.query.length) {
      setLoading(true);
      dispatch(getQueryDetails({ queryId: ids.query, modelId: ids.model }))
        .unwrap()
        .catch((err) => message.error(err.error))
        .finally(() => setLoading(false));
    }
  }, [ids.query]);

  return (
    <QueryContext.Provider
      value={{
        loading: loading,
        query: query,
      }}>
      {children}
    </QueryContext.Provider>
  );
};

export default QueryContextProvider;
