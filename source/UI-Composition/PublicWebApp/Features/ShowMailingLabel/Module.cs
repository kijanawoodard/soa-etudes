using System.Threading.Tasks;

namespace Features.ShowMailingLabel
{
	using PublicWebApp.Infrastructure;
	using Raven.Client;

	public class Module : IModule
	{
		public void Execute(IContainer container)
		{
			container.Register(c => new ShowMailingLabelController(c.Resolve<IMediator>()));

			var mediator = container.Resolve<ISubscribeHandlers>();
			mediator.Subscribe<Features.ShowMailingLabel.ViewRequest, Features.ShowMailingLabel.ViewModel>(async request => await Execute(container, request));
		}

	    private static async Task<ViewModel> Execute(IResolver container, ViewRequest request)
	    {
	        using (var salesSession = container.Resolve<Sales.RavenDb>().OpenAsyncSession())
            using (var shippingSession = container.Resolve<Shipping.RavenDb>().OpenAsyncSession())
	        {
                var sales = new Sales.RequestHandler(salesSession);
                var shipping = new Shipping.RequestHandler(shippingSession);

                await Task.WhenAll(sales.Handle(request), shipping.Handle(request));

                var result = new Features.ShowMailingLabel.ViewModel();
                result = sales.Handle(result);
                result = shipping.Handle(result);
                return result;   
	        }
	    }
	}
}