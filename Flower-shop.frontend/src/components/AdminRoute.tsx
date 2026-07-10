import React, { type ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

interface AdminRouteProps {
  children: ReactNode;
}

const AdminRoute: React.FC<AdminRouteProps> = ({ children }) => {
    const { isAuthenticated, loading, user } = useAuth();

    if (loading) {
        return (
            <div className="flex justify-center items-center min-h-screen bg-surface">
              <div className="w-8 h-8 border-2 border-primary border-t-transparent rounded-full animate-spin" role="status" />
            </div>
        );
    }

    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    if (user?.role !== 'Admin') {
        return <Navigate to="/" replace />;
    }

    return <>{children}</>;
};

export default AdminRoute;
