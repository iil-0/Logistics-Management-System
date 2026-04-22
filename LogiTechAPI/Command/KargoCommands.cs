using LogiTechAPI.Models;
using LogiTechAPI.Factory;
using LogiTechAPI.Decorator;
using LogiTechAPI.Strategy;
using LogiTechAPI.Observer;
using LogiTechAPI.State;
using LogiTechAPI.Services;
using LogiTechAPI.DTOs;

namespace LogiTechAPI.Command
{
    /// <summary>
    /// Command Pattern — Gönderi oluşturma komutu.
    /// Factory + Decorator + Strategy + Observer + State desenlerini kullanarak gönderi oluşturur.
    /// Undo ile gönderiyi iptal eder.
    /// </summary>
    public class GonderiOlusturCommand : IKargoCommand
    {
        private readonly PaketFactory _factory;
        private readonly GonderiService _gonderiService;
        private readonly GonderiRequest _request;
        private readonly int _userId;
        private readonly User _user;
        private Gonderi? _olusturulanGonderi;

        public string KomutAdi => "Create Shipment";

        public GonderiOlusturCommand(
            PaketFactory factory,
            GonderiService gonderiService,
            GonderiRequest request,
            int userId,
            User user)
        {
            _factory = factory;
            _gonderiService = gonderiService;
            _request = request;
            _userId = userId;
            _user = user;
        }

        public KomutSonuc Execute()
        {
            // 1. FACTORY: Paket oluştur
            IPaket paket = _factory.CreatePaket(_request.PaketTipi);

            var ekstraHizmetler = new List<string>();

            // 2. DECORATOR: Ekstraları uygula
            if (_request.Ekstralar != null)
            {
                foreach (var extra in _request.Ekstralar)
                {
                    switch (extra.ToLower())
                    {
                        case "sigorta":
                            paket = new SigortaDecorator(paket);
                            ekstraHizmetler.Add("Insurance Coverage (+75₺)");
                            break;
                        case "hizliteslimat":
                            paket = new HizliTeslimatDecorator(paket);
                            ekstraHizmetler.Add("Fast Delivery (+100₺)");
                            break;
                    }
                }
            }

            // 3. STRATEGY: Taşıma yolu maliyetini hesapla
            var strateji = TasimaStratejisiFactory.GetStrateji(_request.TasimaYolu);
            var temelFiyat = paket.GetFiyat();
            var ekMaliyet = strateji.HesaplaEkMaliyet(temelFiyat);
            var toplamFiyat = Math.Round(temelFiyat + ekMaliyet, 2);

            // 4. OBSERVER: Bildirimleri gönder
            var kargoTakip = new KargoTakipServisi();
            var bildirimObserver = new BildirimObserver();
            var emailObserver = new EmailObserver();
            kargoTakip.Subscribe(bildirimObserver);
            kargoTakip.Subscribe(emailObserver);
            kargoTakip.Notify("Shipment created.", "Order Received");

            // 5. STATE: Başlangıç durumu
            var baslangicDurum = new SiparisAlindiDurum();

            // 6. Gönderi nesnesini oluştur ve kaydet
            var gonderi = new Gonderi
            {
                UserId = _userId,
                TakipNo = _gonderiService.TakipNoUret(),
                GondericiAd = $"{_user.Ad} {_user.Soyad}",
                GondericiEmail = _user.Email,
                GondericiTelefon = _user.Telefon,
                AliciAd = _request.AliciAd,
                AliciAdres = _request.AliciAdres,
                AliciTelefon = _request.AliciTelefon,
                AliciSehir = _request.AliciSehir,
                PaketTipi = paket.GetTip(),
                Ekstralar = ekstraHizmetler,
                TasimaYolu = strateji.GetTip(),
                ToplamFiyat = toplamFiyat,
                Aciklama = paket.GetAciklama() + " | " + strateji.GetAciklama(),
                Durum = baslangicDurum.DurumAdi,
                DurumGecmisi = new List<DurumGecmisi>
                {
                    new DurumGecmisi
                    {
                        Durum = baslangicDurum.DurumAdi,
                        Tarih = DateTime.Now,
                        Aciklama = baslangicDurum.Aciklama
                    }
                }
            };

            _gonderiService.Ekle(gonderi);
            _olusturulanGonderi = gonderi;

            return new KomutSonuc
            {
                Basarili = true,
                Mesaj = $"Gönderi başarıyla oluşturuldu. Takip No: {gonderi.TakipNo}",
                Gonderi = gonderi
            };
        }

        /// <summary>
        /// Undo — Gönderiyi iptal eder (State Pattern ile).
        /// </summary>
        public KomutSonuc Undo()
        {
            if (_olusturulanGonderi == null)
                return new KomutSonuc { Basarili = false, Mesaj = "İptal edilecek gönderi bulunamadı." };

            var mevcutDurum = GonderiDurumFactory.GetDurum(_olusturulanGonderi.Durum);

            if (!mevcutDurum.IptalEdilabilir())
                return new KomutSonuc
                {
                    Basarili = false,
                    Mesaj = $"Bu gönderi '{_olusturulanGonderi.Durum}' durumunda olduğu için iptal edilemez."
                };

            var iptalDurum = new IptalEdildiDurum();
            _olusturulanGonderi.Durum = iptalDurum.DurumAdi;
            _olusturulanGonderi.DurumGecmisi.Add(new DurumGecmisi
            {
                Durum = iptalDurum.DurumAdi,
                Tarih = DateTime.Now,
                Aciklama = iptalDurum.Aciklama
            });

            return new KomutSonuc
            {
                Basarili = true,
                Mesaj = "Shipment successfully cancelled.",
                Gonderi = _olusturulanGonderi
            };
        }
    }

    /// <summary>
    /// Command Pattern — Durum güncelleme komutu.
    /// State Pattern ile bir sonraki duruma geçiş yapar.
    /// </summary>
    public class DurumGuncelleCommand : IKargoCommand
    {
        private readonly GonderiService _gonderiService;
        private readonly string _takipNo;
        private string? _oncekiDurum;

        public string KomutAdi => "Update Status";

        public DurumGuncelleCommand(GonderiService gonderiService, string takipNo)
        {
            _gonderiService = gonderiService;
            _takipNo = takipNo;
        }

        public KomutSonuc Execute()
        {
            var gonderi = _gonderiService.TakipNoIleBul(_takipNo);
            if (gonderi == null)
                return new KomutSonuc { Basarili = false, Mesaj = "Shipment not found." };

            var mevcutDurum = GonderiDurumFactory.GetDurum(gonderi.Durum);
            var sonrakiDurum = mevcutDurum.SonrakiDurum();

            if (sonrakiDurum == null)
                return new KomutSonuc
                {
                    Basarili = false,
                    Mesaj = $"Shipment is already in '{gonderi.Durum}' status. Cannot advance further."
                };

            _oncekiDurum = gonderi.Durum;
            gonderi.Durum = sonrakiDurum.DurumAdi;
            gonderi.DurumGecmisi.Add(new DurumGecmisi
            {
                Durum = sonrakiDurum.DurumAdi,
                Tarih = DateTime.Now,
                Aciklama = sonrakiDurum.Aciklama
            });

            return new KomutSonuc
            {
                Basarili = true,
                Mesaj = $"Status updated: {_oncekiDurum} → {sonrakiDurum.DurumAdi}",
                Gonderi = gonderi
            };
        }

        public KomutSonuc Undo()
        {
            if (_oncekiDurum == null)
                return new KomutSonuc { Basarili = false, Mesaj = "No status to undo." };

            var gonderi = _gonderiService.TakipNoIleBul(_takipNo);
            if (gonderi == null)
                return new KomutSonuc { Basarili = false, Mesaj = "Shipment not found." };

            gonderi.Durum = _oncekiDurum;
            gonderi.DurumGecmisi.Add(new DurumGecmisi
            {
                Durum = _oncekiDurum,
                Tarih = DateTime.Now,
                Aciklama = "Status reverted."
            });

            return new KomutSonuc
            {
                Basarili = true,
                Mesaj = $"Status reverted to: {_oncekiDurum}",
                Gonderi = gonderi
            };
        }
    }

    /// <summary>
    /// Command Pattern — Gönderi iptal komutu.
    /// </summary>
    public class GonderiIptalCommand : IKargoCommand
    {
        private readonly GonderiService _gonderiService;
        private readonly string _takipNo;
        private string? _oncekiDurum;

        public string KomutAdi => "Cancel Shipment";

        public GonderiIptalCommand(GonderiService gonderiService, string takipNo)
        {
            _gonderiService = gonderiService;
            _takipNo = takipNo;
        }

        public KomutSonuc Execute()
        {
            var gonderi = _gonderiService.TakipNoIleBul(_takipNo);
            if (gonderi == null)
                return new KomutSonuc { Basarili = false, Mesaj = "Shipment not found." };

            var mevcutDurum = GonderiDurumFactory.GetDurum(gonderi.Durum);
            if (!mevcutDurum.IptalEdilabilir())
                return new KomutSonuc
                {
                    Basarili = false,
                    Mesaj = $"Shipment cannot be cancelled as it is in '{gonderi.Durum}' status."
                };

            _oncekiDurum = gonderi.Durum;
            var iptalDurum = new IptalEdildiDurum();
            gonderi.Durum = iptalDurum.DurumAdi;
            gonderi.DurumGecmisi.Add(new DurumGecmisi
            {
                Durum = iptalDurum.DurumAdi,
                Tarih = DateTime.Now,
                Aciklama = iptalDurum.Aciklama
            });

            return new KomutSonuc
            {
                Basarili = true,
                Mesaj = "Shipment successfully cancelled.",
                Gonderi = gonderi
            };
        }

        public KomutSonuc Undo()
        {
            if (_oncekiDurum == null)
                return new KomutSonuc { Basarili = false, Mesaj = "No status to undo." };

            var gonderi = _gonderiService.TakipNoIleBul(_takipNo);
            if (gonderi == null)
                return new KomutSonuc { Basarili = false, Mesaj = "Shipment not found." };

            gonderi.Durum = _oncekiDurum;
            gonderi.DurumGecmisi.Add(new DurumGecmisi
            {
                Durum = _oncekiDurum,
                Tarih = DateTime.Now,
                Aciklama = "Cancellation reverted."
            });

            return new KomutSonuc
            {
                Basarili = true,
                Mesaj = $"Cancellation reverted. Status: {_oncekiDurum}",
                Gonderi = gonderi
            };
        }
    }

    /// <summary>
    /// Command Pattern — Invoker.
    /// Komutları çalıştırır ve geçmişi tutar (Undo için).
    /// </summary>
    public class KargoCommandInvoker
    {
        private readonly Stack<IKargoCommand> _gecmis = new();

        public KomutSonuc Calistir(IKargoCommand komut)
        {
            var sonuc = komut.Execute();
            if (sonuc.Basarili)
                _gecmis.Push(komut);
            return sonuc;
        }

        public KomutSonuc GeriAl()
        {
            if (_gecmis.Count == 0)
                return new KomutSonuc { Basarili = false, Mesaj = "No action to undo." };

            var sonKomut = _gecmis.Pop();
            return sonKomut.Undo();
        }
    }
}
