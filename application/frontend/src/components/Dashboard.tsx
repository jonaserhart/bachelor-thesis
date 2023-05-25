import { useEffect } from 'react';
import axios from '../backendClient';
import { getLogger } from '../util/logger';

const logger = getLogger('Dashboard');

const Dashboard: React.FC = () => {
  useEffect(() => {
    axios
      .get<boolean>('/devops/health')
      .then((r) => logger.logDebug(`Backend healthy: ${r.data}`))
      .catch(logger.logError);
  }, []);

  return <div>Hello</div>;
};

export default Dashboard;
