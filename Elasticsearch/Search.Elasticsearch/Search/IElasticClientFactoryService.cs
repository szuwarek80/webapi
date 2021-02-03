using Nest;

namespace Search.Elasticsearch.Search
{
    public interface IElasticClientFactoryService
    {
        IElasticClient CreateElasticClient();
    }
}
