namespace LogiTechAPI.State
{
    /// <summary>
    /// State Pattern — Shipment status interface.
    /// Each status knows its behavior and transition rules.
    /// </summary>
    public interface IGonderiDurum
    {
        string StatusName { get; }
        string Description { get; }
        IGonderiDurum? NextStatus();
        bool IsCancellable();
    }
}
