using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Search.Elasticsearch;
using Search.Elasticsearch.Mapping;
using Search.Elasticsearch.Search;
using Search.WebAPI.Exe.Dto;

namespace Search.WebAPI.Exe.Controllers
{
    [ApiController]
    [Route("api/v1/search")]
    public class SearchController : ControllerBase
    {

        private readonly ILogger<SearchController> _logger;
        private readonly ISearchService _searchSevice;

        public SearchController(ILogger<SearchController> aLogger, ISearchService aSerachService)
        {
            _logger = aLogger;
            _searchSevice = aSerachService;
        }

        [HttpGet]
        public async Task<IActionResult> Search(SearchCriteriaDto aRequest)
        {
            _logger?.LogDebug("'{0}' has been invoked", nameof(SearchController));

            var response = new PagedResponse<Elasticsearch.Mapping.SearchableBaseItem>();
            try
            {
                var result = await _searchSevice.Search(new SimpleSearchRequest()
                {
                    PageSize = aRequest.PageSize,
                    PageStartIndex = aRequest.PageStartIndex,
                    Query = aRequest.Phase,
                    QueryFields = new List<string>() 
                    { 
                        nameof(SearchableBaseItem.Name), 
                        nameof(SearchableBaseItem.Market), 
                        nameof(SearchableBaseItem.State),
                        nameof(SearchablePropertyItem.FormerName),
                        nameof(SearchablePropertyItem.StreetAddres),
                        nameof(SearchablePropertyItem.City)
                    },
                    Filter = aRequest.Market,
                    FilterFields = new List<string>() { nameof(SearchableBaseItem.Market) },
                    Indices = new List<string>() { Config.IndexPropertyItemName, Config.IndexManagementItemName }
                }
                ); 

                response.Model = result.Items;
                response.ItemsCount = result.TotalItems;
                response.PageSize = aRequest.PageSize;
                response.PageNumber = aRequest.PageStartIndex / aRequest.PageSize;

                _logger?.LogInformation("Trasnfer '{0}' has been started", response.Model);
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = "There was an internal error, please contact to technical support.";

                _logger?.LogCritical("There was an error on '{0}' invocation: {1}", nameof(SearchController), ex);
            }
            return response.ToHttpResponse();
        }
    }
}
