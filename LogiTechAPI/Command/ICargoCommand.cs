// Command pattern'in SÖZLEŞMESİ. Her işlem (oluştur, durum güncelle, iptal)
// kendi başına bir nesnedir. Execute() işi yapar, Undo() geri alır.
// Bu sayede işlemler stack'te tutulabilir → undo/redo doğal olarak gelir.
namespace LogiTechAPI.Command
{
    public interface ICargoCommand
    {
        string CommandName { get; }              // Log/UI için komut adı ("Update Status" vb.)
        Task<CommandResult> Execute();           // Asıl işi yap (RECEIVER'a delege ederek)
        Task<CommandResult> Undo();              // İşin etkilerini tersine çevir
    }

    // Komut çalıştırma sonucu. Hem başarı bayrağı hem mesaj hem opsiyonel veri taşır.
    public class CommandResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Shipment { get; set; }  // Oluşturma komutu için dönen Shipment
    }
}
