using Nest;

namespace Search.Elasticsearch.Mapping
{
    [ElasticsearchType(IdProperty = nameof(SearchableBaseItem.Id), RelationName = SearchablePropertyItem.TypeNameDef)]
    public class SearchablePropertyItem : SearchableBaseItem
    {
        public const string TypeNameDef = "searchablepropertyitem";

        [Text(Analyzer = "autocomplete", SearchAnalyzer = "autocomplete_search", Name = nameof(FormerName))]
        public string FormerName { get; set; }

        [Text(Analyzer = "autocomplete", SearchAnalyzer = "autocomplete_search", Name = nameof(StreetAddres))]
        public string StreetAddres { get; set; }

        //multifieds : mapping in code
        [Text(Name = nameof(City))]
        public string City { get; set; }

        public float Lat { get; set; }

        public float Lng { get; set; }

        public override string AdditionalInfo { get { return $"FormerName:{this.FormerName} City:{this.City} Street:{this.StreetAddres}"; } }

        public SearchablePropertyItem()
        {
            this.TypeName = TypeNameDef;
        }

    }
}
