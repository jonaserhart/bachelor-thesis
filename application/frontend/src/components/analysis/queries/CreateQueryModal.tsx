import { Button, Modal, StepProps, Steps, Typography } from 'antd';
import * as React from 'react';
import { useParams } from 'react-router-dom';
import { useAppSelector } from '../../../app/hooks';
import { selectModel } from '../../../features/analysis/analysisSlice';
import QueryForm from './QueryForm';

const { Title } = Typography;

type Props = {
    open: boolean;
    handleClose: () => void;
}

export default function CreateQueryModal(props: Props) {

    const { open, handleClose } = props;

    const params = useParams();

    const modelId = React.useMemo(() => {
        return params.modelId ?? '';
    }, [params]);

    const model = useAppSelector(selectModel(modelId));
    
    const steps : (StepProps & { content: JSX.Element })[] = [
        {
            title: 'Define query',
            content: <QueryForm/>
        },
        {
            title: 'Select team',
            content: <div>Select a team</div>
        },
        {
            title: 'Review fields',
            content: <div>Review fields</div>
        },
        {
            title: 'Summary',
            content: <div>Summary</div>
        }
    ];
    
    const [currentStep, setCurrentStep] = React.useState(0);

    const next = React.useCallback(() => {
        setCurrentStep(p => p + 1);
    }, []);

    const prev = React.useCallback(() => {
        setCurrentStep(p => p - 1);
    }, []);

    const finish = React.useCallback(() => {
        // TODO
    }, []);

    const handleCancel = React.useCallback(() => {
        handleClose();
        setCurrentStep(0);
    }, [handleClose]);

    const isLastStep = React.useMemo(() => currentStep >= steps.length - 1, [currentStep, steps.length]);

    return (   
      <Modal
        open={open}
        destroyOnClose
        footer={[
            <Button key="back" disabled={currentStep <= 0} onClick={prev}>
              Previous
            </Button>,
            <Button key="submit" type="primary" onClick={isLastStep ? finish : next}>
              {isLastStep ? 'Done' : 'Next'}
            </Button>
          ]}
        onCancel={handleCancel}
        width={1000}
        >
            <Title
                level={4} style={{ marginTop: 0, marginBottom: 30 }}
            >
                {`Add a query for model ${model?.name}`}
            </Title>
            <Steps size='small' items={steps.map(x => ({ ...x }))} current={currentStep}/>
            <div style={{
                minHeight: '300px',
                display: 'flex',
                justifyContent: 'flex-start',
                alignItems: 'center'
            }}>
                {steps[currentStep]?.content}
            </div>
        </Modal>
    )
}