using System.Collections.Generic;

namespace Search.Elasticsearch.Search
{

    public class SimpleSearchRequest
    {
        public List<string> Indices { get; set; }

        public string AllStringFiledsQuery { get; set; }
        
        public string MarketFilterQuery { get; set; }

        public int PageSize { get; set; }
        public int PageStartIndex { get; set; } 

        public SimpleSearchRequest()
        {
            this.PageSize = 25;
        }
    }

    
}
