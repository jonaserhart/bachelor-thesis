import { useCallback, useContext, useEffect, useMemo } from 'react';
import { KPIContext } from '../../../context/KPIContext';
import { updateKPIConfig } from '../../../features/analysis/analysisSlice';
import debounce from 'lodash.debounce';
import { ModelContext } from '../../../context/ModelContext';
import { BackendError, useAppDispatch } from '../../../app/hooks';
import {
  Button,
  Divider,
  Form,
  Input,
  InputNumber,
  Select,
  Switch,
  message,
} from 'antd';
import { useForm, useWatch } from 'antd/es/form/Form';
import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';
import {
  NumberRange,
  SingleValue,
  ValueArray,
  ValuesType,
  acceptableValuesToString,
  stringToAcceptableValues,
} from '../../../util/acceptableValueFunctions';

interface FormType {
  unit: string;
  type: ValuesType;
  showInReport: boolean;
  acceptableValues: NumberRange | SingleValue | ValueArray;
}

const formItemLayout = {
  labelCol: {
    xs: { span: 24 },
    sm: { span: 4 },
  },
  wrapperCol: {
    xs: { span: 24 },
    sm: { span: 20 },
  },
};

const formItemLayoutWithOutLabel = {
  wrapperCol: {
    xs: { span: 24, offset: 0 },
    sm: { span: 20, offset: 4 },
  },
};

const KPIConfigForm: React.FC = () => {
  const { model } = useContext(ModelContext);
  const { kpi } = useContext(KPIContext);

  const dispatch = useAppDispatch();

  const [form] = useForm<FormType>();

  const type = useWatch(['type'], form);

  const ids = useMemo(
    () => ({
      model: model?.id ?? '',
      kpi: kpi?.id ?? '',
    }),
    [model, kpi]
  );

  const updateConfig = useCallback(() => {
    form
      .validateFields()
      .then((values) => {
        const acceptableValues = acceptableValuesToString(
          values.acceptableValues
        );
        const unit = values.unit;
        const showInReport = values.showInReport;
        dispatch(
          updateKPIConfig({
            config: {
              acceptableValues,
              unit,
              showInReport,
            },
            id: ids.kpi,
            modelId: ids.model,
          })
        )
          .unwrap()
          .then(() => message.success('Updated kpi configuration'))
          .catch((err: BackendError) => message.error(err.message));
      })
      .catch(console.error);
  }, [dispatch, ids, form]);

  const debouncedUpdateConfig = useCallback(debounce(updateConfig, 1000), [
    updateConfig,
  ]);

  useEffect(() => {
    if (kpi) {
      const parsed = stringToAcceptableValues(kpi.acceptableValues);

      const valuesToSet: Partial<FormType> = {
        type: parsed.type,
        unit: kpi.unit,
        showInReport: kpi.showInReport,
      };
      if (parsed.type === 'range') {
        const v = parsed.value as NumberRange;
        valuesToSet.acceptableValues = {
          from: v.from,
          to: v.to,
        };
      } else {
        valuesToSet.acceptableValues = parsed.value;
      }

      form.setFieldsValue({
        ...valuesToSet,
      });
    }
  }, [kpi, form]);

  const onAccepableValuesUpdate = useCallback(() => {
    debouncedUpdateConfig();
  }, [debouncedUpdateConfig]);

  const acceplableValuesForm = useMemo(() => {
    switch (type) {
      case 'string':
        return (
          <Form.Item
            {...formItemLayout}
            label="Acceptable value"
            name={['acceptableValues']}
            rules={[{ required: true, message: 'Value is required' }]}>
            <Input onChange={onAccepableValuesUpdate} placeholder="Value" />
          </Form.Item>
        );
      case 'number':
        return (
          <Form.Item
            {...formItemLayout}
            label="Acceptable value"
            rules={[{ required: true, message: 'Value is required' }]}
            name={['acceptableValues']}>
            <InputNumber
              onChange={onAccepableValuesUpdate}
              style={{ width: '100%' }}
            />
          </Form.Item>
        );
      case 'numberArray':
      case 'stringArray':
        return (
          <Form.List
            name={['acceptableValues']}
            {...formItemLayout}
            rules={[
              {
                validator: async (_, vals) => {
                  if (!vals || vals.length < 2) {
                    return Promise.reject(new Error('At least 2 values'));
                  }
                },
              },
            ]}>
            {(fields, { add, remove }, { errors }) => (
              <>
                {fields.map((field, index) => (
                  <Form.Item
                    {...(index === 0
                      ? formItemLayout
                      : formItemLayoutWithOutLabel)}
                    label={index === 0 ? 'Acceptable values' : ''}
                    required={false}
                    key={field.key}>
                    <Form.Item
                      {...field}
                      validateTrigger={['onChange', 'onBlur']}
                      rules={[
                        {
                          required: true,
                          message: 'Please input a value or delete this field.',
                        },
                      ]}
                      noStyle>
                      {type === 'numberArray' ? (
                        <InputNumber
                          onChange={onAccepableValuesUpdate}
                          style={{ width: '60%' }}
                          placeholder="Value"
                        />
                      ) : (
                        <Input
                          onChange={onAccepableValuesUpdate}
                          style={{ width: '60%' }}
                          placeholder="Value"
                        />
                      )}
                    </Form.Item>
                    {fields.length > 1 ? (
                      <MinusCircleOutlined
                        onClick={() => {
                          remove(field.name);
                          onAccepableValuesUpdate();
                        }}
                      />
                    ) : null}
                  </Form.Item>
                ))}
                <Form.Item {...formItemLayoutWithOutLabel}>
                  <Button
                    type="dashed"
                    onClick={() => add()}
                    style={{ width: '60%' }}
                    icon={<PlusOutlined />}>
                    Add
                  </Button>
                  <Form.ErrorList errors={errors} />
                </Form.Item>
              </>
            )}
          </Form.List>
        );
      case 'range':
        return (
          <Form.Item {...formItemLayout} label="Acceptable value">
            <Form.Item
              name={['acceptableValues', 'from']}
              rules={[
                {
                  required: true,
                  message: 'Value is required',
                },
              ]}>
              <Input type="number" onChange={onAccepableValuesUpdate} />
            </Form.Item>
            <Form.Item
              name={['acceptableValues', 'to']}
              rules={[
                {
                  required: true,
                  message: 'Value is required',
                },
              ]}>
              <Input type="number" onChange={onAccepableValuesUpdate} />
            </Form.Item>
          </Form.Item>
        );
      case 'any':
        return null;
    }
  }, [type, form, onAccepableValuesUpdate]);

  return (
    <Form form={form}>
      <Divider orientationMargin={0} orientation="left">
        Basics
      </Divider>
      <Form.Item {...formItemLayout} name={['unit']} label="Unit">
        <Input
          onChange={debouncedUpdateConfig}
          placeholder="Unit to display in report"
        />
      </Form.Item>
      <Form.Item
        {...formItemLayout}
        name={['showInReport']}
        label="Show in report?"
        valuePropName="checked">
        <Switch onChange={updateConfig} />
      </Form.Item>
      <Divider orientationMargin={0} orientation="left">
        Acceptable items
      </Divider>
      <Form.Item
        {...formItemLayout}
        name={['type']}
        label="Type"
        rules={[
          { required: true, message: 'type of acceptable values is reuired' },
        ]}>
        <Select
          placeholder="Select the type of acceptable values"
          onChange={(val) => {
            if (val === 'any') {
              debouncedUpdateConfig();
            }
          }}
          options={[
            {
              label: 'Any value',
              value: 'any',
            },
            {
              label: 'Number value',
              value: 'number',
            },
            {
              label: 'String value',
              value: 'string',
            },
            {
              label: 'Multiple string values',
              value: 'stringArray',
            },
            {
              label: 'Multiple number values',
              value: 'numberArray',
            },
            {
              label: 'Range',
              value: 'range',
            },
          ]}
        />
      </Form.Item>
      {acceplableValuesForm}
    </Form>
  );
};

export default KPIConfigForm;
