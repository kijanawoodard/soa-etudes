using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;

namespace PublicWebApp.Infrastructure
{
    public class RavenModule : IModule
    {
        public void Execute(IContainer container)
        {
            var store = new DocumentStore
            {
                ConnectionStringName = "ravendb"
            }.Initialize();

            container.Register<IDocumentStore>(store);
            container.Register<Sales.RavenDb>( c => new Sales.RavenDb(c.Resolve<IDocumentStore>()));
            container.Register<Shipping.RavenDb>(c => new Shipping.RavenDb(c.Resolve<IDocumentStore>()));
        }
    }

    class RavenDb
    {
        private readonly IDocumentStore _store;
        private readonly string _database;

        public RavenDb(IDocumentStore store, string database)
        {
            _store = store;
            _database = database;
        }

        public IDocumentSession OpenSession()
        {
            return _store.OpenSession(_database);
        }

        public IAsyncDocumentSession OpenAsyncSession()
        {
            return _store.OpenAsyncSession(_database);
        }
    }
}

namespace Sales
{
    class RavenDb : PublicWebApp.Infrastructure.RavenDb
    {
        public RavenDb(IDocumentStore store) : base(store, "Sales") { }
    }
}

namespace Shipping
{
    class RavenDb : PublicWebApp.Infrastructure.RavenDb
    {
        public RavenDb(IDocumentStore store) : base(store, "Shipping") { }
    }
}