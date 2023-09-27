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
    if (payload && payload.length) {
      return payload[0].payload.label ?? label ?? payload[0].name;
    }
  }, [payload]);

  if (active && payload && payload.length) {
    return (
      <div
        style={{
          backgroundColor: 'rgba(0, 0, 0, 0.85)',
          paddingInline: 10,
          paddingTop: 3,
          paddingBottom: 3,
        }}>
        <p
          style={{
            color: payload[0].payload.fill,
          }}>{`${displayName} : ${payload[0].value}`}</p>
      </div>
    );
  }

  return null;
};

export default CustomChartTooltip;
