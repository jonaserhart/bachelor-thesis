export type ValuesType =
  | 'number'
  | 'string'
  | 'numberArray'
  | 'stringArray'
  | 'range'
  | 'any';

export type SingleValue = string | number;
export type ValueArray = SingleValue[];
export type NumberRange = { from: number; to: number };

export type AcceptableValuesType =
  | SingleValue
  | ValueArray
  | NumberRange
  | 'any';

export function acceptableValuesToString(values: AcceptableValuesType): string {
  if (typeof values === 'string') {
    return `string:${values}`;
  } else if (typeof values === 'number') {
    return `number:${values}`;
  } else if (Array.isArray(values)) {
    const arrayValues = values.map((item) => {
      if (typeof item === 'string') {
        return `string:${item}`;
      } else if (typeof item === 'number') {
        return `number:${item}`;
      } else {
        throw new Error(`Unsupported array item type: ${typeof item}`);
      }
    });
    return `Array[${arrayValues.join(',')}]`;
  } else if (
    typeof values === 'object' &&
    values !== null &&
    'from' in values &&
    'to' in values
  ) {
    return `Range[${values.from}:${values.to}]`;
  } else {
    return 'any';
  }
}

export function isAcceptable(value: string | number, acceptableValues: string) {
  const values = stringToAcceptableValues(`${acceptableValues}`);

  switch (values.type) {
    case 'string':
    case 'number':
      return values.value === value;
    case 'numberArray':
      if (Array.isArray(values.value) && typeof value === 'number') {
        return values.value.includes(value);
      }
      return false;
    case 'stringArray':
      if (Array.isArray(values.value) && typeof value === 'string') {
        return values.value.includes(value);
      }
      return false;
    case 'range':
      const range = values.value as NumberRange;
      if (!range || typeof value !== 'number') return false;
      return value <= range.to && value >= range.from;
    case 'any':
      return true;
  }
}

export function stringToAcceptableValues(valueString: string): {
  type: ValuesType;
  value: SingleValue | ValueArray | NumberRange | 'any';
} {
  if (valueString === 'any') {
    return { type: 'any', value: 'any' };
  }

  const arrayMatch = valueString.match(/^Array\[(.*)]$/);
  if (arrayMatch) {
    const arrayValues = arrayMatch[1].split(',');
    const parsedValues = arrayValues.map((item) => {
      const [itemType, itemValue] = item.split(':');
      return parseValue(itemType as ValuesType, itemValue);
    });

    const arrayType: ValuesType = parsedValues.every(
      (item) => typeof item === 'number'
    )
      ? 'numberArray'
      : 'stringArray';

    return { type: arrayType, value: parsedValues as ValueArray };
  }

  const rangeMatch = valueString.match(/^Range\[(.*):(.*)]$/);
  if (rangeMatch) {
    const from = parseFloat(rangeMatch[1]);
    const to = parseFloat(rangeMatch[2]);
    return { type: 'range', value: { from, to } };
  }

  const [valueType, value] = valueString.split(':');
  return {
    type: valueType as ValuesType,
    value: parseValue(valueType as ValuesType, value),
  };
}

function parseValue(
  type: ValuesType,
  value: string
): SingleValue | ValueArray | NumberRange {
  switch (type) {
    case 'string':
      return value;
    case 'number':
      return parseFloat(value);
    default:
      throw new Error(`Unsupported value type: ${type}`);
  }
}

export function compareAcceptableValues(
  value1: AcceptableValuesType,
  value2: AcceptableValuesType
): boolean {
  if (typeof value1 !== typeof value2) {
    return false;
  }

  if (
    typeof value1 === 'string' ||
    typeof value1 === 'number' ||
    typeof value1 === 'boolean'
  ) {
    return value1 === value2;
  }

  if (Array.isArray(value1) && Array.isArray(value2)) {
    if (value1.length !== value2.length) {
      return false;
    }

    for (let i = 0; i < value1.length; i++) {
      if (!compareAcceptableValues(value1[i], value2[i])) {
        return false;
      }
    }

    return true;
  }

  if (
    typeof value1 === 'object' &&
    value1 !== null &&
    'from' in value1 &&
    'to' in value1
  ) {
    //@ts-ignore
    return value1.from === value2.from && value1.to === value2.to;
  }

  return false;
}
