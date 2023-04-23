import { FormInstance } from "antd";
import * as React from "react";

export const EditableContext = React.createContext<FormInstance<any> | null>(null);
