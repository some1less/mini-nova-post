import OperatorDashboard from "./OperatorDashboard";
import './AdminDashboard.css';

const AdminDashboard = () => {
    return (
        <div className="admin-dash-container">
            <div className="admin-banner">
                <h2>🛡️ ADMIN CONTROL CENTER</h2>
            </div>

            <OperatorDashboard />

            <div className="admin-section">
                <h2 className="section-title">Personnel Management</h2>
                <div className="admin-card">
                    <h3>List of Operators</h3>
                    <p>Here you can view, add, and delete operators.</p>
                </div>
            </div>
        </div>
    );
};

export default AdminDashboard;