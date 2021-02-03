using Nest;

namespace Search.Elasticsearch.Mapping
{
    [ElasticsearchType(IdProperty = nameof(SearchableBaseItem.Id), RelationName = SearchablePropertyItem.TypeName)]
    public class SearchablePropertyItem : SearchableBaseItem
    {
        public const string TypeName = "searchablepropertyitem";

        [Text(/*Analyzer = "autocomplete", */Name = nameof(FormerName))]
        public string FormerName { get; set; }

        [Text(/*Analyzer = "autocomplete", */Name = nameof(StreetAddres))]
        public string StreetAddres { get; set; }

        [Text(/*Analyzer = "autocomplete", */Name = nameof(City))]
        public string City { get; set; }

        [Text(Name=nameof(Lat))]
        public string Lat { get; set; }

        [Text(Name = nameof(Lng))]        
        public string Lng { get; set; }
    }
}
