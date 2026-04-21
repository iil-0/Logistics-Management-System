import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "./HomePage.css";

export default function HomePage() {
  const [takipNo, setTakipNo] = useState("");
  const navigate = useNavigate();

  const handleTakip = (e) => {
    e.preventDefault();
    if (takipNo.trim()) {
      navigate(`/takip?no=${takipNo.trim()}`);
    }
  };

  return (
    <div className="home">
      {/* Hero Section */}
      <section className="hero-section">
        <div className="hero-content">
          <h1>
            Güvenli ve Hızlı <br />
            <span className="gradient-text">Kargo Gönderimi</span>
          </h1>
          <p className="hero-desc">
            Türkiye'nin her yerine güvenilir, sigortalı ve ekonomik kargo
            hizmetleri. Gönderinizi birkaç adımda oluşturun, anında takip edin.
          </p>
          <div className="hero-actions">
            <Link to="/gonderi" className="btn-primary-lg">
              📦 Gönderi Oluştur
            </Link>
            <Link to="/takip" className="btn-outline-lg">
              🔍 Kargo Takip
            </Link>
          </div>
        </div>

        {/* Hızlı Takip */}
        <div className="quick-track">
          <form onSubmit={handleTakip} className="track-form">
            <div className="track-label">📍 Hızlı Kargo Takip</div>
            <div className="track-input-group">
              <input
                type="text"
                placeholder="Takip numaranızı girin (ör: LT-20260421-1001)"
                value={takipNo}
                onChange={(e) => setTakipNo(e.target.value)}
                className="track-input"
              />
              <button type="submit" className="track-btn">
                Sorgula
              </button>
            </div>
          </form>
        </div>
      </section>

      {/* Hizmetler */}
      <section className="services-section">
        <h2 className="section-title">Hizmetlerimiz</h2>
        <p className="section-desc">
          İhtiyacınıza uygun kargo çözümleriyle yanınızdayız
        </p>
        <div className="services-grid">
          <div className="service-card">
            <div className="service-icon">✈️</div>
            <h3>Hızlı Teslimat</h3>
            <p>
              Havayolu ile 1-2 iş günü içinde gönderiniz kapınızda. Acil
              kargolarınız için ideal çözüm.
            </p>
          </div>
          <div className="service-card">
            <div className="service-icon">🛡️</div>
            <h3>Sigortalı Gönderim</h3>
            <p>
              Tam hasar koruma güvencesiyle gönderiniz sigorta altında.
              Kırılabilir ürünleriniz güvende.
            </p>
          </div>
          <div className="service-card">
            <div className="service-icon">🚛</div>
            <h3>Ekonomik Kargo</h3>
            <p>
              Karayolu ve denizyolu seçenekleriyle uygun fiyatlı gönderim.
              Bütçenize uygun çözümler.
            </p>
          </div>
        </div>
      </section>

      {/* İstatistikler */}
      <section className="stats-section">
        <div className="stats-grid">
          <div className="stat-card">
            <div className="stat-number">10.000+</div>
            <div className="stat-text">Mutlu Müşteri</div>
          </div>
          <div className="stat-card">
            <div className="stat-number">81</div>
            <div className="stat-text">İl Kapsamı</div>
          </div>
          <div className="stat-card">
            <div className="stat-number">50.000+</div>
            <div className="stat-text">Teslim Edilen Kargo</div>
          </div>
          <div className="stat-card">
            <div className="stat-number">%99.8</div>
            <div className="stat-text">Zamanında Teslimat</div>
          </div>
        </div>
      </section>

      {/* CTA */}
      <section className="cta-section">
        <h2>Hemen Başlayın</h2>
        <p>Ücretsiz hesap oluşturun ve ilk gönderinizi dakikalar içinde oluşturun.</p>
        <div className="cta-buttons">
          <Link to="/kayit" className="btn-primary-lg">
            Ücretsiz Kayıt Ol
          </Link>
          <Link to="/giris" className="btn-outline-lg">
            Giriş Yap
          </Link>
        </div>
      </section>
    </div>
  );
}
