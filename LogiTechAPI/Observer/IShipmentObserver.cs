namespace LogiTechAPI.Observer
{
    public interface IShipmentObserver
    {
        void Update(string message, string status);
    }
}
