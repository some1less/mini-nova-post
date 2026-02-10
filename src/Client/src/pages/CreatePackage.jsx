import { useState, useEffect } from 'react';
import apiClient from '../api/apiClient';
import { useNavigate } from 'react-router-dom';
import { Box, MapPin, User, Scale, ArrowLeft, AlertCircle } from 'lucide-react';
import './CreatePackage.css';

const CreatePackage = () => {
    const navigate = useNavigate();
    const [locations, setLocations] = useState([]);
    const [error, setError] = useState(null);

    const sizeMapping = {
        'S': 1,
        'M': 2,
        'L': 3,
        'XL': 4
    };

    const [formData, setFormData] = useState({
        consigneeEmail: '',
        description: '',
        size: 'S',
        weight: '',
        originId: '',
        destinationId: ''
    });

    useEffect(() => {
        apiClient.get('/locations')
            .then(res => setLocations(res.data))
            .catch(err => console.error("Failed to load locations", err));
    }, []);

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
        if (error) setError(null);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);

        const payload = {
            ConsigneeEmail: formData.consigneeEmail,
            Description: formData.description,
            SizeId: sizeMapping[formData.size],
            Weight: formData.weight ? parseFloat(formData.weight) : 0,
            OriginId: parseInt(formData.originId),
            DestinationId: parseInt(formData.destinationId),
        };

        try {
            await apiClient.post('/shipments', payload);
            navigate('/');
        } catch (err) {
            console.error("Error creating shipment:", err);

            let errorMessage = "Error creating package.";

            if (err.response && err.response.data) {
                const data = err.response.data;

                if (data.detail) {
                    errorMessage = data.detail;
                }
                else if (data.errors) {
                    errorMessage = Object.values(data.errors).flat().join('\n');
                }
                else if (data.title) {
                    errorMessage = data.title;
                }
                else if (typeof data === 'string') {
                    errorMessage = data;
                }
            } else {
                errorMessage = "Server is not responding.";
            }

            setError(errorMessage);
        }
    };

    return (
        <div className="create-page-container">
            <button onClick={() => navigate('/')} className="back-btn">
                <ArrowLeft size={20} /> Back to Dashboard
            </button>

            <div className="create-card">
                <h1 className="create-title">New Shipment</h1>
                <p className="create-subtitle">Fill in the details below</p>

                {error && (
                    <div className="error-banner">
                        <AlertCircle size={20} />
                        <span style={{ whiteSpace: 'pre-line' }}>{error}</span>
                    </div>
                )}

                <form onSubmit={handleSubmit}>

                    <div className="form-group">
                        <label><User size={16}/> Consignee Email</label>
                        <input
                            name="consigneeEmail"
                            type="email"
                            placeholder="receiver@email.com"
                            onChange={handleChange}
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label><Box size={16}/> Description</label>
                        <input
                            name="description"
                            type="text"
                            placeholder="What's inside?"
                            onChange={handleChange}
                            required
                        />
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label><Scale size={16}/> Weight (kg)</label>
                            <input
                                name="weight"
                                type="number"
                                step="0.1"
                                min="0.1"
                                max="40"
                                placeholder="0.0"
                                onChange={handleChange}
                                required
                            />
                        </div>

                        <div className="form-group">
                            <label><Box size={16}/> Size</label>
                            <select name="size" onChange={handleChange} value={formData.size}>
                                <option value="S">Small (S)</option>
                                <option value="M">Medium (M)</option>
                                <option value="L">Large (L)</option>
                                <option value="XL">Extra Large (XL)</option>
                            </select>
                        </div>
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label><MapPin size={16}/> From (Origin)</label>
                            <select name="originId" onChange={handleChange} required defaultValue="">
                                <option value="" disabled>Select origin...</option>
                                {locations.map(l => (
                                    <option key={l.id} value={l.id}>{l.city}, {l.address}</option>
                                ))}
                            </select>
                        </div>

                        <div className="form-group">
                            <label><MapPin size={16}/> To (Destination)</label>
                            <select name="destinationId" onChange={handleChange} required defaultValue="">
                                <option value="" disabled>Select destination...</option>
                                {locations.map(l => (
                                    <option key={l.id} value={l.id}>{l.city}, {l.address}</option>
                                ))}
                            </select>
                        </div>
                    </div>

                    <button type="submit" className="submit-btn">Create Shipment</button>
                </form>
            </div>
        </div>
    );
};

export default CreatePackage;