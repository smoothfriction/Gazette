using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Gazette.Infrastructure.XmlRpc;

namespace Gazette.Controllers
{
    [XmlRpcService]
    public class BloggerController : GazetteController
    {
        [ActionName("getUsersBlogs")]
        public ActionResult GetBlogs(string appKey, string username, string password)
        {
            return new XmlRpcResponseResult(new [] { new BlogInfo { blogid = "0", blogName = "EasyBlog", url = "http://localhost/EasyBlog"}});
        }

        [ActionName("deletePost")]
        public ActionResult DeletePost()
        {
            throw new NotImplementedException();
        }
    }

    [XmlRpcService]
    public class MetaweblogController : GazetteController
    {
        [ActionName("newPost")]
        public ActionResult NewPost(string blogid, string username, string password, BlogPost post, bool publish)
        {
            var article = new Article
                                {
                                    Title = post.Title,
                                    Content = post.Description,
                                    Published = post.Date_Created_Gmt,
                                    Author = username,
                                    Categories = post.Categories.ToArray(),
                                    Slug = SlugGenerator.Generate(post.Title)
                                };
            RavenSession.Store(article);
            return new XmlRpcResponseResult(article.Id);
        }

        [ActionName("editPost")]
        public ActionResult EditPost()
        {
            throw new NotImplementedException();
        }

        [ActionName("getCategories")]
        public ActionResult GetCategories()
        {
            var articles = RavenSession.Query<Article>().ToList();
            var actual = from article in articles
                            from c in article.Categories
                            select new BlogCategory { description = c};

            return new XmlRpcResponseResult(actual.Distinct().ToArray());
        }

        [ActionName("getRecentPosts")]
        public ActionResult GetRecentPosts()
        {
            throw new NotImplementedException();
        }

        [ActionName("NewMediaObject")]
        public ActionResult CreateMediaObject()
        {
            throw new NotImplementedException();
        }
    }

    public static class SlugGenerator
    {
        public static string Generate(string input)
        {
            return input.Normalize(nor);
        }
    }

    [XmlRpcService]
    public class WpController : GazetteController
    {
        [ActionName("newCategory")]
        public ActionResult CreateCategory(string blogId, string username, string password, Category category)
        {
            RavenSession.Store(category);
            return new XmlRpcResponseResult(category.Id);
        }
    }

    public class Category : IEquatable<Category>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool Equals(Category other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Category)) return false;
            return Equals((Category) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(Category left, Category right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Category left, Category right)
        {
            return !Equals(left, right);
        }
    }

    public class BlogPost
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date_Created_Gmt { get; set; }
        public IEnumerable<string> Categories { get; set; }
    }

    public class BlogCategory
    {
        public string description { get; set; }
        public string htmlUrl { get; set; }
        public string rssUrl { get; set; }

        public BlogCategory()
        {
            htmlUrl = string.Empty;
            rssUrl = string.Empty;
        }
    }

    public class BlogInfo
    {
        public string blogid { get; set; }
        public string blogName { get; set; }
        public string url { get; set; }
    }
}