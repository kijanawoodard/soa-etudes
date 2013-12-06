using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Document;
using Raven.Client.Extensions;

namespace Shipping.Backend
{
	class Program
	{
		public const string Database = "Shipping";

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
