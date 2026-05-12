// Command pattern'in SÖZLEŞMESİ. Her işlem (oluştur, durum güncelle, iptal)
// kendi başına bir nesnedir. Execute() işi yapar, Undo() geri alır.
// Bu sayede işlemler stack'te tutulabilir → undo/redo doğal olarak gelir.
namespace LogiTechAPI.Command
{
    public interface IKargoCommand
    {
        string KomutAdi { get; }              // Log/UI için komut adı ("Update Status" vb.)
        Task<KomutSonuc> Execute();           // Asıl işi yap (RECEIVER'a delege ederek)
        Task<KomutSonuc> Undo();              // İşin etkilerini tersine çevir
    }

    // Komut çalıştırma sonucu. Hem başarı bayrağı hem mesaj hem opsiyonel veri taşır.
    public class KomutSonuc
    {
        public bool Basarili { get; set; }
        public string Mesaj { get; set; } = string.Empty;
        public object? Gonderi { get; set; }  // Oluşturma komutu için dönen Gonderi
    }
}
