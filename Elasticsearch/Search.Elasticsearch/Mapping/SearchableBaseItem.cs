using Nest;

namespace Search.Elasticsearch.Mapping
{
    public class SearchableBaseItem
    {
        [Keyword(Name = nameof(Id))]
        public int Id { get; set; }

        [Text(/*Analyzer = "autocomplete", */Name = nameof(Name))]
        public string Name { get; set; }

        [Text(/*Analyzer = "autocomplete", */Name = nameof(Market))]
        public string Market { get; set; }

        [Text(/*Analyzer = "autocomplete", */Name = nameof(State))]
        public string State { get; set; }
        
        public SearchableBaseItem()
        {
        }
    }
}
