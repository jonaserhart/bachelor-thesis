import { Button, Form, Input, Select, Space, message } from "antd";
import { useForm } from "antd/es/form/Form";
import axios from "../../../backendClient";
import { useCallback, useEffect, useMemo, useState } from "react";
import { FieldInfo, Operand, Project } from "../../../features/analysis/types";
import { MinusCircleOutlined, PlusOutlined } from "@ant-design/icons";

const { Option } = Select;

type Props = {
  project: Project;
};

export default function QueryForm(props: Props) {
  const { project } = props;

  const [form] = useForm<{
    fields: string[];
  }>();

  const selectedFieldValues = Form.useWatch("select", form);

  const [fieldInfos, setFieldInfos] = useState<FieldInfo[]>([]);
  const [fieldInfosLoading, setFieldInfosLoading] = useState(false);

  useEffect(() => {
    setFieldInfosLoading(true);
    axios
      .get<FieldInfo[]>(`/devops/fields/${project.id}`)
      .then(({ data }) => setFieldInfos(data))
      .catch((err) => message.error(err.error))
      .finally(() => setFieldInfosLoading(false));
  }, [project.id]);

  const selectedFields = useMemo(() => {
    if (!selectedFieldValues) {
      return [];
    }
    return fieldInfos.filter((x) =>
      selectedFieldValues.includes(x.referenceName)
    );
  }, [selectedFieldValues, fieldInfos]);

  const getSelectedFieldByName = useCallback(
    (name: string) => {
      console.log("Requesting field :", name, selectedFields);
      return selectedFields.find((x) => x.referenceName === name);
    },
    [selectedFields]
  );

  const getFieldOperands = (type: string) => {
    console.log("Type: ", type);
    switch (type.toLowerCase()) {
      case "html":
      case "plaintext":
        return [];
      case "string":
        return [Operand.eq, Operand.neq];
      case "integer":
      case "datetime":
        return Object.values(Operand);
      default:
        return [];
    }
  };

  const handleFieldsChange = useCallback(() => {
    form.setFieldValue("where", []);
  }, [form]);

  return (
    <Form
      form={form}
      style={{
        marginTop: "20px",
        flex: "0 0 100%",
      }}
      layout="vertical"
      labelCol={{ span: 10 }}
      wrapperCol={{ span: 24 }}
    >
      <Form.Item
        name="select"
        label="Select (fields)"
        rules={[
          {
            required: true,
            message: "Please select at leas one field!",
            type: "array",
          },
        ]}
      >
        <Select
          mode="multiple"
          loading={fieldInfosLoading}
          onChange={handleFieldsChange}
          placeholder="Select all fields that you'd like to query"
        >
          {fieldInfos.map((fi) => (
            <Option key={fi.referenceName} value={fi.referenceName}>
              {`${fi.name} (${fi.type})`}
            </Option>
          ))}
        </Select>
      </Form.Item>
      <Form.Item name="from" label="From">
        <Input disabled value={"workItems"} />
      </Form.Item>
      <Form.Item label="Where (without filter for iteration)">
        <Form.List name="where">
          {(fields, { add, remove }) => (
            <>
              {fields.map((field) => (
                <Space key={field.key} align="start">
                  <Form.Item
                    noStyle
                    shouldUpdate={(prevValues, curValues) =>
                      prevValues.fields !== curValues.fields
                    }
                  >
                    {() => (
                      <Form.Item
                        {...field}
                        label="Field"
                        name={[field.name, "field"]}
                        rules={[{ required: true, message: "Missing field" }]}
                      >
                        <Select
                          disabled={!form.getFieldValue("select")?.length}
                        >
                          {selectedFields.map((item) => (
                            <Option
                              key={item.referenceName}
                              value={item.referenceName}
                            >
                              {item.name}
                            </Option>
                          ))}
                        </Select>
                      </Form.Item>
                    )}
                  </Form.Item>
                  <Form.Item
                    noStyle
                    shouldUpdate={(prevValues, newValues) => {
                      const shouldUpdate =
                        prevValues?.where?.[field.name]?.field !==
                        newValues?.where?.[field.name]?.field;
                      console.log(
                        "Should update?",
                        prevValues,
                        newValues,
                        shouldUpdate
                      );
                      return shouldUpdate;
                    }}
                  >
                    {() => (
                      <Form.Item
                        {...field}
                        label="Operand"
                        name={[field.name, "operand"]}
                        rules={[{ required: true, message: "Missing operand" }]}
                      >
                        <Select
                          disabled={
                            !form.getFieldValue(["where", field.name, "field"])
                          }
                        >
                          {getFieldOperands(
                            getSelectedFieldByName(
                              form.getFieldValue(["where", field.name, "field"])
                            )?.type ?? ""
                          ).map((item) => (
                            <Option key={item} value={item}>
                              {item}
                            </Option>
                          ))}
                        </Select>
                      </Form.Item>
                    )}
                  </Form.Item>

                  <MinusCircleOutlined onClick={() => remove(field.name)} />
                </Space>
              ))}

              <Form.Item>
                <Button
                  type="dashed"
                  onClick={() => add()}
                  block
                  icon={<PlusOutlined />}
                >
                  Add sights
                </Button>
              </Form.Item>
            </>
          )}
        </Form.List>
      </Form.Item>
    </Form>
  );
}
