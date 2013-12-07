using System.Threading.Tasks;
using Raven.Client;

namespace Sales
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
			    _customer = await session.LoadAsync<Sales.Customer>(request.CustomerId);
			}
		}

        public Features.ShowMailingLabel.ViewModel Handle(Features.ShowMailingLabel.ViewModel result)
		{
            result.Name = string.Format("{0} {1} {2}", _customer.FirstName, _customer.MiddleName, _customer.LastName);
			return result;
		}

		private IAsyncDocumentSession OpenSession()
		{
			return _store.OpenAsyncSession("Sales");
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