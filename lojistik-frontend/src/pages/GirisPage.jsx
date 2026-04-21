import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import "./GirisPage.css";

export default function GirisPage() {
  const { giris } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [sifre, setSifre] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      await giris(email, sifre);
      navigate("/");
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-header">
          <div className="auth-icon">🔑</div>
          <h1>Giriş Yap</h1>
          <p>Hesabınıza giriş yaparak gönderilerinizi yönetin</p>
        </div>

        <form onSubmit={handleSubmit} className="auth-form">
          {error && <div className="auth-error">⚠️ {error}</div>}

          <div className="form-group">
            <label htmlFor="email">E-posta Adresi</label>
            <input
              id="email"
              type="email"
              placeholder="ornek@email.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="sifre">Şifre</label>
            <input
              id="sifre"
              type="password"
              placeholder="••••••••"
              value={sifre}
              onChange={(e) => setSifre(e.target.value)}
              required
            />
          </div>

          <button type="submit" className="btn-primary-lg auth-submit" disabled={loading}>
            {loading ? "Giriş yapılıyor..." : "Giriş Yap"}
          </button>
        </form>

        <div className="auth-footer">
          Hesabınız yok mu?{" "}
          <Link to="/kayit">Kayıt Ol</Link>
        </div>
      </div>
    </div>
  );
}
