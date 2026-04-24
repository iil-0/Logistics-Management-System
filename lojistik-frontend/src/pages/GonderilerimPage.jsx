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

  const handleIptal = async (trackingNo) => {
    // if (!window.confirm("Are you sure you want to cancel this shipment?")) return;

    try {
      // Show loading state or immediate feedback if desired
      const res = await fetch(`${API}/cancel/${trackingNo}`, {
        method: "POST",
        credentials: "include",
      });
      
      if (!res.ok) {
        const data = await res.json();
        throw new Error(data.message || "Cancellation failed.");
      }

      const updatedShipment = await res.json();
      setGonderiler((prev) =>
        prev.map((g) => (g.trackingNo === trackingNo ? updatedShipment : g))
      );
      alert("Shipment cancelled successfully.");
    } catch (err) {
      console.error(err);
      alert(err.message || "An error occurred during cancellation.");
    }
  };

  const durumRenk = (durum) => {
    switch (durum) {
      case "Order Received": return "status-blue";
      case "Preparing": return "status-orange";
      case "On the Way": return "status-purple";
      case "Delivered": return "status-green";
      case "Cancelled": return "status-red";
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
            💫 Create Shipment
          </Link>
        </div>
      ) : (
        <div className="gonderi-list">
          {gonderiler.map((g) => (
            <div key={g.trackingNo} className="gonderi-card">
              <div className="gonderi-header">
                <div>
                  <div className="gonderi-takip">{g.trackingNo}</div>
                  <div className="gonderi-tarih">
                    {new Date(g.createdAt).toLocaleDateString("en-US", {
                      day: "numeric", month: "long", year: "numeric", hour: "2-digit", minute: "2-digit"
                    })}
                  </div>
                </div>
                <span className={`gonderi-durum ${durumRenk(g.status)}`}>
                  {g.status}
                </span>
              </div>
              <div className="gonderi-body">
                <div className="gonderi-info">
                  <span>📬 {g.receiverName}</span>
                  <span>💫 {g.packageType}</span>
                  <span>🚚 {g.transportMethod}</span>
                </div>
                <div className="gonderi-fiyat">
                  ₺{g.totalPrice?.toLocaleString("tr-TR", { minimumFractionDigits: 2 })}
                </div>
              </div>
              <div className="gonderi-actions" style={{ display: "flex", gap: "10px", marginTop: "1rem" }}>
                {g.isCancellable && (
                  <button className="btn-cancel" onClick={() => handleIptal(g.trackingNo)} style={{ backgroundColor: "#ef4444", color: "white", border: "none", padding: "0.5rem 1rem", borderRadius: "6px", cursor: "pointer" }}>
                    Cancel Shipment
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
