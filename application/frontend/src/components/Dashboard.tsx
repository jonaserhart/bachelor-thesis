import { useEffect } from "react";
import axios from "../backendClient";

export default function Dashboard() {
  useEffect(() => {
    axios
      .get<boolean>("/devops/health")
      .then((r) => console.log(`Backend healthy: ${r.data}`))
      .catch(console.log);
  }, []);

  return <div>Hello</div>;
}
