// Factory Method pattern — abstract Creator (üretici) sözleşmesi.
// Her concrete creator yalnızca KENDİ paket türünü üretmeyi bilir.
// Üretim sorumluluğu fabrika class'ından çıkıp her bir creator'a dağıtılır.
namespace LogiTechAPI.Factory
{
    public interface IPaketCreator
    {
        IPaket CreatePackage();           // Factory Method — concrete creator override eder
    }
}
