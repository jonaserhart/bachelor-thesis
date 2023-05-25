import { message } from 'antd';
import { BackendError } from '../app/hooks';
import { getLogger } from './logger';

const logger = getLogger('ErrorHandler');

const handleError = (err: any) => {
  if (err instanceof BackendError) {
    message.error(err.message);
  } else {
    logger.logError(err);
  }
};

export default handleError;
