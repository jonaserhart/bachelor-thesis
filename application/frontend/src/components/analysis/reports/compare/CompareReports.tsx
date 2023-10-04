import { ModelContext } from '../../../../context/ModelContext';
import { useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { ReportContext } from '../../../../context/ReportContext';
import {
  Divider,
  Form,
  Radio,
  RadioChangeEvent,
  Select,
  Tooltip,
  TreeSelect,
  Typography,
} from 'antd';
import {
  getAllKPIs,
  isLeafNode,
  mapToTreeStructureForReport,
} from '../../../../util/kpiFolderUtils';
import CompareTable from './CompareTable';
import { useAppDispatch } from '../../../../app/hooks';
import { getReportDetails } from '../../../../features/analysis/analysisSlice';
import { formatDate } from '../../../../util/formatDate';
import CompareGraphical from './CompareGraphical';

type CompareMode = 'table' | 'graphical';

const { Title } = Typography;

const CompareReports: React.FC = () => {
  const { model } = useContext(ModelContext);
  const { report } = useContext(ReportContext);

  const selectableReports = useMemo(
    () => model?.reports?.filter((x) => x.id !== report?.id) ?? [],
    [model, report]
  );

  const dispatch = useAppDispatch();

  const [selectedReportIds, setSelectedReportIds] = useState<string[]>([]);

  const selectedReports = useMemo(() => {
    if (!report) return [];
    return [
      report,
      ...selectableReports.filter((x) => selectedReportIds.includes(x.id)),
    ]
      .slice()
      .sort((a, b) => b.created - a.created);
  }, [selectableReports, selectedReportIds]);

  const onSelectReports = useCallback(
    (vals: string[]) => {
      setSelectedReportIds(vals);
    },
    [setSelectedReportIds]
  );

  const kpis = useMemo(() => (model ? getAllKPIs(model) : []), [model]);

  const kpiFolders = useMemo(() => {
    return [
      ...(model?.kpiFolders
        ?.filter((x) => x.kpis.some((x) => x.showInReport))
        ?.map(mapToTreeStructureForReport) ?? []),
      ...(model?.kpis
        ?.filter((x) => x.showInReport)
        ?.map(mapToTreeStructureForReport) ?? []),
    ];
  }, [model]);
  const [selectedKPIIds, setSelectedKPIIds] = useState<string[]>([]);

  const [loadingReportdata, setLoadingReportData] = useState(false);

  const selectedKPIs = useMemo(() => {
    return kpis.filter((x) => selectedKPIIds.includes(x.id));
  }, [kpis, selectedKPIIds]);

  const onSelectKPIs = useCallback(
    (vals: string[]) => {
      const isLeafValue = vals.every((value) => isLeafNode(value, kpis));
      if (isLeafValue) {
        setSelectedKPIIds(vals);
      }
    },
    [setSelectedKPIIds, kpis]
  );

  const [selectedMode, setSelectedMode] = useState<CompareMode>('table');

  const onSelectMode = useCallback(
    (e: RadioChangeEvent) => {
      if (['table', 'graphical'].includes(e.target.value)) {
        setSelectedMode(e.target.value);
      }
    },
    [setSelectedMode]
  );

  useEffect(() => {
    selectedReports.forEach((x) => {
      if (!x?.reportData?.kpisAndValues && report && x.id !== report.id) {
        setLoadingReportData(true);
        console.log('TRUE');
        dispatch(getReportDetails({ modelId: model?.id ?? '', reportId: x.id }))
          .then(() => {
            setLoadingReportData(false);
          })
          .catch(() => setLoadingReportData(false));
      }
    });
  }, [selectedReports.length, dispatch, setLoadingReportData, model, report]);

  const reportsAndData = useMemo(() => {
    const vals = selectedReports.map((x) => {
      let obj = {
        reportTitle: x.title,
        created: x.created,
      };
      selectedKPIs.forEach((kpi) => {
        let kpiVal = undefined;
        if (
          x.reportData?.kpisAndValues &&
          kpi.id in x.reportData.kpisAndValues
        ) {
          kpiVal = x.reportData.kpisAndValues[kpi.id];
        }
        let valueToSet = undefined;
        if (kpiVal) {
          valueToSet = Array.isArray(kpiVal)
            ? `List of length ${kpiVal.length}`
            : `${typeof kpiVal === 'object' ? 'Object' : kpiVal}`;
        }
        Object.assign(obj, {
          [kpi.id]: {
            kpiId: kpi.id,
            kpiName: kpi.name,
            kpiUnit: kpi.unit,
            value: valueToSet,
          },
        });
      });
      return obj;
    });

    return {
      reports: vals,
      fields: [
        {
          title: 'Title',
          dataIndex: 'reportTitle',
          key: 'reportTitle',
        },
        {
          title: 'Created',
          dataIndex: 'created',
          key: 'created',
          render: (val: any) => formatDate(val),
          sorter: {
            compare: (a: any, b: any) => a.created - b.created,
          },
          defaultSortOrder: 'descend',
        },
        ...selectedKPIs.map((kpi) => ({
          title: kpi.name,
          dataIndex: kpi.id,
          key: kpi.id,
          render: (val: any) =>
            val?.value ? (
              `${val.value} ${val.kpiUnit}`
            ) : (
              <Tooltip title="Value was not computed in the context of this report">
                Unknown value
              </Tooltip>
            ),
        })),
      ],
    };
  }, [
    selectedKPIs,
    selectedReports.length,
    setLoadingReportData,
    model,
    dispatch,
  ]);

  const content = useMemo(() => {
    switch (selectedMode) {
      case 'table':
        return (
          <CompareTable
            loading={loadingReportdata}
            fields={reportsAndData.fields}
            reportsWithData={reportsAndData.reports}
          />
        );
      case 'graphical':
        return (
          <CompareGraphical
            loading={loadingReportdata}
            fields={reportsAndData.fields}
            reportsWithData={reportsAndData.reports}
          />
        );
    }
  }, [
    selectedMode,
    selectedKPIs,
    selectedReports,
    reportsAndData,
    loadingReportdata,
  ]);

  return (
    <div>
      <Title level={4} style={{ marginTop: 0 }}>
        Compare reports
      </Title>
      <Typography>
        On this page you can compare the current report to other reports.
        Reports can be compared using a table or by using graphs to illustrate a
        trend.
      </Typography>
      <Typography>
        By comparing reports and KPIs you can extract valuable information about
        trends in your data.
      </Typography>
      <div style={{ marginTop: 32 }}>
        <Form disabled={loadingReportdata} layout="inline" autoComplete="off">
          <Form.Item label="Compare mode" style={{ marginBottom: 20 }}>
            <Radio.Group
              onChange={onSelectMode}
              value={selectedMode}
              defaultValue="table">
              <Radio.Button value="table">Table</Radio.Button>
              <Radio.Button value="graphical">Graphical</Radio.Button>
            </Radio.Group>
          </Form.Item>
          <Form.Item label="Reports" style={{ marginBottom: 20 }}>
            <Select
              style={{
                minWidth: 300,
              }}
              mode="multiple"
              allowClear
              value={selectedReportIds}
              onChange={onSelectReports}
              options={selectableReports.map((x) => ({
                value: x.id,
                label: x.title,
              }))}
              placeholder="Select reports to compare this one to"
            />
          </Form.Item>
          <Form.Item label="KPIs to compare" style={{ marginBottom: 20 }}>
            <TreeSelect
              onChange={onSelectKPIs}
              value={selectedKPIIds}
              style={{ minWidth: 300, maxWidth: '50%' }}
              multiple={true}
              treeData={kpiFolders}
              placeholder="Please select some KPIs"
              fieldNames={{
                label: 'name',
                value: 'id',
              }}
            />
          </Form.Item>
        </Form>
      </div>
      <Divider />
      <div>{content}</div>
    </div>
  );
};

export default CompareReports;
