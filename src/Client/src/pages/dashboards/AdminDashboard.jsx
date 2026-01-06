import { useEffect, useState } from 'react';
import OperatorDashboard from "./OperatorDashboard";
import apiClient from '../../api/apiClient';
import { Users, Shield, Briefcase } from 'lucide-react';
import './AdminDashboard.css';

const AdminDashboard = () => {
    const [operators, setOperators] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchOperators = async () => {
            try {
                const response = await apiClient.get('/operators');
                setOperators(response.data);
            } catch (error) {
                console.error("Failed to fetch operators", error);
            } finally {
                setLoading(false);
            }
        };

        fetchOperators();
    }, []);

    const getInitials = (name) => name.split(' ').map(n => n[0]).join('').slice(0, 2).toUpperCase();

    return (
        <div className="admin-dash-container">
            <div className="admin-banner">
                <Shield size={32} />
                <div>
                    <h2>ADMIN CONTROL CENTER</h2>
                    <span className="banner-sub">System Overview & Management</span>
                </div>
            </div>

            <OperatorDashboard />

            <div className="admin-section">
                <div className="section-header">
                    <div className="title-group">
                        <Users size={24} className="text-gray-600"/>
                        <h2 className="section-title">Personnel Management</h2>
                    </div>
                </div>

                <div className="admin-card">
                    {loading ? (
                        <div className="p-8 text-center text-gray-500">Loading personnel data...</div>
                    ) : (
                        <table className="admin-table">
                            <thead>
                            <tr>
                                <th>Employee</th>
                                <th>Role / Occupation</th>
                                <th>System ID</th>
                            </tr>
                            </thead>
                            <tbody>
                            {operators.map((op) => (
                                <tr key={op.id}>
                                    <td>
                                        <div className="user-cell">
                                            <div className="user-avatar">
                                                {getInitials(op.name)}
                                            </div>
                                            <span className="user-name">{op.name}</span>
                                        </div>
                                    </td>
                                    <td>
                                        <div className="role-badge">
                                            <Briefcase size={14} />
                                            {op.occupation}
                                        </div>
                                    </td>
                                    <td className="id-text">OP-{op.id.toString().padStart(4, '0')}</td>
                                </tr>
                            ))}
                            </tbody>
                        </table>
                    )}
                </div>
            </div>
        </div>
    );
};

export default AdminDashboard;