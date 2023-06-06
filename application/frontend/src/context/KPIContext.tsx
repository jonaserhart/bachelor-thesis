import { createContext, useEffect, useMemo, useState } from 'react';
import { KPI } from '../features/analysis/types';
import { useParams } from 'react-router-dom';
import { BackendError, useAppDispatch, useAppSelector } from '../app/hooks';
import { getKPIDetails, selectKPI } from '../features/analysis/analysisSlice';
import { message } from 'antd';

interface KPIContextType {
  kpi: KPI | undefined;
  loading: boolean;
}

const initial: KPIContextType = {
  kpi: undefined,
  loading: false,
};

export const KPIContext = createContext(initial);

const KPIContextProvider = (props: React.PropsWithChildren) => {
  const { children } = props;

  const params = useParams();

  const ids = useMemo(() => {
    return {
      kpiId: params.kpiId ?? '',
      modelId: params.modelId ?? '',
    };
  }, [params]);

  const [loading, setLoading] = useState(false);

  const kpi = useAppSelector(selectKPI(ids.modelId, ids.kpiId));

  const dispatch = useAppDispatch();

  useEffect(() => {
    if (ids.kpiId.length) {
      setLoading(true);
      dispatch(getKPIDetails({ ...ids }))
        .unwrap()
        .catch((err: BackendError) => message.error(err.message))
        .finally(() => setLoading(false));
    }
  }, [ids.kpiId]);

  const value = useMemo(
    () => ({
      loading,
      kpi,
    }),
    [loading, kpi]
  );

  return <KPIContext.Provider value={value}>{children}</KPIContext.Provider>;
};

export default KPIContextProvider;
