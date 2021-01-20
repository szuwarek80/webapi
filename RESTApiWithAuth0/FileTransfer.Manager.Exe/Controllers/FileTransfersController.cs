using FileTransfer.Definitions;
using FileTransfer.Definitions.Dto;
using FileTransfer.Manager.Exe.Models;
using FileTransfer.Manager.Exe.Services;
using FileTransfer.Manager.Persistence.Connection;
using FileTransfer.WebAPI.Definitions;
using FileTransfer.WebAPI.Definitions.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileTransfer.Manager.Exe.Controllers
{
    [ApiController]
    [Route("api/v1/transfers")]
    public class FileTransfersController : ControllerBase
    {
        private readonly ILogger<FileTransfersController> _logger;
        private readonly IConnection _connection;

        public FileTransfersController(ILogger<FileTransfersController> aLogger, IConnectionFactory aConnectionFactory)
        {
            _logger = aLogger;
            _connection = aConnectionFactory.CreateConnection();
        }        

        [HttpPost]
      //  [Authorize]
        public IActionResult FileTransferCreateRequest(FileTransferCreateDto<SystemFileID> aRequest)
        {
            _logger?.LogDebug("'{0}' has been invoked", nameof(FileTransferCreateRequest));

            var response = new SingleResponse<Guid>();

            try
            {
                var tr = _connection.TransferRequestRepository.RequestStart(
                      new Persistence.Repositories.RequestStartDto()
                      {
                          SourceID = aRequest.FileID.SourceID,
                          FileID = aRequest.FileID.FileID
                      });

                response.Model = tr.TransferRequestID;

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
        public IActionResult FileTransferGetAllRequest()
        {
            _logger?.LogDebug("'{0}' has been invoked", nameof(FileTransferGetAllRequest));

            var response = new ListResponse<FileTransferDto>();
            try
            {
                response.Model = new List<FileTransferDto>();

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
        public IActionResult FileTransferGetRequest(Guid aID)
        {
            _logger?.LogDebug("'{0}' has been invoked", nameof(FileTransferGetRequest));

            var response = new SingleResponse<FileTransferDto>();

            try
            {
                var tr = _connection.TransferRequestRepository.GetById(aID);

                if (tr != null)
                    response.Model =  new FileTransferDto()
                    {
                        TransferRequestID = tr.TransferRequestID,
                        Status = (TransferResultStatus)tr.Status,
                        Description = tr.Description,
                        Result = tr.Result
                    }; ;

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
