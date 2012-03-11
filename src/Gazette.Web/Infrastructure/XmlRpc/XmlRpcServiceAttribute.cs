using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Gazette.Infrastructure.XmlRpc
{
    public class XmlRpcServiceAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Set the parameter values from the XML-RPC request XML
            if (filterContext.HttpContext.Request.InputStream != null)
            {
                filterContext.HttpContext.Request.InputStream.Seek(0, System.IO.SeekOrigin.Begin); //seek to the begining of the stream
                var xml = XDocument.Load(filterContext.HttpContext.Request.InputStream);
                
                var xmlParams = xml.Document.Element("methodCall").Element("params").Elements("param").ToArray();
                int index = 0;
                foreach (var paramDescriptor in filterContext.ActionDescriptor.GetParameters())
                {
                    var node = xmlParams[index].Element("value").Elements().First();
                    filterContext.ActionParameters[paramDescriptor.ParameterName] = DeserialiseXmlRpcData(node, paramDescriptor.ParameterType);
                    index++;
                }
            }

            base.OnActionExecuting(filterContext);
        }

        private object DeserialiseXmlRpcData(XElement data, Type targetType)
        {
            string dataType = data.Name.LocalName;
            switch (dataType)
            {
                case "string":
                    return data.Value;
                case "int":
                    return int.Parse(data.Value, NumberFormatInfo.InvariantInfo);
                case "double":
                    return double.Parse(data.Value, NumberFormatInfo.InvariantInfo);
                case "boolean":
                    return data.Value == "1";
                case "dateTime.iso8601":
                    var dateString = string.Format("{0}-{1}-{2}{3}", data.Value.Substring(0, 4),
                                                   data.Value.Substring(4, 2), data.Value.Substring(6, 2),
                                                   data.Value.Substring(8));
                    return DateTime.Parse(dateString, DateTimeFormatInfo.InvariantInfo);
                case "array":
                    {
                        var values = from value in data.Element("data").Elements("value")
                                     select DeserialiseXmlRpcData(value.Elements().First(), targetType.GetElementType()).ToString();
                        return values.ToArray();
                    }
                case "struct":
                    {
                        var members = data.Elements("member");
                        var targetObject = Activator.CreateInstance(targetType);
                        var propertyInfos = targetType.GetProperties();
                        foreach (var member in members)
                        {
                            var propertyInfo = propertyInfos.SingleOrDefault(x => x.Name.Equals(member.Element("name").Value, StringComparison.InvariantCultureIgnoreCase));
                            if (propertyInfo == null) continue;

                            var propertyValue = DeserialiseXmlRpcData(member.Element("value").Elements().First(), propertyInfo.PropertyType);
                            propertyInfo.SetValue(targetObject, propertyValue, null);
                        }
                        return targetObject;
                    }
                default:
                    throw new ArgumentException("Could not deserialise xml-rpc data");
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //If there are unhandled exception, convert to the response to an xml-rpc fault
            if (filterContext.Exception != null && !filterContext.ExceptionHandled)
            {
                filterContext.Result = new XmlRpcResponseResult(filterContext.Exception);
                filterContext.ExceptionHandled = true;
            }
            else
            {
                base.OnActionExecuted(filterContext);
            }
        }
    }
}