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

namespace Features.ShowMailingLabel
{
    using System.Web.Mvc;
    using PublicWebApp.Infrastructure;

    public class ShowMailingLabelController : Controller
    {
        private readonly IMediator _mediator;

        public ShowMailingLabelController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult Index(ViewRequest request)
        {
            request.CustomerId = "customers/1234"; //fake out this coming in on the request
            var model = _mediator.Send<ViewRequest, ViewModel>(request);
            return View(model);
        }
    }

    public class ViewRequest
    {
        public string CustomerId { get; set; }
    }

    public class ViewModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string CityStateZip { get; set; }
    }
}

namespace Sales
{
    using Raven.Client;

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
            return _store.OpenSession(Sales.Initializer.Database);
        }
    }
}

namespace Shipping
{
    using Raven.Client;

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
                var customer = session.Load<Shipping.Customer>(request.CustomerId);
                result.Address = customer.ShippingAddress.Street;
                result.CityStateZip = string.Format("{0}, {1} {2}", 
                                                customer.ShippingAddress.City,
                                                customer.ShippingAddress.State,
                                                customer.ShippingAddress.Zip);
                return result;
            }
        }

        private IDocumentSession OpenSession()
        {
            return _store.OpenSession(Shipping.Initializer.Database);
        }
    }
}