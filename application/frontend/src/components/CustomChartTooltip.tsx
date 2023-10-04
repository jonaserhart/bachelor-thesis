import { Typography } from 'antd';
import { useMemo } from 'react';
import { TooltipProps } from 'recharts';
import {
  NameType,
  ValueType,
} from 'recharts/types/component/DefaultTooltipContent';

const CustomChartTooltip = <TValue extends ValueType, TName extends NameType>({
  active,
  payload,
  label,
}: TooltipProps<TValue, TName>) => {
  const displayName = useMemo(() => {
    if (payload?.length) {
      return label ?? payload[0].payload.label ?? payload[0].name;
    }
    return '';
  }, [payload, label]);

  const content = useMemo(() => {
    if (!payload) return undefined;

    if (payload.length > 1) {
      return (
        <>
          <Typography.Title level={5}>{displayName}</Typography.Title>
          {payload.map((p) => {
            return (
              <p
                style={{
                  color: p.payload.fill ?? p.payload.color,
                }}>{`${p.name} : ${p.value}`}</p>
            );
          })}
        </>
      );
    } else if (payload.length > 0) {
      return (
        <p
          style={{
            color: payload[0].payload.fill ?? payload[0].payload.color,
          }}>{`${displayName} : ${payload[0].value}`}</p>
      );
    } else {
      return undefined;
    }
  }, [payload, displayName]);

  if (active && payload && payload.length) {
    return (
      <div
        style={{
          backgroundColor: 'rgba(0, 0, 0, 0.85)',
          paddingInline: 10,
          paddingTop: 3,
          paddingBottom: 3,
        }}>
        {content}
      </div>
    );
  }

  return null;
};

export default CustomChartTooltip;
