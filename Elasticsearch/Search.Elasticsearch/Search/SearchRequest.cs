using System;
using System.Collections.Generic;
using System.Text;

namespace Search.Elasticsearch.Search
{
   
    public class SimpleSearchRequest
    {
        public List<string> Indices { get; set; }


        public string Query { get; set; }
        public List<string> QueryFields { get; set; }
        

        public string Filter { get; set; }
        public List<string> FilterFields { get; set; }

        public int PageSize { get; set; }
        public int PageStartIndex { get; set; } 

        public SimpleSearchRequest()
        {
            this.PageSize = 25;
        }
    }

    
}
