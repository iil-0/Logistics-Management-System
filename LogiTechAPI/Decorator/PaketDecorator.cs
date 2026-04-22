using LogiTechAPI.Factory;

namespace LogiTechAPI.Decorator
{
    public abstract class PaketDecorator : IPaket
    {
        protected readonly IPaket _paket;

        protected PaketDecorator(IPaket paket)
        {
            _paket = paket;
        }

        public virtual string Name => _paket.Name;
        public virtual decimal CalculatePrice() => _paket.CalculatePrice();
        public virtual string GetDescription() => _paket.GetDescription();
    }
}
