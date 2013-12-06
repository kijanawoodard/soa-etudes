using System.Web.Mvc;
using PublicWebApp.Infrastructure;

namespace Features.ShowMailingLabel
{
	public class ShowMailingLabelController : Controller
	{
		private readonly IMediator _mediator;

		public ShowMailingLabelController(IMediator mediator)
		{
			_mediator = mediator;
		}

		public ActionResult Index(ViewRequest request)
		{
			request.CustomerId = "customers/1234"; //fake the id coming in on the request
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