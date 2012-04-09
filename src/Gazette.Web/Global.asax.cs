// // Copyright 2012, Smoothfriction
// // Author: Erik van Brakel
// // Licensed under the BSD 3-Clause License, see license.txt for details, or go to // http://www.opensource.org/licenses/BSD-3-Clause
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using Gazette.Controllers;
using Gazette.XmlRpc;
using Ninject;

namespace Gazette
{
    public class MvcApplication : HttpApplication
    {
        private static readonly IKernel Kernel = new StandardKernel(new RavenDbModule(), new GazetteModule());

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapXmlRpcRoute(name: "API", url: "{controller}");

            routes.MapRoute(
                name: "Archive",
                url: "Archive/{year}/{month}/{currentPage}/{pageSize}",
                defaults: new { controller = "Article", action = "Index", currentPage = 0, pageSize = 10 }
                );

            routes.MapRoute(
                name: "ArticleIndex",
                url: "Articles/{currentPage}/{pageSize}",
                defaults: new {controller = "Article", action = "Index", pageSize=10},
                constraints: new {currentPage = @"\d+", pageSize = @"\d+"});

            routes.MapRoute(
                name: "ArticleDetails",
                url: "Article/{id}/{slug}",
                defaults: new { controller = "Article", action = "Details" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Article", action = "Index", id=UrlParameter.Optional });

        }

        protected void Application_Start()
        {
            InitMapper();
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            var moduleBundle = new Bundle("~/Scripts/modules/js", new JsMinify());
            moduleBundle.AddDirectory("~/Scripts/modules","*.js");
            BundleTable.Bundles.Add(moduleBundle);
            BundleTable.Bundles.RegisterTemplateBundles();

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerBuilder(Kernel));
        }

        private void InitMapper()
        {
            Mapper.CreateMap<Article, ArticleDetailViewModel>();
        }
    }
}