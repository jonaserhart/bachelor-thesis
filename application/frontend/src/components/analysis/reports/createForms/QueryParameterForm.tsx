import { useCallback, useContext, useEffect, useMemo, useState } from 'react';
import {
  QueryParameter,
  QueryParameterValueType,
} from '../../../../features/queries/types';
import axios from '../../../../backendClient';
import { ModelContext } from '../../../../context/ModelContext';
import { Form, Input, InputNumber, Select, Spin, message } from 'antd';
import { BackendError } from '../../../../app/hooks';

function distinctByProperty<T>(arr: T[], property: keyof T) {
  const distinctValues = new Set();
  const distinctObjects = [];

  for (const obj of arr) {
    const value = obj[property];
    if (!distinctValues.has(value)) {
      distinctValues.add(value);
      distinctObjects.push(obj);
    }
  }

  return distinctObjects;
}

const QueryParameterForm: React.FC = () => {
  const [queryParameters, setQueryParameters] = useState<QueryParameter[]>([]);

  const { model } = useContext(ModelContext);
  const modelId = useMemo(() => model?.id ?? '', []);

  const [loading, setLoading] = useState(false);

  useEffect(() => {
    console.log(queryParameters);
  }, [queryParameters]);

  useEffect(() => {
    setLoading(true);
    axios
      .get<string[]>(`/analysis/requiredQueries?modelId=${modelId}`)
      .then((response) => {
        const queries = response.data;
        const p = queries.map((q) =>
          axios.get<QueryParameter[]>(`/analysis/queryparameters?queryId=${q}`)
        );
        Promise.all(p)
          .then((values) => {
            const parameters = values.flatMap((x) => x.data);
            setQueryParameters(distinctByProperty(parameters, 'name'));
          })
          .then(console.error)
          .finally(() => setLoading(false));
      })
      .catch((err: BackendError) => {
        message.error(err.message);
        setLoading(false);
      });
  }, [setLoading]);

  const renderFormItem = useCallback((qp: QueryParameter) => {
    switch (qp.type) {
      case QueryParameterValueType.Number:
        return <InputNumber placeholder={qp.description} />;
      case QueryParameterValueType.String:
        return <Input placeholder={qp.description} />;
      case QueryParameterValueType.Select:
        return <Select options={qp.data} placeholder={qp.description} />;
      case QueryParameterValueType.Date:
        return <div>Not available yet</div>;
    }
  }, []);

  if (loading) {
    return (
      <div
        style={{
          width: '100%',
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
        }}>
        <Spin tip="Loading parameters" size="default" />
      </div>
    );
  }

  return (
    <Form.Item>
      {queryParameters.map((qp) => (
        <Form.Item
          key={qp.name}
          label={qp.displayName}
          name={['queryParameters', qp.name]}>
          {renderFormItem(qp)}
        </Form.Item>
      ))}
    </Form.Item>
  );
};

export default QueryParameterForm;
