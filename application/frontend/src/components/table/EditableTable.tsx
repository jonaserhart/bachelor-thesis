import { Table } from 'antd';
import * as React from 'react';
import EditableRow from './EditableRow';
import EditableCell from './EditableCell';


type EditableTableProps = Parameters<typeof Table>[0];

type ColumnTypes = Exclude<EditableTableProps['columns'], undefined>;

type Props<T> = {
    dataSource: T[]
    defaultColumns: (ColumnTypes[number] & { editable?: boolean; dataIndex: keyof T | 'actions' })[],
    handleSave: (row: T) => void | Promise<void>
}

export default function EditableTable<T>(props: EditableTableProps & Props<T>) {

    const {
        defaultColumns, 
        dataSource,
        handleSave,
        ...rest
    } = props;

    const columns = defaultColumns.map((col) => {
        if (!col.editable) {
          return col;
        }
        return {
          ...col,
          onCell: (record: T) => ({
            record,
            editable: col.editable,
            dataIndex: col.dataIndex,
            title: col.title,
            handleSave,
          }),
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
            dataSource={dataSource}
            columns={columns as ColumnTypes}
            {...rest}
            />
      )
}