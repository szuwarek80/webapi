using Search.Elasticsearch.Mapping;
using System.Collections.Generic;

namespace Search.Elasticsearch.Indexing
{
    public interface IDataProvider
    {
        List<SearchablePropertyItem> Properties { get; }

        List<SearchableManagementItem> Managements { get; }
    }
}
