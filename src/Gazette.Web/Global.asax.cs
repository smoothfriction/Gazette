using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using Gazette.Controllers;
using Gazette.Infrastructure.XmlRpc;
using Ninject;
using Ninject.Modules;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace Gazette
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        private static readonly IKernel Kernel = new StandardKernel(new RavenDbModule());

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapXmlRpcRoute(name: "API", url: "{controller}");

            routes.MapRoute(
                name: "ArticleIndex",
                url: "Article/{currentPage}/{pageSize}",
                defaults:  new {controller = "Article",action="Index",currentPage = 0, pageSize=10}
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Article", action = "Index", id=UrlParameter.Optional }
            );

        }

        protected void Application_Start()
        {
            InitMapper();
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleTable.Bundles.RegisterTemplateBundles();

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerBuilder(Kernel));
        }

        private void InitMapper()
        {
            Mapper.CreateMap<Article, ArticleDetailViewModel>();
        }
    }

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

    internal class RavenDbModule : NinjectModule
    {
        private IDocumentStore _documentStore;

        public override void Load()
        {
            InitDocumentStore();

            Bind<IDocumentSession>()
                .ToMethod(ctx => _documentStore.OpenSession())
                .OnDeactivation(session =>
                    {
                        if (HttpContext.Current.Server.GetLastError() == null)
                            session.SaveChanges();
                    });
        }

        private void InitDocumentStore()
        {
            _documentStore = new DocumentStore {Url = "http://localhost:8080", DefaultDatabase = "Blog"};
            _documentStore.Initialize();
            IndexCreation.CreateIndexes(typeof (MvcApplication).Assembly, _documentStore);
        }
    }
}