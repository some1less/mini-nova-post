import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, AuthContext } from './context/AuthContext';
import { useContext } from 'react';
import Login from './pages/Login';
import Register from './pages/Register';

import Dashboard from './pages/Dashboard';

import CreatePackage from './pages/CreatePackage';
import PackageDetails from './pages/PackageDetails';

const ProtectedRoute = ({ children }) => {
    const { user } = useContext(AuthContext);
    const token = localStorage.getItem('token');

    if (!token) {
        return <Navigate to="/login" replace />;
    }
    return children;
};

function App() {
    return (
        <AuthProvider>
            <Router>
                <Routes>
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />

                    <Route path="/" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />
                    <Route path="/create-package" element={<ProtectedRoute><CreatePackage /></ProtectedRoute>} />
                    <Route path="/package/:id" element={<ProtectedRoute><PackageDetails /></ProtectedRoute>} />  
                </Routes>
            </Router>
        </AuthProvider>
    );
}

export default App;