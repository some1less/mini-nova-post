import { useState } from 'react';
import { X } from 'lucide-react';
import apiClient from '../api/apiClient';
import './StatusModal.css';

const StatusModal = ({ pkg, onClose, onUpdateSuccess }) => {
    const [newStatus, setNewStatus] = useState(pkg.status);
    const [updating, setUpdating] = useState(false);

    const STATUS_OPTIONS = ["Registered", "Submitted", "In Transit", "Delivered", "Canceled"];

    const STATUS_MAP = {
        "Registered": 1,
        "Submitted": 2,
        "In Transit": 3,
        "Delivered": 4,
        "Canceled": 5
    }
    
    const handleUpdate = async () => {
        if (!newStatus) return;

        setUpdating(true);
        try {
            const statusIdToSend = STATUS_MAP[newStatus];

            if (!statusIdToSend) {
                alert("Invalid status selected");
                return;
            }
            
            await apiClient.post('/trackings', {
                packageId: pkg.id,
                statusId: statusIdToSend
            });

            onUpdateSuccess(pkg.id, newStatus);
            onClose();
        } catch (error) {
            console.error("Failed to update status", error);
            alert("Error updating status.");
        } finally {
            setUpdating(false);
        }
    };

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                <div className="modal-header">
                    <h3>Update Status</h3>
                    <button className="close-btn" onClick={onClose}>
                        <X size={20} />
                    </button>
                </div>

                <div className="modal-body">
                    <p className="modal-desc">
                        Changing status for package <strong>#{pkg.id}</strong>
                    </p>

                    <div className="form-group">
                        <label>New Status</label>
                        <select
                            className="modal-select"
                            value={newStatus}
                            onChange={(e) => setNewStatus(e.target.value)}
                        >
                            {STATUS_OPTIONS.map(status => (
                                <option key={status} value={status}>{status}</option>
                            ))}
                        </select>
                    </div>
                </div>

                <div className="modal-footer">
                    <button className="modal-btn cancel" onClick={onClose}>Cancel</button>
                    <button
                        className="modal-btn confirm"
                        onClick={handleUpdate}
                        disabled={updating}
                    >
                        {updating ? "Updating..." : "Confirm Update"}
                    </button>
                </div>
            </div>
        </div>
    );
};

export default StatusModal;