import { useContext, useEffect, useMemo, useState } from 'react';
import RGL, { WidthProvider } from 'react-grid-layout';
import { ModelContext } from '../../../context/ModelContext';
import { Select, Form, Typography, theme, Divider } from 'antd';
import { useAppDispatch } from '../../../app/hooks';
import { getGraphicalConfigDetails } from '../../../features/analysis/analysisSlice';
import GraphicalKPICard from './GraphicalKPICard';
import { Link } from 'react-router-dom';

const ResponsiveReactGridLayout = WidthProvider(RGL);

const { Title } = Typography;

const gridWidth = 12;

const GraphicalReportDisplay: React.FC = () => {
  const { model } = useContext(ModelContext);

  const modelId = useMemo(() => model?.id ?? '', [model]);

  const [selectedLayout, setSelectedLayout] = useState<string | undefined>(
    undefined
  );

  const {
    token: { colorPrimary },
  } = theme.useToken();

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
      <Title level={4} style={{ marginTop: 0 }}>
        Graphical report display
      </Title>
      <Typography>
        Here you can select the graphical dashboards you configured in the{' '}
        <Link
          style={{
            color: colorPrimary,
          }}
          to={`/analyze/${modelId}?tab=settings`}>
          model settings
        </Link>
        .
      </Typography>
      <Typography>
        This is helpful to view the report in a clearly structured manner.
      </Typography>
      <div style={{ marginTop: 32 }}>
        <Form>
          <Form.Item name={['layout']} label="Layout">
            <Select
              style={{ maxWidth: 300 }}
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
        <Divider />
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
    </div>
  );
};

export default GraphicalReportDisplay;
