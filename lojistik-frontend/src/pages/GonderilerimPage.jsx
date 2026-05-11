import { useState, useEffect, useCallback } from "react";
import { Link } from "react-router-dom";
import "./GonderilerimPage.css";

const API = "http://localhost:5085/api/cargo";

export default function GonderilerimPage() {
  const [gonderiler, setGonderiler] = useState([]);
  const [loading, setLoading] = useState(true);
  const [historyStatus, setHistoryStatus] = useState({ canUndo: false, canRedo: false });
  const [actionLoading, setActionLoading] = useState(false);

  const fetchShipments = useCallback(async () => {
    try {
      const res = await fetch(`${API}/my-shipments`, { credentials: "include" });
      const data = await res.json();
      setGonderiler(data);
    } catch {
      setGonderiler([]);
    } finally {
      setLoading(false);
    }
  }, []);

  const fetchHistoryStatus = useCallback(async () => {
    try {
      const res = await fetch(`${API}/history-status`, { credentials: "include" });
      if (res.ok) setHistoryStatus(await res.json());
    } catch {
      // sessizce geç
    }
  }, []);

  useEffect(() => {
    fetchShipments();
    fetchHistoryStatus();
  }, [fetchShipments, fetchHistoryStatus]);

  const handleIptal = async (trackingNo) => {
    try {
      const res = await fetch(`${API}/cancel/${trackingNo}`, {
        method: "POST",
        credentials: "include",
      });
      if (!res.ok) {
        const data = await res.json();
        throw new Error(data.message || "Cancellation failed.");
      }
      await fetchShipments();
      await fetchHistoryStatus();
    } catch (err) {
      alert(err.message || "An error occurred during cancellation.");
    }
  };

  const handleUndo = async () => {
    setActionLoading(true);
    try {
      const res = await fetch(`${API}/undo`, { method: "POST", credentials: "include" });
      const data = await res.json().catch(() => ({}));
      if (!res.ok) throw new Error(data.message || `Undo failed (HTTP ${res.status}).`);
      await fetchShipments();
      await fetchHistoryStatus();
    } catch (err) {
      alert(err.message || "Undo failed.");
    } finally {
      setActionLoading(false);
    }
  };

  const handleRedo = async () => {
    setActionLoading(true);
    try {
      const res = await fetch(`${API}/redo`, { method: "POST", credentials: "include" });
      const data = await res.json().catch(() => ({}));
      if (!res.ok) throw new Error(data.message || `Redo failed (HTTP ${res.status}).`);
      await fetchShipments();
      await fetchHistoryStatus();
    } catch (err) {
      alert(err.message || "Redo failed.");
    } finally {
      setActionLoading(false);
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

      <div className="undo-redo-bar" style={{ display: "flex", gap: "10px", marginBottom: "1.5rem", justifyContent: "flex-end" }}>
        <button
          onClick={handleUndo}
          disabled={!historyStatus.canUndo || actionLoading}
          title="Undo last action"
          style={{
            padding: "0.5rem 1rem",
            borderRadius: "6px",
            border: "1px solid #6b7280",
            background: historyStatus.canUndo ? "#374151" : "#1f2937",
            color: historyStatus.canUndo ? "white" : "#6b7280",
            cursor: historyStatus.canUndo && !actionLoading ? "pointer" : "not-allowed"
          }}
        >
          ↶ Undo
        </button>
        <button
          onClick={handleRedo}
          disabled={!historyStatus.canRedo || actionLoading}
          title="Redo last undone action"
          style={{
            padding: "0.5rem 1rem",
            borderRadius: "6px",
            border: "1px solid #6b7280",
            background: historyStatus.canRedo ? "#374151" : "#1f2937",
            color: historyStatus.canRedo ? "white" : "#6b7280",
            cursor: historyStatus.canRedo && !actionLoading ? "pointer" : "not-allowed"
          }}
        >
          ↷ Redo
        </button>
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
