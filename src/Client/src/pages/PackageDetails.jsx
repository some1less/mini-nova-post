import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import apiClient from '../api/apiClient';
import { Package, MapPin, User, ArrowLeft } from 'lucide-react';
import './PackageDetails.css';

const PackageDetails = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [pkg, setPkg] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchDetails = async () => {
            try {
                const response = await apiClient.get(`/packages/${id}`);
                setPkg(response.data);
            } catch (error) {
                console.error("Error fetching details", error);
            } finally {
                setLoading(false);
            }
        };
        fetchDetails();
    }, [id]);

    if (loading) return <div className="details-loading">Loading details...</div>;
    if (!pkg) return <div className="details-error">Package not found.</div>;

    return (
        <div className="details-container">
            <button onClick={() => navigate('/')} className="back-btn">
                <ArrowLeft size={20} /> Back
            </button>

            <div className="details-card">
                <div className="details-header">
                    <div>
                        <h1 className="pkg-title">Package #{pkg.id}</h1>
                        <span className="status-badge">Active</span>
                    </div>
                    <div className="qr-placeholder">QR</div>
                </div>

                <div className="divider"></div>

                <div className="participants-row">
                    <div className="participant">
                        <span className="label">From</span>
                        <div className="p-info">
                            <User size={18} className="icon"/>
                            <div>
                                <strong>{pkg.sender.fullName}</strong>
                                <div className="sub-text">{pkg.sender.email}</div>
                                <div className="sub-text">{pkg.sender.phone}</div>
                            </div>
                        </div>
                    </div>

                    <div className="arrow">➝</div>

                    <div className="participant">
                        <span className="label">To</span>
                        <div className="p-info">
                            <User size={18} className="icon"/>
                            <div>
                                <strong>{pkg.receiver.fullName}</strong>
                                <div className="sub-text">{pkg.receiver.email}</div>
                                <div className="sub-text">{pkg.receiver.phone}</div>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="divider"></div>

                <div className="info-grid">
                    <div className="info-box">
                        <span className="label">Destination</span>
                        <div className="val-row">
                            <MapPin size={20} />
                            <span>{pkg.destinationAddress}</span>
                        </div>
                    </div>
                    <div className="info-box">
                        <span className="label">Package Info</span>
                        <div className="val-row">
                            <Package size={20} />
                            <span>{pkg.size} • {pkg.weight} kg</span>
                        </div>
                    </div>
                    <div className="info-box full-width">
                        <span className="label">Description</span>
                        <div className="val-row">
                            <span>"{pkg.description}"</span>
                        </div>
                    </div>
                </div>

                <div className="history-placeholder">
                    <h3>Tracking History</h3>

                    {pkg.history && pkg.history.length > 0 ? (
                        <div className="history-list">
                            {pkg.history.map((record) => (
                                <div key={record.id} className="history-item">
                                    <div className="history-main">
                                        <div className="dot"></div>
                                        <div>
                                            <strong>{record.status}</strong>
                                            <div className="sub-text">{record.updateTime}</div>
                                        </div>
                                    </div>

                                    <div className="operator-profile">
                                        <div className="operator-info">
                                            <span className="operator-name">{record.operatorName}</span>
                                            <span className="operator-role">{record.operatorRole}</span>
                                        </div>
                                        
                                        <div className="operator-avatar">
                                            {record.operatorName.slice(0, 2).toUpperCase()}
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    ) : (
                        <p className="sub-text">No tracking updates yet.</p>
                    )}
                </div>
            </div>
        </div>
    );
};

export default PackageDetails;