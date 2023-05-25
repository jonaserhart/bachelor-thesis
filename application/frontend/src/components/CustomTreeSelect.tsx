import { Button, Select, Space, TreeSelect, message } from "antd";
import { HasId, HierachyItem } from "../features/analysis/types";
import axios from "../backendClient";
import { useCallback, useEffect, useMemo, useState } from "react";

interface Props<T> {
  request: string;
  title: string;
  onSubmit: (val: T | undefined) => void;
  labelSelector?: (val: T) => string;
  loading: boolean;
}

interface TreeSelectOption {
  value: string;
  title: string;
  selectable?: boolean;
  children: TreeSelectOption[];
}

const CustomTreeSelect = <T,>(props: Props<HierachyItem<T>>) => {
  const {
    onSubmit,
    request,
    labelSelector,
    title,
    loading: submitLoading,
  } = props;

  const [options, setOptions] = useState<HierachyItem<T>[]>([]);
  const [selected, setSelected] = useState<string>("");
  const [loading, setLoading] = useState(false);

  const selectLabel = useMemo(
    () => labelSelector ?? ((val: HasId) => val.id),
    [labelSelector]
  );

  useEffect(() => {
    setLoading(true);
    axios
      .get<HierachyItem<T>[]>(request)
      .then((response) => {
        console.log(response.data);
        setOptions(response.data);
      })
      .catch((err) => message.error(err.error))
      .finally(() => setLoading(false));
  }, []);

  const findByIdRec = useCallback(
    (id: string, objs: HierachyItem<T>[]): HierachyItem<T> | undefined => {
      const idArray = id.split("/");

      for (const obj of objs) {
        if (obj.id === idArray[0]) {
          if (idArray.length === 1) {
            return obj;
          } else if (obj.children) {
            const foundObj = findByIdRec(
              idArray.slice(1).join("/"),
              obj.children
            );
            if (foundObj) {
              return foundObj;
            }
          }
        }
      }

      return undefined;
    },
    []
  );

  const findById = useCallback(
    (id: string) => {
      const foundObject = findByIdRec(id, options);
      return foundObject;
    },
    [options]
  );

  const selectedOption = useMemo(() => {
    return findById(selected);
  }, [selected]);

  const mapOption = useCallback(
    (o: HierachyItem<T>, id: string = ""): TreeSelectOption => {
      const prefix = id.length > 0 ? `${id}/` : "";
      const idPath = `${prefix}${o.id}`;
      return {
        value: idPath,
        title: selectLabel(o),
        selectable: !o.hasChildren,
        children: o.children?.map((c) => mapOption(c, idPath)) ?? [],
      };
    },
    [selectLabel]
  );

  return (
    <Space direction="vertical" style={{ width: "400px" }}>
      <TreeSelect
        loading={loading || submitLoading}
        showSearch
        style={{ width: "100%" }}
        value={selected}
        dropdownStyle={{ maxHeight: 400, overflow: "auto" }}
        placeholder={title}
        allowClear
        onChange={(id: string) => {
          setSelected(id);
        }}
        treeData={options.map((o) => mapOption(o))}
      />
      <div
        style={{
          width: "100%",
          display: "flex",
          flexDirection: "row-reverse",
        }}
      >
        <Button
          loading={submitLoading || loading}
          disabled={selected === ""}
          type="primary"
          ghost
          onClick={() => onSubmit(selectedOption)}
        >
          Create
        </Button>
      </div>
    </Space>
  );
};

export default CustomTreeSelect;
