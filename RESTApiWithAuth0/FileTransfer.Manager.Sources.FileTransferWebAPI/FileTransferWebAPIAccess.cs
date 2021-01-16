using FileTransfer.Definitions.Dto;
using FileTransfer.WebAPI.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer.Manager.Sources.FileTransferWebAPI
{
    public class FileTransferWebAPIAccess
    {
        const string _audience = "https://file-transfer";
        private API _api;
        private string _url;
        private object _lock = new object();

        public string ID { get; }

        public bool IsConnected { get; set; }


        public FileTransferWebAPIAccess(TransferSourceDto aSource)
        {
            this.ID = aSource.ID.ToString();
            this.IsConnected = false;

            _api = JsonConvert.DeserializeObject<API>(aSource.ConnectionDescription);
            _url = $"{_api.url.TrimEnd('/')}/api/transfers";

            Task task = GetAccessToken();
        }

       

        public async Task<Guid> SendTransferCreateRequest(HttpClient aClient, TransferRequestDto aRequest)
        {
            var request = new CreateFileTransferDto() { FileID = aRequest.FileID };

            var json = JsonConvert.SerializeObject(request);

            var data = new StringContent(json, Encoding.UTF8, "application/json");
            
            aClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", this.AccessToken);
           
            var response = await aClient.PostAsync(_url, data);

            if (!response.IsSuccessStatusCode)
            {
                var ex = new Exception($"Cannot create a transfer. Status code: {response.StatusCode}, message: {response.ReasonPhrase}.");

                throw ex;
            }

            string ret = await response.Content?.ReadAsStringAsync();
            return Guid.Parse(ret.Trim('"'));
        }

        public async Task<FileTransferDto> SendTransferGetRequest(HttpClient aClient, Guid aTaskID)
        {
            var response = await aClient.GetAsync($"{_url}/{aTaskID}");

            if (!response.IsSuccessStatusCode)
            {
                var ex = new Exception($"Cannot get a transfer. Status code: {response.StatusCode}, message: {response.ReasonPhrase}.");
                throw ex;
            }

            string resultContent = await response.Content.ReadAsStringAsync();
           
            FileTransferDto statusResponse = JsonConvert.DeserializeObject<FileTransferDto>(resultContent);

            return statusResponse;
        }      

        public async Task SendTransferDeleteRequest(HttpClient aClient, Guid aTaskID)
        {
            aClient.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", this.AccessToken);

            var response = await aClient.DeleteAsync($"{_url}/{aTaskID}");

            if (!response.IsSuccessStatusCode)
            {
                var ex = new Exception($"Cannot delete a transfer. Status code: {response.StatusCode}, message: {response.ReasonPhrase}.");
                throw ex;
            }
        }



        string _accessToken;
        private string AccessToken
        {
            get { lock (_lock) { return _accessToken; } }
            set { lock (_lock) { _accessToken = value; } }
        }

        private async Task GetAccessToken()
        {
            AccessTokenRequest request = new AccessTokenRequest()
            {
                client_id = _api.client_id,
                client_secret = _api.client_secret,
                audience = _audience,
                grant_type = "client_credentials"
            };

            var requestJson = JsonConvert.SerializeObject(request);
            var client = new HttpClient();
            var data = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"https://{_api.domanin}/oauth/token", data);


            if (response.IsSuccessStatusCode)
            {
                string ret  = await response.Content.ReadAsStringAsync();
                AccessTokenResponse responseToken = JsonConvert.DeserializeObject<AccessTokenResponse>(ret);
                this.AccessToken = responseToken.access_token;
                this.IsConnected = true;
            }         
        }

        struct AccessTokenRequest
        {
            public string client_id { get; set; }
            public string client_secret { get; set; }
            public string audience { get; set; }
            public string grant_type { get; set; }
        }

        class AccessTokenResponse
        { 
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
        }

        class API
        {
            public string url { get; set; }
            public string client_id { get; set; }
            public string client_secret { get; set; }
            public string domanin { get; set; }
        }
    }
}
