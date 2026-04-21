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
          <div className="brand-icon">📦</div>
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
              Ana Sayfa
            </Link>
            <Link
              to="/gonderi"
              className={`nav-link ${isActive("/gonderi") ? "active" : ""}`}
              onClick={() => setMenuOpen(false)}
            >
              Gönderi Oluştur
            </Link>
            <Link
              to="/takip"
              className={`nav-link ${isActive("/takip") ? "active" : ""}`}
              onClick={() => setMenuOpen(false)}
            >
              Kargo Takip
            </Link>
            {user && (
              <Link
                to="/gonderilerim"
                className={`nav-link ${isActive("/gonderilerim") ? "active" : ""}`}
                onClick={() => setMenuOpen(false)}
              >
                Gönderilerim
              </Link>
            )}
          </div>

          <div className="nav-auth">
            {user ? (
              <div className="user-menu">
                <span className="user-name">👤 {user.ad} {user.soyad}</span>
                <button className="btn-logout" onClick={handleCikis}>
                  Çıkış Yap
                </button>
              </div>
            ) : (
              <div className="auth-buttons">
                <Link
                  to="/giris"
                  className="btn-login"
                  onClick={() => setMenuOpen(false)}
                >
                  Giriş Yap
                </Link>
                <Link
                  to="/kayit"
                  className="btn-register"
                  onClick={() => setMenuOpen(false)}
                >
                  Kayıt Ol
                </Link>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}
