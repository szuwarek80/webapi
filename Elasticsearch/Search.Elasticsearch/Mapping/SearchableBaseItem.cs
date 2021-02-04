using Nest;

namespace Search.Elasticsearch.Mapping
{
    public class SearchableBaseItem
    {
        [Keyword(Name = nameof(Id))]
        public int Id { get; set; }

        [Text(Analyzer = "autocomplete", SearchAnalyzer = "autocomplete_search", Name = nameof(Name))]
        public string Name { get; set; }

        [Text(Analyzer = "autocomplete", SearchAnalyzer = "autocomplete_search", Name = nameof(Market))]
        public string Market { get; set; }

        public string State { get; set; }
        
        public SearchableBaseItem()
        {
        }
    }
}
