import './OperatorDashboard.css';

const OperatorDashboard = () => {
    return (
        <div className="operator-dash-container">
            <h1 className="dash-title">Operator Panel</h1>
            <p className="dash-subtitle">Manage all packages in the system.</p>

            <div className="db-panel">
                <h3 className="panel-header">Global Packages Database</h3>
                <p>Table with actions: Change Status | View History</p>
            </div>
        </div>
    );
};

export default OperatorDashboard;