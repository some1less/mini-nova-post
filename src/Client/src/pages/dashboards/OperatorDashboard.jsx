import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import apiClient from '../../api/apiClient';
import { Search, Filter, Eye, Edit3, Truck, ArrowRight } from 'lucide-react';
import './OperatorDashboard.css';

import StatusModal from '../../components/StatusModal';

const OperatorDashboard = () => {
    const navigate = useNavigate();
    const [packages, setPackages] = useState([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState("");

    const [selectedPkg, setSelectedPkg] = useState(null);
    useEffect(() => {
        const fetchPackages = async () => {
            try {
                const response = await apiClient.get('/shipments');
                setPackages(Array.isArray(response.data) ? response.data : response.data.items || []);
            } catch (error) {
                console.error("Error fetching packages:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchPackages();
    }, []);

    const handleUpdateSuccess = (id, newStatus) => {
        setPackages(prev => prev.map(p =>
            p.id === id ? { ...p, status: newStatus } : p
        ));
    };

    const getStatusClass = (status) => {
        switch (status?.toLowerCase()) {
            case 'registered': return 'status-active';
            case 'submitted': return 'status-active';
            case 'delivered': return 'status-finished';
            case 'canceled': return 'status-canceled';
            case 'in transit': return 'status-transit';
            default: return '';
        }
    };

    const filteredPackages = packages.filter(pkg =>
        pkg.id.toString().includes(searchTerm) ||
        pkg.sender?.fullName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        pkg.receiver?.fullName?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const activeCount = packages.filter(p =>
        p.status === 'Submitted' || p.status === 'Registered' || p.status === 'In Transit'
    ).length;

    return (
        <div className="operator-container">
            <div className="op-header">
                <div>
                    <h1 className="op-title">Operator Panel</h1>
                    <p className="op-subtitle">Global Packages Database</p>
                </div>
                <div className="op-stats">
                    <div className="stat-card">
                        <span className="stat-val">{packages.length}</span>
                        <span className="stat-label">Total</span>
                    </div>
                    <div className="stat-card">
                        <span className="stat-val active">{activeCount}</span>
                        <span className="stat-label">Active</span>
                    </div>
                </div>
            </div>

            <div className="op-controls">
                <div className="search-wrapper">
                    <Search size={18} className="search-icon" />
                    <input
                        type="text"
                        placeholder="Search package ID or Name..."
                        className="op-search-input"
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                    />
                </div>
                <button className="filter-btn">
                    <Filter size={18} /> Filter
                </button>
            </div>

            <div className="table-card">
                {loading ? (
                    <div className="p-8 text-center text-gray-500">Loading database...</div>
                ) : (
                    <table className="op-table">
                        <thead>
                        <tr>
                            <th>ID</th>
                            <th>Route</th>
                            <th>Destination</th>
                            <th>Status</th>
                            <th className="text-right">Actions</th>
                        </tr>
                        </thead>
                        <tbody>
                        {filteredPackages.map((pkg) => (
                            <tr key={pkg.id}>
                                <td className="id-cell">#{pkg.id}</td>

                                <td>
                                    <div className="route-cell">
                                        <span className="sender" title={pkg.sender?.fullName}>
                                            {pkg.sender?.fullName}
                                        </span>
                                        <div className="arrow-icon">
                                            <ArrowRight size={16} />
                                        </div>
                                        <span className="receiver" title={pkg.receiver?.fullName}>
                                            {pkg.receiver?.fullName}
                                        </span>
                                    </div>
                                </td>

                                <td className="loc-cell">
                                    <div className="loc-wrapper">
                                        <Truck size={14} />
                                        <span className="truncate max-w-[150px]" title={pkg.destinationAddress}>
                                            {pkg.destinationAddress || "No destination"}
                                        </span>
                                    </div>
                                </td>

                                <td>
                                    <span className={`pkg-status ${getStatusClass(pkg.status)}`}>
                                        {pkg.status}
                                    </span>
                                </td>

                                <td className="actions-cell">
                                    <button className="action-btn secondary" onClick={() => navigate(`/package/${pkg.id}`)}>
                                        <Eye size={16} /> View
                                    </button>

                                    <button className="action-btn primary" onClick={() => setSelectedPkg(pkg)}>
                                        <Edit3 size={16} /> Update
                                    </button>
                                </td>
                            </tr>
                        ))}
                        {filteredPackages.length === 0 && !loading && (
                            <tr>
                                <td colSpan="5" className="text-center p-8 text-gray-400">
                                    No packages found.
                                </td>
                            </tr>
                        )}
                        </tbody>
                    </table>
                )}
            </div>

            {selectedPkg && (
                <StatusModal
                    pkg={selectedPkg}
                    onClose={() => setSelectedPkg(null)}
                    onUpdateSuccess={handleUpdateSuccess}
                />
            )}
        </div>
    );
};

export default OperatorDashboard;