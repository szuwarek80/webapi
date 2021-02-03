using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Search.WebAPI.Exe.Dto
{
    public class SearchCriteriaDto
    {
        public string Phase { get; set; }
        public string Market { get; set; }
        public int PageSize { get; set; }
        public int PageStartIndex { get; set; }

        public SearchCriteriaDto()
        {
            this.PageSize = 25;
            this.PageStartIndex = 0;
        }
    }
}
