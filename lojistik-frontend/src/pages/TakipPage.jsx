import { useState, useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import "./TakipPage.css";

const API = "http://localhost:5085/api/kargo";

export default function TakipPage() {
  const [searchParams] = useSearchParams();
  const [takipNo, setTakipNo] = useState(searchParams.get("no") || "");
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const no = searchParams.get("no");
    if (no) { setTakipNo(no); sorgula(no); }
  }, []);

  const sorgula = async (no) => {
    const query = no || takipNo.trim();
    if (!query) return;
    setLoading(true); setError(null); setResult(null);
    try {
      const res = await fetch(`${API}/takip/${query}`);
      const data = await res.json();
      if (!res.ok) throw new Error(data.mesaj || "Gönderi bulunamadı.");
      setResult(data);
    } catch (err) { setError(err.message); }
    finally { setLoading(false); }
  };

  const handleSubmit = (e) => { e.preventDefault(); sorgula(); };

  const durumIcon = (durum) => {
    switch (durum) {
      case "Sipariş Alındı": return "📋";
      case "Hazırlanıyor": return "📦";
      case "Yolda": return "🚚";
      case "Teslim Edildi": return "✅";
      case "İptal Edildi": return "❌";
      default: return "📌";
    }
  };

  return (
    <div className="takip-page">
      <div className="page-header" style={{ textAlign: "center" }}>
        <h1>🔍 Kargo Takip</h1>
        <p>Takip numaranız ile gönderinizin durumunu sorgulayın</p>
      </div>

      <form onSubmit={handleSubmit} className="takip-form">
        <input type="text" placeholder="Takip numaranızı girin (ör: LT-20260421-1001)" value={takipNo} onChange={(e) => setTakipNo(e.target.value)} className="takip-input" />
        <button type="submit" className="btn-primary-lg" disabled={loading}>
          {loading ? "Sorgulanıyor..." : "Sorgula"}
        </button>
      </form>

      {error && <div className="takip-error">⚠️ {error}</div>}

      {result && (
        <div className="takip-result">
          <div className="takip-header-card">
            <div className="takip-no-big">{result.takipNo}</div>
            <span className={`gonderi-durum ${result.durum === "Teslim Edildi" ? "status-green" : result.durum === "İptal Edildi" ? "status-red" : result.durum === "Yolda" ? "status-purple" : result.durum === "Hazırlanıyor" ? "status-orange" : "status-blue"}`}>
              {result.durum}
            </span>
          </div>

          <div className="takip-details">
            <div className="takip-row"><span>Gönderici</span><strong>{result.gondericiAd}</strong></div>
            <div className="takip-row"><span>Alıcı</span><strong>{result.aliciAd}</strong></div>
            <div className="takip-row"><span>Paket Tipi</span><strong>{result.paketTipi}</strong></div>
            <div className="takip-row"><span>Taşıma Yolu</span><strong>{result.tasimaYolu}</strong></div>
            <div className="takip-row"><span>Tutar</span><strong>₺{result.toplamFiyat?.toLocaleString("tr-TR",{minimumFractionDigits:2})}</strong></div>
          </div>

          {result.durumGecmisi?.length > 0 && (
            <div className="timeline-section">
              <h3>Gönderi Geçmişi</h3>
              <div className="timeline">
                {result.durumGecmisi.map((d, i) => (
                  <div key={i} className={`timeline-item ${i === result.durumGecmisi.length - 1 ? "active" : ""}`}>
                    <div className="timeline-dot">{durumIcon(d.durum)}</div>
                    <div className="timeline-content">
                      <div className="timeline-status">{d.durum}</div>
                      <div className="timeline-desc">{d.aciklama}</div>
                      <div className="timeline-date">{new Date(d.tarih).toLocaleString("tr-TR")}</div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
