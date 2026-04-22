namespace LogiTechAPI.State
{
    public class OnTheWayStatus : IGonderiDurum
    {
        public string StatusName => "On the Way";
        public string Description => "Your cargo is on its way to the delivery address.";
        public IGonderiDurum? NextStatus() => new DeliveredStatus();
        public bool IsCancellable() => false;
    }
}
