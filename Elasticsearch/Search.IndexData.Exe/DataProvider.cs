using Nancy.Json;
using Newtonsoft.Json;
using RestSharp;
using Search.Elasticsearch.Indexing;
using Search.Elasticsearch.Mapping;
using Search.WebAPI.Exe.Dto;
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

        public DataProvider()
        {
            this.Properties = new List<SearchablePropertyItem>()
            {
                new SearchablePropertyItem()
                {
                    Id= 1,
                    Name ="My granny has a wooden  super chair",
                    StreetAddres = "test",
                    Market="Austin",
                    State ="GS"
                },
                new SearchablePropertyItem()
                {
                    Id = 2,
                    Name ="prop2",
                    StreetAddres = "nowa",
                    Market="Austin",
                    State ="GS"
                },
                new SearchablePropertyItem()
                {
                    Id = 3,
                    Name ="prop3",
                    StreetAddres = "nowa",
                    Market="San Francisco",
                    State ="CA"
                },
            };

            this.Managements = new List<SearchableManagementItem>()
                {
                    new SearchableManagementItem()
                    {
                        Id = 1,
                        Name = "Company A",
                        Market="San Paulo",
                        State ="TX"
                    }
                };

        }

        public int LoadManagemetns(string aPath)
        {
            int iErrorCounter = 0;
            var data = System.Text.Json.JsonSerializer.Deserialize<List<ManagementItemWrapper>>(File.ReadAllText(aPath));

            foreach (var mw in data)
                if (mw.mgmt != null)
                    this.Managements.Add(
                        new SearchableManagementItem()
                        {
                            Id = mw.mgmt.mgmtID,
                            Name = mw.mgmt.name,
                            State = mw.mgmt.state,
                            Market = mw.mgmt.market
                        }
                        );
                else 
                {
                    iErrorCounter++;
                }

            return iErrorCounter;
        }

        public void LoadProperties(string aPath)
        {
            int iErrorCounter = 0;
            var data = System.Text.Json.JsonSerializer.Deserialize<List<PropertyItemWrapper>>(File.ReadAllText(aPath));

            foreach (var pw in data)
                if (pw.property != null)
                    this.Properties.Add(
                        new SearchablePropertyItem()
                        {
                            Id = pw.property.propertyID,
                            Name = pw.property.name,
                            State = pw.property.state,
                            Market = pw.property.market,
                            FormerName = pw.property.formerName,
                            City = pw.property.city,
                            StreetAddres = pw.property.streetAddres,
                            Lat = pw.property.lat,
                            Lng = pw.property.lng
                        }
                    );
                else
                {
                    iErrorCounter++;
                }


        }

        class PropertyItemWrapper
        {
            public PropertyItem property { get; set; }
        }

        public class PropertyItem 
        {
            public int propertyID { get; set; }
            public string name { get; set; }
            public string formerName { get; set; }
            public string streetAddres { get; set; }
            public string city { get; set; }
            public string market { get; set; }
            public string state { get; set; }
            public float lat { get; set; }
            public float lng { get; set; }
        }

        class ManagementItemWrapper
        {
            public ManagementItem mgmt { get; set; }
        }

        public class ManagementItem
        {
            public int mgmtID { get; set; }
            public string name { get; set; }
            public string market { get; set; }
            public string state { get; set; }
        }
    }
}
