using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebApp.Infrastructure
{
    public class VesselConfig
    {
        public static void RegisterContainer()
        {
            var mediator = new Mediator();
            var container = new Vessel();
            container.Register<IMediator>(mediator);
            container.Register<ISubscribeHandlers>(mediator);
            container.RegisterModules();

            DependencyResolver.SetResolver(new VesselDependencyResolver(container)); //for asp.net mvc
        }
    }
}