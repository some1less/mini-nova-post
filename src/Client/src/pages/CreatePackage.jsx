import { useState, useEffect } from 'react';
import apiClient from '../api/apiClient';
import { useNavigate } from 'react-router-dom';
import { Box, MapPin, User, Scale, ArrowLeft } from 'lucide-react';
import './CreatePackage.css';

const CreatePackage = () => {
    const navigate = useNavigate();
    const [destinations, setDestinations] = useState([]);

    const [formData, setFormData] = useState({
        receiverEmail: '',
        description: '',
        size: 'S',
        weight: '',
        destinationId: ''
    });

    useEffect(() => {
        apiClient.get('/destinations').then(res => setDestinations(res.data));
    }, []);

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await apiClient.post('/packages', {
                ...formData,
                weight: parseFloat(formData.weight),
                destinationId: parseInt(formData.destinationId)
            });
            navigate('/');
        } catch (error) {
            console.error("Error:", error);

            let errorMessage = "Error creating package.";

            if (error.response) {
                if (error.response.data.errors) {
                    errorMessage = Object.values(error.response.data.errors).flat().join('\n');
                } else if (error.response.data.error) {
                    errorMessage = error.response.data.error;
                }
            }

            alert(errorMessage);
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

                <form onSubmit={handleSubmit}>
                    <div className="form-row">
                        <div className="form-group">
                            <label><User size={16}/> Receiver Email</label>
                            <input
                                name="receiverEmail" type="email" placeholder="receiver@email.com" onChange={handleChange} required
                            />
                        </div>
                    </div>

                    <div className="form-group">
                        <label><Box size={16}/> Description</label>
                        <input name="description" type="text" placeholder="What's inside?" onChange={handleChange} required />
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label><Scale size={16}/> Weight (kg)</label>
                            <input name="weight" type="number" step="0.1" placeholder="0.0" onChange={handleChange} required />
                        </div>
                        <div className="form-group">
                            <label><Box size={16}/> Size</label>
                            <select name="size" onChange={handleChange} value={formData.size}> {/* Додав value для контрольованого інпута */}
                                <option value="S">Small (S)</option>
                                <option value="M">Medium (M)</option>
                                <option value="L">Large (L)</option>
                                <option value="XL">Extra Large (XL)</option>
                            </select>
                        </div>
                    </div>

                    <div className="form-group">
                        <label><MapPin size={16}/> Destination</label>
                        <select name="destinationId" onChange={handleChange} required>
                            <option value="">Select a city...</option>
                            {destinations.map(d => (
                                <option key={d.id} value={d.id}>{d.address}</option>
                            ))}
                        </select>
                    </div>

                    <button type="submit" className="submit-btn">Create Shipment</button>
                </form>
            </div>
        </div>
    );
};

export default CreatePackage;