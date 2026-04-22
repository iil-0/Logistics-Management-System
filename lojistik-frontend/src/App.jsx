import { Routes, Route } from "react-router-dom";
import Navbar from "./components/Navbar";
import Footer from "./components/Footer";
import ProtectedRoute from "./components/ProtectedRoute";
import HomePage from "./pages/HomePage";
import GirisPage from "./pages/GirisPage";
import KayitPage from "./pages/KayitPage";
import GonderiPage from "./pages/GonderiPage";
import GonderilerimPage from "./pages/GonderilerimPage";
import TakipPage from "./pages/TakipPage";

export default function App() {
  return (
    <div className="app-layout">
      <Navbar />
      <main className="app-main">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<GirisPage />} />
          <Route path="/register" element={<KayitPage />} />
          <Route path="/create-shipment" element={<ProtectedRoute><GonderiPage /></ProtectedRoute>} />
          <Route path="/my-shipments" element={<ProtectedRoute><GonderilerimPage /></ProtectedRoute>} />
          <Route path="/track" element={<TakipPage />} />
        </Routes>
      </main>
      <Footer />
    </div>
  );
}
