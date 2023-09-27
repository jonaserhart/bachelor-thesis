import { useContext, useEffect, useMemo, useState } from 'react';
import RGL, { WidthProvider } from 'react-grid-layout';
import { ModelContext } from '../../../context/ModelContext';
import { Select, Form } from 'antd';
import { useAppDispatch } from '../../../app/hooks';
import { getGraphicalConfigDetails } from '../../../features/analysis/analysisSlice';
import GraphicalKPICard from './GraphicalKPICard';

const ResponsiveReactGridLayout = WidthProvider(RGL);

const gridWidth = 12;

const GraphicalReportDisplay: React.FC = () => {
  const { model } = useContext(ModelContext);

  const modelId = useMemo(() => model?.id ?? '', [model]);

  const [selectedLayout, setSelectedLayout] = useState<string | undefined>(
    undefined
  );

  const availableConfigs = useMemo(() => model?.graphical ?? [], [model]);

  const configItems = useMemo(
    () => availableConfigs.find((x) => x.id === selectedLayout)?.items ?? [],
    [selectedLayout, availableConfigs]
  );

  const dispatch = useAppDispatch();

  useEffect(() => {
    if (selectedLayout) {
      dispatch(
        getGraphicalConfigDetails({
          modelId,
          configId: selectedLayout,
        })
      );
    }
  }, [selectedLayout]);

  return (
    <div>
      <Form>
        <Form.Item name={['layout']} label="Layout">
          <Select
            onChange={(newVal) => setSelectedLayout(newVal)}
            value={selectedLayout}
            placeholder="Please select a layout"
            options={availableConfigs.map((x) => ({
              label: x.name,
              value: x.id,
            }))}
          />
        </Form.Item>
      </Form>
      <ResponsiveReactGridLayout
        measureBeforeMount={true}
        isDraggable={false}
        isResizable={false}
        cols={gridWidth}
        rowHeight={100}>
        {configItems.map((x) => (
          <div key={x.id} data-grid={x.layout}>
            <GraphicalKPICard item={x} />
          </div>
        ))}
      </ResponsiveReactGridLayout>
    </div>
  );
};

export default GraphicalReportDisplay;
