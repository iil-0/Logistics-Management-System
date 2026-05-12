// Concrete Creator — Fragile paket üretmekten sorumlu.
namespace LogiTechAPI.Factory
{
    public class FragilePackageCreator : IPaketCreator
    {
        public IPaket CreatePackage() => new FragilePackage();
    }
}
