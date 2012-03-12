// // Copyright 2012, Smoothfriction
// // Author: Erik van Brakel
// // Licensed under the BSD 3-Clause License, see license.txt for details, or go to // http://www.opensource.org/licenses/BSD-3-Clause
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Gazette.Controllers;
using Raven.Client;
using Raven.Client.Document;

namespace Gazette.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            IDocumentStore store = new DocumentStore {Url = "http://localhost:8080", DefaultDatabase = "Blog"};
            store.Initialize();
            var doc = XDocument.Load(File.OpenRead("export.xml"));
            XNamespace ns = "http://www.blogml.com/2006/09/BlogML";

            var posts = doc.Descendants(ns.GetName("post")).ToArray();
            var categories = doc.Root.Element(ns.GetName("categories")).Elements(ns.GetName("category"))
                .ToDictionary(x => x.Attribute("id").Value, x=>x.Attribute("description").Value);

            using (var session = store.OpenSession())
            {
                foreach (var post in posts)
                {
                    var contentElement = post.Element(ns.GetName("content"));
                    var bytes = Convert.FromBase64String(contentElement.Value);
                    var content = Encoding.UTF8.GetString(bytes);

                    string title = post.Element(ns.GetName("title")).Value;
                    var article = new Article
                                      {
                                          Author = "Erik van Brakel",
                                          Content = content,
                                          Title = title,
                                          Published = DateTime.Parse(post.Attribute("date-modified").Value),
                                          Categories = post.Descendants(ns.GetName("category")).Select(x => categories[x.Attribute("ref").Value]).ToArray()
                                      };
                    session.Store(article);
                }
                session.SaveChanges();
            }
            Console.ReadLine();

        }
    }
}
