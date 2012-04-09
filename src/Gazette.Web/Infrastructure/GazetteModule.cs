using Gazette.Controllers;
using Ninject.Modules;

namespace Gazette
{
    internal class GazetteModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAuthenticationService>().To<AuthenticationService>().InRequestScope();
        }
    }
}