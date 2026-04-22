namespace LogiTechAPI.State
{
    public class PreparingStatus : IGonderiDurum
    {
        public string StatusName => "Preparing";
        public string Description => "Your cargo is being prepared in the warehouse.";
        public IGonderiDurum? NextStatus() => new OnTheWayStatus();
        public bool IsCancellable() => false;
    }
}
