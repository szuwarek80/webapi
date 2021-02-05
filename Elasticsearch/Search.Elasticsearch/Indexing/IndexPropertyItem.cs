using Nest;
using Search.Elasticsearch.Mapping;
using System.Threading.Tasks;

namespace Search.Elasticsearch.Indexing
{
    public class IndexPropertyItem :
        IndexBaseItem
    {
        public override string Name => Config.IndexPropertyItemName;

        
        public override async Task<CreateIndexResponse> CreateIndex(IElasticClient aClient)
        {
            await this.DeleteIndex(aClient);

            var res = await aClient.Indices.CreateAsync(Name, i => i
                                .Settings(InitCommonIndexSettingsDescriptor)
                                .Map(m => m
                                    .AutoMap<SearchablePropertyItem>()
                                    .Properties( ps => ps
                                            .Text(p => p
                                               .Name("Market")
                                               .Fields(fs => fs                                                 
                                                   .Keyword(f => f
                                                       .Name("keyword")
                                                       )
                                                   )
                                               .Analyzer("autocomplete")
                                               .SearchAnalyzer("autocomplete_search")
                                                )
                                            .Text( p => p
                                                .Name("City")
                                                .Fields( fs => fs                                                  
                                                   .Keyword(f => f
                                                       .Name("keyword")
                                                       )
                                                    )
                                                .Analyzer("autocomplete")
                                                .SearchAnalyzer("autocomplete_search")
                                                )                                            
                                        )
                                    )
                );

            return res;
        }


        public override async Task<BulkResponse> InsertDataIntoIndex(IElasticClient aClient, IDataProvider aDataProvider)
        {
            var res = await InsertDataIntoIndex(aClient, aDataProvider.Properties);
            return res;
        }

    }
}
