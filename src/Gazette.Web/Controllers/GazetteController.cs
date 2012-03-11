using System.Web.Mvc;
using Ninject;
using Raven.Client;

namespace Gazette.Controllers
{
    public abstract class GazetteController : Controller
    {
        [Inject]
        public IDocumentSession RavenSession { get; set; }
    }
}