using Nest;

namespace Search.Elasticsearch.Mapping
{
    public class SearchableBaseItem
    {
        [Keyword(Name = nameof(Id))]
        public int Id { get; set; }

        [Keyword(Name = nameof(TypeName))]
        public string TypeName { get; protected set; }

        [Text(Analyzer = "autocomplete", SearchAnalyzer = "autocomplete_search", Name = nameof(Name))]
        public string Name { get; set; }

        [Keyword(Name = nameof(State))]
        public string State { get; set; }

        //multifieds : mapping in code
        [Text(Name = nameof(Market))]
        public string Market { get; set; }
                
        [Ignore]
        public virtual string AdditionalInfo { get; }

        public SearchableBaseItem()
        {
        }
    }
}
