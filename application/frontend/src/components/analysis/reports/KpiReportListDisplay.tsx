import { useContext, useMemo, useState } from 'react';
import { ModelContext } from '../../../context/ModelContext';
import CustomTable from '../../table/CustomTable';
import { useAppSelector } from '../../../app/hooks';
import { selectAllKPIs } from '../../../features/analysis/analysisSlice';
import { ReportContext } from '../../../context/ReportContext';
import { Divider, Tag, Tree, Typography, theme } from 'antd';
import { KPI, KPIFolder } from '../../../features/analysis/types';
import {
  getAllKPIsInFolder,
  mapToFolderStructure,
} from '../../../util/kpiFolderUtils';
import {
  NumberRange,
  ValueArray,
  isAcceptable,
  stringToAcceptableValues,
} from '../../../util/acceptableValueFunctions';

const { Title } = Typography;

const KpiReportListDisplay: React.FC = () => {
  const { report } = useContext(ReportContext);

  const {
    token: { colorPrimary },
  } = theme.useToken();

  const { model } = useContext(ModelContext);

  const modelId = useMemo(() => model?.id ?? '', [model]);

  const [selectedFolder, setSelectedFolder] = useState<KPIFolder | undefined>();

  const kpiValues = useMemo(() => {
    return report?.reportData?.kpisAndValues ?? {};
  }, [report]);

  const kpis = useAppSelector(selectAllKPIs(modelId));

  const kpiFolders = useMemo(() => {
    return [
      ...(model?.kpiFolders?.map(mapToFolderStructure) ?? []),
      ...(model?.kpis?.map(mapToFolderStructure) ?? []),
    ];
  }, [model]);

  const treeData = useMemo(() => {
    return {
      //@ts-ignore
      name: 'KPIS',
      id: 'root',
      children: [...kpiFolders],
      kpis: model?.kpis ?? [],
    };
  }, [kpiFolders, kpis]);

  const kpisToDisplay = useMemo(() => {
    return getAllKPIsInFolder(model, selectedFolder?.id);
  }, [model, selectedFolder]);

  return (
    <div>
      <Title level={4} style={{ marginTop: 0 }}>
        KPI table
      </Title>
      <Typography>
        This page contains every KPI that was computed in the context of this
        report.
      </Typography>
      <Typography>
        Navigate the folder structure below to view KPIs, their values and if
        they were in the desired range.
      </Typography>
      <Divider />
      <div
        style={{
          display: 'flex',
          flexDirection: 'row',
          justifyContent: 'start',
          alignItems: 'start',
          marginTop: 32,
        }}>
        <div
          style={{
            width: '20%',
            display: treeData.children.length > 0 ? 'block' : 'none',
          }}>
          <Tree
            onSelect={(keys, info) => {
              if (info.selectedNodes.length > 0) {
                if (
                  //@ts-ignore
                  info.selectedNodes[0].id === 'root'
                ) {
                  setSelectedFolder(undefined);
                  return;
                }
                if (
                  //@ts-ignore
                  !info.selectedNodes[0].isKPI
                ) {
                  //@ts-ignore
                  setSelectedFolder(info.selectedNodes[0]);
                } else {
                  //@ts-ignore
                  nav(`/analyze/${modelId}/kpi/${info.selectedNodes[0].id}`);
                }
              }
            }}
            defaultExpandedKeys={['root']}
            defaultSelectedKeys={['root']}
            //@ts-ignore
            treeData={[treeData]}
            titleRender={(node) => {
              return (
                <Title
                  level={5}
                  style={{
                    margin: 0,
                    fontSize: 'medium',
                    fontWeight: 'inherit',
                    color:
                      // @ts-ignore
                      node.id === selectedFolder?.id ||
                      // @ts-ignore
                      (node.id === 'root' && !selectedFolder)
                        ? colorPrimary
                        : 'unset',
                  }}>
                  {
                    // @ts-ignore
                    node.name
                  }
                </Title>
              );
            }}
            showLine={true}
            fieldNames={{
              title: 'name',
              key: 'id',
              children: 'children',
            }}
          />
        </div>
        <div style={{ width: treeData.children.length > 0 ? '80%' : '100%' }}>
          <CustomTable
            dataSource={kpisToDisplay}
            defaultColumns={[
              {
                key: 'name',
                dataIndex: 'name',
                title: 'Name',
                searchable: true,
              },
              {
                key: 'value',
                dataIndex: 'id',
                title: 'Computed Value',
                render(value, record, index) {
                  if (value in kpiValues) {
                    if (Array.isArray(kpiValues[value])) {
                      return `List of length ${kpiValues[value].length}`;
                    }
                    return `${kpiValues[value]} ${record.unit}`;
                  }
                  return 'Unknown value';
                },
              },
              {
                key: 'acceptablevalues',
                dataIndex: 'acceptableValues',
                title: 'Optimal value',
                render(value) {
                  const values = stringToAcceptableValues(`${value}`);

                  switch (values.type) {
                    case 'string':
                      return `"${values.value}"`;
                    case 'number':
                      return values.value.toString();
                    case 'numberArray':
                      return `in: ${(values.value as ValueArray).join(', ')}`;
                    case 'stringArray':
                      return `in: ${(values.value as ValueArray).join(', ')}`;
                    case 'range':
                      const range = values.value as NumberRange;
                      return `Between ${range.from} and ${range.to}`;
                    case 'any':
                      return 'Any value';
                  }
                },
              },
              {
                key: 'isOptimal',
                dataIndex: 'id',
                title: 'Is optimal',
                filters: [
                  {
                    text: 'Yes',
                    value: true,
                  },
                  {
                    text: 'No',
                    value: false,
                  },
                ],
                onFilter(value, record) {
                  const kpi = record as KPI;

                  if (record.id in kpiValues) {
                    const isOptimal = isAcceptable(
                      kpiValues[record.id],
                      kpi.acceptableValues
                    );
                    return isOptimal === value;
                  }

                  return true;
                },
                render(value, record) {
                  const kpi = record as KPI;

                  if (value in kpiValues) {
                    const isOptimal = isAcceptable(
                      kpiValues[value],
                      kpi.acceptableValues
                    );
                    if (isOptimal) return <Tag color="green">Yes</Tag>;
                    else return <Tag color="red">No</Tag>;
                  }
                  return null;
                },
              },
            ]}
          />
        </div>
      </div>
    </div>
  );
};

export default KpiReportListDisplay;
