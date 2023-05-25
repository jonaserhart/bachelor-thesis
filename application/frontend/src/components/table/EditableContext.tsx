import { FormInstance } from 'antd';
import React from 'react';

export const EditableContext = React.createContext<FormInstance<any> | null>(
  null
);
