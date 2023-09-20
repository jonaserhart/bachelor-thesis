import { Form, Input, InputRef } from 'antd';
import { EditableContext } from './EditableContext';
import { useContext, useEffect, useRef, useState } from 'react';
import { getLogger } from '../../util/logger';
import { EditProps } from './CustomTable';

interface EditableCellProps<T> {
  title: React.ReactNode;
  editable: boolean;
  editProps: EditProps<T>;
  children: React.ReactNode;
  dataIndex: keyof T;
  record: T;
  handleSave: (record: T) => void;
}

const logger = getLogger('EditableCell');

const EditableCell = <T,>(props: EditableCellProps<T>) => {
  const [editing, setEditing] = useState(false);
  const inputRef = useRef<InputRef>(null);
  const form = useContext(EditableContext)!;

  const {
    title,
    editable,
    editProps,
    children,
    dataIndex,
    record,
    handleSave,
    ...restProps
  } = props;

  useEffect(() => {
    if (editing && inputRef.current) {
      inputRef.current.focus();
    }
  }, [editing]);

  const toggleEdit = () => {
    setEditing(!editing);
    form.setFieldsValue({ [dataIndex]: record[dataIndex] });
  };

  const save = async () => {
    try {
      const values = await form.validateFields();

      toggleEdit();
      handleSave({ ...record, ...values });
    } catch (errInfo) {
      logger.logError('Save failed:', errInfo);
    }
  };

  let childNode = children;

  if (editable) {
    childNode = editing ? (
      <Form.Item
        style={{ margin: 0 }}
        name={dataIndex.toString()}
        rules={[
          {
            required: true,
            message: `${title} is required.`,
          },
        ]}>
        {editProps.renderEditControl ? (
          editProps.renderEditControl(save, inputRef, record)
        ) : (
          <Input ref={inputRef} onPressEnter={save} onBlur={save} />
        )}
      </Form.Item>
    ) : (
      <div
        className="editable-cell-value-wrap"
        style={{ paddingRight: 24 }}
        onClick={toggleEdit}>
        {children}
      </div>
    );
  }

  return <td {...restProps}>{childNode}</td>;
};

export default EditableCell;
