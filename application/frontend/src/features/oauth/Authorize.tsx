import { Button, Card } from "antd";
import axios from "../../backendClient";
import * as React from "react";
import { WindowsOutlined } from "@ant-design/icons";

const keyOptions = ["azure-oauth"] as const;

type KeyOption = (typeof keyOptions)[number];

const Authorize: React.FC = () => {
  const [activeKey, setActiveKey] = React.useState<KeyOption>("azure-oauth");

  const onTabChange = React.useCallback((tab: string) => {
    setActiveKey(tab as KeyOption);
  }, []);

  const onAuthorize = React.useCallback(() => {
    axios.get<string>("/oauth/authorize").then((s) => {
      window.location.href = s.data;
    });
  }, []);

  const contentList: Record<KeyOption, React.ReactNode> = React.useMemo(
    () => ({
      "azure-oauth": (
        <Button onClick={onAuthorize} icon={<WindowsOutlined />}>
          Log in with OAuth
        </Button>
      ),
    }),
    [onAuthorize]
  );

  return (
    <div
      style={{
        height: "100vh",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      <Card
        title="Log in"
        tabList={[
          {
            key: "azure-oauth",
            tab: "Microsoft OAuth",
          },
        ]}
        activeTabKey={activeKey}
        onTabChange={onTabChange}
      >
        {contentList[activeKey]}
      </Card>
    </div>
  );
};

export default Authorize;
