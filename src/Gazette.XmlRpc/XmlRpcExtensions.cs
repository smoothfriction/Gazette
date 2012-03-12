// // Copyright 2012, Smoothfriction
// // Author: Erik van Brakel
// // Licensed under the BSD 3-Clause License, see license.txt for details, or go to // http://www.opensource.org/licenses/BSD-3-Clause

using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gazette.XmlRpc
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