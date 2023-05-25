import { createContext, useEffect, useMemo, useState } from 'react';
import { AnalysisModel } from '../features/analysis/types';
import { useParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../app/hooks';
import {
  getModelDetails,
  selectModel,
} from '../features/analysis/analysisSlice';
import { message } from 'antd';

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
    if (!model && modelId) {
      setLoading(true);
      dispatch(getModelDetails(modelId))
        .unwrap()
        .catch((err) => message.error(err.error))
        .finally(() => setLoading(false));
    }
  }, [model, modelId, dispatch]);

  return (
    <ModelContext.Provider
      value={{
        model: model,
        loading: loading,
      }}>
      {children}
    </ModelContext.Provider>
  );
};

export default ModelContextProvider;
