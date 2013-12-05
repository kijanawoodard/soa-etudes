using System.Web.Mvc;
using PublicWebApp.Controllers;
using PublicWebApp.Infrastructure;
using Raven.Client;

namespace PublicWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public ActionResult Index(IndexRequest request)
        {
            request.CustomerId = "customers/1234"; //fake out this coming in on the request
            var model = _mediator.Send<IndexRequest, IndexViewModel>(request);
            return View(model);
        }

        public class IndexRequest
        {
            public string CustomerId { get; set; }
        }

        public class IndexViewModel
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string CityStateZip { get; set; }
        }
    }

    public class HomeModule : IModule
    {
        public void Execute(IContainer container)
        {
            container.Register(c => new HomeController(c.Resolve<IMediator>()));

            var mediator = container.Resolve<ISubscribeHandlers>();
            mediator.Subscribe<HomeController.IndexRequest, HomeController.IndexViewModel>(request =>
            {
                var store = container.Resolve<IDocumentStore>();
                var result = new HomeController.IndexViewModel();
                result = new Sales.RequestHandler(store).Handle(request, result);
                result = new Shipping.RequestHandler(store).Handle(request, result);
                return result;
            });
        }
    }
}

namespace Sales
{
    public class RequestHandler
    {
        private readonly IDocumentStore _store;

        public RequestHandler(IDocumentStore store)
        {
            _store = store;
        }

        public HomeController.IndexViewModel Handle(
                HomeController.IndexRequest request,
                HomeController.IndexViewModel result)
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
    public class RequestHandler
    {
        private readonly IDocumentStore _store;

        public RequestHandler(IDocumentStore store)
        {
            _store = store;
        }

        public HomeController.IndexViewModel Handle(
                HomeController.IndexRequest request,
                HomeController.IndexViewModel result)
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