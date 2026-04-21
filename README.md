# 🚀 LogiTech — Lojistik Yönetim Sistemi

> **Tasarım Desenleri (Design Patterns)** ile güçlendirilmiş, Full-Stack Kargo Hesaplama Platformu

![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React_19-61DAFB?style=for-the-badge&logo=react&logoColor=black)
![Vite](https://img.shields.io/badge/Vite_7-646CFF?style=for-the-badge&logo=vite&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black)

---

## 📋 Proje Hakkında

**LogiTech**, kullanıcıların kargo paket tipini, ek hizmetleri ve taşıma yolunu seçerek toplam kargo fiyatını hesaplamasını sağlayan bir web uygulamasıdır. Proje, **yazılım mühendisliği tasarım desenleri** üzerine kurulmuş bir öğretici demo uygulamasıdır.

### Ne Yapar?

- 📦 Farklı paket tipleri arasından seçim (Standart, Hassas, Ağır Yük)
- 🛡️ Ek hizmetler ekleme (Sigorta, Hızlı Teslimat)
- ✈️ Taşıma yolu belirleme (Havayolu, Karayolu, Denizyolu)
- 💰 Toplam fiyatı otomatik hesaplama
- 🔔 Gerçek zamanlı bildirim simülasyonu (Observer Pattern)

---

## 🏗️ Kullanılan Teknolojiler

### Backend

| Teknoloji                         | Açıklama                                                 |
| --------------------------------- | -------------------------------------------------------- |
| **C#**                            | Ana programlama dili                                     |
| **.NET 8 (ASP.NET Core Web API)** | Backend framework                                        |
| **REST API**                      | İstemci-sunucu iletişim mimarisi                         |
| **CORS**                          | Cross-Origin Resource Sharing (Frontend bağlantısı için) |

### Frontend

| Teknoloji            | Açıklama                                         |
| -------------------- | ------------------------------------------------ |
| **JavaScript (JSX)** | Ana programlama dili                             |
| **React 19**         | UI framework                                     |
| **Vite 7**           | Build aracı ve geliştirme sunucusu               |
| **Vanilla CSS**      | Styling (Glassmorphism, animasyonlar, dark tema) |
| **Fetch API**        | Backend ile HTTP iletişimi                       |

---

## 🧠 Kullanılan 4 Tasarım Deseni (Design Patterns)

### 1. 🏭 Factory Pattern (Yaratımsal Desen)

**Dosya:** `Factory/PaketFactory.cs`

**Ne yapar?** Kullanıcının seçtiği paket tipine göre doğru paket nesnesini oluşturur. Yeni paket tipi eklemek için sadece factory sınıfına bir satır eklemek yeterlidir.

**Nasıl çalışır?**

```csharp
IPaket paket = _factory.CreatePaket("Hassas");
// → HassasPaket nesnesi döner (fiyat: 120₺)
```

**Paket Tipleri:**
| Tip | Baz Fiyat | Açıklama |
|-----|-----------|----------|
| Standart | 50₺ | Normal kargo |
| Hassas | 120₺ | Kırılabilir ürünler için özel ambalaj |
| Ağır Yük | 250₺ | 50kg üzeri endüstriyel kargo |

---

### 2. 🎨 Decorator Pattern (Yapısal Desen)

**Dosya:** `Decorator/PaketDecorators.cs`

**Ne yapar?** Bir pakete çalışma zamanında yeni özellikler (Sigorta, Hızlı Teslimat) ekler. Orijinal nesneyi değiştirmeden, onu "sarar" ve fiyat/açıklama bilgisini genişletir.

**Nasıl çalışır?**

```csharp
IPaket paket = new HassasPaket();                    // 120₺
paket = new SigortaDecorator(paket);                  // 120₺ + 75₺ = 195₺
paket = new HizliTeslimatDecorator(paket);            // 195₺ + 100₺ = 295₺
```

**Ek Hizmetler:**
| Hizmet | Ek Ücret |
|--------|----------|
| Sigorta Güvencesi | +75₺ |
| Hızlı Teslimat (24 Saat) | +100₺ |

---

### 3. 📐 Strategy Pattern (Davranışsal Desen)

**Dosya:** `Strategy/TasimaStratejileri.cs`

**Ne yapar?** Taşıma yoluna göre farklı maliyet hesaplama algoritmaları uygular. Her strateji kendi hesaplama mantığını barındırır ve birbirleriyle değiştirilebilir.

**Nasıl çalışır?**

```csharp
var strateji = TasimaStratejisiFactory.GetStrateji("Havayolu");
decimal ekMaliyet = strateji.HesaplaEkMaliyet(temelFiyat);
// Havayolu: temelFiyat * 0.80 (%80 ek maliyet)
```

**Taşıma Yolları:**
| Yol | Ek Maliyet | Teslim Süresi |
|-----|------------|---------------|
| ✈️ Havayolu | +%80 | 1-2 iş günü |
| 🚛 Karayolu | +%20 | 3-5 iş günü |
| 🚢 Denizyolu | +%10 | 7-14 iş günü |

---

### 4. 👁️ Observer Pattern (Davranışsal Desen)

**Dosya:** `Observer/KargoObserver.cs`

**Ne yapar?** Kargo durumu değiştiğinde (sipariş alındı, rota planlanıyor, hazırlanıyor) tüm abone edilmiş gözlemcilere (observer) otomatik bildirim gönderir. Yeni bir bildirim kanalı eklemek için sadece yeni bir observer sınıfı yazmak yeterlidir.

**Nasıl çalışır?**

```csharp
var kargoTakip = new KargoTakipServisi();
kargoTakip.Subscribe(new BildirimObserver());    // Bildirim paneli
kargoTakip.Subscribe(new EmailObserver());       // E-posta simülasyonu

kargoTakip.Notify("Sipariş alındı!", "Sipariş Alındı ✅");
// → Her iki observer'a da mesaj gider
```

**Observer Tipleri:**
| Observer | Görevi |
|----------|--------|
| BildirimObserver | Uygulama içi bildirim oluşturur |
| EmailObserver | E-posta bildirimi simüle eder |

---

## 📁 Proje Klasör Yapısı

```
LogiTech/
│
├── 📄 README.md                          ← Bu dosya
│
├── 🔵 LogiTechAPI/                       ← Backend (.NET 8 Web API)
│   ├── Controllers/
│   │   └── KargoController.cs            ← POST /api/kargo/hesapla endpoint'i
│   ├── Factory/
│   │   ├── IPaket.cs                     ← Paket arayüzü (interface)
│   │   ├── Paketler.cs                   ← Standart, Hassas, AgirYuk sınıfları
│   │   └── PaketFactory.cs              ← Factory deseni implementasyonu
│   ├── Decorator/
│   │   └── PaketDecorators.cs            ← Sigorta & HızlıTeslimat decorator'ları
│   ├── Strategy/
│   │   └── TasimaStratejileri.cs         ← Havayolu, Karayolu, Denizyolu stratejileri
│   ├── Observer/
│   │   └── KargoObserver.cs              ← Bildirim & Email observer'ları
│   ├── DTOs/
│   │   └── KargoSiparisRequest.cs        ← Request/Response veri modelleri
│   ├── Program.cs                        ← Uygulama başlangıcı, CORS, DI ayarları
│   └── LogiTechAPI.csproj                ← Proje yapılandırma dosyası
│
└── 🟡 lojistik-frontend/                 ← Frontend (React + Vite)
    ├── src/
    │   ├── App.jsx                       ← Ana bileşen (form, API çağrısı, sonuç gösterimi)
    │   ├── App.css                       ← Bileşen stilleri (glassmorphism, animasyonlar)
    │   ├── index.css                     ← Global CSS değişkenleri ve tasarım sistemi
    │   └── main.jsx                      ← React giriş noktası
    ├── index.html                        ← HTML şablonu
    ├── vite.config.js                    ← Vite yapılandırması
    └── package.json                      ← Bağımlılıklar ve script'ler
```

---

## 🔌 API Endpoint'leri

### `POST /api/kargo/hesapla`

Kargo fiyatını hesaplar.

**İstek (Request):**

```json
{
  "paketTipi": "Hassas",
  "ekstralar": ["Sigorta", "HizliTeslimat"],
  "tasimaYolu": "Havayolu"
}
```

**Yanıt (Response):**

```json
{
  "toplamFiyat": 531.0,
  "aciklama": "Hassas Paket - Kırılabilir ürün... + Sigorta + Hızlı Teslimat | Havayolu...",
  "paketTipi": "Hassas",
  "tasimaYolu": "Havayolu",
  "ekstraHizmetler": ["Sigorta Güvencesi (+75₺)", "Hızlı Teslimat (+100₺)"],
  "kargoDurumu": "Hazırlanıyor 📦",
  "bildirimler": [
    "[12:05:30] 📦 Sipariş Alındı: ...",
    "📧 E-posta gönderildi: ..."
  ],
  "olusturulmaTarihi": "2026-02-22T00:13:01"
}
```

### `GET /api/kargo/saglik`

API sağlık kontrolü.

**Yanıt:**

```json
{
  "durum": "Çalışıyor ✅",
  "zaman": "2026-02-22T00:13:01",
  "versiyon": "1.0.0"
}
```

---

## 💰 Fiyat Hesaplama Mantığı

```
Toplam Fiyat = (BazFiyat + EkHizmetler) + TaşımaEkMaliyeti
```

**Örnek hesaplama:**

```
Hassas Paket:        120₺    ← Factory
+ Sigorta:           +75₺    ← Decorator
+ Hızlı Teslimat:   +100₺   ← Decorator
= Ara toplam:        295₺

Havayolu ek maliyet: 295 × 0.80 = 236₺    ← Strategy

TOPLAM:              295 + 236 = 531₺
```

---

## 🚀 Kurulum ve Çalıştırma

### Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- Herhangi bir modern tarayıcı

### Adım 1: Projeyi İndir

```bash
cd LogiTech
```

### Adım 2: Backend'i Başlat (Terminal 1)

```bash
cd LogiTechAPI
dotnet run --launch-profile http
```

> Backend başladığında → `http://localhost:5085` adresinde çalışır.

### Adım 3: Frontend'i Başlat (Terminal 2)

```bash
cd lojistik-frontend
npm install     # İlk seferlik bağımlılıkları yükle
npm run dev
```

> Frontend başladığında → `http://localhost:5173` adresinde çalışır.

### Adım 4: Tarayıcıda Aç

```
http://localhost:5173
```

---

## 🧪 API Test Etme (PowerShell ile)

```powershell
# Standart Paket + Karayolu
Invoke-RestMethod -Method POST -Uri "http://localhost:5085/api/kargo/hesapla" `
  -ContentType "application/json" `
  -Body '{"paketTipi":"Standart","ekstralar":[],"tasimaYolu":"Karayolu"}'
# → 60₺

# Hassas Paket + Tüm Ekstralar + Havayolu
Invoke-RestMethod -Method POST -Uri "http://localhost:5085/api/kargo/hesapla" `
  -ContentType "application/json" `
  -Body '{"paketTipi":"Hassas","ekstralar":["Sigorta","HizliTeslimat"],"tasimaYolu":"Havayolu"}'
# → 531₺

# Sağlık kontrolü
Invoke-RestMethod -Uri "http://localhost:5085/api/kargo/saglik"
```

---

## 🎨 Frontend Arayüz Özellikleri

- **Dark Theme** — Koyu renk paleti ile modern görünüm
- **Glassmorphism** — Yarı saydam cam efektli kartlar
- **Micro-animations** — Hover efektleri, geçiş animasyonları
- **Responsive Design** — Mobil ve masaüstü uyumlu
- **Pattern Chips** — Her form alanında hangi desenin kullanıldığını gösteren etiketler
- **Observer Bildirim Paneli** — Backend'den gelen bildirimlerin gerçek zamanlı gösterimi

---

## 📐 Mimari Diyagram

```
┌──────────────────────────────────────────────────────────────┐
│                    FRONTEND (React + Vite)                    │
│                   http://localhost:5173                       │
│                                                              │
│  ┌──────────┐  ┌───────────┐  ┌──────────┐  ┌────────────┐  │
│  │ Dropdown │  │ Checkbox  │  │  Radio   │  │   Sonuç    │  │
│  │ PaketTip │  │ Ekstralar │  │  Taşıma  │  │   Kartı    │  │
│  └────┬─────┘  └─────┬─────┘  └────┬─────┘  └──────▲─────┘  │
│       │              │             │                │        │
│       └──────────────┴─────────────┘                │        │
│                      │ fetch POST                   │        │
└──────────────────────┼──────────────────────────────┼────────┘
                       │  JSON                        │  JSON
                       ▼                              │
┌──────────────────────┴──────────────────────────────┼────────┐
│                  BACKEND (.NET 8 Web API)            │        │
│                  http://localhost:5085                │        │
│                                                      │        │
│  ┌─────────────┐  KargoController                    │        │
│  │   Factory   │───► IPaket paket = CreatePaket()    │        │
│  │  Pattern    │                                     │        │
│  └─────────────┘              │                      │        │
│                               ▼                      │        │
│  ┌─────────────┐  paket = new SigortaDecorator()     │        │
│  │  Decorator  │  paket = new HizliTeslimatDeco()    │        │
│  │  Pattern    │                                     │        │
│  └─────────────┘              │                      │        │
│                               ▼                      │        │
│  ┌─────────────┐  strateji.HesaplaEkMaliyet()        │        │
│  │  Strategy   │  (Havayolu / Karayolu / Denizyolu)  │        │
│  │  Pattern    │                                     │        │
│  └─────────────┘              │                      │        │
│                               ▼                      │        │
│  ┌─────────────┐  kargoTakip.Notify(...)             │        │
│  │  Observer   │  → BildirimObserver                 │        │
│  │  Pattern    │  → EmailObserver              Response       │
│  └─────────────┘──────────────────────────────►──────┘        │
│                                                               │
└───────────────────────────────────────────────────────────────┘
```

---

## 👩‍💻 Geliştirici

Bu proje **Yazılım Mühendisliği Tasarım Desenleri** dersi kapsamında geliştirilmiştir.

---

## 📝 Lisans

Bu proje eğitim amaçlıdır.
