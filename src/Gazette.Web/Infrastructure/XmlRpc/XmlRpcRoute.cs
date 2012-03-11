using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;

namespace Gazette.Infrastructure.XmlRpc
{
    public class XmlRpcRoute : Route
    {
        public XmlRpcRoute(string url, IRouteHandler routeHandler) : base(url, routeHandler) { }
        public XmlRpcRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler) : base(url, defaults, routeHandler) {}
        public XmlRpcRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler) : base(url, defaults, constraints, routeHandler) { }
        public XmlRpcRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler) : base(url, defaults, constraints, dataTokens, routeHandler) { }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData routeData = base.GetRouteData(httpContext);

            if (routeData == null) return null;

            if (httpContext.Request.InputStream != null && httpContext.Request.InputStream.Length > 0)
            {
                var xml = XDocument.Load(httpContext.Request.InputStream);
                var methodName = xml.Document.Element("methodCall").Element("methodName").Value;
                var methodNameParts = methodName.Split('.');
                routeData.Values["controller"] = methodNameParts[0];
                routeData.Values["action"] = methodNameParts[1];
            }

            return routeData;
        }
    }
}