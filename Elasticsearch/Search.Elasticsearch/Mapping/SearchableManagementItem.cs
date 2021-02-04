using Nest;

namespace Search.Elasticsearch.Mapping
{
    [ElasticsearchType(IdProperty = nameof(SearchableBaseItem.Id), RelationName = SearchableManagementItem.TypeNameDef)]
    public class SearchableManagementItem : SearchableBaseItem
    {
        public const string TypeNameDef = "searchablemanagementitem";  

        public SearchableManagementItem()
        {
            this.TypeName = TypeNameDef;
        }
    }
}
