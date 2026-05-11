// State pattern'in SÖZLEŞMESİ. Gönderinin geçtiği her durum (Order Received,
// Preparing, On the Way, Delivered, Cancelled) bunu uygular. Her durum kendi
// adını, açıklamasını, sonraki durumu ve iptal kuralını BİLİR.
namespace LogiTechAPI.State
{
    public interface IGonderiDurum
    {
        string StatusName { get; }            // İnsan okur durum adı (DB'de saklanan değer)
        string Description { get; }           // Bildirim/UI için açıklama
        IGonderiDurum? NextStatus();          // FSM geçişi — terminal durumda null
        bool IsCancellable();                 // İptal edilebilir mi? (sadece "Order Received" için true)
    }
}
