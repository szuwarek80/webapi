using Nest;
using Search.Elasticsearch.Indexing;
using System;

namespace Search.IndexData.Exe
{
    class Program
    {
        const string AWSESUri = "https://search-domain-test-1-ivcixqe6sjc275qcdzile5tonq.us-east-2.es.amazonaws.com";
        const string AWSESUser = "ula";
        const string AWSESPwd = "Marchewka1980!";

        static void Main(string[] args)
        {
            var node = new Uri(AWSESUri);
            var settings = new ConnectionSettings(node).BasicAuthentication(AWSESUser, AWSESPwd);
            settings.DisableDirectStreaming();

            var client = new ElasticClient(settings);

            DataProvider dataProvider = new DataProvider();
            //try
            //{
            //    dataProvider.LoadManagemetns(@"D:\sandbox-private\GitHub\webapi\Elasticsearch\mgmt.json");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}

            //try
            //{
            //    dataProvider.LoadProperties(@"D:\sandbox-private\GitHub\webapi\Elasticsearch\properties.json");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}

            IndexPropertyItem indexPropertyItem = new IndexPropertyItem();
            var resCreate = indexPropertyItem.CreateIndex(client).Result;
            var resInsert = indexPropertyItem.InsertDataIntoIndex(client, dataProvider).Result;

            IndexManagementItem indexManagementItem = new IndexManagementItem();
            resCreate = indexManagementItem.CreateIndex(client).Result;
            resInsert = indexManagementItem.InsertDataIntoIndex(client, dataProvider).Result;

        }
    }
}
