using Search.Elasticsearch.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Search.Elasticsearch.Search
{
    public class SimpleSerachResponse
    {
        public IReadOnlyCollection<SearchableBaseItem> Items { get; set; }
        public long TotalItems { get; set; }
    }
}
