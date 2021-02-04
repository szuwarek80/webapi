using Nest;

namespace Search.Elasticsearch.Searching
{
    public interface IElasticClientFactoryService
    {
        IElasticClient CreateElasticClient();
    }
}
