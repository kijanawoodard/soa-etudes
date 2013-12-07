using System.Threading.Tasks;
using Raven.Client;

namespace Sales
{
	public class RequestHandler
	{
		private readonly IDocumentStore _store;

		public RequestHandler(IDocumentStore store)
		{
			_store = store;
		}

		public async Task<Features.ShowMailingLabel.ViewModel> Handle(Features.ShowMailingLabel.ViewRequest request, Features.ShowMailingLabel.ViewModel result)
		{
			using (var session = OpenSession())
			{
				var customer = await session.LoadAsync<Sales.Customer>(request.CustomerId);
                result.Name = string.Format("{0} {1} {2}", customer.FirstName, customer.MiddleName, customer.LastName);
				return result;
			}
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