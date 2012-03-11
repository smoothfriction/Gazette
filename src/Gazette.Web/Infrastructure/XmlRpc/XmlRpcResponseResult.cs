using System;
using System.Collections;
using System.Reflection;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Gazette.Infrastructure.XmlRpc
{
    public class XmlRpcResponseResult : ContentResult
    {
        public XmlRpcResponseResult(object data)
        {
            //Set content type to xml
            ContentType = "text/xml";

            //Serialise data into base.Content
            Content = SerialiseXmlRpcResponse(data);
        }

        private string SerialiseXmlRpcResponse(object data)
        {
            if (data is Exception)
            {
                Exception ex = data as Exception;
                return new XDocument(
                    new XElement("methodResponse",
                                 new XElement("fault",
                                              new XElement("value",
                                                           new XElement("struct",
                                                                        new XElement("member",
                                                                                     new XElement("name", "faultCode"),
                                                                                     new XElement("value", SerialiseXmlRpcData(0))),
                                                                        new XElement("member",
                                                                                     new XElement("name", "faultString"),
                                                                                     new XElement("value", SerialiseXmlRpcData(ex.Message)))))))).ToString();
            }
            else
            {
                return new XDocument(
                    new XElement("methodResponse",
                                 new XElement("params",
                                              new XElement("param",
                                                           new XElement("value",
                                                                        SerialiseXmlRpcData(data)))))).ToString();
            }
        }

        private XElement SerialiseXmlRpcData(object data)
        {
            if (data is IEnumerable && !IsPrimitiveXmlRpcType(data))
            {
                XElement arrayData = new XElement("data");
                foreach (var dataItem in (data as IEnumerable))
                {
                    arrayData.Add(new XElement("value",
                                               SerialiseXmlRpcData(dataItem)));
                }
                return new XElement("array", arrayData);
            }
            else if (data.GetType().IsClass && !IsPrimitiveXmlRpcType(data))
            {
                return SerialiseXmlRpcStruct(data);
            }
            else if (IsPrimitiveXmlRpcType(data))
            {
                return SerialiseXmlRpcPrimitive(data);
            }

            throw new ArgumentException("Could not serialise xml-rpc data");
        }

        private bool IsPrimitiveXmlRpcType(object data)
        {
            return data.GetType().IsPrimitive || data is string || data is DateTime;
        }

        private XElement SerialiseXmlRpcStruct(object data)
        {
            XElement structElement = new XElement("struct");

            //Serialise all properties as a name-value pair. Value can be any supported type
            PropertyInfo[] propInfos = data.GetType().GetProperties();
            foreach (PropertyInfo propInfo in propInfos)
            {
                XElement member = new XElement("member");
                string name = propInfo.Name;
                object value = propInfo.GetValue(data, null);
                member.Add(new XElement("name", name), new XElement("value", SerialiseXmlRpcData(value)));
                structElement.Add(member);
            }

            return structElement;
        }

        private XElement SerialiseXmlRpcPrimitive(object data)
        {
            if (data is string)
            {
                return new XElement("string", data);
            }
            else if (data is int)
            {
                return new XElement("int", data.ToString());
            }
            else if (data is double)
            {
                return new XElement("double", data.ToString());
            }
            else if (data is bool)
            {
                return new XElement("boolean", (bool)data ? "1" : "0");
            }
            else if (data is DateTime)
            {
                return new XElement("dateTime.iso8601", ((DateTime)data).ToString("s"));
            }

            throw new ArgumentException("Serialised for this object type is not handled");
        }
    }
}