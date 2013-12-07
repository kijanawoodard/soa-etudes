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

			//write original data so we can run this version of the project first if desired
			using (var session = store.OpenSession(Database))
			{
				var customer = new V1.Customer
				{
					Id = "customers/1234",
					Name = "John Q. Public"
				};
				session.Store(customer);
				session.SaveChanges();
			}

			//feature/split-name - It was a mistake to have the name as a single field, split
			//Take a naive approach. We will let customer care sort out the mess. :-[
            using (var session = store.OpenSession(Database))
			using (var enumerator = session.Advanced.Stream<V1.Customer>(fromEtag: Etag.Empty))
			{
				while (enumerator.MoveNext())
				{
					var v1 = enumerator.Current.Document;
				    var names = v1.Name.Split(' ');

				    var v2 = new V2.Customer();
				    v2.Id = v1.Id;
				    v2.FirstName = names[0];

				    if (names.Length == 2)
				    {
				        v2.LastName = names[1];
				    }
                    else if (names.Length > 2)
                    {
                        v2.MiddleName = names[1];
                        v2.LastName = names[2];
                    }

                    session.Store(v2);
                    session.SaveChanges();
				}
			}

		}
	}

    namespace V1
    {
        public class Customer
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }

    namespace V2
    {
        public class Customer
        {
            public string Id { get; set; }
            public string Name { get { return string.Format("{0} {1} {2}", FirstName, MiddleName, LastName); } }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
        }
    }
}
