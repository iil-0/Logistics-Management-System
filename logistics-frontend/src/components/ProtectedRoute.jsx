import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function ProtectedRoute({ children, requireAdmin = false }) {
  const { user, loading } = useAuth();
  const location = useLocation();

  if (loading) {
    return (
      <div style={{ textAlign: "center", padding: "4rem", color: "#9ca3af" }}>
        Loading...
      </div>
    );
  }

  if (!user) {
    return <Navigate to="/login" state={{ redirectedFrom: location.pathname }} replace />;
  }

  if (requireAdmin && user.role !== "Admin") {
    return <Navigate to="/" replace />;
  }

  return children;
}
