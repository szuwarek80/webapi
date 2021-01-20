using FileTransfer.Definitions.Dto;
using FileTransfer.Manager.Exe.Models;
using FileTransfer.Manager.Exe.Services;
using FileTransfer.WebAPI.Definitions;
using FileTransfer.WebAPI.Definitions.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FileTransfer.Manager.Exe.Controllers
{
    [ApiController]
    [Route("api/v1/transfers")]
    public class FileTransfersController : ControllerBase
    {
        private readonly ILogger<FileTransfersController> _logger;
        private readonly IFileTransferService _service;

        public FileTransfersController(ILogger<FileTransfersController> aLogger, IFileTransferService aService)
        {
            _logger = aLogger;
            _service = aService;
        }

        [HttpPost]
      //  [Authorize]
        public async Task<IActionResult> FileTransferCreateRequest(FileTransferCreateDto<SystemFileID> aRequest)
        {
            _logger?.LogDebug("'{0}' has been invoked", nameof(FileTransferCreateRequest));

            var response = new SingleResponse<Guid>();

            try
            {
                response.Model = await _service.TransferCreate(aRequest);

                _logger?.LogInformation("Trasnfer '{0}' has been started", response.Model);
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = "There was an internal error, please contact to technical support.";

                _logger?.LogCritical("There was an error on '{0}' invocation: {1}", nameof(FileTransferCreateRequest), ex);
            }

            return response.ToHttpResponse();
        }

        [HttpGet]
        public async Task<IActionResult> FileTransferGetAllRequest()
        {
            _logger?.LogDebug("'{0}' has been invoked", nameof(FileTransferGetAllRequest));

            var response = new ListResponse<FileTransferDto>();
            try
            {
                response.Model = await _service.TransferGet();

                _logger?.LogInformation("Trasnfer '{0}' has been started", response.Model);
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = "There was an internal error, please contact to technical support.";

                _logger?.LogCritical("There was an error on '{0}' invocation: {1}", nameof(FileTransferCreateRequest), ex);
            }
            return response.ToHttpResponse();
        }


        [HttpGet("{aID}")]
        public async Task<IActionResult> FileTransferGetRequest(Guid aID)
        {
            _logger?.LogDebug("'{0}' has been invoked", nameof(FileTransferGetRequest));

            var response = new SingleResponse<FileTransferDto>();

            try
            {
                response.Model =  await _service.TransferGet(aID);

                _logger?.LogInformation("Status retrieved successfully {0}.", response.Model);
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = "There was an internal error, please contact to technical support.";

                _logger?.LogCritical("There was an error on '{0}' invocation: {1}", nameof(FileTransferGetRequest), ex);
            }

            return response.ToHttpResponse();
        }      
    }
}
