import { Routes, Route, Navigate } from "react-router-dom";
import Navbar from "./components/Navbar";
import Footer from "./components/Footer";
import ProtectedRoute from "./components/ProtectedRoute";
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import CreateShipmentPage from "./pages/CreateShipmentPage";
import MyShipmentsPage from "./pages/MyShipmentsPage";
import TrackPage from "./pages/TrackPage";
import AdminDashboard from "./pages/AdminDashboard";

export default function App() {
  return (
    <div className="app-layout">
      <Navbar />
      <main className="app-main">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/create-shipment" element={<ProtectedRoute><CreateShipmentPage /></ProtectedRoute>} />
          <Route path="/my-shipments" element={<ProtectedRoute><MyShipmentsPage /></ProtectedRoute>} />
          <Route path="/track" element={<TrackPage />} />
          <Route path="/admin" element={<ProtectedRoute requireAdmin={true}><AdminDashboard /></ProtectedRoute>} />

          {/* Redirect old Turkish routes to new English routes */}
          <Route path="/giris" element={<Navigate to="/login" replace />} />
          <Route path="/kayit" element={<Navigate to="/register" replace />} />
          <Route path="/gonderi" element={<Navigate to="/create-shipment" replace />} />
          <Route path="/gonderilerim" element={<Navigate to="/my-shipments" replace />} />
          <Route path="/takip" element={<Navigate to="/track" replace />} />
        </Routes>
      </main>
      <Footer />
    </div>
  );
}
