import { useState, useEffect } from "react";
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

  useEffect(() => {
    fetchShipments();
  }, []);

  const fetchShipments = async () => {
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
  };

  const handleNextStatus = async (trackingNo, currentStatus) => {
    const currentIndex = STATUS_ORDER.indexOf(currentStatus);
    if (currentIndex === -1 || currentIndex === STATUS_ORDER.length - 1) return;

    const nextStatus = STATUS_ORDER[currentIndex + 1];

    try {
      const res = await fetch(`${API}/update-status/${trackingNo}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(nextStatus),
      });
      const data = await res.json();
      if (!res.ok) throw new Error(data.message || "Update failed.");

      // Update local state
      setShipments((prev) =>
        prev.map((s) => (s.trackingNo === trackingNo ? data : s))
      );
    } catch (err) {
      alert(err.message);
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
                  {s.status !== "Delivered" && s.status !== "Cancelled" && (
                    <button 
                      className="btn-next"
                      onClick={() => handleNextStatus(s.trackingNo, s.status)}
                    >
                      Next Status ➔
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
