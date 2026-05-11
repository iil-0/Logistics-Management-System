import { useState, useEffect, useCallback } from "react";
import { useAuth } from "../context/AuthContext";
import "./AdminDashboard.css";

const API = "http://localhost:5085/api/cargo";

const STATUS_ORDER = [
  "Order Received",
  "Preparing",
  "On the Way",
  "Delivered"
];

export default function AdminDashboard() {
  const { user } = useAuth();
  const [shipments, setShipments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [historyStatus, setHistoryStatus] = useState({ canUndo: false, canRedo: false });
  const [actionLoading, setActionLoading] = useState(false);

  const fetchShipments = useCallback(async () => {
    try {
      const res = await fetch(`${API}/all-shipments`, { credentials: "include" });
      if (!res.ok) throw new Error("Failed to fetch shipments.");
      const data = await res.json();
      setShipments(data);
    } catch (err) {
      setError(err.message);
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

  const handleStatusChange = async (trackingNo, targetStatus) => {
    try {
      const res = await fetch(`${API}/update-status/${trackingNo}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(targetStatus),
      });
      const data = await res.json();
      if (!res.ok) throw new Error(data.message || "Update failed.");

      await fetchShipments();
      await fetchHistoryStatus();
    } catch (err) {
      alert(err.message);
    }
  };

  const handleNextStatus = (trackingNo, currentStatus) => {
    const currentIndex = STATUS_ORDER.indexOf(currentStatus);
    if (currentIndex === -1 || currentIndex === STATUS_ORDER.length - 1) return;
    handleStatusChange(trackingNo, STATUS_ORDER[currentIndex + 1]);
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

  const statusColor = (status) => {
    switch (status) {
      case "Order Received": return "status-blue";
      case "Preparing": return "status-orange";
      case "On the Way": return "status-purple";
      case "Delivered": return "status-green";
      case "Cancelled": return "status-red";
      default: return "";
    }
  };

  if (loading) return <div className="admin-page"><p>Loading...</p></div>;
  if (error) return <div className="admin-page"><p className="error">⚠️ {error}</p></div>;

  return (
    <div className="admin-page">
      <div className="page-header">
        <h1>👑 Admin Dashboard</h1>
        <p>Manage all system shipments and update delivery statuses</p>
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

      <div className="admin-stats">
        <div className="stat-box">
          <span className="stat-val">{shipments.length}</span>
          <span className="stat-lbl">Total Shipments</span>
        </div>
        <div className="stat-box">
          <span className="stat-val">{shipments.filter(s => s.status === "Delivered").length}</span>
          <span className="stat-lbl">Completed</span>
        </div>
        <div className="stat-box">
          <span className="stat-val">{shipments.filter(s => s.status === "On the Way").length}</span>
          <span className="stat-lbl">In Transit</span>
        </div>
      </div>

      <div className="shipment-table-container">
        <table className="shipment-table">
          <thead>
            <tr>
              <th>Tracking No</th>
              <th>Customer / Receiver</th>
              <th>Details</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {shipments.map((s) => (
              <tr key={s.trackingNo}>
                <td>
                  <div className="t-no">{s.trackingNo}</div>
                  <div className="t-date">{new Date(s.createdAt).toLocaleDateString()}</div>
                </td>
                <td>
                  <div className="t-person">👤 From: {s.senderName}</div>
                  <div className="t-person">📬 To: {s.receiverName}</div>
                </td>
                <td>
                  <div className="t-info">{s.packageType} · {s.transportMethod}</div>
                  <div className="t-price">₺{s.totalPrice.toLocaleString()}</div>
                </td>
                <td>
                  <span className={`status-badge ${statusColor(s.status)}`}>
                    {s.status}
                  </span>
                </td>
                <td>
                  <div style={{ display: "flex", gap: "0.5rem", flexWrap: "wrap" }}>
                    {s.status !== "Delivered" && s.status !== "Cancelled" && (
                      <button
                        className="btn-next"
                        onClick={() => handleNextStatus(s.trackingNo, s.status)}
                      >
                        Next 🡆
                      </button>
                    )}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
