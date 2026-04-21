import { useState } from "react";
import "./App.css";

// ─── Sabitler ────────────────────────────────────────────────────────────────
const API_URL = "http://localhost:5085/api/kargo/hesapla";

const PAKET_TIPLERI = [
  {
    value: "Standart",
    label: "📦 Standart Paket",
    desc: "Normal kargo - 50₺ baz fiyat",
  },
  {
    value: "Hassas",
    label: "🔮 Hassas Paket",
    desc: "Kırılabilir - 120₺ baz fiyat",
  },
  {
    value: "AgirYuk",
    label: "🏋️ Ağır Yük",
    desc: "Endüstriyel - 250₺ baz fiyat",
  },
];

const EKSTRALAR = [
  { value: "Sigorta", label: "🛡️ Sigorta", price: "+75₺" },
  { value: "HizliTeslimat", label: "⚡ Hızlı Teslimat", price: "+100₺" },
];

const TASIMA_YOLLARI = [
  {
    value: "Havayolu",
    label: "✈️ Havayolu",
    desc: "1-2 iş günü",
    badge: "Ekspres",
    badgeClass: "fast",
    extra: "+%80",
  },
  {
    value: "Karayolu",
    label: "🚛 Karayolu",
    desc: "3-5 iş günü",
    badge: "Ekonomik",
    badgeClass: "mid",
    extra: "+%20",
  },
  {
    value: "Denizyolu",
    label: "🚢 Denizyolu",
    desc: "7-14 iş günü",
    badge: "En Ucuz",
    badgeClass: "slow",
    extra: "+%10",
  },
];

// ─── App Bileşeni ─────────────────────────────────────────────────────────────
export default function App() {
  // Form state
  const [paketTipi, setPaketTipi] = useState("Standart");
  const [ekstralar, setEkstralar] = useState([]);
  const [tasimaYolu, setTasimaYolu] = useState("Karayolu");

  // UI state
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);

  // ─── Checkbox Toggle ──────────────────────────────────────────────────────
  const toggleExtra = (val) =>
    setEkstralar((prev) =>
      prev.includes(val) ? prev.filter((e) => e !== val) : [...prev, val],
    );

  // ─── API İsteği ───────────────────────────────────────────────────────────
  const handleHesapla = async () => {
    setLoading(true);
    setError(null);
    setResult(null);

    const body = { paketTipi, ekstralar, tasimaYolu };

    try {
      const res = await fetch(API_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body),
      });

      if (!res.ok) {
        const errData = await res.json().catch(() => ({}));
        throw new Error(errData.mesaj || `Hata: ${res.status}`);
      }

      const data = await res.json();
      setResult(data);
    } catch (err) {
      setError(err.message || "API bağlantı hatası. Backend çalışıyor mu?");
    } finally {
      setLoading(false);
    }
  };

  // ─── Render ───────────────────────────────────────────────────────────────
  return (
    <div className="app">
      {/* Header */}
      <header className="header">
        <div className="header-inner">
          <div className="logo">
            <div className="logo-icon">🚀</div>
            <span className="logo-text gradient-text">LogiTech</span>
          </div>
          <div className="header-badge">
            <div className="pulse-dot" />
            API Aktif
          </div>
        </div>
      </header>

      {/* Main */}
      <main className="main">
        {/* Hero */}
        <section className="hero">
          <div className="hero-eyebrow">🏗️ Design Patterns Demo</div>
          <h1>
            Lojistik <span className="gradient-text">Yönetim Sistemi</span>
          </h1>
          <p className="hero-subtitle">
            Factory · Decorator · Strategy · Observer tasarım desenleri ile
            güçlendirilmiş modern kargo hesaplama platformu
          </p>
          <div className="hero-stats">
            <div className="stat-item">
              <div className="stat-value">4</div>
              <div className="stat-label">Tasarım Deseni</div>
            </div>
            <div className="stat-divider" />
            <div className="stat-item">
              <div className="stat-value">3</div>
              <div className="stat-label">Paket Tipi</div>
            </div>
            <div className="stat-divider" />
            <div className="stat-item">
              <div className="stat-value">3</div>
              <div className="stat-label">Taşıma Yolu</div>
            </div>
            <div className="stat-divider" />
            <div className="stat-item">
              <div className="stat-value">2</div>
              <div className="stat-label">Ek Hizmet</div>
            </div>
          </div>
          {/* Pattern chips */}
          <div
            className="pattern-chips"
            style={{ justifyContent: "center", marginTop: "1.5rem" }}
          >
            <span className="pattern-chip chip-factory">Factory</span>
            <span className="pattern-chip chip-decorator">Decorator</span>
            <span className="pattern-chip chip-strategy">Strategy</span>
            <span className="pattern-chip chip-observer">Observer</span>
          </div>
        </section>

        {/* ─── Form Kartı ─────────────────────────────────────────────── */}
        <div className="card">
          <div className="card-title">
            <div className="card-icon">📋</div>
            Kargo Hesaplama Formu
          </div>

          {/* Paket Tipi */}
          <div className="form-group">
            <label className="form-label">
              📦 Paket Tipi
              <span
                className="pattern-chip chip-factory"
                style={{ marginLeft: 8 }}
              >
                Factory
              </span>
            </label>
            <select
              id="paketTipi"
              className="form-select"
              value={paketTipi}
              onChange={(e) => setPaketTipi(e.target.value)}
            >
              {PAKET_TIPLERI.map((p) => (
                <option key={p.value} value={p.value}>
                  {p.label} — {p.desc}
                </option>
              ))}
            </select>
          </div>

          {/* Ek Hizmetler */}
          <div className="form-group">
            <label className="form-label">
              ✨ Ek Hizmetler
              <span
                className="pattern-chip chip-decorator"
                style={{ marginLeft: 8 }}
              >
                Decorator
              </span>
            </label>
            <div className="checkbox-grid">
              {EKSTRALAR.map((e) => (
                <div
                  key={e.value}
                  className={`checkbox-item ${ekstralar.includes(e.value) ? "checked" : ""}`}
                  onClick={() => toggleExtra(e.value)}
                >
                  <input
                    type="checkbox"
                    id={`extra-${e.value}`}
                    checked={ekstralar.includes(e.value)}
                    onChange={() => toggleExtra(e.value)}
                  />
                  <span className="checkbox-label">{e.label}</span>
                  <span className="checkbox-price">{e.price}</span>
                </div>
              ))}
            </div>
          </div>

          {/* Taşıma Yolu */}
          <div className="form-group">
            <label className="form-label">
              🗺️ Taşıma Yolu
              <span
                className="pattern-chip chip-strategy"
                style={{ marginLeft: 8 }}
              >
                Strategy
              </span>
            </label>
            <div className="radio-group">
              {TASIMA_YOLLARI.map((t) => (
                <div
                  key={t.value}
                  className={`radio-item ${tasimaYolu === t.value ? "selected" : ""}`}
                  onClick={() => setTasimaYolu(t.value)}
                >
                  <input
                    type="radio"
                    id={`tasima-${t.value}`}
                    name="tasimaYolu"
                    value={t.value}
                    checked={tasimaYolu === t.value}
                    onChange={() => setTasimaYolu(t.value)}
                  />
                  <div className="radio-content">
                    <div className="radio-label">{t.label}</div>
                    <div className="radio-desc">
                      {t.desc} · Ek maliyet: {t.extra}
                    </div>
                  </div>
                  <span className={`radio-badge ${t.badgeClass}`}>
                    {t.badge}
                  </span>
                </div>
              ))}
            </div>
          </div>

          {/* Hata Gösterimi */}
          {error && <div className="error-box">⚠️ {error}</div>}

          {/* Submit */}
          <button
            id="btn-hesapla"
            className="btn-calculate"
            onClick={handleHesapla}
            disabled={loading}
          >
            {loading ? (
              <>
                <span className="btn-spin">⚙️</span>
                Hesaplanıyor...
              </>
            ) : (
              <>🔢 Fiyatı Hesapla</>
            )}
          </button>
        </div>

        {/* ─── Sonuç Kartı ─────────────────────────────────────────────── */}
        <div className="card">
          <div className="card-title">
            <div className="card-icon">📊</div>
            Hesaplama Sonucu
            {result && (
              <span
                className="pattern-chip chip-observer"
                style={{ marginLeft: "auto" }}
              >
                Observer
              </span>
            )}
          </div>

          {result ? (
            <div className="result-panel">
              {/* Fiyat */}
              <div className="price-display">
                <div className="price-label">Toplam Tutar</div>
                <div className="price-value">
                  <span className="price-currency">₺</span>
                  {result.toplamFiyat?.toLocaleString("tr-TR", {
                    minimumFractionDigits: 2,
                  })}
                </div>
              </div>

              {/* Durum */}
              <div className="status-badge">✅ {result.kargoDurumu}</div>

              {/* Detaylar */}
              <div className="info-list">
                <div className="info-row">
                  <span className="info-icon">📦</span>
                  <div className="info-content">
                    <div className="info-key">Paket Tipi</div>
                    <div className="info-val">{result.paketTipi}</div>
                  </div>
                </div>

                <div className="info-row">
                  <span className="info-icon">🗺️</span>
                  <div className="info-content">
                    <div className="info-key">Taşıma Yolu</div>
                    <div className="info-val">{result.tasimaYolu}</div>
                  </div>
                </div>

                <div className="info-row">
                  <span className="info-icon">📝</span>
                  <div className="info-content">
                    <div className="info-key">Açıklama</div>
                    <div className="info-val">{result.aciklama}</div>
                  </div>
                </div>

                {result.ekstraHizmetler?.length > 0 && (
                  <div className="info-row">
                    <span className="info-icon">✨</span>
                    <div className="info-content">
                      <div className="info-key">Ek Hizmetler</div>
                      <div className="extras-container">
                        {result.ekstraHizmetler.map((ex, i) => (
                          <span key={i} className="extra-tag">
                            {ex}
                          </span>
                        ))}
                      </div>
                    </div>
                  </div>
                )}

                <div className="info-row">
                  <span className="info-icon">🕐</span>
                  <div className="info-content">
                    <div className="info-key">Oluşturulma</div>
                    <div className="info-val">
                      {new Date(result.olusturulmaTarihi).toLocaleString(
                        "tr-TR",
                      )}
                    </div>
                  </div>
                </div>
              </div>

              {/* Observer Bildirimleri */}
              {result.bildirimler?.length > 0 && (
                <div className="notifications-section">
                  <div className="notif-title">
                    🔔 Sistem Bildirimleri (Observer Pattern)
                  </div>
                  <div className="notif-list">
                    {result.bildirimler.map((b, i) => (
                      <div key={i} className="notif-item">
                        {b}
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>
          ) : (
            <div className="empty-state">
              <div className="empty-state-icon">📭</div>
              <div className="empty-state-title">Henüz hesaplama yapılmadı</div>
              <div className="empty-state-desc">
                Formu doldurup "Fiyatı Hesapla" butonuna basın
              </div>
            </div>
          )}
        </div>
      </main>

      {/* Footer */}
      <footer className="footer">
        <div>
          🏗️ LogiTech — Factory · Decorator · Strategy · Observer Desenleri ile
          .NET Core + React
        </div>
      </footer>
    </div>
  );
}
