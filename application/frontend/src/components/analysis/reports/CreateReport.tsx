import { Button, Divider, Form, Input, Steps } from 'antd';
import { useForm, useWatch } from 'antd/es/form/Form';
import React, { useCallback, useContext, useEffect } from 'react';
import { useMemo, useState } from 'react';
import QueryParameterForm from './createForms/QueryParameterForm';
import TextArea from 'antd/es/input/TextArea';
import { LoadingOutlined } from '@ant-design/icons';
import { BackendError, useAppDispatch } from '../../../app/hooks';
import { createReport } from '../../../features/analysis/analysisSlice';
import { ModelContext } from '../../../context/ModelContext';
import { QueryParameter } from '../../../features/queries/types';
import { useNavigate } from 'react-router-dom';

interface FormType {
  title: string;
  notes: string;
  queryParameters: {
    [key: string]: any;
  };
}

const CreateReport: React.FC = () => {
  const [current, setCurrent] = useState(0);

  const [form] = useForm<FormType>();

  const { model } = useContext(ModelContext);
  const modelId = useMemo(() => model?.id ?? '', []);

  const [basicInfo, setBasicInfo] = useState({ title: '', notes: '' });
  const [parameters, setQueryParameters] = useState<{
    [key: string]: any;
  }>({});

  const title = useWatch(['title'], form);
  const notes = useWatch(['notes'], form);
  const queryParameters = useWatch(['queryParameters'], form);

  const nav = useNavigate();

  const [status, setStatus] = useState<'error' | undefined>(undefined);

  const handleNext = useCallback(() => {
    if (current === 0) {
      setBasicInfo({
        title,
        notes,
      });
    }

    if (current === 1) {
      setQueryParameters(queryParameters);
    }

    if (current >= 3) {
      nav(`/analyze/${modelId}#l8estreports`);
    }

    setCurrent((prev) => (prev >= 3 ? prev : prev + 1));
  }, [setCurrent, current, title, notes, queryParameters]);

  const handleBack = useCallback(() => {
    setCurrent((prev) => (prev <= 0 ? prev : prev - 1));
  }, [setCurrent]);

  const dispatch = useAppDispatch();

  const [isExecuting, setIsExecuting] = useState(false);

  useEffect(() => {
    setStatus(undefined);
    if (current === 2) {
      dispatch(
        createReport({
          modelId,
          queryParameterValues: parameters,
          ...basicInfo,
        })
      )
        .unwrap()
        .then(() => {
          setIsExecuting(false);
          handleNext();
        })
        .catch((err: BackendError) => {
          setStatus('error');
          setIsExecuting(false);
          console.log(err);
        });
    }
  }, [current, basicInfo, parameters]);

  const canClickNext = useMemo(() => {
    return [
      title && title.trim() !== '',
      queryParameters &&
        Object.keys(queryParameters).every(
          (x) => queryParameters[x] !== undefined
        ),
      false,
      true,
    ];
  }, [title, queryParameters]);

  const stepContent = useMemo(
    () => [
      <React.Fragment key="0">
        <Divider orientationMargin={0} orientation="left">
          Your new report
        </Divider>
        <Form.Item name={['title']} label="Title">
          <Input placeholder="Title of your report" />
        </Form.Item>
        <Form.Item name={['notes']} label="Notes">
          <TextArea placeholder="Some notes about this sprint" />
        </Form.Item>
      </React.Fragment>,
      <React.Fragment key="1">
        <Divider orientationMargin={0} orientation="left">
          Parameters for your queries
        </Divider>
        <QueryParameterForm />
      </React.Fragment>,
      <React.Fragment key="2">
        <Divider orientationMargin={0} orientation="left">
          Executing queries
        </Divider>
      </React.Fragment>,
    ],
    []
  );

  return (
    <div>
      <Steps
        current={current}
        status={status}
        items={[
          {
            title: 'New report',
            description: 'Add a title and some notes',
          },
          {
            title: 'Parameters',
            description: 'Define parameters for queries',
          },
          {
            title: 'Execute',
            description: 'Get data',
            icon: isExecuting ? <LoadingOutlined /> : undefined,
          },
          {
            title: 'Summary',
            description: 'View your new report',
          },
        ]}
      />
      <div
        style={{
          marginTop: '20px',
        }}>
        <Form labelCol={{ span: 2 }} wrapperCol={{ span: 16 }} form={form}>
          {stepContent[current]}
        </Form>
      </div>
      <div
        style={{
          width: '100%',
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
        }}>
        <Button
          onClick={handleBack}
          disabled={current <= 0 || current >= 3}
          style={{ marginRight: '20px' }}>
          Back
        </Button>
        <Button onClick={handleNext} disabled={!canClickNext[current]}>
          {current >= 3 ? 'Done' : 'Next'}
        </Button>
      </div>
    </div>
  );
};

export default CreateReport;
