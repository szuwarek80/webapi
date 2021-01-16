using FileTransfer.WebAPI.Dto;
using FileTransfer.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer.WebAPI.Controllers
{
    [ApiController]
    [Route("api/transfers")]
    public class FileTransfersController : ControllerBase
    {
        private readonly Logger<FileTransfersController> _logger;
        private FileTransfersControllerService _service;
        
        public FileTransfersController(FileTransfersControllerService aService)
        {
            _logger = new Logger<FileTransfersController>();
            _service = aService;
        }

         [HttpPost]
         [Authorize]
         public async Task<IActionResult> CreateFileTransferRequest(CreateFileTransferDto aRequest)
         {
             _logger.LogDebug($"Request: {JsonConvert.SerializeObject(aRequest)}");

             return Ok(await _service.CreateFileTransfer(aRequest));
         }

        [HttpGet("{aID}")]
        public async Task<IActionResult> GetFileTransferRequest(Guid aID)
        {
            var status = await _service.GetFileTransferStatus(aID);

            if (status == null)
            {
                _logger.LogDebug($"{aID} not found");

                return NotFound();
            }

            _logger.LogDebug($"Status: {JsonConvert.SerializeObject(status)}");

            return Ok(status);            
        }
     
        [HttpDelete("{aID}")]
        [Authorize]
        public async Task<IActionResult> DeleteFileTransferRequest(Guid aID)
        {
            if (await _service.DeleteTransfer(aID, out bool forbidden))
            {
                _logger.LogDebug($"{aID} removed");

                return Ok();
            }

            if (forbidden)
            {
                _logger.LogDebug($"{aID} not removed, forbidden");

                return StatusCode(403);
            }
            else
            {
                _logger.LogDebug($"{aID} not found");

                return NotFound();
            }
        }
    }
}
