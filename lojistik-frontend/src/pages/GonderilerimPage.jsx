import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import "./GonderilerimPage.css";

const API = "http://localhost:5085/api/cargo";

export default function GonderilerimPage() {
  const [gonderiler, setGonderiler] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetch(`${API}/my-shipments`, { credentials: "include" })
      .then((res) => res.json())
      .then((data) => setGonderiler(data))
      .catch(() => setGonderiler([]))
      .finally(() => setLoading(false));
  }, []);

  const handleIptal = async (takipNo) => {
    if (!window.confirm("Are you sure you want to cancel this shipment?")) return;

    const res = await fetch(`${API}/cancel/${takipNo}`, {
      method: "POST",
      credentials: "include",
    });
    const data = await res.json();

    if (res.ok) {
      setGonderiler((prev) =>
        prev.map((g) => (g.takipNo === takipNo ? data : g))
      );
    } else {
      alert(data.mesaj || "Cancellation failed.");
    }
  };

  const durumRenk = (durum) => {
    switch (durum) {
      case "Order Received":
      case "Sipariş Alındı": return "status-blue";
      case "Preparing":
      case "Hazırlanıyor": return "status-orange";
      case "On the Way":
      case "Yolda": return "status-purple";
      case "Delivered":
      case "Teslim Edildi": return "status-green";
      case "Cancelled":
      case "İptal Edildi": return "status-red";
      default: return "";
    }
  };

  if (loading) {
    return <div className="gonderilerim-page"><p style={{ textAlign: "center", padding: "4rem", color: "#9ca3af" }}>Loading...</p></div>;
  }

  return (
    <div className="gonderilerim-page">
      <div className="page-header">
        <h1>📋 My Shipments</h1>
        <p>You can track all your shipments from here</p>
      </div>

      {gonderiler.length === 0 ? (
        <div className="empty-state">
          <div className="empty-icon">📭</div>
          <h3>You haven't created any shipments yet</h3>
          <p>Click the button below to create your first shipment.</p>
          <Link to="/create-shipment" className="btn-primary-lg" style={{ marginTop: "1rem" }}>
            📦 Create Shipment
          </Link>
        </div>
      ) : (
        <div className="gonderi-list">
          {gonderiler.map((g) => (
            <div key={g.takipNo} className="gonderi-card">
              <div className="gonderi-header">
                <div>
                  <div className="gonderi-takip">{g.takipNo}</div>
                  <div className="gonderi-tarih">
                    {new Date(g.olusturulmaTarihi).toLocaleDateString("en-US", {
                      day: "numeric", month: "long", year: "numeric", hour: "2-digit", minute: "2-digit"
                    })}
                  </div>
                </div>
                <span className={`gonderi-durum ${durumRenk(g.durum)}`}>
                  {g.durum}
                </span>
              </div>
              <div className="gonderi-body">
                <div className="gonderi-info">
                  <span>📬 {g.aliciAd}</span>
                  <span>📦 {g.paketTipi}</span>
                  <span>🚚 {g.tasimaYolu}</span>
                </div>
                <div className="gonderi-fiyat">
                  ₺{g.toplamFiyat?.toLocaleString("tr-TR", { minimumFractionDigits: 2 })}
                </div>
              </div>
              {g.iptalEdilabilir && (
                <div className="gonderi-actions">
                  <button className="btn-cancel" onClick={() => handleIptal(g.takipNo)}>
                    Cancel
                  </button>
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
