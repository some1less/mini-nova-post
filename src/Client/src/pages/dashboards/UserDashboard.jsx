import { useEffect, useState } from 'react';
import apiClient from '../../api/apiClient';

import { Package, ArrowRight, MapPin, Box, Truck } from 'lucide-react';
import './UserDashboard.css';

import { useNavigate } from 'react-router-dom';

const UserDashboard = () => {
    const navigate = useNavigate();
    
    const [packages, setPackages] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(1);
    const pageSize = 5;

    const fetchPackages = async () => {
        setLoading(true);
        try {
            const response = await apiClient.get(`/packages/my?page=${page}&pageSize=${pageSize}`);
            setPackages(response.data.items);
            setTotalCount(response.data.totalCount);
        } catch (error) {
            console.error("Failed to fetch packages", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchPackages();
    }, [page]);

    const totalPages = Math.ceil(totalCount / pageSize);

    return (
        <div className="user-dash-container">
            <div className="dash-header-row">
                <h1 className="dash-title">📦 My Shipments</h1>
                <button className="create-pkg-btn" onClick={() => navigate('/create-package')}>
                    <Box size={18} />
                    New Shipment
                </button>
            </div>

            {/* Контент */}
            {loading ? (
                <div className="loading-state">Loading your packages...</div>
            ) : packages.length === 0 ? (
                <div className="empty-state">
                    <Package size={48} style={{marginBottom: '1rem', opacity: 0.5}} />
                    <p>No packages found. Create your first one!</p>
                </div>
            ) : (
                <div className="packages-grid">
                    {packages.map((pkg) => (
                        <div key={pkg.id} className="package-card">

                            <div className="card-header">
                                <span className="pkg-id">#{pkg.id}</span>
                                <span className="pkg-status active">{pkg.status}</span>
                            </div>

                            <div className="route-row">
                                <div className="route-point">
                                    <span className="city-label">From</span>
                                    <span className="person-name">{pkg.sender.fullName}</span>
                                </div>
                                <ArrowRight size={16} color="#9ca3af" />
                                <div className="route-point">
                                    <span className="city-label">To</span>
                                    <span className="person-name">{pkg.receiver.fullName}</span>
                                </div>
                            </div>

                            <div className="card-details">
                                <div className="detail-item">
                                    <MapPin size={16} color="#6b7280" style={{minWidth: '16px'}} />
                                    <span>{pkg.destinationAddress}</span>
                                </div>
                                <div className="detail-item">
                                    <Truck size={16} color="#6b7280" style={{minWidth: '16px'}} />
                                    <span>{pkg.weight} kg • {pkg.size} • "{pkg.description}"</span>
                                </div>
                            </div>

                            <button className="track-btn" onClick={() => navigate(`/package/${pkg.id}`)}>
                                View Details
                            </button>
                        </div>
                    ))}
                </div>
            )}

            {totalCount > 0 && (
                <div className="pagination-controls">
                    <button
                        onClick={() => setPage(p => Math.max(1, p - 1))}
                        disabled={page === 1}
                    >
                        Previous
                    </button>

                    <span style={{color: '#4b5563', fontSize: '0.9rem'}}>
                        Page <b>{page}</b> of <b>{totalPages}</b>
                    </span>

                    <button
                        onClick={() => setPage(p => p + 1)}
                        disabled={page >= totalPages}
                    >
                        Next
                    </button>
                </div>
            )}
        </div>
    );
};

export default UserDashboard;