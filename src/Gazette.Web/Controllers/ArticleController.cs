// // Copyright 2012, Smoothfriction
// // Author: Erik van Brakel
// // Licensed under the BSD 3-Clause License, see license.txt for details, or go to // http://www.opensource.org/licenses/BSD-3-Clause
using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Gazette.Infrastructure;

namespace Gazette.Controllers
{
    public class ArticleController : GazetteController
    {
        public PartialViewResult ArchiveList()
        {
            var entries = RavenSession.Query<ArchiveEntry, ArchiveIndex>().OrderByDescending(x => x.Month);
            var a = entries.ToArray();
            return PartialView("_Archive", a);
        }

        public ActionResult Index(int? year, int? month, int pageSize = 10, int currentPage = 0)
        {
            var articles = RavenSession.Query<Article>().Where(x => x.Published <= DateTime.Now);

            // if year or month is an invalid value it just returns nothing, so no need to check for weird input
            if (year.HasValue) articles = articles.Where(x => x.Published.Year == year);
            if (month.HasValue) articles = articles.Where(x => x.Published.Month == month);

            articles = articles.OrderByDescending(x => x.Published);
            var viewModel = new ListViewModel<Article>(
                articles,
                pageSize,
                currentPage);
            return View(viewModel);
        }

        public ActionResult Details(int id)
        {
            var article = RavenSession.Load<Article>(id);
            if (article == null) return HttpNotFound();
            var viewModel = Mapper.Map<Article, ArticleDetailViewModel>(article);

            viewModel.Previous = RavenSession.Query<Article>().FirstOrDefault(x => x.Published < article.Published);
            viewModel.Next = RavenSession.Query<Article>().FirstOrDefault(x => x.Published > article.Published);

            
            ViewBag.Title = article.Title;
            return View(viewModel);
        }
    }

    public class ListViewModel<T>
    {
        public T[] Items { get; set; }
        public int? NextPage { get; set; }
        public int? PreviousPage { get; set; }

        public ListViewModel(IQueryable<T> dataSource, int pageSize, int currentPage)
        {
            Items = dataSource.Skip(currentPage*pageSize).Take(pageSize).ToArray();
        }
    }

    public class ArticleDetailViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set;}
        public DateTime Published { get; set; }
        public string Author { get; set; }

        public Article Next { get; set; }
        public Article Previous { get; set; }
    }

    public class Article
    {
        public string Slug { get; set; }
        public string[] Categories { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Published { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
    }
}