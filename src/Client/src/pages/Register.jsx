import { useState } from 'react';
import apiClient from '../api/apiClient';
import { useNavigate, Link } from 'react-router-dom';
import { UserPlus, CheckCircle } from 'lucide-react';
import './Register.css';

const Register = () => {
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        email: '',
        phone: '',
        login: '',
        password: ''
    });

    const [errors, setErrors] = useState({});
    const [generalError, setGeneralError] = useState('');

    const [showSuccess, setShowSuccess] = useState(false);

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
        if (errors[e.target.name]) {
            setErrors({ ...errors, [e.target.name]: '' });
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setErrors({});
        setGeneralError('');

        const payload = {
            ...formData,
            phone: formData.phone.trim() === '' ? null : formData.phone
        };

        try {
            await apiClient.post('/auth/register', payload);

            setShowSuccess(true);
            setTimeout(() => {
                navigate('/login');
            }, 2000);

        } catch (err) {
            console.error("Registration error:", err);

            if (err.response) {
                const data = err.response.data;

               
                if (err.response.status === 400 && data.field && data.detail) {
                    setErrors({
                        [data.field.toLowerCase()]: data.detail
                    });
                }
                else if (err.response.status === 400 && data.errors) {
                    const serverErrors = data.errors;
                    const formattedErrors = {};

                    Object.keys(serverErrors).forEach((key) => {
                        const fieldName = key.charAt(0).toLowerCase() + key.slice(1);
                        formattedErrors[fieldName] = serverErrors[key][0];
                    });

                    setErrors(formattedErrors);
                }
                else if (typeof data === 'string') {
                    setGeneralError(data);
                } else if (data.detail) {
                    setGeneralError(data.detail);
                } else if (data.message) {
                    setGeneralError(data.message);
                } else {
                    setGeneralError("Something went wrong. Please check your data.");
                }
            } else {
                setGeneralError("Server is not responding.");
            }
        }
    };

    return (
        <div className="register-container">
            
            {showSuccess && (
                <div className="success-toast">
                    <CheckCircle size={24} />
                    <span>Registration successful! Redirecting...</span>
                </div>
            )}

            <form onSubmit={handleSubmit} className="register-form">

                <div className="logo-container">
                    <UserPlus size={40} />
                </div>
                <h2 className="register-title">Create Account</h2>
                <p className="register-subtitle">Join the logistics future</p>

                {generalError && (
                    <div className="general-error">
                        {generalError}
                    </div>
                )}

                <div className="input-row">
                    <div className="input-half">
                        <input
                            name="firstName"
                            type="text"
                            placeholder="First Name *"
                            onChange={handleChange}
                            className={`register-input ${errors.firstName ? 'error' : ''}`}
                            required
                        />
                        {errors.firstName && <span className="field-error-text">{errors.firstName}</span>}
                    </div>
                    <div className="input-half">
                        <input
                            name="lastName"
                            type="text"
                            placeholder="Last Name *"
                            onChange={handleChange}
                            className={`register-input ${errors.lastName ? 'error' : ''}`}
                            required
                        />
                        {errors.lastName && <span className="field-error-text">{errors.lastName}</span>}
                    </div>
                </div>

                <div className="input-group">
                    <input
                        name="email"
                        type="email"
                        placeholder="Email *"
                        onChange={handleChange}
                        className={`register-input ${errors.email ? 'error' : ''}`}
                        required
                    />
                    {errors.email && <span className="field-error-text">{errors.email}</span>}
                </div>

                <div className="input-group">
                    <input
                        name="phone"
                        type="tel"
                        placeholder="Phone (+48...) (Optional)"
                        onChange={handleChange}
                        className={`register-input ${errors.phone ? 'error' : ''}`}
                    />
                    {errors.phone && <span className="field-error-text">{errors.phone}</span>}
                </div>

                <div className="input-group">
                    <input
                        name="login"
                        type="text"
                        placeholder="Login *"
                        onChange={handleChange}
                        className={`register-input ${errors.login ? 'error' : ''}`}
                        required
                    />
                    {errors.login && <span className="field-error-text">{errors.login}</span>}
                </div>

                <div className="input-group">
                    <input
                        name="password"
                        type="password"
                        placeholder="Password *"
                        onChange={handleChange}
                        className={`register-input ${errors.password ? 'error' : ''}`}
                        required
                    />
                    {errors.password && <span className="field-error-text">{errors.password}</span>}
                </div>

                <button type="submit" className="register-button">
                    Sign Up
                </button>

                <div className="register-footer">
                    Already have an account?{' '}
                    <Link to="/login" className="register-link">
                        Sign In
                    </Link>
                </div>

            </form>
        </div>
    );
};

export default Register;