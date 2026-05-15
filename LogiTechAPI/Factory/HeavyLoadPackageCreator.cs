// Concrete Creator — HeavyLoad paket üretmekten sorumlu.
namespace LogiTechAPI.Factory
{
    public class HeavyLoadPackageCreator : IPackageCreator
    {
        public IPackage CreatePackage() => new HeavyLoadPackage();
    }
}
