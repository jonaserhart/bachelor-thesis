import { Button, Input, InputRef, Space, Table } from 'antd';
import EditableRow from './EditableRow';
import EditableCell from './EditableCell';
import { SearchOutlined } from '@ant-design/icons';
import { ColumnType } from 'antd/es/table';
import { FilterConfirmProps } from 'antd/es/table/interface';
import { useState, useRef } from 'react';
import Highlighter from 'react-highlight-words';

type EditableTableProps = Parameters<typeof Table>[0];

type ColumnTypes = Exclude<EditableTableProps['columns'], undefined>;

export type EditProps<T> = {
  renderEditControl?: (
    save: () => Promise<void>,
    ref: React.RefObject<InputRef>,
    value: T
  ) => React.ReactNode;
};

interface Props<T> {
  dataSource: readonly T[];
  defaultColumns: (ColumnTypes[number] & {
    editable?: boolean;
    editProps?: EditProps<T>;
    searchable?: boolean;
    dataIndex: keyof T | 'actions';
  })[];
  handleSave?: (row: T) => void | Promise<void>;
}

const CustomTable = <T,>(
  props: Omit<EditableTableProps, 'dataSource'> & Props<T>
) => {
  const { defaultColumns, dataSource, handleSave, ...rest } = props;

  const [searchText, setSearchText] = useState('');
  const [searchedColumn, setSearchedColumn] = useState('');
  const searchInput = useRef<InputRef>(null);

  const handleSearch = (
    selectedKeys: string[],
    confirm: (param?: FilterConfirmProps) => void,
    dataIndex: keyof T
  ) => {
    confirm();
    setSearchText(selectedKeys[0]);
    setSearchedColumn(dataIndex.toString());
  };

  const handleReset = (clearFilters: () => void) => {
    clearFilters();
    setSearchText('');
  };

  const getColumnSearchProps = (dataIndex: keyof T): ColumnType<T> => ({
    filterDropdown: ({
      setSelectedKeys,
      selectedKeys,
      confirm,
      clearFilters,
      close,
    }) => (
      <div style={{ padding: 8 }} onKeyDown={(e) => e.stopPropagation()}>
        <Input
          ref={searchInput}
          placeholder={`Search ${dataIndex.toString()}`}
          value={selectedKeys[0]}
          onChange={(e) =>
            setSelectedKeys(e.target.value ? [e.target.value] : [])
          }
          onPressEnter={() =>
            handleSearch(selectedKeys as string[], confirm, dataIndex)
          }
          style={{ marginBottom: 8, display: 'block' }}
        />
        <Space>
          <Button
            type="primary"
            onClick={() =>
              handleSearch(selectedKeys as string[], confirm, dataIndex)
            }
            icon={<SearchOutlined />}
            size="small"
            style={{ width: 90 }}>
            Search
          </Button>
          <Button
            onClick={() => {
              if (clearFilters) {
                handleReset(clearFilters);
              }
              confirm({ closeDropdown: false });
            }}
            size="small"
            style={{ width: 90 }}>
            Reset
          </Button>
        </Space>
      </div>
    ),
    filterIcon: (filtered: boolean) => (
      <SearchOutlined style={{ color: filtered ? '#1677ff' : undefined }} />
    ),
    onFilter: (value, record) =>
      String(record[dataIndex])
        .toLowerCase()
        .includes((value as string).toLowerCase()),
    onFilterDropdownOpenChange: (visible) => {
      if (visible) {
        setTimeout(() => searchInput.current?.select(), 100);
      }
    },
    render: (text) =>
      searchedColumn === dataIndex ? (
        <Highlighter
          highlightStyle={{ backgroundColor: '#ffc069', padding: 0 }}
          searchWords={[searchText]}
          autoEscape
          textToHighlight={text ? text.toString() : ''}
        />
      ) : (
        text
      ),
  });

  const columns = defaultColumns.map((col) => {
    let additionalColProps = {};
    if (col.editable) {
      additionalColProps = {
        ...additionalColProps,
        onCell: (record: T) => ({
          record,
          editable: col.editable,
          editProps: col.editProps,
          dataIndex: col.dataIndex,
          title: col.title,
          handleSave,
        }),
      };
    }
    if (col.searchable && col.dataIndex !== 'actions') {
      additionalColProps = {
        ...additionalColProps,
        ...getColumnSearchProps(col.dataIndex),
      };
    }
    return {
      ...col,
      ...additionalColProps,
    };
  });

  const components = {
    body: {
      row: EditableRow,
      cell: EditableCell<T>,
    },
  };

  return (
    <Table
      components={components}
      rowClassName={() => 'editable-row'}
      bordered
      dataSource={dataSource as readonly object[]}
      columns={columns as ColumnTypes}
      {...rest}
    />
  );
};

export default CustomTable;
