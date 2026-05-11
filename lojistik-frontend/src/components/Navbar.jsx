import { Link, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { useState } from "react";
import "./Navbar.css";

export default function Navbar() {
  const { user, cikis } = useAuth();
  const location = useLocation();
  const [menuOpen, setMenuOpen] = useState(false);

  const isActive = (path) => location.pathname === path;

  const handleCikis = async () => {
    await cikis();
    setMenuOpen(false);
  };

  return (
    <nav className="navbar">
      <div className="navbar-inner">
        <Link to="/" className="navbar-brand" onClick={() => setMenuOpen(false)}>
          <div className="brand-icon">💫</div>
          <span className="brand-text">LogiTech</span>
        </Link>

        <button className="menu-toggle" onClick={() => setMenuOpen(!menuOpen)}>
          {menuOpen ? "✕" : "☰"}
        </button>

        <div className={`navbar-menu ${menuOpen ? "open" : ""}`}>
          <div className="nav-links">
            <Link
              to="/"
              className={`nav-link ${isActive("/") ? "active" : ""}`}
              onClick={() => setMenuOpen(false)}
            >
              Home
            </Link>
            <Link
              to="/create-shipment"
              className={`nav-link ${isActive("/create-shipment") ? "active" : ""}`}
              onClick={() => setMenuOpen(false)}
            >
              Create Shipment
            </Link>
            <Link
              to="/track"
              className={`nav-link ${isActive("/track") ? "active" : ""}`}
              onClick={() => setMenuOpen(false)}
            >
              Track Shipment
            </Link>
            {user && user.role !== "Admin" && (
              <Link
                to="/my-shipments"
                className={`nav-link ${isActive("/my-shipments") ? "active" : ""}`}
                onClick={() => setMenuOpen(false)}
              >
                My Shipments
              </Link>
            )}
            {user && user.role === "Admin" && (
              <Link
                to="/admin"
                className={`nav-link ${isActive("/admin") ? "active" : ""}`}
                onClick={() => setMenuOpen(false)}
              >
                Admin Dashboard
              </Link>
            )}
          </div>

          <div className="nav-auth">
            {user ? (
              <div className="user-menu">
                <span className="user-name">👤 {user.firstName} {user.lastName}</span>
                <button className="btn-logout" onClick={handleCikis}>
                  Logout
                </button>
              </div>
            ) : (
              <div className="auth-buttons">
                <Link
                  to="/login"
                  className="btn-login"
                  onClick={() => setMenuOpen(false)}
                >
                  Login
                </Link>
                <Link
                  to="/register"
                  className="btn-register"
                  onClick={() => setMenuOpen(false)}
                >
                  Register
                </Link>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}
