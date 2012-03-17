using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Gazette.Controllers
{
    public class SyndicateController : GazetteController
    {
        public ActionResult Index()
        {
            var articles = from article in RavenSession.Query<Article>()
                           where article.Published <= DateTime.Now
                           orderby article.Published descending
                           select article;

            var items = articles.Take(10).ToList()
                .Select(article => new XElement("item",
                                                new XElement("title", article.Title),
                                                new XElement("link", Url.Action("Details", "Article", new {id = article.Id})),
                                                new XElement("pubDate", article.Published.ToPubdateString()),
                                                new XElement("description", article.Content))).ToArray();

            var feed = new XElement("rss");
            feed.SetAttributeValue("version", "2.0");
            

            var channelDescription = new XElement("channel");
            channelDescription.Add(
                    new XElement("title","Smoothfriction Blog"),
                    new XElement("link","http://blog.smoothfriction.nl"),
                    new XElement("description","Smoothfriction Blog"),
                items);

            feed.Add(channelDescription);

            var feedDocument = new XDocument(feed);
            var output = new StringBuilder();
            using(var writer = new StringWriter(output))
            {
                feedDocument.Save(writer);
            }

            return Content(output.ToString(), "application/rss+xml");
        }
    }

    public static class DateTimeExtensions
    {
        public static string ToPubdateString(this DateTime input)
        {
            const string rfc822Format = "ddd, dd MMM yyyy HH:mm:ss";
            return input.ToUniversalTime().ToString(rfc822Format, DateTimeFormatInfo.InvariantInfo) + " UT";
        }
    }
}