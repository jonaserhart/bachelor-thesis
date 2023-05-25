import { useEffect } from "react";
import axios from "../backendClient";

const Dashboard: React.FC = () => {
  useEffect(() => {
    axios
      .get<boolean>("/devops/health")
      .then((r) => console.log(`Backend healthy: ${r.data}`))
      .catch(console.log);
  }, []);

  return <div>Hello</div>;
};

export default Dashboard;
