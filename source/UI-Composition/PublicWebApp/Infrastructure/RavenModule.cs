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
        }
    }
}