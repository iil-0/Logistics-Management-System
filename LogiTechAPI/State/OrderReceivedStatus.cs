namespace LogiTechAPI.State
{
    public class OrderReceivedStatus : IGonderiDurum
    {
        public string StatusName => "Order Received";
        public string Description => "Your cargo order has been successfully registered.";
        public IGonderiDurum? NextStatus() => new PreparingStatus();
        public bool IsCancellable() => true;
    }
}
