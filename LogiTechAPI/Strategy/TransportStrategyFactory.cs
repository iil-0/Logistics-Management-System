// Strateji seçici — frontend'den gelen string'i ITransportStrategy somut nesnesine
// çevirir. Strategy + Factory pattern birlikte: hangi algoritmayı seçeceğimizi
// tek noktada karar verir, CreateShipmentCommand bu nesneyi kullanır.
namespace LogiTechAPI.Strategy
{
    public static class TransportStrategyFactory
    {
        public static ITransportStrategy GetStrategy(string method)
        {
            return method?.ToLower() switch
            {
                "air" or "havayolu"   => new AirTransportStrategy(),
                "land" or "karayolu"  => new LandTransportStrategy(),
                "sea" or "denizyolu"  => new SeaTransportStrategy(),
                _                     => new LandTransportStrategy()  // Bilinmeyen → karayolu
            };
        }
    }
}
