import { useState } from "react";
import { Link, useNavigate, Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import "./GirisPage.css";

export default function KayitPage() {
  const { user, kayit } = useAuth();
  const navigate = useNavigate();
  
  if (user) {
    return <Navigate to="/" replace />;
  }

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
      <div className="auth-card">
        <div className="auth-header">
          <h1>Register</h1>
          <p>Create a free account and start sending cargo</p>
        </div>

        <form onSubmit={handleSubmit} className="auth-form">
          {error && <div className="auth-error">⚠️ {error}</div>}

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="ad">First Name</label>
              <input id="ad" type="text" placeholder="Your name" value={form.ad} onChange={update("ad")} required />
            </div>
            <div className="form-group">
              <label htmlFor="soyad">Last Name</label>
              <input id="soyad" type="text" placeholder="Your surname" value={form.soyad} onChange={update("soyad")} required />
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="email">Email Address</label>
            <input id="email" type="email" placeholder="example@email.com" value={form.email} onChange={update("email")} required />
          </div>

          <div className="form-group">
            <label htmlFor="telefon">Phone Number</label>
            <input id="telefon" type="tel" placeholder="05XX XXX XX XX" value={form.telefon} onChange={update("telefon")} required />
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="sifre">Password</label>
              <input id="sifre" type="password" placeholder="••••••••" value={form.sifre} onChange={update("sifre")} required />
            </div>
            <div className="form-group">
              <label htmlFor="sifreTekrar">Confirm Password</label>
              <input id="sifreTekrar" type="password" placeholder="••••••••" value={form.sifreTekrar} onChange={update("sifreTekrar")} required />
            </div>
          </div>

          <button type="submit" className="btn-primary-lg auth-submit" disabled={loading}>
            {loading ? "Registering..." : "Register"}
          </button>
        </form>

        <div className="auth-footer">
          Already have an account?{" "}
          <Link to="/login">Login</Link>
        </div>
      </div>
    </div>
  );
}
