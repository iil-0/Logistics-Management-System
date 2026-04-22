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
                _                     => new LandTransportStrategy()
            };
        }
    }
}
