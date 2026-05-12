// Concrete Creator — HeavyLoad paket üretmekten sorumlu.
namespace LogiTechAPI.Factory
{
    public class HeavyLoadPackageCreator : IPaketCreator
    {
        public IPaket CreatePackage() => new HeavyLoadPackage();
    }
}
