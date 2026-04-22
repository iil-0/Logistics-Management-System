import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "./HomePage.css";

export default function HomePage() {
  const [takipNo, setTakipNo] = useState("");
  const navigate = useNavigate();

  const handleTakip = (e) => {
    e.preventDefault();
    if (takipNo.trim()) {
      navigate(`/track?no=${takipNo.trim()}`);
    }
  };

  return (
    <div className="home">
      {/* Hero Section */}
      <section className="hero-section">
        <div className="hero-content">
          <h1>
            Secure and Fast <br />
            <span className="gradient-text">Cargo Shipping</span>
          </h1>
          <p className="hero-desc">
            Reliable, insured, and economical cargo services throughout the country. 
            Create your shipment in a few steps, track it instantly.
          </p>
          <div className="hero-actions">
            <Link to="/create-shipment" className="btn-primary-lg">
              💫 Create Shipment
            </Link>
            <Link to="/track" className="btn-outline-lg">
              🔍 Track Shipment
            </Link>
          </div>
        </div>

        {/* Quick Track */}
        <div className="quick-track">
          <form onSubmit={handleTakip} className="track-form">
            <div className="track-label">📍 Quick Shipment Tracking</div>
            <div className="track-input-group">
              <input
                type="text"
                placeholder="Enter your tracking number (e.g., LT-20260421-1001)"
                value={takipNo}
                onChange={(e) => setTakipNo(e.target.value)}
                className="track-input"
              />
              <button type="submit" className="track-btn">
                Track
              </button>
            </div>
          </form>
        </div>
      </section>

      {/* Services */}
      <section className="services-section">
        <h2 className="section-title">Our Services</h2>
        <p className="section-desc">
          We are with you with cargo solutions that suit your needs
        </p>
        <div className="services-grid">
          <div className="service-card">
            <div className="service-icon">✈️</div>
            <h3>Fast Delivery</h3>
            <p>
              Your shipment is at your door within 1-2 business days by air. 
              Ideal solution for your urgent cargo.
            </p>
          </div>
          <div className="service-card">
            <div className="service-icon">🛡️</div>
            <h3>Insured Shipping</h3>
            <p>
              Your shipment is under insurance with full damage protection. 
              Your fragile products are safe.
            </p>
          </div>
          <div className="service-card">
            <div className="service-icon">🚛</div>
            <h3>Economical Shipping</h3>
            <p>
              Affordable shipping with road and sea options. 
              Solutions that fit your budget.
            </p>
          </div>
        </div>
      </section>

      {/* Stats */}
      <section className="stats-section">
        <div className="stats-grid">
          <div className="stat-card">
            <div className="stat-number">10,000+</div>
            <div className="stat-text">Happy Customers</div>
          </div>
          <div className="stat-card">
            <div className="stat-number">81</div>
            <div className="stat-text">City Coverage</div>
          </div>
          <div className="stat-card">
            <div className="stat-number">50,000+</div>
            <div className="stat-text">Delivered Cargo</div>
          </div>
          <div className="stat-card">
            <div className="stat-number">99.8%</div>
            <div className="stat-text">On-time Delivery</div>
          </div>
        </div>
      </section>

      {/* CTA */}
      <section className="cta-section">
        <h2>Get Started Now</h2>
        <p>Create a free account and create your first shipment in minutes.</p>
        <div className="cta-buttons">
          <Link to="/register" className="btn-primary-lg">
            Register for Free
          </Link>
          <Link to="/login" className="btn-outline-lg">
            Login
          </Link>
        </div>
      </section>
    </div>
  );
}
