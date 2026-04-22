import { useState } from "react";
import { useAuth } from "../context/AuthContext";
import "./GonderiPage.css";

const API = "http://localhost:5085/api/cargo";

const PAKET_TIPLERI = [
  { value: "Standart", label: "💫 Standard Parcel", desc: "Normal cargo - 50₺", price: "50₺" },
  { value: "Hassas", label: "🔮 Fragile Parcel", desc: "Fragile items - 120₺", price: "120₺" },
  { value: "AgirYuk", label: "🏋️ Heavy Load", desc: "50kg+ industrial - 250₺", price: "250₺" },
];

const EKSTRALAR = [
  { value: "Sigorta", label: "🛡️ Insurance Coverage", price: "+75₺" },
  { value: "HizliTeslimat", label: "⚡ Fast Delivery (24 Hours)", price: "+100₺" },
];

const TASIMA_YOLLARI = [
  { value: "Havayolu", label: "✈️ Airway", desc: "1-2 business days", extra: "+80%", tag: "Express" },
  { value: "Karayolu", label: "🚛 Roadway", desc: "3-5 business days", extra: "+20%", tag: "Standard" },
  { value: "Denizyolu", label: "🚢 Seaway", desc: "7-14 business days", extra: "+10%", tag: "Economic" },
];

export default function GonderiPage() {
  const { user } = useAuth();
  const [form, setForm] = useState({
    aliciAd: "", aliciAdres: "", aliciTelefon: "", aliciSehir: "",
    paketTipi: "Standart", ekstralar: [], tasimaYolu: "Karayolu",
  });
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const update = (field) => (e) =>
    setForm((p) => ({ ...p, [field]: e.target.value }));

  const toggleExtra = (val) =>
    setForm((p) => ({
      ...p,
      ekstralar: p.ekstralar.includes(val)
        ? p.ekstralar.filter((e) => e !== val)
        : [...p.ekstralar, val],
    }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);

    try {
      const res = await fetch(`${API}/create-shipment`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(form),
      });
      const data = await res.json();
      if (!res.ok) throw new Error(data.mesaj || "An error occurred.");
      setResult(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  if (result) {
    return (
      <div className="gonderi-page">
        <div className="result-container">
          <div className="result-success">
            <div className="result-icon">✅</div>
            <h2>Shipment Successfully Created!</h2>
            <div className="takip-no-display">
              <span className="takip-label">Your Tracking Number</span>
              <span className="takip-value">{result.takipNo}</span>
            </div>
            <div className="result-details">
              <div className="detail-row">
                <span>Receiver</span>
                <strong>{result.aliciAd}</strong>
              </div>
              <div className="detail-row">
                <span>Parcel Type</span>
                <strong>{result.paketTipi}</strong>
              </div>
              <div className="detail-row">
                <span>Transport Method</span>
                <strong>{result.tasimaYolu}</strong>
              </div>
              <div className="detail-row">
                <span>Status</span>
                <strong>{result.durum}</strong>
              </div>
              <div className="detail-row total">
                <span>Total Amount</span>
                <strong>₺{result.toplamFiyat?.toLocaleString("tr-TR", { minimumFractionDigits: 2 })}</strong>
              </div>
            </div>
            <button className="btn-primary-lg" onClick={() => { setResult(null); setForm({ aliciAd: "", aliciAdres: "", aliciTelefon: "", aliciSehir: "", paketTipi: "Standart", ekstralar: [], tasimaYolu: "Karayolu" }); }} style={{ marginTop: "1.5rem" }}>
              Create New Shipment
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="gonderi-page">
      <div className="page-header">
        <h1>💫 Create Shipment</h1>
        <p>Enter your cargo details and create your shipment instantly</p>
      </div>

      <form onSubmit={handleSubmit} className="gonderi-form">
        {/* Sender Info */}
        <div className="form-card">
          <h3>👤 Sender Information</h3>
          <div className="sender-info">
            <div className="info-chip">{user?.ad} {user?.soyad}</div>
            <div className="info-chip">{user?.email}</div>
            {user?.telefon && <div className="info-chip">{user?.telefon}</div>}
          </div>
        </div>

        {/* Receiver Info */}
        <div className="form-card">
          <h3>📬 Receiver Information</h3>
          <div className="form-grid">
            <div className="form-group">
              <label>Full Name</label>
              <input type="text" placeholder="Receiver full name" value={form.aliciAd} onChange={update("aliciAd")} required />
            </div>
            <div className="form-group">
              <label>Phone</label>
              <input type="tel" placeholder="05XX XXX XX XX" value={form.aliciTelefon} onChange={update("aliciTelefon")} required />
            </div>
            <div className="form-group">
              <label>City</label>
              <input type="text" placeholder="Istanbul" value={form.aliciSehir} onChange={update("aliciSehir")} required />
            </div>
            <div className="form-group full-width">
              <label>Address</label>
              <textarea placeholder="Full delivery address" value={form.aliciAdres} onChange={update("aliciAdres")} rows={2} required />
            </div>
          </div>
        </div>

        {/* Parcel Type */}
        <div className="form-card">
          <h3>💫 Parcel Type</h3>
          <div className="option-grid">
            {PAKET_TIPLERI.map((p) => (
              <div key={p.value}
                className={`option-card ${form.paketTipi === p.value ? "selected" : ""}`}
                onClick={() => setForm((f) => ({ ...f, paketTipi: p.value }))}>
                <div className="option-label">{p.label}</div>
                <div className="option-desc">{p.desc}</div>
                <div className="option-price">{p.price}</div>
              </div>
            ))}
          </div>
        </div>

        {/* Additional Services */}
        <div className="form-card">
          <h3>✨ Additional Services</h3>
          <div className="checkbox-list">
            {EKSTRALAR.map((e) => (
              <div key={e.value}
                className={`checkbox-card ${form.ekstralar.includes(e.value) ? "checked" : ""}`}
                onClick={() => toggleExtra(e.value)}>
                <input type="checkbox" checked={form.ekstralar.includes(e.value)} onChange={(ev) => ev.stopPropagation()} />
                <span className="cb-label">{e.label}</span>
                <span className="cb-price">{e.price}</span>
              </div>
            ))}
          </div>
        </div>

        {/* Transport Method */}
        <div className="form-card">
          <h3>🚚 Transport Method</h3>
          <div className="option-grid">
            {TASIMA_YOLLARI.map((t) => (
              <div key={t.value}
                className={`option-card ${form.tasimaYolu === t.value ? "selected" : ""}`}
                onClick={() => setForm((f) => ({ ...f, tasimaYolu: t.value }))}>
                <div className="option-label">{t.label}</div>
                <div className="option-desc">{t.desc} · Extra: {t.extra}</div>
                <div className="option-tag">{t.tag}</div>
              </div>
            ))}
          </div>
        </div>

        {error && <div className="auth-error">⚠️ {error}</div>}

        <button type="submit" className="btn-primary-lg submit-btn" disabled={loading}>
          {loading ? "Creating..." : "💫 Create Shipment"}
        </button>
      </form>
    </div>
  );
}
