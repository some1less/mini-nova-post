import { useState } from 'react';
import apiClient from '../api/apiClient';
import { useNavigate, Link } from 'react-router-dom';
import { UserPlus } from 'lucide-react';

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

        try {
            await apiClient.post('/auth/register', formData);
            alert('Registration successful! Please sign in.');
            navigate('/login');
        } catch (err) {
            console.error("Registration error:", err);

            if (err.response) {
                if (err.response.status === 400 && err.response.data.errors) {
                    const serverErrors = err.response.data.errors;
                    const formattedErrors = {};

                    Object.keys(serverErrors).forEach((key) => {
                        const fieldName = key.charAt(0).toLowerCase() + key.slice(1);
                        formattedErrors[fieldName] = serverErrors[key][0];
                    });

                    setErrors(formattedErrors);
                } else if (typeof err.response.data === 'string') {
                    setGeneralError(err.response.data);
                } else if (err.response.data.message) {
                    setGeneralError(err.response.data.message);
                } else {
                    setGeneralError("Something went wrong. Please check your data.");
                }
            } else {
                setGeneralError("Server is not responding.");
            }
        }
    };

    const inputContainerStyle = { marginBottom: '1rem' };

    const inputStyle = (hasError) => ({
        width: '100%',
        padding: '0.75rem',
        border: `1px solid ${hasError ? '#ef4444' : '#d1d5db'}`,
        borderRadius: '6px',
        fontSize: '14px',
        outline: 'none',
        transition: 'border-color 0.2s'
    });

    const errorTextStyle = {
        color: '#ef4444',
        fontSize: '0.8rem',
        marginTop: '0.25rem',
        display: 'block'
    };

    return (
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', backgroundColor: '#f3f4f6' }}>
            <form onSubmit={handleSubmit} style={{ background: 'white', padding: '2rem', borderRadius: '8px', boxShadow: '0 4px 6px rgba(0,0,0,0.1)', width: '360px' }}>

                <div style={{ display: 'flex', justifyContent: 'center', marginBottom: '1rem', color: '#2563eb' }}>
                    <UserPlus size={40} />
                </div>
                <h2 style={{ textAlign: 'center', marginBottom: '0.5rem', color: '#111827' }}>Create Account</h2>
                <p style={{ textAlign: 'center', marginBottom: '1.5rem', color: '#6b7280', fontSize: '0.9rem' }}>Join the logistics revolution</p>

                {generalError && (
                    <div style={{ backgroundColor: '#fee2e2', color: '#b91c1c', padding: '0.75rem', borderRadius: '6px', marginBottom: '1rem', fontSize: '0.9rem', textAlign: 'center' }}>
                        {generalError}
                    </div>
                )}

                <div style={{ display: 'flex', gap: '0.5rem' }}>
                    <div style={{ width: '50%', marginBottom: '1rem' }}>
                        <input
                            name="firstName"
                            type="text"
                            placeholder="First Name *"
                            onChange={handleChange}
                            style={inputStyle(errors.firstName)}
                            required
                        />
                        {errors.firstName && <span style={errorTextStyle}>{errors.firstName}</span>}
                    </div>
                    <div style={{ width: '50%', marginBottom: '1rem' }}>
                        <input
                            name="lastName"
                            type="text"
                            placeholder="Last Name *"
                            onChange={handleChange}
                            style={inputStyle(errors.lastName)}
                            required
                        />
                        {errors.lastName && <span style={errorTextStyle}>{errors.lastName}</span>}
                    </div>
                </div>

                <div style={inputContainerStyle}>
                    <input
                        name="email"
                        type="email"
                        placeholder="Email *"
                        onChange={handleChange}
                        style={inputStyle(errors.email)}
                        required
                    />
                    {errors.email && <span style={errorTextStyle}>{errors.email}</span>}
                </div>

                <div style={inputContainerStyle}>
                    <input
                        name="phone"
                        type="tel"
                        placeholder="Phone (+48...) (Optional)"
                        onChange={handleChange}
                        style={inputStyle(errors.phone)}
                    />
                    {errors.phone && <span style={errorTextStyle}>{errors.phone}</span>}
                </div>

                <div style={inputContainerStyle}>
                    <input
                        name="login"
                        type="text"
                        placeholder="Login *"
                        onChange={handleChange}
                        style={inputStyle(errors.login)}
                        required
                    />
                    {errors.login && <span style={errorTextStyle}>{errors.login}</span>}
                </div>

                <div style={inputContainerStyle}>
                    <input
                        name="password"
                        type="password"
                        placeholder="Password *"
                        onChange={handleChange}
                        style={inputStyle(errors.password)}
                        required
                    />
                    {errors.password && <span style={errorTextStyle}>{errors.password}</span>}
                </div>

                <button type="submit" style={{ width: '100%', padding: '0.75rem', background: '#2563eb', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: '600', fontSize: '14px', marginTop: '0.5rem' }}>
                    Sign Up
                </button>

                <div style={{ marginTop: '1.5rem', textAlign: 'center', fontSize: '0.875rem', color: '#6b7280' }}>
                    Already have an account?{' '}
                    <Link to="/login" style={{ color: '#2563eb', textDecoration: 'none', fontWeight: '600' }}>
                        Sign In
                    </Link>
                </div>

            </form>
        </div>
    );
};

export default Register;