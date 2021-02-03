using Newtonsoft.Json;
using RestSharp;
using Search.Elasticsearch.Indexing;
using Search.Elasticsearch.Mapping;
using System;
using System.Collections.Generic;
using System.IO;

namespace Search.IndexData.Exe
{
    class DataProvider :
        IDataProvider
    {
        public List<SearchablePropertyItem> Properties { get; protected set; }
        public List<SearchableManagementItem> Managements { get; protected set; }

        public DataProvider(string aPropertiesJsonFilePath, string aMgmtJsonFilePath)
        {            
            //List<JsonObject> data = JsonConvert.DeserializeObject<List<JsonObject>>(File.ReadAllText(aPropertiesJsonFilePath), new JsonSerializerSettings()
            //{ 
            //  Culture = global.C
            //}); 
            //new JsonSerializerSettings()
            //{ 
            // FloatFormatHandling = FloatFormatHandling.
            //});

            this.Properties = new List<SearchablePropertyItem>()
            {
                new SearchablePropertyItem()
                {
                    Id= 1,
                    Name ="prop1",
                    StreetAddres = "test",
                    Market="Warszawa",
                    State ="a"
                },
                new SearchablePropertyItem()
                {
                    Id = 2,
                    Name ="prop2",
                    StreetAddres = "nowa",
                    Market="Warszawa",
                    State ="a"
                },
                new SearchablePropertyItem()
                {
                    Id = 3,
                    Name ="prop3",
                    StreetAddres = "nowa",
                    Market="Szczecin",
                    State ="a"
                },
            };

            this.Managements = new List<SearchableManagementItem>()
                {
                    new SearchableManagementItem()
                    {
                        Id = 1,
                        Name = "test",
                        Market="Szczecin",
                        State ="a"
                    }
                };           
        }
    }
}
