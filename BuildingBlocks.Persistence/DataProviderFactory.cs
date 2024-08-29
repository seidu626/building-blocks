using BuildingBlocks.Exceptions;

namespace BuildingBlocks.Persistence
{
    public abstract class DataProviderFactory
    {
        protected DataProviderFactory(DataSettings settings)
        {
            Guard.AgainstNull(settings, nameof(settings));
            Settings = settings;
        }

        protected DataSettings Settings { get; private set; }

        public abstract IDataProvider LoadDataProvider();
    }
}