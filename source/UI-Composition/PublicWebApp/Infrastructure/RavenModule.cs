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
            
            InitializeData(store);
        }

        private void InitializeData(IDocumentStore store)
        {
            new Sales.Initializer().Init(store);
            new Shipping.Initializer().Init(store);
        }
    }
}

namespace Sales
{
    public class Initializer
    {
        public const string Database = "Sales";
        public void Init(IDocumentStore store)
        {
            store.DatabaseCommands.EnsureDatabaseExists(Database);
            using (var session = store.OpenSession(Database))
            {
                var customer = new Customer
                {
                    Id = "customers/1234",
                    Name = "John Q. Public"
                };
                session.Store(customer);
                session.SaveChanges();
            }
        }
    }

    public class Customer
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}

namespace Shipping
{
    public class Initializer
    {
        public const string Database = "Shipping";
        public void Init(IDocumentStore store)
        {
            store.DatabaseCommands.EnsureDatabaseExists(Database);
            using (var session = store.OpenSession(Database))
            {
                var customer = new Customer
                {
                    Id = "customers/1234",
                    ShippingAddress = new Address
                    {
                        Street = "123 Main St.",
                        City = "Dallas",
                        State = "TX",
                        Zip = "75230"
                    }
                };
                session.Store(customer);
                session.SaveChanges();
            }
        }
    }

    public class Customer
    {
        public string Id { get; set; }
        public Address ShippingAddress { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
}