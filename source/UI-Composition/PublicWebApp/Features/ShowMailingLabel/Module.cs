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
					result = await new Sales.RequestHandler(store).Handle(request, result);
					result = await new Shipping.RequestHandler(store).Handle(request, result);
					return result;
				});
		}
	}
}