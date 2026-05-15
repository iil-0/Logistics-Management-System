import { useState, useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import "./TrackPage.css";

const API = "http://localhost:5085/api/cargo";

export default function TrackPage() {
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
      if (!res.ok) throw new Error(data.message || "Shipment not found.");
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
      case "Hazırlanıyor": return "💫";
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
            <div className="takip-no-big">{result.trackingNo}</div>
            <span className={`gonderi-durum ${result.status === "Delivered" || result.status === "Teslim Edildi" ? "status-green" : result.status === "Cancelled" || result.status === "İptal Edildi" ? "status-red" : result.status === "On the Way" || result.status === "Yolda" ? "status-purple" : result.status === "Preparing" || result.status === "Hazırlanıyor" ? "status-orange" : "status-blue"}`}>
              {result.status}
            </span>
          </div>

          <div className="takip-details">
            <div className="takip-row"><span>Sender</span><strong>{result.senderName}</strong></div>
            <div className="takip-row"><span>Receiver</span><strong>{result.receiverName}</strong></div>
            <div className="takip-row"><span>Parcel Type</span><strong>{result.packageType}</strong></div>
            <div className="takip-row"><span>Transport Method</span><strong>{result.transportMethod}</strong></div>
            <div className="takip-row"><span>Total Amount</span><strong>₺{result.totalPrice?.toLocaleString("tr-TR",{minimumFractionDigits:2})}</strong></div>
          </div>

          {result.statusHistory?.length > 0 && (
            <div className="timeline-section">
              <h3>Shipment History</h3>
              <div className="timeline">
                {result.statusHistory.map((d, i) => (
                  <div key={i} className={`timeline-item ${i === result.statusHistory.length - 1 ? "active" : ""}`}>
                    <div className="timeline-dot">{durumIcon(d.status)}</div>
                    <div className="timeline-content">
                      <div className="timeline-status">{d.status}</div>
                      <div className="timeline-desc">{d.description}</div>
                      <div className="timeline-date">{new Date(d.date).toLocaleString("en-US")}</div>
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
