import { useState } from "react";
import { useAuth } from "../context/AuthContext";
import "./GonderiPage.css";

const API = "http://localhost:5085/api/kargo";

const PAKET_TIPLERI = [
  { value: "Standart", label: "📦 Standart Paket", desc: "Normal kargo - 50₺", price: "50₺" },
  { value: "Hassas", label: "🔮 Hassas Paket", desc: "Kırılabilir ürünler - 120₺", price: "120₺" },
  { value: "AgirYuk", label: "🏋️ Ağır Yük", desc: "50kg+ endüstriyel - 250₺", price: "250₺" },
];

const EKSTRALAR = [
  { value: "Sigorta", label: "🛡️ Sigorta Güvencesi", price: "+75₺" },
  { value: "HizliTeslimat", label: "⚡ Hızlı Teslimat (24 Saat)", price: "+100₺" },
];

const TASIMA_YOLLARI = [
  { value: "Havayolu", label: "✈️ Havayolu", desc: "1-2 iş günü", extra: "+%80", tag: "Ekspres" },
  { value: "Karayolu", label: "🚛 Karayolu", desc: "3-5 iş günü", extra: "+%20", tag: "Standart" },
  { value: "Denizyolu", label: "🚢 Denizyolu", desc: "7-14 iş günü", extra: "+%10", tag: "Ekonomik" },
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
      const res = await fetch(`${API}/gonderi-olustur`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(form),
      });
      const data = await res.json();
      if (!res.ok) throw new Error(data.mesaj || "Bir hata oluştu.");
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
            <h2>Gönderi Başarıyla Oluşturuldu!</h2>
            <div className="takip-no-display">
              <span className="takip-label">Takip Numaranız</span>
              <span className="takip-value">{result.takipNo}</span>
            </div>
            <div className="result-details">
              <div className="detail-row">
                <span>Alıcı</span>
                <strong>{result.aliciAd}</strong>
              </div>
              <div className="detail-row">
                <span>Paket Tipi</span>
                <strong>{result.paketTipi}</strong>
              </div>
              <div className="detail-row">
                <span>Taşıma Yolu</span>
                <strong>{result.tasimaYolu}</strong>
              </div>
              <div className="detail-row">
                <span>Durum</span>
                <strong>{result.durum}</strong>
              </div>
              <div className="detail-row total">
                <span>Toplam Tutar</span>
                <strong>₺{result.toplamFiyat?.toLocaleString("tr-TR", { minimumFractionDigits: 2 })}</strong>
              </div>
            </div>
            <button className="btn-primary-lg" onClick={() => { setResult(null); setForm({ aliciAd: "", aliciAdres: "", aliciTelefon: "", aliciSehir: "", paketTipi: "Standart", ekstralar: [], tasimaYolu: "Karayolu" }); }} style={{ marginTop: "1.5rem" }}>
              Yeni Gönderi Oluştur
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="gonderi-page">
      <div className="page-header">
        <h1>📦 Gönderi Oluştur</h1>
        <p>Kargo bilgilerinizi girin ve gönderinizi hemen oluşturun</p>
      </div>

      <form onSubmit={handleSubmit} className="gonderi-form">
        {/* Gönderici Bilgileri */}
        <div className="form-card">
          <h3>👤 Gönderici Bilgileri</h3>
          <div className="sender-info">
            <div className="info-chip">{user?.ad} {user?.soyad}</div>
            <div className="info-chip">{user?.email}</div>
            {user?.telefon && <div className="info-chip">{user?.telefon}</div>}
          </div>
        </div>

        {/* Alıcı Bilgileri */}
        <div className="form-card">
          <h3>📬 Alıcı Bilgileri</h3>
          <div className="form-grid">
            <div className="form-group">
              <label>Ad Soyad</label>
              <input type="text" placeholder="Alıcı adı soyadı" value={form.aliciAd} onChange={update("aliciAd")} required />
            </div>
            <div className="form-group">
              <label>Telefon</label>
              <input type="tel" placeholder="05XX XXX XX XX" value={form.aliciTelefon} onChange={update("aliciTelefon")} required />
            </div>
            <div className="form-group">
              <label>Şehir</label>
              <input type="text" placeholder="İstanbul" value={form.aliciSehir} onChange={update("aliciSehir")} required />
            </div>
            <div className="form-group full-width">
              <label>Adres</label>
              <textarea placeholder="Açık adres" value={form.aliciAdres} onChange={update("aliciAdres")} rows={2} required />
            </div>
          </div>
        </div>

        {/* Paket Tipi */}
        <div className="form-card">
          <h3>📦 Paket Tipi</h3>
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

        {/* Ek Hizmetler */}
        <div className="form-card">
          <h3>✨ Ek Hizmetler</h3>
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

        {/* Taşıma Yolu */}
        <div className="form-card">
          <h3>🚚 Taşıma Yolu</h3>
          <div className="option-grid">
            {TASIMA_YOLLARI.map((t) => (
              <div key={t.value}
                className={`option-card ${form.tasimaYolu === t.value ? "selected" : ""}`}
                onClick={() => setForm((f) => ({ ...f, tasimaYolu: t.value }))}>
                <div className="option-label">{t.label}</div>
                <div className="option-desc">{t.desc} · Ek: {t.extra}</div>
                <div className="option-tag">{t.tag}</div>
              </div>
            ))}
          </div>
        </div>

        {error && <div className="auth-error">⚠️ {error}</div>}

        <button type="submit" className="btn-primary-lg submit-btn" disabled={loading}>
          {loading ? "Oluşturuluyor..." : "📦 Gönderiyi Oluştur"}
        </button>
      </form>
    </div>
  );
}
