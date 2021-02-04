using Nest;
using RestSharp;
using Search.Elasticsearch.Mapping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Search.Elasticsearch.Searching
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
            BoolQuery filterQuery = BuildMarketFilterQuery(aSearchRequest.MarketFilterQuery);

            //check the API to be able to get the specific SearchableBaseItem derived types (something similar to ConcreteTypeSelector?)
            //for now we use JsonObject and convert it to specific object base on the "TypeName"            

            var results = await _client.SearchAsync<JsonObject>(s => s
                  .Size(aSearchRequest.PageSize)
                  .Skip(aSearchRequest.PageStartIndex)
                  .Index(Indices.Index(aSearchRequest.Indices))                    
                  .Query(q => q
                      .Bool(b => b
                        .Should(
                          // for City and Market a 'phrase' field is created, which allowes 
                          // to better place the item in result when the city/market was put as the AllStringFiledsQuery
                          // e.g. if someone put the AllStringFiledsQuery="San Francisco" ...
                          // but this match will not work in case of putting the AllStringFiledsQuery="San Francisco properties" 
                          bs => bs.MatchPhrase(x => x
                                      .Query(aSearchRequest.AllStringFiledsQuery.ToLower())
                                      .Field("City.phrase")
                              ),
                          bs => bs.MatchPhrase(x => x
                                      .Query(aSearchRequest.AllStringFiledsQuery.ToLower())
                                      .Field("Market.phrase")
                              )
                          )
                        .Must(
                          // we search all text fields with the AllStringFiledsQuery                          
                          // Name, Market, FormerName, StreetAdress, City text field uses:
                          //        'autocomplete' analyzer when indexing data
                          //        'autocomplete_search' analyzer when searching
                          //        both analyzers are crated on index creation
                          bs => bs.MultiMatch( m => m
                                         .Query(aSearchRequest.AllStringFiledsQuery.ToLower())
                                         .Fields(ff => ff
                                                .Field($"{nameof(SearchableBaseItem.Name)}")
                                                .Field($"{nameof(SearchableBaseItem.Market)}")
                                                .Field($"{nameof(SearchableBaseItem.State)}")
                                                .Field($"{nameof(SearchablePropertyItem.FormerName)}")
                                                .Field($"{nameof(SearchablePropertyItem.StreetAddres)}")
                                                .Field($"{nameof(SearchablePropertyItem.City)}")
                                                )                                          
                                          .Fuzziness(Fuzziness.Auto)
                                          ),
                          bs => filterQuery
                              )
                         )                      
                      )                   
                  );

            return new SimpleSerachResponse()
            {
                TotalItems = results.Total,
                Items =  ConvertResults(results)
            };
        }

        //we assume that:
        //  1: the MarketFilterQuery is in fromat Market1,Market2,Market3 e.g. Austin, San Paulo
        //  2: the market names are coming from predefined table, so no spelling mistakes, no autocompletition
        //  3: keyword field, and keyword_list_serach analyzer are created with the index
        private BoolQuery BuildMarketFilterQuery(string aMarket)
        {
            BoolQuery filterQuery = new BoolQuery();
            if (!string.IsNullOrEmpty(aMarket))
            {
                var filterQueryParts = new List<QueryContainer>();
                filterQueryParts.Add(
                    new MatchQuery()
                    {
                        Field = "Market.keyword",
                        Query = aMarket,
                        Analyzer = "keyword_list_serach"
                    }
                );

                filterQuery.Filter = filterQueryParts;
            }

            return filterQuery;
        }

        private List<SearchableBaseItem> ConvertResults(ISearchResponse<JsonObject> aSerachResponse)
        {            
            List<SearchableBaseItem> ret = new List<SearchableBaseItem>();

            foreach (var item in aSerachResponse.Documents)
            {
                switch (item["TypeName"])
                {
                    case "searchablemanagementitem":
                        ret.Add(new SearchableManagementItem()
                        {
                            Id = int.Parse(item["Id"].ToString()),
                            Name = item.ContainsKey("Name") ? item["Name"].ToString() : "not exist",
                            State = item.ContainsKey("State") ? item["State"].ToString() : "not exist",
                            Market = item.ContainsKey("Market") ? item["Market"].ToString() : "not exist",
                        });
                        break;
                    case "searchablepropertyitem":
                        ret.Add(new SearchablePropertyItem()
                        {
                            Id = int.Parse(item["Id"].ToString()),
                            Name = item.ContainsKey("Name") ? item["Name"].ToString(): "not exist",
                            State = item.ContainsKey("State") ? item["State"].ToString(): "not exist",
                            Market = item.ContainsKey("Market") ? item["Market"].ToString(): "not exist",
                            FormerName = item.ContainsKey("FormerName") ? item["FormerName"].ToString() : "not exist",
                            StreetAddres = item.ContainsKey("StreetAddres") ? item["StreetAddres"].ToString() : "not exist",
                            City = item.ContainsKey("City") ? (string)item["City"].ToString() : "not exist",
                            //Lat = (float) item["lat"],
                            //Lng = (float) item["lng"]
                        });
                        break;
                }
            }

            return ret;
        }
    }
}
