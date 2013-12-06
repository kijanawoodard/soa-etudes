using Raven.Client;

namespace Shipping
{
	public class RequestHandler
	{
		private readonly IDocumentStore _store;

		public RequestHandler(IDocumentStore store)
		{
			_store = store;
		}

		public Features.ShowMailingLabel.ViewModel Handle(Features.ShowMailingLabel.ViewRequest request, Features.ShowMailingLabel.ViewModel result)
		{
			using (var session = OpenSession())
			{
				var customer = session.Load<Shipping.Customer>(request.CustomerId);
				result.Address = customer.ShippingAddress.Street;
				result.CityStateZip = string.Format("{0}, {1} {2}", 
					customer.ShippingAddress.City,
					customer.ShippingAddress.State,
					customer.ShippingAddress.Zip);
				return result;
			}
		}

		private IDocumentSession OpenSession()
		{
			return _store.OpenSession("Shipping");
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