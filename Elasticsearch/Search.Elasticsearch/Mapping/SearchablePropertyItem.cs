using Nest;

namespace Search.Elasticsearch.Mapping
{
    [ElasticsearchType(IdProperty = nameof(SearchableBaseItem.Id), RelationName = SearchablePropertyItem.TypeName)]
    public class SearchablePropertyItem : SearchableBaseItem
    {
        public const string TypeName = "searchablepropertyitem";

        [Text(Analyzer = "autocomplete", SearchAnalyzer = "autocomplete_search", Name = nameof(FormerName))]
        public string FormerName { get; set; }

        [Text(Analyzer = "autocomplete", SearchAnalyzer = "autocomplete_search", Name = nameof(StreetAddres))]
        public string StreetAddres { get; set; }

        [Text(Analyzer = "autocomplete", SearchAnalyzer = "autocomplete_search", Name = nameof(City))]
        public string City { get; set; }

        public float Lat { get; set; }

        public float Lng { get; set; }
    }
}
