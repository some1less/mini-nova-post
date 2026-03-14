import { jwtDecode } from "jwt-decode";

export const getUserRole = () => {
    const token = sessionStorage.getItem('token');
    if (!token) return null;
    try {
        const decoded = jwtDecode(token);
        return decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.role || null;
    } catch (error) {
        return null;
    }
};

export const getUserName = () => {
    const token = sessionStorage.getItem('token');
    if (!token) return null;
    try {
        const decoded = jwtDecode(token);
        return decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || decoded.name || decoded.sub || "User";
    } catch (error) {
        return "User";
    }
};