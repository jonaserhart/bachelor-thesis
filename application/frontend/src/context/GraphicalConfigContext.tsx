import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import { GraphicalConfiguration } from '../features/analysis/types';
import { useSelector } from 'react-redux';
import {
  getGraphicalConfigDetails,
  selectGraphicalConfig,
} from '../features/analysis/analysisSlice';
import { useParams } from 'react-router-dom';
import { BackendError, useAppDispatch, useAppSelector } from '../app/hooks';
import { message } from 'antd';
import { ModelContext } from './ModelContext';

interface GraphicalConfigContextType {
  graphicalConfig: GraphicalConfiguration | undefined;
  loading: boolean;
}

export const GraphicalConfigContext = createContext<GraphicalConfigContextType>(
  {
    graphicalConfig: undefined,
    loading: false,
  }
);

export const GraphicalConfigContextProvider: React.FC<
  React.PropsWithChildren
> = (props) => {
  const { children } = props;

  const params = useParams();

  const { model, loading: loadingModel } = useContext(ModelContext);

  const ids = useMemo(() => {
    return {
      configId: params.configId ?? '',
      modelId: params.modelId ?? '',
    };
  }, [params]);

  const [loading, setLoading] = useState(false);

  const config = useAppSelector(
    selectGraphicalConfig(ids.modelId, ids.configId)
  );

  const dispatch = useAppDispatch();

  useEffect(() => {
    if (!loadingModel && model && ids.configId.length) {
      setLoading(true);
      dispatch(getGraphicalConfigDetails({ ...ids }))
        .unwrap()
        .catch((err: BackendError) => message.error(err.message))
        .finally(() => setLoading(false));
    }
  }, [ids.configId, loadingModel]);

  const value = useMemo(
    () => ({
      loading,
      graphicalConfig: config,
    }),
    [loading, config]
  );
  return (
    <GraphicalConfigContext.Provider value={value}>
      {children}
    </GraphicalConfigContext.Provider>
  );
};
