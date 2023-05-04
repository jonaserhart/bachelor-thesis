import { Form, Input } from "antd";
import { useForm } from "antd/es/form/Form";
import TextArea from "antd/es/input/TextArea";
import * as React from "react";

export default function QueryForm() {
  const [form] = useForm();

  return (
    <Form form={form}>
      <Form.Item label="Select">
        <TextArea rows={4} />
      </Form.Item>
      <Form.Item label="From">
        <Input disabled value={"workItems"}/>
      </Form.Item>
      <Form.Item label="Where (without filter for iteration)">
        <TextArea rows={4} />
      </Form.Item>
    </Form>
  );
}
