using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using Raven.Client.Extensions;

namespace Sales.Backend
{
	class Program
	{
		public const string Database = "Sales";

		static void Main(string[] args)
		{
			var store = new DocumentStore
			{
				ConnectionStringName = "ravendb"
			}.Initialize();

			store.DatabaseCommands.EnsureDatabaseExists(Database);

			//feature/fix web app - now that the web app is fixed, drop the Name property
            using (var session = store.OpenSession(Database))
			using (var enumerator = session.Advanced.Stream<V2.Customer>(fromEtag: Etag.Empty))
			{
				while (enumerator.MoveNext())
				{
					var v2 = enumerator.Current.Document;
                    session.Store(v2);
				    session.SaveChanges();
				}
			}
		}
	}

    namespace V2
    {
        public class Customer
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
        }
    }
}
