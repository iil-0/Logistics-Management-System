import { useState, useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import "./TakipPage.css";

const API = "http://localhost:5085/api/cargo";

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
      const res = await fetch(`${API}/track/${query}`);
      const data = await res.json();
      if (!res.ok) throw new Error(data.mesaj || "Shipment not found.");
      setResult(data);
    } catch (err) { setError(err.message); }
    finally { setLoading(false); }
  };

  const handleSubmit = (e) => { e.preventDefault(); sorgula(); };

  const durumIcon = (durum) => {
    switch (durum) {
      case "Order Received":
      case "Sipariş Alındı": return "📋";
      case "Preparing":
      case "Hazırlanıyor": return "📦";
      case "On the Way":
      case "Yolda": return "🚚";
      case "Delivered":
      case "Teslim Edildi": return "✅";
      case "Cancelled":
      case "İptal Edildi": return "❌";
      default: return "📌";
    }
  };

  return (
    <div className="takip-page">
      <div className="page-header" style={{ textAlign: "center" }}>
        <h1>🔍 Track Shipment</h1>
        <p>Track your shipment status with your tracking number</p>
      </div>

      <form onSubmit={handleSubmit} className="takip-form">
        <input type="text" placeholder="Enter tracking number (e.g., LT-20260421-1001)" value={takipNo} onChange={(e) => setTakipNo(e.target.value)} className="takip-input" />
        <button type="submit" className="btn-primary-lg" disabled={loading}>
          {loading ? "Tracking..." : "Track"}
        </button>
      </form>

      {error && <div className="takip-error">⚠️ {error}</div>}

      {result && (
        <div className="takip-result">
          <div className="takip-header-card">
            <div className="takip-no-big">{result.takipNo}</div>
            <span className={`gonderi-durum ${result.durum === "Delivered" || result.durum === "Teslim Edildi" ? "status-green" : result.durum === "Cancelled" || result.durum === "İptal Edildi" ? "status-red" : result.durum === "On the Way" || result.durum === "Yolda" ? "status-purple" : result.durum === "Preparing" || result.durum === "Hazırlanıyor" ? "status-orange" : "status-blue"}`}>
              {result.durum}
            </span>
          </div>

          <div className="takip-details">
            <div className="takip-row"><span>Sender</span><strong>{result.gondericiAd}</strong></div>
            <div className="takip-row"><span>Receiver</span><strong>{result.aliciAd}</strong></div>
            <div className="takip-row"><span>Parcel Type</span><strong>{result.paketTipi}</strong></div>
            <div className="takip-row"><span>Transport Method</span><strong>{result.tasimaYolu}</strong></div>
            <div className="takip-row"><span>Total Amount</span><strong>₺{result.toplamFiyat?.toLocaleString("tr-TR",{minimumFractionDigits:2})}</strong></div>
          </div>

          {result.durumGecmisi?.length > 0 && (
            <div className="timeline-section">
              <h3>Shipment History</h3>
              <div className="timeline">
                {result.durumGecmisi.map((d, i) => (
                  <div key={i} className={`timeline-item ${i === result.durumGecmisi.length - 1 ? "active" : ""}`}>
                    <div className="timeline-dot">{durumIcon(d.durum)}</div>
                    <div className="timeline-content">
                      <div className="timeline-status">{d.durum}</div>
                      <div className="timeline-desc">{d.aciklama}</div>
                      <div className="timeline-date">{new Date(d.tarih).toLocaleString("en-US")}</div>
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
