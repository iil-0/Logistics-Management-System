import { Link } from "react-router-dom";
import "./Footer.css";

export default function Footer() {
  return (
    <footer className="site-footer">
      <div className="footer-inner">
        <div className="footer-grid">
          <div className="footer-col">
            <div className="footer-brand">
              <span className="footer-logo">📦</span>
              <span className="footer-brand-text">LogiTech</span>
            </div>
            <p className="footer-desc">
              Güvenilir, hızlı ve ekonomik kargo gönderim hizmetleri.
            </p>
          </div>

          <div className="footer-col">
            <h4>Hızlı Erişim</h4>
            <Link to="/">Ana Sayfa</Link>
            <Link to="/gonderi">Gönderi Oluştur</Link>
            <Link to="/takip">Kargo Takip</Link>
          </div>

          <div className="footer-col">
            <h4>Hesap</h4>
            <Link to="/giris">Giriş Yap</Link>
            <Link to="/kayit">Kayıt Ol</Link>
            <Link to="/gonderilerim">Gönderilerim</Link>
          </div>

          <div className="footer-col">
            <h4>İletişim</h4>
            <span>📞 0850 123 45 67</span>
            <span>📧 destek@logitech-kargo.com</span>
            <span>📍 İstanbul, Türkiye</span>
          </div>
        </div>

        <div className="footer-bottom">
          <span>© 2026 LogiTech Kargo. Tüm hakları saklıdır.</span>
        </div>
      </div>
    </footer>
  );
}
