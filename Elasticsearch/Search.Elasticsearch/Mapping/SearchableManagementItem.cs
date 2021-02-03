using Nest;

namespace Search.Elasticsearch.Mapping
{
    [ElasticsearchType(IdProperty = nameof(SearchableBaseItem.Id), RelationName = SearchableManagementItem.TypeName)]
    public class SearchableManagementItem : SearchableBaseItem
    {
        public const string TypeName = "searchablemanagementitem";       
    }
}
