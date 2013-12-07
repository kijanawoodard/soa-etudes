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
			mediator.Subscribe<Features.ShowMailingLabel.ViewRequest, Features.ShowMailingLabel.ViewModel>
				(async request =>
				{
					var store = container.Resolve<IDocumentStore>();
					var result = new Features.ShowMailingLabel.ViewModel();
				    var sales = new Sales.RequestHandler(store);
				    var shipping = new Shipping.RequestHandler(store);

				    await Task.WhenAll(sales.Handle(request), shipping.Handle(request));
                    
					result = sales.Handle(result);
					result = shipping.Handle(result);
					return result;
				});
		}
	}
}