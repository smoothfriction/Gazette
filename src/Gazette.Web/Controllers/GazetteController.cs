// // Copyright 2012, Smoothfriction
// // Author: Erik van Brakel
// // Licensed under the BSD 3-Clause License, see license.txt for details, or go to // http://www.opensource.org/licenses/BSD-3-Clause
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