import { useCallback, useContext, useEffect, useMemo, useState } from 'react';
import {
  QueryParameter,
  QueryParameterValueType,
} from '../../../../features/queries/types';
import axios from '../../../../backendClient';
import { ModelContext } from '../../../../context/ModelContext';
import {
  Card,
  DatePicker,
  Descriptions,
  Divider,
  Form,
  Input,
  InputNumber,
  Select,
  Spin,
  message,
} from 'antd';
import { BackendError, useAppSelector } from '../../../../app/hooks';
import { selectQueries } from '../../../../features/queries/querySclice';

type QueryParameterAndId = QueryParameter & { query: any };

function distinctByValueAggregate<T>(
  arr: T[],
  groupByProperty: keyof T,
  aggregateProperty: keyof T
) {
  const aggregatedMap = new Map<any, T>();

  for (const obj of arr) {
    const groupByValue = obj[groupByProperty];
    const aggregateValue = obj[aggregateProperty];

    if (!aggregatedMap.has(groupByValue)) {
      //@ts-ignore
      aggregatedMap.set(groupByValue, {
        ...obj,
        [groupByProperty]: groupByValue,
        [aggregateProperty]: [aggregateValue],
      });
    } else {
      const existingObj = aggregatedMap.get(groupByValue);
      if (existingObj) {
        //@ts-ignore
        existingObj[aggregateProperty].push(aggregateValue);
      }
    }
  }

  return Array.from(aggregatedMap.values());
}

const QueryParameterForm: React.FC = () => {
  const [queryParameters, setQueryParameters] = useState<QueryParameterAndId[]>(
    []
  );

  const { model } = useContext(ModelContext);
  const modelId = useMemo(() => model?.id ?? '', []);

  const queries = useAppSelector(selectQueries);

  const [loading, setLoading] = useState(false);

  useEffect(() => {
    setLoading(true);
    axios
      .get<string[]>(`/analysis/models/${modelId}/requiredQueries`)
      .then((response) => {
        const queries = response.data;
        const p = queries
          .filter((v, i, a) => a.indexOf(v) === i)
          .map(
            (q) =>
              new Promise<QueryParameterAndId[]>((resolve, reject) => {
                axios
                  .get<QueryParameter[]>(
                    `/analysis/customqueries/${q}/queryparameters`
                  )
                  .then((response) => {
                    const result = response.data.map((x) => ({
                      ...x,
                      query: q,
                    }));
                    resolve(result);
                  })
                  .catch(reject);
              })
          );
        Promise.all(p)
          .then((values) => {
            const flat = values.flatMap((x) => x);
            const dist = distinctByValueAggregate(flat, 'name', 'query');
            dist.sort(function (a, b) {
              const textA = a.name.toUpperCase();
              const textB = b.name.toUpperCase();
              if (textA < textB) return -1;
              if (textA > textB) return 1;
              return 0;
            });
            setQueryParameters(dist);
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
        return <DatePicker showTime placeholder={qp.description} />;
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
    <div>
      {queryParameters.map((qp) => (
        <Card key={qp.name} style={{ marginBottom: '20px' }}>
          <Descriptions
            title={qp.displayName}
            items={[
              {
                key: 'type',
                label: 'Type',
                children: <>{qp.type}</>,
              },
              {
                key: 'paramName',
                label: 'Parameter name',
                children: <>{qp.name}</>,
              },
              {
                key: 'desc',
                label: 'Description',
                children: <>{qp.description}</>,
              },
              {
                key: 'queries',
                label: 'Used by',
                children: (
                  <>
                    {(qp.query as string[])
                      .map((x) => queries.find((q) => q.id === x)?.name)
                      .join(', ')}
                  </>
                ),
              },
            ]}
          />
          <Divider />
          <Form.Item
            key={qp.name}
            label="Value"
            rules={[{ required: true, message: 'Value is required' }]}
            name={['queryParameters', qp.name]}>
            {renderFormItem(qp)}
          </Form.Item>
        </Card>
      ))}
    </div>
  );
};

export default QueryParameterForm;
