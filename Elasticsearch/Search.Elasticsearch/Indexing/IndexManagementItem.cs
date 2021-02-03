using Nest;
using Search.Elasticsearch.Mapping;
using System.Threading.Tasks;

namespace Search.Elasticsearch.Indexing
{
    public class IndexManagementItem :
        IndexBaseItem
    {
        public override string Name => Config.IndexManagementItemName;


        public override async Task<CreateIndexResponse> CreateIndex(IElasticClient aClient)
        {
            await this.DeleteIndex(aClient);

            var res = await aClient.Indices.CreateAsync(Name, i => i
                                .Settings(InitCommonIndexSettingsDescriptor)
                                .Map(m => m.AutoMap<SearchableManagementItem>())
                );

            return res;
        }


        public override async Task<BulkResponse> InsertDataIntoIndex(IElasticClient aClient, IDataProvider aDataProvider)
        {
            var res = await InsertDataIntoIndex(aClient, aDataProvider.Managements);
            return res;
        }

    }
}
