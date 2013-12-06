using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
