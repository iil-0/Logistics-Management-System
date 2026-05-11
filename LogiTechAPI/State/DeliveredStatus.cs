// Concrete State — mutlu yol sonu, TERMİNAL durum.
// NextStatus() null döner çünkü teslim sonrası başka durum yok.
namespace LogiTechAPI.State
{
    public class DeliveredStatus : IGonderiDurum
    {
        public string StatusName => "Delivered";
        public string Description => "Your cargo has been successfully delivered.";
        public IGonderiDurum? NextStatus() => null;
        public bool IsCancellable() => false;
    }
}
