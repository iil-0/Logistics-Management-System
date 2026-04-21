namespace LogiTechAPI.Strategy
{
    public interface ITasimaStratejisi
    {
        decimal HesaplaEkMaliyet(decimal temelFiyat);
        string GetAciklama();
        string GetTip();
    }

    // Havayolu Stratejisi
    public class HavayoluStratejisi : ITasimaStratejisi
    {
        public decimal HesaplaEkMaliyet(decimal temelFiyat) =>
            temelFiyat * 0.80m; // %80 ek maliyet

        public string GetAciklama() => "Havayolu Taşımacılığı - Ekspres Teslim (1-2 İş Günü)";
        public string GetTip() => "Havayolu";
    }

    // Karayolu Stratejisi
    public class KarayoluStratejisi : ITasimaStratejisi
    {
        public decimal HesaplaEkMaliyet(decimal temelFiyat) =>
            temelFiyat * 0.20m; // %20 ek maliyet

        public string GetAciklama() => "Karayolu Taşımacılığı - Ekonomik Teslim (3-5 İş Günü)";
        public string GetTip() => "Karayolu";
    }

    // Denizyolu Stratejisi (bonus)
    public class DenizyoluStratejisi : ITasimaStratejisi
    {
        public decimal HesaplaEkMaliyet(decimal temelFiyat) =>
            temelFiyat * 0.10m; // %10 ek maliyet

        public string GetAciklama() => "Denizyolu Taşımacılığı - En Ekonomik (7-14 İş Günü)";
        public string GetTip() => "Denizyolu";
    }

    // Strategy Factory
    public class TasimaStratejisiFactory
    {
        public static ITasimaStratejisi GetStrateji(string tasimaYolu)
        {
            return tasimaYolu?.ToLower() switch
            {
                "havayolu"  => new HavayoluStratejisi(),
                "karayolu"  => new KarayoluStratejisi(),
                "denizyolu" => new DenizyoluStratejisi(),
                _           => new KarayoluStratejisi()
            };
        }
    }
}
