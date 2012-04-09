using System.Web.Mvc;
using System.Web.Routing;
using Ninject;

namespace Gazette
{
    public class NinjectControllerBuilder : DefaultControllerFactory
    {
        private readonly IKernel _kernel;

        protected override IController GetControllerInstance(RequestContext requestContext, System.Type controllerType)
        {
            return _kernel.Get(controllerType) as IController;
        }

        public NinjectControllerBuilder(IKernel kernel)
        {
            _kernel = kernel;
        }
    }
}