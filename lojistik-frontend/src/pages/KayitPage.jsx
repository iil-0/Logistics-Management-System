import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import "./GirisPage.css";

export default function KayitPage() {
  const { kayit } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState({
    ad: "", soyad: "", email: "", telefon: "", sifre: "", sifreTekrar: "",
  });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const update = (field) => (e) =>
    setForm((prev) => ({ ...prev, [field]: e.target.value }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    if (form.sifre !== form.sifreTekrar) {
      setError("Şifreler eşleşmiyor.");
      return;
    }
    if (form.sifre.length < 4) {
      setError("Şifre en az 4 karakter olmalıdır.");
      return;
    }

    setLoading(true);
    try {
      await kayit(form.ad, form.soyad, form.email, form.telefon, form.sifre);
      navigate("/");
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-card" style={{ maxWidth: 480 }}>
        <div className="auth-header">
          <div className="auth-icon">📝</div>
          <h1>Kayıt Ol</h1>
          <p>Ücretsiz hesap oluşturun ve kargo göndermeye başlayın</p>
        </div>

        <form onSubmit={handleSubmit} className="auth-form">
          {error && <div className="auth-error">⚠️ {error}</div>}

          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1rem" }}>
            <div className="form-group">
              <label htmlFor="ad">Ad</label>
              <input id="ad" type="text" placeholder="Adınız" value={form.ad} onChange={update("ad")} required />
            </div>
            <div className="form-group">
              <label htmlFor="soyad">Soyad</label>
              <input id="soyad" type="text" placeholder="Soyadınız" value={form.soyad} onChange={update("soyad")} required />
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="email">E-posta Adresi</label>
            <input id="email" type="email" placeholder="ornek@email.com" value={form.email} onChange={update("email")} required />
          </div>

          <div className="form-group">
            <label htmlFor="telefon">Telefon</label>
            <input id="telefon" type="tel" placeholder="05XX XXX XX XX" value={form.telefon} onChange={update("telefon")} />
          </div>

          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1rem" }}>
            <div className="form-group">
              <label htmlFor="sifre">Şifre</label>
              <input id="sifre" type="password" placeholder="••••••••" value={form.sifre} onChange={update("sifre")} required />
            </div>
            <div className="form-group">
              <label htmlFor="sifreTekrar">Şifre Tekrar</label>
              <input id="sifreTekrar" type="password" placeholder="••••••••" value={form.sifreTekrar} onChange={update("sifreTekrar")} required />
            </div>
          </div>

          <button type="submit" className="btn-primary-lg auth-submit" disabled={loading}>
            {loading ? "Kaydediliyor..." : "Kayıt Ol"}
          </button>
        </form>

        <div className="auth-footer">
          Zaten hesabınız var mı?{" "}
          <Link to="/giris">Giriş Yap</Link>
        </div>
      </div>
    </div>
  );
}
