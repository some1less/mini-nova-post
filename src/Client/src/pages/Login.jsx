import { useState, useContext } from 'react';
import { AuthContext } from '../context/AuthContext';
import apiClient from '../api/apiClient';
import { useNavigate, Link } from 'react-router-dom';
import { Package } from 'lucide-react';

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
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', backgroundColor: '#f3f4f6' }}>
            <form onSubmit={handleSubmit} style={{ background: 'white', padding: '2rem', borderRadius: '8px', boxShadow: '0 4px 6px rgba(0,0,0,0.1)', width: '320px' }}>
                <div style={{ display: 'flex', justifyContent: 'center', marginBottom: '1rem', color: '#2563eb' }}>
                    <Package size={48} />
                </div>
                <h2 style={{ textAlign: 'center', marginBottom: '1.5rem', color: '#111827' }}>Mini Nova Post</h2>

                {error && <div style={{ color: '#ef4444', marginBottom: '1rem', fontSize: '0.9rem', textAlign: 'center' }}>{error}</div>}

                <div style={{ marginBottom: '1rem' }}>
                    <input
                        type="text"
                        placeholder="Login"
                        value={loginData.login}
                        onChange={(e) => setLoginData({ ...loginData, login: e.target.value })}
                        style={{ width: '100%', padding: '0.75rem', border: '1px solid #d1d5db', borderRadius: '6px', fontSize: '14px' }}
                        required
                    />
                </div>
                <div style={{ marginBottom: '1.5rem' }}>
                    <input
                        type="password"
                        placeholder="Password"
                        value={loginData.password}
                        onChange={(e) => setLoginData({ ...loginData, password: e.target.value })}
                        style={{ width: '100%', padding: '0.75rem', border: '1px solid #d1d5db', borderRadius: '6px', fontSize: '14px' }}
                        required
                    />
                </div>

                <button type="submit" style={{ width: '100%', padding: '0.75rem', background: '#2563eb', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: '600', fontSize: '14px', transition: 'background 0.2s' }}>
                    Sign In
                </button>

                <div style={{ marginTop: '1.5rem', textAlign: 'center', fontSize: '0.875rem', color: '#6b7280' }}>
                    New to Mini Nova?{' '}
                    <Link to="/register" style={{ color: '#2563eb', textDecoration: 'none', fontWeight: '600' }}>
                        Create an account
                    </Link>
                </div>

            </form>
        </div>
    );
};

export default Login;