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
				(request =>
				{
					var store = container.Resolve<IDocumentStore>();
					var result = new Features.ShowMailingLabel.ViewModel();
					result = new Sales.RequestHandler(store).Handle(request, result);
					result = new Shipping.RequestHandler(store).Handle(request, result);
					return result;
				});
		}
	}
}