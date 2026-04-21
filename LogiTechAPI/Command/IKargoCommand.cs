using LogiTechAPI.Models;

namespace LogiTechAPI.Command
{
    /// <summary>
    /// Command Pattern — Komut arayüzü.
    /// Her kargo işlemi bir komut nesnesidir.
    /// </summary>
    public interface IKargoCommand
    {
        KomutSonuc Execute();
        KomutSonuc Undo();
        string KomutAdi { get; }
    }

    /// <summary>
    /// Komut çalıştırma sonucu.
    /// </summary>
    public class KomutSonuc
    {
        public bool Basarili { get; set; }
        public string Mesaj { get; set; } = string.Empty;
        public Gonderi? Gonderi { get; set; }
    }
}
