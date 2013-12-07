using System.Threading.Tasks;
using Raven.Client;

namespace Sales
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
			_customer = await _session.LoadAsync<Sales.Customer>(request.CustomerId);
		}

        public Features.ShowMailingLabel.ViewModel Handle(Features.ShowMailingLabel.ViewModel result)
		{
            result.Name = string.Format("{0} {1} {2}", _customer.FirstName, _customer.MiddleName, _customer.LastName);
			return result;
		}
	}

	public class Customer
	{
		public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
	}
}