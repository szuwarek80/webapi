using Search.Elasticsearch.Mapping;
using System.Collections.Generic;

namespace Search.Elasticsearch.Searching
{
    public class SimpleSerachResponse
    {
        public IReadOnlyCollection<SearchableBaseItem> Items { get; set; }
        public long TotalItems { get; set; }
    }
}
