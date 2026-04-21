namespace LogiTechAPI.State
{
    /// <summary>
    /// State Pattern — Gönderi durum arayüzü.
    /// Her durum kendi davranışını ve geçiş kurallarını bilir.
    /// </summary>
    public interface IGonderiDurum
    {
        string DurumAdi { get; }
        string Aciklama { get; }
        IGonderiDurum? SonrakiDurum();
        bool IptalEdilabilir();
    }
}
