using LogiTechAPI.Models;
using LogiTechAPI.Factory;
using LogiTechAPI.Decorator;
using LogiTechAPI.Strategy;
using LogiTechAPI.Observer;
using LogiTechAPI.State;
using LogiTechAPI.Services;

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

        public string KomutAdi => "Gönderi Oluştur";

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
                            ekstraHizmetler.Add("Sigorta Güvencesi (+75₺)");
                            break;
                        case "hizliteslimat":
                            paket = new HizliTeslimatDecorator(paket);
                            ekstraHizmetler.Add("Hızlı Teslimat (+100₺)");
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
            kargoTakip.Notify("Gönderi oluşturuldu.", "Sipariş Alındı");

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
                Mesaj = "Gönderi başarıyla iptal edildi.",
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

        public string KomutAdi => "Durum Güncelle";

        public DurumGuncelleCommand(GonderiService gonderiService, string takipNo)
        {
            _gonderiService = gonderiService;
            _takipNo = takipNo;
        }

        public KomutSonuc Execute()
        {
            var gonderi = _gonderiService.TakipNoIleBul(_takipNo);
            if (gonderi == null)
                return new KomutSonuc { Basarili = false, Mesaj = "Gönderi bulunamadı." };

            var mevcutDurum = GonderiDurumFactory.GetDurum(gonderi.Durum);
            var sonrakiDurum = mevcutDurum.SonrakiDurum();

            if (sonrakiDurum == null)
                return new KomutSonuc
                {
                    Basarili = false,
                    Mesaj = $"Gönderi zaten '{gonderi.Durum}' durumunda. İleri geçiş yapılamaz."
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
                Mesaj = $"Durum güncellendi: {_oncekiDurum} → {sonrakiDurum.DurumAdi}",
                Gonderi = gonderi
            };
        }

        public KomutSonuc Undo()
        {
            if (_oncekiDurum == null)
                return new KomutSonuc { Basarili = false, Mesaj = "Geri alınacak durum yok." };

            var gonderi = _gonderiService.TakipNoIleBul(_takipNo);
            if (gonderi == null)
                return new KomutSonuc { Basarili = false, Mesaj = "Gönderi bulunamadı." };

            gonderi.Durum = _oncekiDurum;
            gonderi.DurumGecmisi.Add(new DurumGecmisi
            {
                Durum = _oncekiDurum,
                Tarih = DateTime.Now,
                Aciklama = "Durum geri alındı."
            });

            return new KomutSonuc
            {
                Basarili = true,
                Mesaj = $"Durum geri alındı: {_oncekiDurum}",
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

        public string KomutAdi => "Gönderi İptal";

        public GonderiIptalCommand(GonderiService gonderiService, string takipNo)
        {
            _gonderiService = gonderiService;
            _takipNo = takipNo;
        }

        public KomutSonuc Execute()
        {
            var gonderi = _gonderiService.TakipNoIleBul(_takipNo);
            if (gonderi == null)
                return new KomutSonuc { Basarili = false, Mesaj = "Gönderi bulunamadı." };

            var mevcutDurum = GonderiDurumFactory.GetDurum(gonderi.Durum);
            if (!mevcutDurum.IptalEdilabilir())
                return new KomutSonuc
                {
                    Basarili = false,
                    Mesaj = $"Gönderi '{gonderi.Durum}' durumunda olduğu için iptal edilemez."
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
                Mesaj = "Gönderi başarıyla iptal edildi.",
                Gonderi = gonderi
            };
        }

        public KomutSonuc Undo()
        {
            if (_oncekiDurum == null)
                return new KomutSonuc { Basarili = false, Mesaj = "Geri alınacak durum yok." };

            var gonderi = _gonderiService.TakipNoIleBul(_takipNo);
            if (gonderi == null)
                return new KomutSonuc { Basarili = false, Mesaj = "Gönderi bulunamadı." };

            gonderi.Durum = _oncekiDurum;
            gonderi.DurumGecmisi.Add(new DurumGecmisi
            {
                Durum = _oncekiDurum,
                Tarih = DateTime.Now,
                Aciklama = "İptal geri alındı."
            });

            return new KomutSonuc
            {
                Basarili = true,
                Mesaj = $"İptal geri alındı. Durum: {_oncekiDurum}",
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
                return new KomutSonuc { Basarili = false, Mesaj = "Geri alınacak işlem yok." };

            var sonKomut = _gecmis.Pop();
            return sonKomut.Undo();
        }
    }
}
