using FileTransfer.Definitions.Dto;
using FileTransfer.WebAPI.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer.Manager.Sources.FileTransferWebAPI
{
    public class FileTransferWebAPIAccess
    {
        private string _url;

        public FileTransferWebAPIAccess(TransferSourceDto aSource)
        {
            this.ID = aSource.ID;
            this.RESTAddress = aSource.ConnectionDescription;

            string url = RESTAddress.TrimEnd('/');
            _url = $"{url}/api/transfers";
        }

        public Guid ID { get; }
        public string RESTAddress { get; }


        public async Task<string> SendTransferRequest(HttpClient aClient, TransferRequestDto aRequest)
        {
            var request = new CreateTransferDto() { FileID = aRequest.FileID };

            var json = JsonConvert.SerializeObject(request);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await aClient.PostAsync(_url, data);

            if (!response.IsSuccessStatusCode)
            {
                var ex = new Exception($"Cannot create a transfer request. Status code: {response.StatusCode}, message: {response.ReasonPhrase}.");

                throw ex;
            }

            string ret = await response.Content?.ReadAsStringAsync();

            return ret.Trim('"');
        }


        public async Task<TransferStatusDto> SendTransferStatusRequest(HttpClient aClient, string aTaskID)
        {
            string url = RESTAddress.TrimEnd('/');

            var response = await aClient.GetAsync($"{_url}/{aTaskID}/status");

            string resultContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }

            TransferStatusDto statusResponse = JsonConvert.DeserializeObject<TransferStatusDto>(resultContent);

            return statusResponse;
        }

        public async Task SendTransferCancelRequest(HttpClient aClient, string aTaskID)
        {
            var response = await aClient.PutAsync($"{_url}/{aTaskID}/cancel", null);

            if (!response.IsSuccessStatusCode)
            {
                var ex = new Exception($"Cannot delete a transfer request. Status code: {response.StatusCode}, message: {response.ReasonPhrase}.");
                throw ex;
            }
        }

        public async Task SendTransferDeleteRequest(HttpClient aClient, string aTaskID)
        {         
            var response = await aClient.DeleteAsync($"{_url}/{aTaskID}");

            if (!response.IsSuccessStatusCode)
            {
                var ex = new Exception($"Cannot delete a transfer request. Status code: {response.StatusCode}, message: {response.ReasonPhrase}.");
                throw ex;
            }
        }

      
    }
}
