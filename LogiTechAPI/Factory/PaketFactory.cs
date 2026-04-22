namespace LogiTechAPI.Factory
{
    public class PaketFactory
    {
        public IPaket CreatePackage(string packageType)
        {
            return packageType?.ToLower() switch
            {
                "standard" => new StandardPackage(),
                "fragile"  => new FragilePackage(),
                "heavyload" => new HeavyLoadPackage(),
                "standart" => new StandardPackage(), // Backward compatibility
                "hassas"   => new FragilePackage(),
                "agiryuk"  => new HeavyLoadPackage(),
                _          => new StandardPackage()
            };
        }
    }
}
