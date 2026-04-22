namespace LogiTechAPI.Command
{
    public interface IKargoCommand
    {
        string KomutAdi { get; }
        Task<KomutSonuc> Execute();
        Task<KomutSonuc> Undo();
    }

    public class KomutSonuc
    {
        public bool Basarili { get; set; }
        public string Mesaj { get; set; } = string.Empty;
        public object? Gonderi { get; set; }
    }
}
