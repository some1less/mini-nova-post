import {useEffect, useState, useContext} from 'react';
import {AuthContext} from '../context/AuthContext';
import {useNavigate} from 'react-router-dom';
import {getUserRole, getUserName} from '../utils/authUtils';
import './Dashboard.css';

import UserDashboard from './dashboards/UserDashboard';
import OperatorDashboard from './dashboards/OperatorDashboard';
import AdminDashboard from './dashboards/AdminDashboard';

const Dashboard = () => {
    const {logout} = useContext(AuthContext);
    const navigate = useNavigate();
    const [role, setRole] = useState(null);
    const [name, setName] = useState("");

    useEffect(() => {
        const userRole = getUserRole();
        const userName = getUserName();

        if (!userRole) {
            logout();
            navigate('/login');
        } else {
            setRole(userRole);
            setName(userName);
        }
    }, [logout, navigate]);

    const renderDashboardContent = () => {
        switch (role) {
            case 'Admin':
                return <AdminDashboard/>;
            case 'Operator':
                return <OperatorDashboard/>;
            case 'User':
                return <UserDashboard/>;
            default:
                return <div className="loading-text">Loading access rights...</div>;
        }
    };

    return (
        <div className="dashboard-container">
            <header className="dashboard-header">
                <div className="header-left">
                    <h2 className="brand-title">Mini Nova</h2>
                    {role && <span className="role-badge">{role}</span>}
                </div>
                <div className="header-right">
                    <span className="user-greeting">Hello, {name}</span>
                    <button onClick={logout} className="logout-btn">Logout</button>
                </div>
            </header>

            <main className="dashboard-main">
                {renderDashboardContent()}
            </main>
        </div>
    );
};

export default Dashboard;