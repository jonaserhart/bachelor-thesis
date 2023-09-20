import { Button, Card, Form, Input, message } from 'antd';
import axios from '../../backendClient';
import { LockOutlined, UserOutlined, WindowsOutlined } from '@ant-design/icons';
import { useForm } from 'antd/es/form/Form';
import { useAppDispatch } from '../../app/hooks';
import { setToken, setTokenExpired, setUser } from './authSlice';
import { useNavigate } from 'react-router-dom';
import { AuthResponse } from './types';
import { useCallback, useEffect, useMemo, useState } from 'react';

const keyOptions = ['azure-oauth', 'email'] as const;

type KeyOption = (typeof keyOptions)[number];

interface FormType {
  email?: string;
  password?: string;
}

interface AuthMethod {
  key: KeyOption;
  name: string;
}

const Authorize: React.FC = () => {
  const [selectedAuthMethod, setSelectedAuthMethod] =
    useState<KeyOption>('azure-oauth');

  const [authMethods, setAuthMethods] = useState<AuthMethod[]>([]);

  useEffect(() => {
    axios.get<AuthMethod[]>('/auth/methods').then((r) => {
      setAuthMethods(r.data);
    });
  }, [setAuthMethods]);

  const [form] = useForm();
  const dispatch = useAppDispatch();
  const nav = useNavigate();

  const onTabChange = useCallback((tab: string) => {
    setSelectedAuthMethod(tab as KeyOption);
  }, []);

  const onAuthorize = useCallback(
    (vals: FormType) => {
      switch (selectedAuthMethod) {
        case 'azure-oauth':
          axios.get<string>('/auth/oauth-authorize').then((s) => {
            window.location.href = s.data;
          });
          break;
        case 'email':
          axios
            .post<AuthResponse>('/auth/login', {
              email: vals.email,
              password: vals.password,
            })
            .then((r) => {
              dispatch(setUser(r.data.user));
              dispatch(setToken(r.data.token));
              setTimeout(() => {
                dispatch(setTokenExpired(true));
              }, r.data.token.expires);
              nav('/');
            })
            .catch((err) => console.error(err));
          break;
      }
    },
    [selectedAuthMethod]
  );

  const contentList: Record<KeyOption, React.ReactNode> = useMemo(
    () => ({
      'azure-oauth': (
        <Button htmlType="submit" icon={<WindowsOutlined />}>
          Log in with OAuth
        </Button>
      ),
      email: (
        <>
          <Form.Item
            name="email"
            rules={[
              { required: true, message: 'Please input your Email!' },
              { type: 'email', message: 'The input is not valid Email!' },
            ]}>
            <Input
              prefix={<UserOutlined className="site-form-item-icon" />}
              placeholder="Email"
            />
          </Form.Item>
          <Form.Item
            name="password"
            rules={[
              { required: true, message: 'Please input your Password!' },
            ]}>
            <Input
              prefix={<LockOutlined className="site-form-item-icon" />}
              type="password"
              placeholder="Password"
            />
          </Form.Item>
          <Form.Item>
            <Button htmlType="submit">Log in</Button>
          </Form.Item>
        </>
      ),
    }),
    [onAuthorize]
  );

  return (
    <div
      style={{
        height: '100vh',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
      }}>
      <Card
        title="Log in"
        tabList={authMethods.map((x) => ({ key: x.key, tab: x.name }))}
        activeTabKey={selectedAuthMethod}
        onTabChange={onTabChange}>
        <Form form={form} onFinish={onAuthorize}>
          {contentList[selectedAuthMethod]}
        </Form>
      </Card>
    </div>
  );
};

export default Authorize;
