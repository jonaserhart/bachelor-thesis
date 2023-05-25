import { Table } from "antd";
import EditableRow from "./EditableRow";
import EditableCell from "./EditableCell";

type EditableTableProps = Parameters<typeof Table>[0];

type ColumnTypes = Exclude<EditableTableProps["columns"], undefined>;

interface Props<T> {
  dataSource: readonly T[];
  defaultColumns: (ColumnTypes[number] & {
    editable?: boolean;
    dataIndex: keyof T | "actions";
  })[];
  handleSave?: (row: T) => void | Promise<void>;
};

const CustomTable = <T,>(
  props: Omit<EditableTableProps, "dataSource"> & Props<T>
) => {
  const { defaultColumns, dataSource, handleSave, ...rest } = props;

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
      rowClassName={() => "editable-row"}
      bordered
      dataSource={dataSource as readonly object[]}
      columns={columns as ColumnTypes}
      {...rest}
    />
  );
}

export default CustomTable;
