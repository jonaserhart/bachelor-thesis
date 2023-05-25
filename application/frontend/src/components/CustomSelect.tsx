import { Button, Select, Space, message } from 'antd';
import { HasId, HierachyItem } from '../features/analysis/types';
import axios from '../backendClient';
import { useEffect, useMemo, useState } from 'react';

type EitherProps<T> = HasId | HierachyItem<T>;

interface Props<T extends HasId> {
  request: string;
  title: string;
  onSubmit: (val: T | undefined) => void;
  labelSelector?: (val: T) => string;
}

const CustomSelect = <T extends HasId>(props: Props<T>) => {
  const { onSubmit, request, labelSelector, title } = props;

  const [options, setOptions] = useState<T[]>([]);
  const [selected, setSelected] = useState<string>('');
  const [loading, setLoading] = useState(false);

  const selectLabel = useMemo(
    () => labelSelector ?? ((val: T) => val.id),
    [labelSelector]
  );

  useEffect(() => {
    setLoading(true);
    axios
      .get<T[]>(request)
      .then((response) => {
        setOptions(response.data);
      })
      .catch((err) => message.error(err.message))
      .finally(() => setLoading(false));
  }, []);

  const selectedOption = useMemo(() => {
    return options.find((x) => x.id === selected);
  }, [options, selected]);

  return (
    <Space direction="vertical">
      <Select
        defaultValue={title}
        style={{ width: 200 }}
        loading={loading}
        onSelect={(id) => setSelected(id)}
        showSearch
        filterOption={(input, option) =>
          (option?.label ?? '').toLowerCase().includes(input.toLowerCase())
        }
        options={options.map((p) => ({
          label: selectLabel(p),
          value: p.id,
        }))}
      />

      <div
        style={{
          width: '100%',
          display: 'flex',
          flexDirection: 'row-reverse',
        }}>
        <Button
          disabled={selected === ''}
          type="primary"
          ghost
          onClick={() => onSubmit(selectedOption)}>
          Create
        </Button>
      </div>
    </Space>
  );
};

export default CustomSelect;
