import { useContext, useMemo, useState } from 'react';
import { ModelContext } from '../../../context/ModelContext';
import {
  Button,
  Form,
  Input,
  Popover,
  Select,
  Space,
  Typography,
  message,
} from 'antd';
import CustomTable from '../../table/CustomTable';
import {
  BackendError,
  useAppDispatch,
  useAppSelector,
} from '../../../app/hooks';
import {
  addUserToModel,
  changeUserPermission,
} from '../../../features/analysis/analysisSlice';
import { ModelPermission } from '../../../features/analysis/types';
import { User } from '../../../features/auth/types';
import { selectAuthenticatedUser } from '../../../features/auth/authSlice';

const { Title } = Typography;

const toFirstCharCamelCase = (str: string) =>
  str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();

const ModelAccess: React.FC = () => {
  const { model } = useContext(ModelContext);

  const me = useAppSelector(selectAuthenticatedUser);

  const modelId = useMemo(() => model?.id ?? '', [model]);

  const dispatch = useAppDispatch();

  const [popoverOpen, setPopoverOpen] = useState(false);
  const [submitLoading, setSubmitLoading] = useState(false);

  const modelUsers = useMemo(() => model?.modelUsers ?? [], [model]);

  const dataSource = useMemo(
    () =>
      modelUsers.map((x) => ({
        userId: x.user.id,
        user: x.user.displayName,
        permission: toFirstCharCamelCase(x.permission),
      })),
    [modelUsers]
  );

  return (
    <div>
      <Title level={4} style={{ marginTop: 0 }}>
        Model access
      </Title>
      <Typography>
        These settings are a way to control if someone has access to your model
        and what they can do.
      </Typography>
      <Typography>
        You can grant a user either read, write or admin permissions to your
        model.
      </Typography>
      <div
        style={{
          width: '100%',
          display: 'flex',
          flexDirection: 'row-reverse',
        }}>
        <Popover
          content={
            <Form
              style={{ minWidth: 300 }}
              onFinish={(vals) => {
                setSubmitLoading(true);
                dispatch(
                  addUserToModel({
                    email: vals.email,
                    permission: vals.permission,
                    modelId,
                  })
                )
                  .unwrap()
                  .then((val) => {
                    if (!val) {
                      message.info(
                        'As soon as a user with this email has logged in, access to the model will be granted'
                      );
                    } else {
                      message.success(
                        `User ${val.displayName} now has access to the model`
                      );
                    }
                    setPopoverOpen(false);
                  })
                  .catch((err: BackendError) => message.error(err.message))
                  .finally(() => setSubmitLoading(false));
              }}>
              <Form.Item
                label="Email"
                name="email"
                rules={[
                  { required: true, message: 'Please input an email address!' },
                  { type: 'email', message: 'Please input a valid email!' },
                ]}>
                <Input />
              </Form.Item>
              <Form.Item
                label="Permission"
                name="permission"
                rules={[
                  { required: true, message: 'Please input a permission!' },
                ]}>
                <Select
                  options={[
                    {
                      label: 'Reader',
                      value: 'READER',
                    },
                    {
                      label: 'Editor',
                      value: 'EDITOR',
                    },
                    {
                      label: 'Admin',
                      value: 'ADMIN',
                    },
                  ]}
                />
              </Form.Item>
              <Form.Item>
                <Button
                  loading={submitLoading}
                  type="primary"
                  htmlType="submit">
                  Submit
                </Button>
              </Form.Item>
            </Form>
          }
          open={popoverOpen}>
          <Button
            ghost
            type="primary"
            style={{ marginBottom: 16 }}
            onClick={() => {
              setPopoverOpen(true);
            }}>
            Add User to model
          </Button>
        </Popover>
      </div>
      <CustomTable
        dataSource={dataSource}
        handleSave={(row) => {
          dispatch(
            changeUserPermission({
              modelId,
              userId: row.userId,
              permission: row.permission as ModelPermission,
            })
          );
        }}
        defaultColumns={[
          {
            key: 'userId',
            dataIndex: 'userId',
            title: 'User id',
          },
          {
            key: 'user',
            dataIndex: 'user',
            title: 'Name',
          },
          {
            key: 'permission',
            dataIndex: 'permission',
            title: 'Permission',
            editable: true,
            render(value, rec, index) {
              return rec.permission;
            },
            editProps: {
              renderEditControl(save, ref, value) {
                return (
                  <Select
                    disabled={value.userId === me?.id}
                    defaultValue={value.permission}
                    options={[
                      {
                        label: 'Reader',
                        value: 'READER',
                      },
                      {
                        label: 'Editor',
                        value: 'EDITOR',
                      },
                      {
                        label: 'Admin',
                        value: 'ADMIN',
                      },
                    ]}
                    onChange={() => {
                      save();
                    }}
                  />
                );
              },
            },
          },
          {
            dataIndex: 'actions',
            title: 'Actions',
            render(value, record, index) {
              const user = record as User;
              return (
                <Space size="middle">
                  <Button
                    disabled={user.id === me?.id}
                    danger
                    type="text"
                    onClick={() => {
                      // TODO
                    }}>
                    Remove from model
                  </Button>
                </Space>
              );
            },
          },
        ]}
      />
    </div>
  );
};

export default ModelAccess;
