// Concrete Creator — Fragile paket üretmekten sorumlu.
namespace LogiTechAPI.Factory
{
    public class FragilePackageCreator : IPackageCreator
    {
        public IPackage CreatePackage() => new FragilePackage();
    }
}
