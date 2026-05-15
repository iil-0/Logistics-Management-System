import { Link } from "react-router-dom";
import "./Footer.css";

export default function Footer() {
  return (
    <footer className="site-footer">
      <div className="footer-inner">
        <div className="footer-grid">
          <div className="footer-col">
            <div className="footer-brand">
              <span className="footer-logo">💫</span>
              <span className="footer-brand-text">LogiTech</span>
            </div>
            <p className="footer-desc">
              Reliable, fast, and economical cargo shipping services.
            </p>
          </div>

          <div className="footer-col">
            <h4>Quick Access</h4>
            <Link to="/">Home</Link>
            <Link to="/create-shipment">Create Shipment</Link>
            <Link to="/track">Track Shipment</Link>
          </div>

          <div className="footer-col">
            <h4>Account</h4>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
            <Link to="/my-shipments">My Shipments</Link>
          </div>

          <div className="footer-col">
            <h4>Contact</h4>
            <span>📞 0850 123 45 67</span>
            <span>📧 support@logitech-cargo.com</span>
            <span>📍 Istanbul, Turkey</span>
          </div>
        </div>

        <div className="footer-bottom">
          <span>© 2026 LogiTech Cargo. All rights reserved.</span>
        </div>
      </div>
    </footer>
  );
}
