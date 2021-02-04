using Nest;
using Search.Elasticsearch.Mapping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Search.Elasticsearch.Search
{
    public interface ISearchService
    {
        Task<SimpleSerachResponse> Search(SimpleSearchRequest aSearchRequest);
    }

    public class SearchService :
        ISearchService
    {
        private IElasticClient _client;

        public SearchService(IElasticClientFactoryService aElasticClientFactoryService)
        {
            _client = aElasticClientFactoryService.CreateElasticClient();
        }

        public async Task<SimpleSerachResponse> Search(SimpleSearchRequest aSearchRequest)
        {
            BoolQuery filterQuery = new BoolQuery();
            if (!string.IsNullOrEmpty(aSearchRequest.Filter))
            {
                var filterQueryParts = new List<QueryContainer>();
                filterQueryParts.Add(
                    new MatchQuery()
                    {
                        Field = $"{nameof(SearchableBaseItem.Market)}",
                        Query = aSearchRequest.Filter.ToLower(),
                        Fuzziness = Fuzziness.Auto
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
                                 .Query(aSearchRequest.Query.ToLower())
                                 .Fields(ff => ff                                        
                                        .Field($"{nameof(SearchableBaseItem.Name)}")
                                        .Field($"{nameof(SearchableBaseItem.Market)}")
                                        .Field($"{nameof(SearchableBaseItem.State)}")
                                        .Field($"{nameof(SearchablePropertyItem.FormerName)}")
                                        .Field($"{nameof(SearchablePropertyItem.StreetAddres)}")
                                        .Field($"{nameof(SearchablePropertyItem.City)}")
                                        )
                                  //.Fuzziness(Fuzziness.Auto)
                                  )                       
                        && filterQuery
                      )                      
                  ); 

            return new SimpleSerachResponse()
            { 
                TotalItems = results.Total,
                Items = results.Documents
            };
        }       
    }
}
