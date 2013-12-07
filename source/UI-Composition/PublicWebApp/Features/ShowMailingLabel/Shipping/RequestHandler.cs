using System.Threading.Tasks;
using Raven.Client;

namespace Shipping
{
	public class RequestHandler
	{
		private readonly IDocumentStore _store;
	    private Customer _customer;

	    public RequestHandler(IDocumentStore store)
		{
			_store = store;
		}

		public async Task Handle(Features.ShowMailingLabel.ViewRequest request)
		{
			using (var session = OpenSession())
			{
				_customer = await session.LoadAsync<Shipping.Customer>(request.CustomerId);
			}
		}

        public Features.ShowMailingLabel.ViewModel Handle(Features.ShowMailingLabel.ViewModel result)
        {
            result.Address = _customer.ShippingAddress.Street;
            result.CityStateZip = string.Format("{0}, {1} {2}",
                _customer.ShippingAddress.City,
                _customer.ShippingAddress.State,
                _customer.ShippingAddress.Zip);
            return result;
        }

		private IAsyncDocumentSession OpenSession()
		{
			return _store.OpenAsyncSession("Shipping");
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