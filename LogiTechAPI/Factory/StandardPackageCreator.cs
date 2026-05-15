// Concrete Creator — Standard paket üretmekten SORUMLU.
// "new StandardPackage()" yalnızca burada geçer; sistem değişirse tek yer.
namespace LogiTechAPI.Factory
{
    public class StandardPackageCreator : IPackageCreator
    {
        public IPackage CreatePackage() => new StandardPackage();
    }
}
