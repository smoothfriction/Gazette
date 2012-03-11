using System;
using System.Linq;
using Gazette.Controllers;

namespace Gazette.Infrastructure
{
    public class ArchiveIndex : Raven.Client.Indexes.AbstractIndexCreationTask<Article, ArchiveEntry>
    {
        public ArchiveIndex()
        {
            Map = records => from a in records
                             let d = a.Published
                             select new
                                        {
                                            Month = new DateTime(d.Year, d.Month,1),
                                            Count = 1
                                        };

            Reduce = results => from result in results
                                group result by result.Month
                                into g
                                select new {Month = g.Key, Count = g.Count()};


        }
    }

    public class ArchiveEntry
    {
        public DateTime Month { get; set; }
        public int Count { get; set; }
    }
}