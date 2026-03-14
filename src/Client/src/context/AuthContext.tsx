import { createContext, useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';
import apiClient from '../api/apiClient';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    const getUserFromToken = (decoded) => {
        return {
            login: decoded.sub || decoded.name || "User",
            role: decoded.role || "User",
            id: decoded.nameid
        };
    };

    useEffect(() => {
        const token = sessionStorage.getItem('token');
        if (token) {
            try {
                const decoded = jwtDecode(token);

                console.log("Token Decoded on Refresh:", decoded);

                if (decoded.exp * 1000 < Date.now()) {
                    logout();
                } else {
                    setUser(getUserFromToken(decoded));
                }
            } catch (error) {
                console.error("Token decode error:", error);
                logout();
            }
        }
        setLoading(false);
    }, []);

    const login = (token) => {
        sessionStorage.setItem('token', token);
        const decoded = jwtDecode(token);

        console.log("Token Decoded on Login:", decoded);

        setUser(getUserFromToken(decoded));
    };

    const logout = () => {
        sessionStorage.removeItem('token');
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, login, logout, loading }}>
            {!loading && children}
        </AuthContext.Provider>
    );
};