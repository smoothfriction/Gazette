using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gazette.Infrastructure.XmlRpc
{
    public static class XmlRpcExtensions
    {
        public static XmlRpcRoute MapXmlRpcRoute(this RouteCollection routes, string name, string url)
        {
            if (routes == null) throw new ArgumentNullException("routes");
            if (url == null) throw new ArgumentNullException("url");

            var route = new XmlRpcRoute(url, new MvcRouteHandler())
                                    {
                                        Defaults = new RouteValueDictionary(),
                                        DataTokens = new RouteValueDictionary(),
                                        Constraints = new RouteValueDictionary(new  { controller = "API"})
                                    };

            routes.Add(name, route);

            return route;
        }
    }
}