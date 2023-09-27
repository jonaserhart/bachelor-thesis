import { useContext, useMemo, useState } from 'react';
import { ModelContext } from '../../../context/ModelContext';
import CustomTable from '../../table/CustomTable';
import { BackendError, useAppSelector } from '../../../app/hooks';
import {
  deleteKPI,
  selectAllKPIs,
} from '../../../features/analysis/analysisSlice';
import { ReportContext } from '../../../context/ReportContext';
import { Button, Space, Tag, Tree, Typography, message, theme } from 'antd';
import { EditOutlined } from '@ant-design/icons';
import { Expression } from 'typescript';
import { KPI, KPIFolder } from '../../../features/analysis/types';
import {
  getAllKPIsInFolder,
  mapToFolderStructure,
} from '../../../util/kpiFolderUtils';

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
    <div
      style={{
        display: 'flex',
        flexDirection: 'row',
        justifyContent: 'start',
        alignItems: 'start',
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
                  return kpiValues[value];
                }
                return 'Unknown value';
              },
            },
          ]}
        />
      </div>
    </div>
  );
};

export default KpiReportListDisplay;
