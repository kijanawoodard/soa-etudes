using System.Threading.Tasks;
using Raven.Client;

namespace Shipping
{
	public class RequestHandler
	{
        private readonly IAsyncDocumentSession _session;
	    private Customer _customer;

        public RequestHandler(IAsyncDocumentSession session)
        {
            _session = session;
        }

		public async Task Handle(Features.ShowMailingLabel.ViewRequest request)
		{
			_customer = await _session.LoadAsync<Shipping.Customer>(request.CustomerId);
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