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

		public Features.ShowMailingLabel.ViewModel Handle(Features.ShowMailingLabel.ViewRequest request, Features.ShowMailingLabel.ViewModel result)
		{
			using (var session = OpenSession())
			{
				var customer = session.Load<Sales.Customer>(request.CustomerId);
				result.Name = customer.Name;
				return result;
			}
		}

		private IDocumentSession OpenSession()
		{
			return _store.OpenSession("Sales");
		}
	}

	public class Customer
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}
}