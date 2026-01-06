import { useState, useContext } from 'react';
import { AuthContext } from '../context/AuthContext';
import apiClient from '../api/apiClient';
import { useNavigate, Link } from 'react-router-dom';
import { Package } from 'lucide-react';
import './Login.css'; // Імпорт CSS

const Login = () => {
    const [loginData, setLoginData] = useState({ login: '', password: '' });
    const [error, setError] = useState('');
    const { login } = useContext(AuthContext);
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        try {
            const response = await apiClient.post('/auth/login', loginData);
            login(response.data.accessToken);
            navigate('/');
        } catch (err) {
            setError('Invalid login or password');
            console.error(err);
        }
    };

    return (
        <div className="auth-container">
            <form onSubmit={handleSubmit} className="auth-form">
                <div className="logo-container">
                    <Package size={48} />
                </div>
                <h2 className="auth-title">Mini Nova Post</h2>

                {error && <div className="error-message">{error}</div>}

                <div className="input-group">
                    <input
                        className="auth-input"
                        type="text"
                        placeholder="Login"
                        value={loginData.login}
                        onChange={(e) => setLoginData({ ...loginData, login: e.target.value })}
                        required
                    />
                </div>
                <div className="input-group last">
                    <input
                        className="auth-input"
                        type="password"
                        placeholder="Password"
                        value={loginData.password}
                        onChange={(e) => setLoginData({ ...loginData, password: e.target.value })}
                        required
                    />
                </div>

                <button type="submit" className="auth-button">
                    Sign In
                </button>

                <div className="auth-footer">
                    New to Mini Nova?{' '}
                    <Link to="/register" className="auth-link">
                        Create an account
                    </Link>
                </div>
            </form>
        </div>
    );
};

export default Login;