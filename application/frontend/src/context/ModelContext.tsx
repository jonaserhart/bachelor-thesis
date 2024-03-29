import { createContext, useEffect, useMemo, useState } from 'react';
import { AnalysisModel } from '../features/analysis/types';
import { useParams } from 'react-router-dom';
import { BackendError, useAppDispatch, useAppSelector } from '../app/hooks';
import {
  getModelDetails,
  selectModel,
} from '../features/analysis/analysisSlice';
import { message } from 'antd';
import { getQueries } from '../features/queries/querySclice';

interface ModelContextType {
  model: AnalysisModel | undefined;
  loading: boolean;
}

const initial: ModelContextType = {
  model: undefined,
  loading: false,
};

export const ModelContext = createContext<ModelContextType>(initial);

const ModelContextProvider = (props: React.PropsWithChildren) => {
  const { children } = props;
  const params = useParams();

  const modelId = useMemo(() => {
    return params.modelId ?? '';
  }, [params]);

  const [loading, setLoading] = useState(false);

  const model = useAppSelector(selectModel(modelId));

  const dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(getQueries())
      .unwrap()
      .then((q) => {
        if (q.length > 0) {
          console.log(`${q.length} custom-queries found`);
        } else {
          console.log('No custom queries found');
        }
      })
      .catch((err: BackendError) => console.error(err.message));
  }, []);

  useEffect(() => {
    if (modelId) {
      setLoading(true);
      dispatch(getModelDetails(modelId))
        .unwrap()
        .catch((err) => message.error(err.error))
        .finally(() => setLoading(false));
    }
  }, [modelId]);

  const value = useMemo(
    () => ({
      model,
      loading,
    }),
    [model, loading]
  );

  return (
    <ModelContext.Provider value={value}>{children}</ModelContext.Provider>
  );
};

export default ModelContextProvider;
