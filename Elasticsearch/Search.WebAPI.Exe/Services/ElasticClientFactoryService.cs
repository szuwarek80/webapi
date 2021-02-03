﻿using Microsoft.Extensions.Configuration;
using Nest;
using Search.Elasticsearch.Search;
using System;

namespace Search.WebAPI.Exe.Services
{
   
    public class ElasticClientFactoryService :
        IElasticClientFactoryService
    {
        IConfiguration _configuration;

        public ElasticClientFactoryService(IConfiguration aConfiguration)
        {
            _configuration = aConfiguration;
        }

        public IElasticClient CreateElasticClient()
        {
            var node = new Uri(_configuration["AWSES:URL"]);
            var settings = new ConnectionSettings(node)
                            .BasicAuthentication(_configuration["AWSES:User"], _configuration["AWSES:Pwd"]);
            settings.DisableDirectStreaming();

            return new ElasticClient(settings);
        }
    }
}
