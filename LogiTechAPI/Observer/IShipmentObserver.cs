// Observer pattern'in DİNLEYİCİ (Observer) sözleşmesi.
// Subject = ObserverRegistry. Konkre observer'lar (NotificationObserver,
// EmailObserver) bunu uygular; durum değişince Update() çağrılır.
namespace LogiTechAPI.Observer
{
    public interface IShipmentObserver
    {
        // message: detaylı açıklama, status: kısa durum kodu (örn. "Delivered")
        void Update(string message, string status);
    }
}
