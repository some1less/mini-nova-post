import { createContext, useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';
import apiClient from '../api/apiClient';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    const getUserFromToken = (decoded) => {
        return {
            // looking for login:
            // 1. decoded.name (JwtRegisteredClaimNames.Name...))
            // 2. long url 
            // 3. decoded.sub (subject standard)
            login: decoded.name ||
                decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ||
                decoded.sub,

            // looking for role:
            // 1. long url (I used ClaimTypes.Role)
            // 2. short decoded.role
            role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
                decoded.role ||
                "User"
        };
    };

    useEffect(() => {
        const token = localStorage.getItem('token');
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
        localStorage.setItem('token', token);
        const decoded = jwtDecode(token);

        console.log("Token Decoded on Login:", decoded);

        setUser(getUserFromToken(decoded));
    };

    const logout = () => {
        localStorage.removeItem('token');
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, login, logout, loading }}>
            {!loading && children}
        </AuthContext.Provider>
    );
};