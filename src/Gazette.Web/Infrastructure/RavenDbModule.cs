using System.Web;
using Gazette.Controllers;
using Ninject.Modules;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace Gazette
{
    internal class RavenDbModule : NinjectModule
    {
        private IDocumentStore _documentStore;

        public override void Load()
        {
            InitDocumentStore();

            Bind<IDocumentSession>()
                .ToMethod(ctx => _documentStore.OpenSession())
                .InRequestScope()
                .OnDeactivation(session =>
                    {
                        if (HttpContext.Current.Server.GetLastError() == null)
                            session.SaveChanges();
                    });
        }

        private void InitDocumentStore()
        {
            _documentStore = new DocumentStore {ConnectionStringName = "RavenDB"};
            _documentStore.Initialize();
            using(var session = _documentStore.OpenSession())
            {
                var articles = session.Query<Article>();
                foreach (var article in articles)
                {
                    article.Slug = SlugGenerator.Generate(article.Title);
                    session.Store(article);
                }
                session.SaveChanges();
            }
            IndexCreation.CreateIndexes(typeof (MvcApplication).Assembly, _documentStore);
        }
    }
}