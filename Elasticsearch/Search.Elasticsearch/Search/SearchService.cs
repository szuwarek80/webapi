using Nest;
using Search.Elasticsearch.Mapping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Search.Elasticsearch.Search
{
    public interface ISearchService
    {
        Task<ISearchResponse<SearchableBaseItem>> Search(SimpleSearchRequest aSearchRequest);
    }

    public class SearchService :
        ISearchService
    {

        private IElasticClient _client;

        public SearchService(IElasticClientFactoryService aElasticClientFactoryService)
        {
            _client = aElasticClientFactoryService.CreateElasticClient();
        }

        public async Task<ISearchResponse<SearchableBaseItem>> Search(SimpleSearchRequest aSearchRequest)
        {
            BoolQuery filterQuery = new BoolQuery();
            if (!string.IsNullOrEmpty(aSearchRequest.Filter))
            {
                var filterQueryParts = new List<QueryContainer>();
                filterQueryParts.Add(new MatchQuery()
                {
                    Field = $"{nameof(SearchableBaseItem.Market)}",
                    Query = aSearchRequest.Filter
                }
                );

                filterQuery.Filter = filterQueryParts;
            }

            var results = await _client.SearchAsync<SearchableBaseItem>(s => s
                  .Size(aSearchRequest.PageSize)
                  .Skip(aSearchRequest.PageStartIndex)
                  .Index(Indices.Index(aSearchRequest.Indices))
                  .Query(q => q   
                      .MultiMatch(m => m
                                 .Query(aSearchRequest.Query)
                                 .Fields(ff => ff
                                        .Field($"{nameof(SearchableBaseItem.Name)}")
                                        .Field($"{nameof(SearchableBaseItem.Market)}")
                                        .Field($"{nameof(SearchableBaseItem.State)}")
                                        .Field($"{nameof(SearchablePropertyItem.FormerName)}")
                                        .Field($"{nameof(SearchablePropertyItem.StreetAddres)}")
                                        .Field($"{nameof(SearchablePropertyItem.City)}")
                                        )
                                  )
                        && filterQuery
                      )
                  ); 

            return results;
          }       
    }
}
