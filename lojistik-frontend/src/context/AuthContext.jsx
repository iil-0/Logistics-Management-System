import { createContext, useContext, useState, useEffect } from "react";

const AuthContext = createContext(null);
const API = "http://localhost:5085/api/auth";

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  // Sayfa yüklendiğinde cookie ile oturum kontrolü
  useEffect(() => {
    fetch(`${API}/ben`, { credentials: "include" })
      .then((res) => (res.ok ? res.json() : null))
      .then((data) => setUser(data))
      .catch(() => setUser(null))
      .finally(() => setLoading(false));
  }, []);

  const giris = async (email, sifre) => {
    const res = await fetch(`${API}/giris`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ email, sifre }),
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.mesaj || "Giriş başarısız.");
    setUser(data);
    return data;
  };

  const kayit = async (ad, soyad, email, telefon, sifre) => {
    const res = await fetch(`${API}/kayit`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ ad, soyad, email, telefon, sifre }),
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.mesaj || "Kayıt başarısız.");
    setUser(data);
    return data;
  };

  const cikis = async () => {
    await fetch(`${API}/cikis`, {
      method: "POST",
      credentials: "include",
    });
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, loading, giris, kayit, cikis }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within AuthProvider");
  return context;
}
