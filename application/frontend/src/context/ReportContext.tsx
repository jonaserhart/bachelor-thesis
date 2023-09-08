import { Report } from '../features/analysis/types';
import { useParams } from 'react-router-dom';
import { createContext, useEffect, useMemo, useState } from 'react';
import { BackendError, useAppDispatch, useAppSelector } from '../app/hooks';
import {
  getReportDetails,
  selectReport,
} from '../features/analysis/analysisSlice';
import { Spin, message } from 'antd';

interface ReportContextType {
  report: Report | undefined;
  loading: boolean;
}

const initial: ReportContextType = {
  report: undefined,
  loading: false,
};

export const ReportContext = createContext(initial);

export const ReportContextProvider: React.FC<React.PropsWithChildren> = (
  props
) => {
  const { children } = props;

  const params = useParams();

  const ids = useMemo(() => {
    return {
      reportId: params.reportId ?? '',
      modelId: params.modelId ?? '',
    };
  }, [params]);

  const [loading, setLoading] = useState(false);

  const report = useAppSelector(selectReport(ids.modelId, ids.reportId));

  const dispatch = useAppDispatch();

  useEffect(() => {
    if (ids.reportId.length) {
      setLoading(true);
      dispatch(getReportDetails({ ...ids }))
        .unwrap()
        .catch((err: BackendError) => message.error(err.message))
        .finally(() => setLoading(false));
    }
  }, [ids.reportId]);

  const value = useMemo(
    () => ({
      loading,
      report,
    }),
    [loading, report]
  );

  return (
    <ReportContext.Provider value={value}>
      <Spin spinning={loading}>{children}</Spin>
    </ReportContext.Provider>
  );
};
