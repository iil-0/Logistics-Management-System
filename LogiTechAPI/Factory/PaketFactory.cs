namespace LogiTechAPI.Factory
{
    public class PaketFactory
    {
        public IPaket CreatePaket(string paketTipi)
        {
            return paketTipi?.ToLower() switch
            {
                "standart" => new StandartPaket(),
                "hassas"   => new HassasPaket(),
                "agiryuk"  => new AgirYukPaket(),
                _          => new StandartPaket()
            };
        }
    }
}
