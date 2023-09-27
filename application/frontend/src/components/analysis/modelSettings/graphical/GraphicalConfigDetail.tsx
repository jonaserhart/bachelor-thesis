import {
  BarChartOutlined,
  EditOutlined,
  EyeOutlined,
  FieldStringOutlined,
  NumberOutlined,
  OrderedListOutlined,
  PieChartOutlined,
} from '@ant-design/icons';
import {
  Avatar,
  Button,
  Card,
  Drawer,
  Spin,
  Typography,
  message,
  theme,
} from 'antd';
import { useCallback, useContext, useMemo, useState } from 'react';
import { GraphicalConfigContext } from '../../../../context/GraphicalConfigContext';
import { ModelContext } from '../../../../context/ModelContext';
import { BackendError, useAppDispatch } from '../../../../app/hooks';
import {
  createGraphicalConfigItem,
  deleteGraphicalConfigItem,
  updateGraphicalConfigDetails,
  updateGraphicalConfigItemDetails,
  updateGraphicalItemKPIs,
  updateGraphicalItemProperties,
  updateLayout,
} from '../../../../features/analysis/analysisSlice';
import RGL, { WidthProvider } from 'react-grid-layout';
import {
  GraphicalItemProperties,
  GraphicalReportItemLayout,
  GraphicalReportItemSubmission,
  GraphicalReportItemType,
} from '../../../../features/analysis/types';
import ExampleCard from './ExampleCard';

const ResponsiveReactGridLayout = WidthProvider(RGL);

const { Title } = Typography;

const cardStyles = {
  margin: '20px',
};

const gridWidth = 12;

const GraphicalConfigDetail: React.FC = () => {
  const {
    token: { colorPrimary },
  } = theme.useToken();

  const { model } = useContext(ModelContext);
  const { graphicalConfig, loading } = useContext(GraphicalConfigContext);

  const modelId = useMemo(() => model?.id ?? '', [model]);
  const graphicalConfigId = useMemo(
    () => graphicalConfig?.id ?? '',
    [graphicalConfig]
  );

  const configItems = useMemo(
    () => graphicalConfig?.items ?? [],
    [graphicalConfig]
  );

  const [nameLoading, setNameLoading] = useState(false);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [edit, setEdit] = useState(false);

  const findNextFreePosition = useCallback(
    (newItemSize: { width: number; height: number }) => {
      let nextX = 0;
      let nextY = 0;

      for (const item of configItems) {
        const itemRight = item.layout.x + item.layout.w;
        if (item.layout.y === nextY && itemRight > nextX) {
          nextX = itemRight;
        }
      }

      if (nextX + newItemSize.width > gridWidth) {
        nextX = 0;
        nextY += 1;
      }

      return { x: nextX, y: nextY };
    },
    [configItems]
  );

  const onWidgetSelect = useCallback(
    (
      type: GraphicalReportItemType,
      width: number,
      height: number,
      maxW?: number,
      maxH?: number,
      minW?: number,
      minH?: number
    ) => {
      const position = findNextFreePosition({ width, height });
      const submission: GraphicalReportItemSubmission = {
        layout: {
          h: height,
          w: width,
          maxW,
          maxH,
          minW,
          minH,
          ...position,
        },
        type,
      };
      dispatch(
        createGraphicalConfigItem({ modelId, graphicalConfigId, submission })
      )
        .unwrap()
        .then(() => {
          setDrawerOpen(false);
        })
        .catch((err: BackendError) => message.error(err.message));
    },
    [configItems, setDrawerOpen, modelId, graphicalConfigId]
  );

  const dispatch = useAppDispatch();

  const onNameChange = useCallback(
    (strVal: string) => {
      const newVal = strVal.trim();
      if (!graphicalConfig || !model) return;
      if (graphicalConfig.name !== newVal) {
        setNameLoading(true);
        dispatch(
          updateGraphicalConfigDetails({
            id: graphicalConfig.id,
            name: newVal,
            modelId: model.id,
          })
        )
          .unwrap()
          .catch((err: BackendError) => message.error(err.message))
          .finally(() => setNameLoading(false));
      }
    },
    [graphicalConfig, model, dispatch]
  );

  const onItemNameChange = useCallback(
    (strVal: string, id: string) =>
      new Promise<void>((resolve, reject) => {
        const newVal = strVal.trim();
        const item = configItems.find((x) => x.id === id);
        if (item && item.name !== newVal) {
          dispatch(
            updateGraphicalConfigItemDetails({
              modelId,
              graphicalConfigId,
              id,
              name: newVal,
            })
          )
            .unwrap()
            .then(() => resolve())
            .catch((err: BackendError) => reject(err));
        }
      }),
    [graphicalConfigId, modelId, dispatch, configItems]
  );

  const onItemDelete = useCallback(
    (id: string) =>
      new Promise<void>((resolve, reject) => {
        dispatch(
          deleteGraphicalConfigItem({
            modelId,
            graphicalConfigId,
            id,
          })
        )
          .unwrap()
          .then(() => {
            resolve();
          })
          .catch((err: BackendError) => {
            reject(err);
          });
      }),
    [modelId, graphicalConfigId]
  );

  const onLayoutUpdate = useCallback(
    (newLayout: RGL.Layout[], oldItem: RGL.Layout, newItem: RGL.Layout) => {
      if (
        oldItem.x === newItem.x &&
        oldItem.y === newItem.y &&
        oldItem.h === newItem.h &&
        oldItem.w === newItem.w
      ) {
        return;
      }
      const layoutsToPut: GraphicalReportItemLayout[] = newLayout
        .filter((l) => {
          const item = configItems.find((x) => x.id === l.i);
          if (!item) {
            return false;
          }

          return (
            item.layout.x !== l.x ||
            item.layout.y !== l.y ||
            item.layout.h !== l.h ||
            item.layout.w !== l.w
          );
        })
        .map((layoutItem) => {
          const item = configItems.find((x) => x.id === layoutItem.i);
          const layoutToSet: GraphicalReportItemLayout = {
            id: item!.layout.id,
            ...layoutItem,
          };
          return layoutToSet;
        });

      const promises = layoutsToPut.map(
        (i) =>
          new Promise<number>((resolve, reject) => {
            dispatch(
              updateLayout({
                modelId,
                graphicalConfigId,
                id: i.i,
                layoutSubmission: i,
              })
            )
              .unwrap()
              .then(() => resolve(1))
              .catch((err: BackendError) => reject(err));
          })
      );

      Promise.all(promises)
        .then((n) => {
          console.log(`Updated ${n.length} layouts.`);
        })
        .catch((err: BackendError) =>
          console.error('Error updating layouts: ', err)
        );
    },
    [configItems, dispatch]
  );

  const onKPIsUpdate = useCallback(
    (id: string, kpis: string[]) =>
      new Promise<void>((res, rej) => {
        dispatch(
          updateGraphicalItemKPIs({
            modelId,
            graphicalConfigId,
            id,
            kpis,
          })
        )
          .unwrap()
          .then(() => {
            message.success('Updated kpis!');
            res();
          })
          .catch((err: BackendError) => {
            rej(err);
            message.error(err.message);
          });
      }),
    [modelId, graphicalConfigId]
  );

  const onItemPropsUpdate = useCallback(
    (id: string, graphicalItemProperties: GraphicalItemProperties) =>
      new Promise<void>((res, rej) => {
        dispatch(
          updateGraphicalItemProperties({
            modelId,
            graphicalConfigId,
            id,
            itemProperties: graphicalItemProperties,
          })
        )
          .unwrap()
          .then(() => {
            message.success('Updated properties!');
            res();
          })
          .catch((err: BackendError) => {
            rej(err);
            message.error(err.message);
          });
      }),
    [modelId, graphicalConfigId]
  );

  return (
    <Spin spinning={loading}>
      <div>
        <Title
          editable={{
            onChange: onNameChange,
            icon: nameLoading ? (
              <Spin />
            ) : (
              <EditOutlined style={{ color: colorPrimary }} />
            ),
            tooltip: 'click to edit name',
          }}
          level={4}
          style={{ marginTop: 0 }}>
          {graphicalConfig?.name}
        </Title>
        <div
          style={{
            width: '100%',
            display: 'flex',
            flexDirection: 'row-reverse',
          }}>
          <Button
            ghost
            onClick={() => setEdit(!edit)}
            icon={edit ? <EyeOutlined /> : <EditOutlined />}
            type="primary"
            style={{ marginBottom: 16 }}>
            {edit ? 'Preview' : 'Change layout'}
          </Button>
          {edit && (
            <Button
              ghost
              onClick={() => setDrawerOpen(true)}
              type="primary"
              style={{ marginBottom: 16, marginRight: 16 }}>
              Add new widget
            </Button>
          )}
        </div>
        <ResponsiveReactGridLayout
          measureBeforeMount={true}
          isDraggable={edit}
          draggableHandle=".drag-handle"
          isResizable={edit}
          onDragStop={onLayoutUpdate}
          onResizeStop={onLayoutUpdate}
          cols={gridWidth}
          rowHeight={100}>
          {configItems.map((x) => (
            <div key={x.id} data-grid={x.layout}>
              <ExampleCard
                title={x.name}
                enableEdit={edit}
                onTitleChange={(newTitle) => onItemNameChange(newTitle, x.id)}
                onKPIsChange={(kpis) => onKPIsUpdate(x.id, kpis)}
                selectedKPIsForItem={x.dataSources?.kpis ?? []}
                graphicalItemProperties={x.properties}
                onGraphicalItemPropertiesChange={(itemProps) =>
                  onItemPropsUpdate(x.id, itemProps)
                }
                onDelete={() => onItemDelete(x.id)}
                type={x.type}
              />
            </div>
          ))}
        </ResponsiveReactGridLayout>
        <Drawer
          title="Choose a widget"
          placement="right"
          closable={true}
          onClose={() => setDrawerOpen(false)}
          open={drawerOpen}>
          <div
            style={{
              width: '100%',
              display: 'flex',
              justifyContent: 'center',
              alignItems: 'center',
              flexDirection: 'column',
            }}>
            <Card
              hoverable
              style={cardStyles}
              onClick={() =>
                onWidgetSelect(GraphicalReportItemType.Text, 4, 1, 12, 1, 1, 1)
              }>
              <Card.Meta
                title="Text"
                description="Displays a text helpful for structuring your dashboard"
                avatar={
                  <Avatar
                    style={{ backgroundColor: colorPrimary }}
                    icon={<FieldStringOutlined />}
                  />
                }
              />
            </Card>
            <Card
              hoverable
              style={cardStyles}
              onClick={() =>
                onWidgetSelect(GraphicalReportItemType.Plain, 2, 2, 2, 2, 1, 2)
              }>
              <Card.Meta
                title="Plain kpi value"
                description="Displays the plain value of a kpi including the unit"
                avatar={
                  <Avatar
                    style={{ backgroundColor: colorPrimary }}
                    icon={<NumberOutlined />}
                  />
                }
              />
            </Card>
            <Card
              hoverable
              style={cardStyles}
              onClick={() =>
                onWidgetSelect(GraphicalReportItemType.List, 6, 6, 12, 12, 4, 4)
              }>
              <Card.Meta
                title="List of objects"
                description="Displays the plain value of a kpi in a comprehensive list"
                avatar={
                  <Avatar
                    style={{ backgroundColor: colorPrimary }}
                    icon={<OrderedListOutlined />}
                  />
                }
              />
            </Card>
            <Card
              hoverable
              onClick={() =>
                onWidgetSelect(
                  GraphicalReportItemType.BarChart,
                  3,
                  3,
                  6,
                  6,
                  2,
                  2
                )
              }
              style={cardStyles}>
              <Card.Meta
                title="Bar chart"
                description="Displays the value of one or more kpis as a bar chart"
                avatar={
                  <Avatar
                    style={{ backgroundColor: colorPrimary }}
                    icon={<BarChartOutlined />}
                  />
                }
              />
            </Card>
            <Card
              hoverable
              onClick={() =>
                onWidgetSelect(
                  GraphicalReportItemType.PieChart,
                  3,
                  3,
                  6,
                  6,
                  2,
                  3
                )
              }
              style={cardStyles}>
              <Card.Meta
                title="Pie chart"
                description="Displays the value of one or more kpis as a pie chart"
                avatar={
                  <Avatar
                    style={{ backgroundColor: colorPrimary }}
                    icon={<PieChartOutlined />}
                  />
                }
              />
            </Card>
          </div>
        </Drawer>
      </div>
    </Spin>
  );
};

export default GraphicalConfigDetail;
